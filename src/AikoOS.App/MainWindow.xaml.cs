using System.Windows;
using AikoOS.App.ViewModels;
using AikoOS.Live2D.Services;
using AikoOS.Core.Models;
using AikoOS.Core.Services;
using AikoOS.Behavior.Emotion;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace AikoOS.App;

public partial class MainWindow : Window
{
    private readonly ILogger<MainWindow> _logger;
    private readonly ILive2DService _live2DService;
    private readonly IEmotionEngine _emotionEngine;

    public MainWindow(
    MainWindowViewModel viewModel,
    ILogger<MainWindow> logger,
    ILive2DService live2DService,
    IEmotionEngine emotionEngine)
    {
        InitializeComponent();

        DataContext = viewModel;

        _logger = logger;
        _live2DService = live2DService;
        _emotionEngine = emotionEngine;

        Loaded += MainWindow_Loaded;
        Closed += MainWindow_Closed;

        _live2DService.ReadyChanged +=
    Live2DService_ReadyChanged;

        _logger.LogInformation(
            "MainWindow was created.");
    }

    private void MainWindow_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        _live2DService.Attach(Live2DView);

        _logger.LogInformation(
            "Live2DControl was attached to Live2DService.");
    }

    private void MainWindow_Closed(
        object? sender,
        EventArgs e)
    {
        _live2DService.ReadyChanged -=
    Live2DService_ReadyChanged;

        _live2DService.Detach(Live2DView);

        _logger.LogInformation(
            "Live2DControl was detached from Live2DService.");
    }

    private void Live2DService_ReadyChanged(
    object? sender,
    bool isReady)
    {
        Dispatcher.Invoke(() =>
        {
            Live2DStatusText.Text =
                isReady
                    ? "Live2D: Ready"
                    : "Live2D: Loading";
        });
    }
    private async void TestLive2DButton_Click(
    object sender,
    RoutedEventArgs e)
    {
        bool isReady =
            await _live2DService.WaitUntilReadyAsync(
                TimeSpan.FromSeconds(10));

        if (!isReady)
        {
            MessageBox.Show(
                "Live2D chưa sẵn sàng sau 10 giây.",
                "Aiko State Test",
                MessageBoxButton.OK,
                MessageBoxImage.Warning);

            return;
        }

        AikoAvatarState happyState = new()
        {
            Emotion = AikoEmotion.Happy,
            MotionGroup = "TapBody",
            MotionIndex = 0,
            ExpressionName = "exp_01",
            LookX = 0.7,
            LookY = 0.2
        };
    }
}