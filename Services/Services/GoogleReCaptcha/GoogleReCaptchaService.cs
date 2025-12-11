using Common;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using Newtonsoft.Json;

using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Services.Services.GoogleReCaptcha;

public class GoogleReCaptchaService : IGoogleReCaptchaService, ITransientDependency
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    public GoogleReCaptchaService(IHttpClientFactory httpClientFactory,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public async Task<bool> IsVerifyCaptcha(CancellationToken cancellationToken)
    {
        var secretKey = _configuration.GetSection("GoogleReCaptcha:SecretKey").Value;
        var response = _httpContextAccessor.HttpContext.Request.Form["g-recaptcha-response"];

        var client = _httpClientFactory.CreateClient();
        var result = await client.PostAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secretKey}&response={response}", null);

        var googleResponse = JsonConvert.DeserializeObject<ReCaptchaResponse>(await result.Content.ReadAsStringAsync(cancellationToken));

        return googleResponse.IsSuccess;
    }
}