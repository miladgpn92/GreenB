using System;

namespace Common
{
    public class ProjectSettings
    {
        public JwtSettings JwtSettings { get; set; }
        public IdentitySettings IdentitySettings { get; set; }
        public PayPingConfiguration PayPing { get; set; }
        public ProjectSetting ProjectSetting { get; set; }
    }

    public class PayPingConfiguration
    {
        public string PayPingReturnUrl { get; set; }
        public string PayPingBearerToken { get; set; }
    }

    public class IdentitySettings
    {
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanumic { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public bool RequireUniqueEmail { get; set; }
    }
    public class JwtSettings
    {
        public string SecretKey { get; set; }
        public string Encryptkey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public int NotBeforeMinutes { get; set; }
        public int ExpirationMinutes { get; set; }
    }

  
    public class ProjectSetting
    {
        public bool IsPhonenumberAuthEnable { get; set; }

        public bool IsEmailAuthEnable { get; set; }

        public string BaseUrl { get; set; }

        public string SMSToken { get; set; }
    }
}
