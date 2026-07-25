using System;
using System.Threading;
using System.Threading.Tasks;

namespace AikoOS.Infrastructure.Communication
{
    public interface IUnityTransport : IAsyncDisposable
    {
        bool IsConnected { get; }

        Task<bool> ConnectAsync(
            CancellationToken cancellationToken = default);

        Task SendAsync(
            string command,
            string parameter = "",
            CancellationToken cancellationToken = default);

        Task DisconnectAsync();
    }
}