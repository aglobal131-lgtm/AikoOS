using AikoOS.Runtime.Brain;
using AikoOS.Runtime.Brain.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;

namespace AikoOS.App.ViewModels.Brain;

public partial class BrainInputViewModel : ObservableObject
{
    private readonly IBrainRequestService _brainRequestService;
    private readonly ILogger<BrainInputViewModel> _logger;

    [ObservableProperty]
    private string _userInput = string.Empty;

    [ObservableProperty]
    private bool _isProcessing;

    [ObservableProperty]
    private string _statusMessage = "Sẵn sàng";

    public BrainInputViewModel(
        IBrainRequestService brainRequestService,
        ILogger<BrainInputViewModel> logger)
    {
        _brainRequestService = brainRequestService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task SendAsync()
    {
        if (IsProcessing)
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(UserInput))
        {
            StatusMessage = "Hãy nhập nội dung trước.";

            return;
        }

        string input = UserInput.Trim();

        try
        {
            IsProcessing = true;
            StatusMessage = "Aiko đang suy nghĩ...";

            BrainResponse response =
    await _brainRequestService.ProcessAsync(input);

            UserInput = string.Empty;

            StatusMessage = response.Success
                ? response.Speech
                : response.ErrorMessage;
        }
        catch (Exception exception)
        {
            StatusMessage = "Không thể xử lý yêu cầu.";

            _logger.LogError(
                exception,
                "Failed to send user input to Brain.");
        }
        finally
        {
            IsProcessing = false;
        }
    }
}