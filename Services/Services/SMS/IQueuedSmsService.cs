using System.Threading;
using System.Threading.Tasks;

namespace Services.Services
{
    public interface IQueuedSmsService
    {
        Task EnqueueAsync(string phone, string text, CancellationToken cancellationToken);
    }
}
