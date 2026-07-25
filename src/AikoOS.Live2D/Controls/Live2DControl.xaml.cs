using Microsoft.Web.WebView2.Core;
using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace AikoOS.Live2D.Controls;

public partial class Live2DControl : UserControl
{
    private const string VirtualHostName = "aiko.live2d";

    private bool _isInitialized;
    private bool _isLive2DReady;

    public bool IsReady => _isLive2DReady;
    public event EventHandler<bool>? ReadyChanged;

    public Live2DControl()
    {
        InitializeComponent();

        Loaded += Live2DControl_Loaded;
        Unloaded += Live2DControl_Unloaded;
    }

    private async void Live2DControl_Loaded(
        object sender,
        RoutedEventArgs e)
    {
        if (_isInitialized)
        {
            return;
        }

        _isInitialized = true;

        try
        {
            await InitializeWebViewAsync();
        }
        catch (Exception exception)
        {
            _isInitialized = false;
            SetReadyState(false);

            MessageBox.Show(
                exception.ToString(),
                "Lỗi Live2D",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
    private void Live2DControl_Unloaded(
    object sender,
    RoutedEventArgs e)
    {
        SetReadyState(false);
    }

    /// <summary>
    /// Khởi tạo WebView2 và mở trang Live2D.
    /// </summary>
    private async Task InitializeWebViewAsync()
    {
        await Browser.EnsureCoreWebView2Async();

        // Chỉ dùng khi đang phát triển.
        // Có thể xóa dòng này khi không cần DevTools nữa.
        // Browser.CoreWebView2.OpenDevToolsWindow();

        string webFolder = Path.Combine(
            AppContext.BaseDirectory,
            "Web");

        if (!Directory.Exists(webFolder))
        {
            throw new DirectoryNotFoundException(
                $"Không tìm thấy thư mục Web: {webFolder}");
        }

        string indexFile = Path.Combine(
            webFolder,
            "index.html");

        if (!File.Exists(indexFile))
        {
            throw new FileNotFoundException(
                "Không tìm thấy index.html.",
                indexFile);
        }

        Browser.CoreWebView2.SetVirtualHostNameToFolderMapping(
            VirtualHostName,
            webFolder,
            CoreWebView2HostResourceAccessKind.Allow);

        Browser.CoreWebView2.NavigationCompleted +=
            CoreWebView2_NavigationCompleted;

        Browser.Source = new Uri(
            $"https://{VirtualHostName}/index.html");
    }

    /// <summary>
    /// Được gọi sau khi WebView2 tải trang xong.
    /// </summary>
    private async void CoreWebView2_NavigationCompleted(
        object? sender,
        CoreWebView2NavigationCompletedEventArgs e)
    {
        if (!e.IsSuccess)
        {
            SetReadyState(false);

            MessageBox.Show(
                $"Không thể tải giao diện Live2D.\n" +
                $"WebErrorStatus: {e.WebErrorStatus}",
                "Lỗi Live2D",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            return;
        }

        try
        {
            bool ready = await WaitForLive2DReadyAsync(
                timeoutSeconds: 20);

            if (!ready)
            {
                SetReadyState(false);

                MessageBox.Show(
                    "Trang đã được mở nhưng AikoBridge hoặc model " +
                    "Live2D chưa sẵn sàng sau 20 giây.",
                    "Lỗi Live2D",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            SetReadyState(true);

            Console.WriteLine(
                "[Live2DControl] AikoBridge và model đã sẵn sàng.");

            // await RunConnectionTestAsync();
        }
        catch (Exception exception)
        {
            SetReadyState(false);

            MessageBox.Show(
                exception.ToString(),
                "Lỗi kiểm tra Live2D",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }

    /// <summary>
    /// Chờ đến khi window.Aiko tồn tại và model đã tải motion.
    ///
    /// Đây là cơ chế kiểm tra liên tục có timeout,
    /// không phải chờ cố định rồi hy vọng model đã tải xong.
    /// </summary>
    private async Task<bool> WaitForLive2DReadyAsync(
        int timeoutSeconds)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();

        while (stopwatch.Elapsed <
               TimeSpan.FromSeconds(timeoutSeconds))
        {
            string? result = await ExecuteScriptAsync(
                """
                (() => {
                    if (
                        !window.Aiko ||
                        typeof window.Aiko.playMotion !== "function" ||
                        typeof window.Aiko.setExpression !== "function" ||
                        typeof window.Aiko.lookAt !== "function" ||
                        typeof window.Aiko.getMotionGroups !== "function" ||
                        typeof window.Aiko.getExpressions !== "function"
                    ) {
                        return false;
                    }

                    const motionGroups =
                        window.Aiko.getMotionGroups();

                    return Array.isArray(motionGroups) &&
                           motionGroups.length > 0;
                })()
                """);

            if (IsJavaScriptTrue(result))
            {
                return true;
            }

            await Task.Delay(100);
        }

        return false;
    }

    /// <summary>
    /// Chạy kiểm tra điều khiển Live2D từ C#.
    /// </summary>
    private async Task RunConnectionTestAsync()
    {
        string[] motionGroups =
            await GetMotionGroupsAsync();

        string[] expressions =
            await GetExpressionsAsync();

        Console.WriteLine(
            $"[Live2D Test] Motion groups: " +
            $"{string.Join(", ", motionGroups)}");

        Console.WriteLine(
            $"[Live2D Test] Expressions: " +
            $"{string.Join(", ", expressions)}");

        bool motionResult =
            await PlayMotionAsync("TapBody", 0);

        Console.WriteLine(
            $"[Live2D Test] PlayMotion TapBody[0]: " +
            $"{motionResult}");

        bool expressionResult =
            await SetExpressionAsync("exp_01");

        Console.WriteLine(
            $"[Live2D Test] SetExpression exp_01: " +
            $"{expressionResult}");

        bool lookAtResult =
            await LookAtAsync(1.0, 0.0);

        Console.WriteLine(
            $"[Live2D Test] LookAt(1, 0): " +
            $"{lookAtResult}");
    }

    /// <summary>
    /// Chạy một lệnh JavaScript trong WebView2.
    /// </summary>
    private async Task<string?> ExecuteScriptAsync(
        string script)
    {
        if (Browser.CoreWebView2 is null)
        {
            Console.WriteLine(
                "[Live2DControl] WebView2 chưa sẵn sàng.");

            return null;
        }

        return await Browser.CoreWebView2.ExecuteScriptAsync(
            script);
    }

    /// <summary>
    /// Phát motion của model Live2D.
    /// </summary>
    public async Task<bool> PlayMotionAsync(
        string group,
        int index)
    {
        if (!_isLive2DReady)
        {
            Console.WriteLine(
                "[Live2DControl] Không thể phát motion: " +
                "model chưa sẵn sàng.");

            return false;
        }

        if (string.IsNullOrWhiteSpace(group))
        {
            return false;
        }

        if (index < 0)
        {
            return false;
        }

        string safeGroup =
            JsonSerializer.Serialize(group);

        string script =
            $"window.Aiko.playMotion({safeGroup}, {index})";

        string? result =
            await ExecuteScriptAsync(script);

        return IsJavaScriptTrue(result);
    }

    /// <summary>
    /// Thay đổi expression của model Live2D.
    /// </summary>
    public async Task<bool> SetExpressionAsync(
        string expressionName)
    {
        if (!_isLive2DReady)
        {
            Console.WriteLine(
                "[Live2DControl] Không thể đổi expression: " +
                "model chưa sẵn sàng.");

            return false;
        }

        if (string.IsNullOrWhiteSpace(expressionName))
        {
            return false;
        }

        string safeExpression =
            JsonSerializer.Serialize(expressionName);

        string script =
            $"window.Aiko.setExpression({safeExpression})";

        string? result =
            await ExecuteScriptAsync(script);

        return IsJavaScriptTrue(result);
    }

    /// <summary>
    /// Điều khiển hướng nhìn của model.
    ///
    /// X và Y được giới hạn trong khoảng -1 đến 1.
    /// </summary>
    public async Task<bool> LookAtAsync(
        double x,
        double y)
    {
        if (!_isLive2DReady)
        {
            Console.WriteLine(
                "[Live2DControl] Không thể điều khiển hướng nhìn: " +
                "model chưa sẵn sàng.");

            return false;
        }

        if (double.IsNaN(x) ||
            double.IsInfinity(x) ||
            double.IsNaN(y) ||
            double.IsInfinity(y))
        {
            return false;
        }

        double normalizedX =
            Math.Clamp(x, -1.0, 1.0);

        double normalizedY =
            Math.Clamp(y, -1.0, 1.0);

        string xValue = normalizedX.ToString(
            CultureInfo.InvariantCulture);

        string yValue = normalizedY.ToString(
            CultureInfo.InvariantCulture);

        string script =
            $"window.Aiko.lookAt({xValue}, {yValue})";

        string? result =
            await ExecuteScriptAsync(script);

        return IsJavaScriptTrue(result);
    }

    /// <summary>
    /// Lấy danh sách motion group từ model.
    /// </summary>
    public async Task<string[]> GetMotionGroupsAsync()
    {
        if (!_isLive2DReady)
        {
            return [];
        }

        string? result = await ExecuteScriptAsync(
            "window.Aiko.getMotionGroups()");

        return DeserializeStringArray(result);
    }

    /// <summary>
    /// Lấy danh sách expression từ model.
    /// </summary>
    public async Task<string[]> GetExpressionsAsync()
    {
        if (!_isLive2DReady)
        {
            return [];
        }

        string? result = await ExecuteScriptAsync(
            "window.Aiko.getExpressions()");

        return DeserializeStringArray(result);
    }


    private void SetReadyState(bool isReady)
    {
        if (_isLive2DReady == isReady)
        {
            return;
        }

        _isLive2DReady = isReady;

        ReadyChanged?.Invoke(
            this,
            isReady);

        Console.WriteLine(
            $"[Live2DControl] Ready state changed: {isReady}");
    }
    /// <summary>
    /// Kiểm tra kết quả boolean trả về từ JavaScript.
    /// </summary>
    private static bool IsJavaScriptTrue(
        string? result)
    {
        return string.Equals(
            result?.Trim(),
            "true",
            StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Chuyển JSON array từ JavaScript thành string array.
    /// </summary>
    private static string[] DeserializeStringArray(
        string? json)
    {
        if (string.IsNullOrWhiteSpace(json) ||
            string.Equals(
                json,
                "null",
                StringComparison.OrdinalIgnoreCase))
        {
            return [];
        }

        try
        {
            return JsonSerializer.Deserialize<string[]>(json)
                   ?? [];
        }
        catch (JsonException exception)
        {
            Console.WriteLine(
                "[Live2DControl] Không thể đọc danh sách JSON: " +
                exception.Message);

            return [];
        }
    }
}