
using Newtonsoft.Json;

using System;

namespace Services.Services.GoogleReCaptcha;

public class ReCaptchaResponse
{
    [JsonProperty("success")]
    public bool IsSuccess { get; set; }

    [JsonProperty("challenge_ts")]
    public DateTimeOffset Challenge { get; set; }

    [JsonProperty("hostname")]
    public string HostName { get; set; }
}