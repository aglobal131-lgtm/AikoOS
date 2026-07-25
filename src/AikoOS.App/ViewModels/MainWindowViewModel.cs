using AikoOS.App.Options;
using AikoOS.App.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Options;

namespace AikoOS.App.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    private readonly INavigationService _navigationService;

    public MainWindowViewModel(
        IOptions<ApplicationOptions> applicationOptions,
        INavigationService navigationService)
    {
        ApplicationOptions options =
            applicationOptions.Value;

        ApplicationName = options.Name;
        ApplicationVersion = options.Version;
        EnvironmentName = options.Environment;

        _navigationService = navigationService;

        _navigationService.CurrentViewModelChanged +=
            OnCurrentViewModelChanged;

        _navigationService.NavigateTo<HomeViewModel>();
    }

    public string ApplicationName { get; }

    public string ApplicationVersion { get; }

    public string EnvironmentName { get; }

    public object? CurrentViewModel =>
        _navigationService.CurrentViewModel;

    [RelayCommand]
    private void NavigateHome()
    {
        _navigationService.NavigateTo<HomeViewModel>();
    }

    [RelayCommand]
    private void NavigateChat()
    {
        _navigationService.NavigateTo<ChatViewModel>();
    }

    [RelayCommand]
    private void NavigateSettings()
    {
        _navigationService.NavigateTo<SettingsViewModel>();
    }

    [RelayCommand]
    private void NavigateMemory()
    {
        _navigationService.NavigateTo<MemoryViewModel>();
    }

    private void OnCurrentViewModelChanged(
        object? sender,
        EventArgs e)
    {
        OnPropertyChanged(nameof(CurrentViewModel));
    }
}