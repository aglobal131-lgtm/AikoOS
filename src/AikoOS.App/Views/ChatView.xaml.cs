using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using AikoOS.App.ViewModels;

namespace AikoOS.App.Views;

public partial class ChatView : UserControl
{
    private ChatViewModel? _viewModel;
    private bool _isInitialized;

    public ChatView()
    {
        InitializeComponent();

        DataContextChanged += OnDataContextChanged;

        Loaded += OnLoaded;
        Unloaded += OnUnloaded;
    }

    private async void OnLoaded(
        object sender,
        RoutedEventArgs e)
    {
        if (!_isInitialized
            && _viewModel is not null)
        {
            _isInitialized = true;

            try
            {
                await _viewModel.InitializeAsync();
            }
            catch
            {
                _isInitialized = false;
                throw;
            }
        }

        MessageInputTextBox.Focus();
        ScrollToLatestMessage();
    }

    private void OnUnloaded(
        object sender,
        RoutedEventArgs e)
    {
        UnsubscribeFromViewModel();
    }

    private void OnDataContextChanged(
        object sender,
        DependencyPropertyChangedEventArgs e)
    {
        UnsubscribeFromViewModel();

        _viewModel = e.NewValue as ChatViewModel;
        _isInitialized = false;

        if (_viewModel is not null)
        {
            _viewModel.Messages.CollectionChanged +=
                OnMessagesCollectionChanged;
        }
    }

    private void UnsubscribeFromViewModel()
    {
        if (_viewModel is not null)
        {
            _viewModel.Messages.CollectionChanged -=
                OnMessagesCollectionChanged;
        }

        _viewModel = null;
    }

    private void OnMessagesCollectionChanged(
        object? sender,
        NotifyCollectionChangedEventArgs e)
    {
        Dispatcher.BeginInvoke(
            ScrollToLatestMessage);
    }

    private void ScrollToLatestMessage()
    {
        if (MessagesListBox.Items.Count == 0)
        {
            return;
        }

        object lastItem =
            MessagesListBox.Items[
                MessagesListBox.Items.Count - 1];

        MessagesListBox.ScrollIntoView(lastItem);
    }

    private void MessageInputTextBox_OnPreviewKeyDown(
        object sender,
        KeyEventArgs e)
    {
        if (e.Key != Key.Enter)
        {
            return;
        }

        bool shiftPressed =
            Keyboard.Modifiers.HasFlag(
                ModifierKeys.Shift);

        if (shiftPressed)
        {
            return;
        }

        e.Handled = true;

        if (_viewModel?.SendMessageCommand.CanExecute(null)
            == true)
        {
            _viewModel.SendMessageCommand.Execute(null);
        }
    }
}