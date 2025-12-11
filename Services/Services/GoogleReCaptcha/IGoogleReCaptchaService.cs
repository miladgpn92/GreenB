using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.GoogleReCaptcha;
public interface IGoogleReCaptchaService
{
    Task<bool> IsVerifyCaptcha(CancellationToken cancellationToken);
}
