using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AikoOS.Infrastructure.Communication
{
    public sealed class NamedPipeUnityTransport : IUnityTransport
    {
        private const string ServerName = ".";
        private const string PipeName = "AikoOS.Unity";
        private const int ConnectionTimeoutMilliseconds = 2000;

        private readonly SemaphoreSlim connectionLock;
        private readonly SemaphoreSlim sendLock;

        private NamedPipeClientStream? pipeClient;
        private StreamWriter? writer;
        private bool isDisposed;

        public NamedPipeUnityTransport()
        {
            connectionLock = new SemaphoreSlim(1, 1);
            sendLock = new SemaphoreSlim(1, 1);
        }

        public bool IsConnected
        {
            get
            {
                return pipeClient != null &&
                       pipeClient.IsConnected;
            }
        }

        public async Task<bool> ConnectAsync(
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (IsConnected)
            {
                return true;
            }

            await connectionLock.WaitAsync(cancellationToken);

            try
            {
                if (IsConnected)
                {
                    return true;
                }

                await DisposeConnectionAsync();

                pipeClient = new NamedPipeClientStream(
                    ServerName,
                    PipeName,
                    PipeDirection.Out,
                    PipeOptions.Asynchronous
                );

                try
                {
                    await pipeClient.ConnectAsync(
                        ConnectionTimeoutMilliseconds,
                        cancellationToken
                    );
                }
                catch
                {
                    await DisposeConnectionAsync();
                    return false;
                }

                writer = new StreamWriter(
                    pipeClient,
                    new UTF8Encoding(false)
                )
                {
                    AutoFlush = true
                };

                return true;
            }
            finally
            {
                connectionLock.Release();
            }
        }

        public async Task SendAsync(
            string command,
            string parameter = "",
            CancellationToken cancellationToken = default)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(command))
            {
                throw new ArgumentException(
                    "Tên lệnh Unity không được để trống.",
                    nameof(command)
                );
            }

            bool connected = await ConnectAsync(
                cancellationToken
            );

            if (!connected || writer == null)
            {
                throw new InvalidOperationException(
                    "Không thể kết nối với Unity. " +
                    "Hãy mở Unity và bật Play Mode trước."
                );
            }

            UnityPipeMessage message = new UnityPipeMessage
            {
                Command = command,
                Parameter = parameter ?? string.Empty
            };

            string json = JsonSerializer.Serialize(message);

            await sendLock.WaitAsync(cancellationToken);

            try
            {
                try
                {
                    await writer.WriteLineAsync(
                        json.AsMemory(),
                        cancellationToken
                    );
                }
                catch
                {
                    await DisposeConnectionAsync();
                    throw;
                }
            }
            finally
            {
                sendLock.Release();
            }
        }

        public async Task DisconnectAsync()
        {
            if (isDisposed)
            {
                return;
            }

            await connectionLock.WaitAsync();

            try
            {
                await DisposeConnectionAsync();
            }
            finally
            {
                connectionLock.Release();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (isDisposed)
            {
                return;
            }

            await DisconnectAsync();

            isDisposed = true;

            connectionLock.Dispose();
            sendLock.Dispose();
        }

        private async Task DisposeConnectionAsync()
        {
            if (writer != null)
            {
                await writer.DisposeAsync();
                writer = null;
            }

            if (pipeClient != null)
            {
                await pipeClient.DisposeAsync();
                pipeClient = null;
            }
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
            {
                throw new ObjectDisposedException(
                    nameof(NamedPipeUnityTransport)
                );
            }
        }

        private sealed class UnityPipeMessage
        {
            public string Command { get; set; } =
                string.Empty;

            public string Parameter { get; set; } =
                string.Empty;
        }
    }
}