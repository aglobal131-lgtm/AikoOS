using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace AikoOS.Infrastructure.Communication
{
    public sealed class UnityCommandService
    {
        private readonly IUnityTransport unityTransport;

        public UnityCommandService(
            IUnityTransport unityTransport)
        {
            this.unityTransport = unityTransport;
        }

        public bool IsConnected
        {
            get
            {
                return unityTransport.IsConnected;
            }
        }

        public Task<bool> ConnectAsync(
            CancellationToken cancellationToken = default)
        {
            return unityTransport.ConnectAsync(
                cancellationToken
            );
        }

        public Task WalkLeftAsync(
            CancellationToken cancellationToken = default)
        {
            return unityTransport.SendAsync(
                "WalkLeft",
                string.Empty,
                cancellationToken
            );
        }

        public Task WalkRightAsync(
            CancellationToken cancellationToken = default)
        {
            return unityTransport.SendAsync(
                "WalkRight",
                string.Empty,
                cancellationToken
            );
        }

        public Task StopAsync(
            CancellationToken cancellationToken = default)
        {
            return unityTransport.SendAsync(
                "Stop",
                string.Empty,
                cancellationToken
            );
        }

        public Task WalkToAsync(
            float worldX,
            CancellationToken cancellationToken = default)
        {
            string parameter = worldX.ToString(
                CultureInfo.InvariantCulture
            );

            return unityTransport.SendAsync(
                "WalkTo",
                parameter,
                cancellationToken
            );
        }

        public Task DisconnectAsync()
        {
            return unityTransport.DisconnectAsync();
        }
    }
}