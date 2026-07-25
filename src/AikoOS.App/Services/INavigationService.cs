namespace AikoOS.App.Services;

public interface INavigationService
{
    object? CurrentViewModel { get; }

    event EventHandler? CurrentViewModelChanged;

    void NavigateTo<TViewModel>()
        where TViewModel : class;
}