namespace BoutiqueEducation.Business.Settings;

// Options Pattern için sınıfımız. appsettings.json'dan bu sınıfa verileri bağlayacağız.
public sealed class JwtSettings
{
    public string SecretKey { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryInMinutes { get; set; }
}
