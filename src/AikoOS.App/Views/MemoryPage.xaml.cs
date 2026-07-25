using System.Windows;
using System.Windows.Controls;
using AikoOS.App.ViewModels;

namespace AikoOS.App.Views;

public partial class MemoryPage : UserControl
{
    private bool _hasLoaded;

    public MemoryPage()
    {
        InitializeComponent();

        Loaded += MemoryPage_Loaded;
    }

    private async void MemoryPage_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        if (_hasLoaded)
        {
            return;
        }

        _hasLoaded = true;

        if (DataContext is MemoryViewModel viewModel)
        {
            await viewModel.LoadCommand.ExecuteAsync(null);
        }
    }
}