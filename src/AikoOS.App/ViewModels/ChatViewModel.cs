using System.Collections.ObjectModel;
using AikoOS.AI.Models;
using AikoOS.App.Models;
using AikoOS.Core.Interfaces;
using AikoOS.Core.Models;
using AikoOS.Runtime.Brain;
using AikoOS.Runtime.Brain.Models;
using AikoOS.Behavior.Context;
using AikoOS.Behavior.Emotions;
using AikoOS.Behavior.State;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AikoOS.App.ViewModels;

public partial class ChatViewModel : ObservableObject
{

    private readonly IBrainRequestService _brainRequestService;
    private readonly IChatRepository _chatRepository;
    private readonly ICharacterContext _characterContext;

    private CancellationTokenSource? _cancellationTokenSource;
    private Guid _conversationId;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private string _inputMessage = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SendMessageCommand))]
    private bool _isSending;

    [ObservableProperty]
    private string _statusMessage = "Đang tải...";

    public ObservableCollection<ChatMessageItem> Messages { get; }
        = [];

    public ChatViewModel(
    IChatRepository chatRepository,
    IBrainRequestService brainRequestService,
    ICharacterContext characterContext)
    {
        _brainRequestService = brainRequestService;
        _chatRepository = chatRepository;
        _characterContext = characterContext;
    }

    public async Task InitializeAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            StatusMessage = "Đang tải cuộc trò chuyện...";

            IReadOnlyList<ChatConversation> conversations =
                await _chatRepository.GetConversationsAsync(
                    cancellationToken);

            if (conversations.Count == 0)
            {
                await CreateNewConversationAsync(
                    cancellationToken);

                StatusMessage = "Sẵn sàng";
                return;
            }

            ChatConversation latestConversation =
                conversations[0];

            _conversationId = latestConversation.Id;

            IReadOnlyList<StoredChatMessage> history =
                await _chatRepository.GetMessagesAsync(
                    _conversationId,
                    cancellationToken);

            Messages.Clear();

            foreach (StoredChatMessage message in history)
            {
                Messages.Add(
                    new ChatMessageItem
                    {
                        Role = message.Role,
                        Content = message.Content
                    });
            }

            if (Messages.Count == 0)
            {
                AddWelcomeMessage();
            }

            StatusMessage = "Sẵn sàng";
        }
        catch (Exception exception)
        {
            Messages.Clear();

            Messages.Add(
                new ChatMessageItem
                {
                    Role = "assistant",
                    Content =
                        $"Không thể tải lịch sử trò chuyện: {exception.Message}"
                });

            StatusMessage = "Không thể tải dữ liệu";
        }
    }

    private bool CanSendMessage()
    {
        return !IsSending
            && !string.IsNullOrWhiteSpace(InputMessage);
    }

    [RelayCommand(CanExecute = nameof(CanSendMessage))]
    private async Task SendMessageAsync()
    {
        string userMessage = InputMessage.Trim();

        if (string.IsNullOrWhiteSpace(userMessage))
        {
            return;
        }

        if (_conversationId == Guid.Empty)
        {
            await CreateNewConversationAsync();
        }

        InputMessage = string.Empty;
        IsSending = true;
        StatusMessage = "Aiko đang trả lời...";

        _characterContext.Update(CharacterStateNames.Listening, EmotionNames.Curious);

        Messages.Add(
            new ChatMessageItem
            {
                Role = "user",
                Content = userMessage
            });

        _cancellationTokenSource?.Dispose();
        _cancellationTokenSource =
            new CancellationTokenSource();

        CancellationToken cancellationToken =
            _cancellationTokenSource.Token;

        try
        {
            await _chatRepository.AddMessageAsync(
                _conversationId,
                "user",
                userMessage,
                cancellationToken);

            IReadOnlyList<ChatMessage> conversation =
    CreateConversationHistory();

            BrainRequest brainRequest =
                new()
                {
                    UserInput = userMessage,
                    ConversationHistory = conversation
                };

            _characterContext.Update(CharacterStateNames.Thinking, EmotionNames.Curious);

            BrainResponse response =
                await _brainRequestService.ProcessAsync(
                    brainRequest,
                    cancellationToken);

            if (response.Success)
            {

                _characterContext.Update(CharacterStateNames.Speaking, EmotionNames.Happy);

                string assistantMessage =
                    response.Speech.Trim();

                Messages.Add(
                    new ChatMessageItem
                    {
                        Role = "assistant",
                        Content = assistantMessage
                    });

                await _chatRepository.AddMessageAsync(
                    _conversationId,
                    "assistant",
                    assistantMessage,
                    cancellationToken);

                await UpdateConversationTitleIfNeededAsync(
                    userMessage,
                    cancellationToken);

                StatusMessage = "Sẵn sàng";
            }
            else
            {

                _characterContext.Update(CharacterStateNames.Idle, EmotionNames.Sad);

                Messages.Add(
                    new ChatMessageItem
                    {
                        Role = "assistant",
                        Content =
                            $"Đã xảy ra lỗi: {response.ErrorMessage}"
                    });

                StatusMessage = "Không thể nhận câu trả lời";
            }
        }
        catch (OperationCanceledException)
        {
            _characterContext.Update(
                CharacterStateNames.Idle,
                EmotionNames.Neutral);

            StatusMessage = "Đã hủy yêu cầu";
        }
        catch (Exception exception)
        {
            _characterContext.Update(
                CharacterStateNames.Idle,
                EmotionNames.Sad);

            Messages.Add(
                new ChatMessageItem
                {
                    Role = "assistant",
                    Content =
                        $"Đã xảy ra lỗi không mong muốn: {exception.Message}"
                });

            StatusMessage = "Đã xảy ra lỗi";
        }
        finally
        {
            IsSending = false;

            _characterContext.Update(
                CharacterStateNames.Idle,
                EmotionNames.Neutral);
        }
    }

    private IReadOnlyList<ChatMessage> CreateConversationHistory()
    {
        List<ChatMessage> conversation =
        [
            new ChatMessage
            {
                Role = "system",
                Content =
                    """
                    Bạn là Aiko, một trợ lý AI thân thiện chạy trong ứng dụng AikoOS.
                    Hãy trả lời rõ ràng, tự nhiên và hữu ích.
                    Mặc định trả lời bằng tiếng Việt, trừ khi người dùng yêu cầu ngôn ngữ khác.
                    """
            }
        ];

        conversation.AddRange(
            Messages
                .Where(message =>
                    message.Role is "user" or "assistant")
                .Select(message =>
                    new ChatMessage
                    {
                        Role = message.Role,
                        Content = message.Content
                    }));

        return conversation;
    }

    private async Task CreateNewConversationAsync(
        CancellationToken cancellationToken = default)
    {
        ChatConversation conversation =
            await _chatRepository.CreateConversationAsync(
                "Cuộc trò chuyện mới",
                cancellationToken);

        _conversationId = conversation.Id;

        Messages.Clear();
        AddWelcomeMessage();
    }

    private void AddWelcomeMessage()
    {
        Messages.Add(
            new ChatMessageItem
            {
                Role = "assistant",
                Content =
                    "Xin chào! Mình là Aiko. Bạn muốn hỏi gì?"
            });
    }

    private async Task UpdateConversationTitleIfNeededAsync(
        string firstUserMessage,
        CancellationToken cancellationToken)
    {
        int userMessageCount =
            Messages.Count(message =>
                message.Role == "user");

        if (userMessageCount != 1)
        {
            return;
        }

        string title = firstUserMessage.Trim();

        if (title.Length > 50)
        {
            title = title[..50].Trim() + "...";
        }

        await _chatRepository.UpdateConversationTitleAsync(
            _conversationId,
            title,
            cancellationToken);
    }

    [RelayCommand]
    private void CancelRequest()
    {
        _cancellationTokenSource?.Cancel();
    }

    [RelayCommand]
    private async Task ClearConversationAsync()
    {
        if (IsSending)
        {
            return;
        }

        try
        {
            StatusMessage = "Đang tạo cuộc trò chuyện mới...";

            if (_conversationId != Guid.Empty)
            {
                await _chatRepository.DeleteConversationAsync(
                    _conversationId);
            }

            await CreateNewConversationAsync();

            Messages.Clear();

            Messages.Add(
                new ChatMessageItem
                {
                    Role = "assistant",
                    Content =
                        "Cuộc trò chuyện đã được làm mới."
                });

            StatusMessage = "Sẵn sàng";
        }
        catch (Exception exception)
        {
            StatusMessage =
                $"Không thể làm mới cuộc trò chuyện: {exception.Message}";
        }
    }
}