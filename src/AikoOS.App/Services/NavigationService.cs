using Microsoft.Extensions.DependencyInjection;

namespace AikoOS.App.Services;

public sealed class NavigationService : INavigationService
{
    private readonly IServiceProvider _serviceProvider;

    public NavigationService(
        IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public object? CurrentViewModel { get; private set; }

    public event EventHandler? CurrentViewModelChanged;

    public void NavigateTo<TViewModel>()
        where TViewModel : class
    {
        CurrentViewModel =
            _serviceProvider.GetRequiredService<TViewModel>();

        CurrentViewModelChanged?.Invoke(
            this,
            EventArgs.Empty);
    }
}