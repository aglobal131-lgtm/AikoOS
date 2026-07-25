using System.Threading;
using System.Threading.Tasks;

namespace AikoOS.Behavior.EventSources;

public interface IBehaviorEventSource
{
    Task UpdateAsync(
        CancellationToken cancellationToken = default);
}