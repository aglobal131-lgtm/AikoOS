using System.Threading;
using System.Threading.Tasks;

namespace AikoOS.Behavior.Dispatchers;

public interface IActionDispatcher
{
    Task DispatchAsync(
        CancellationToken cancellationToken = default);
}