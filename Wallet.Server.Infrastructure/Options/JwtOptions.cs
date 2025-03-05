using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Wallet.Server.Infrastructure.Options;

public class JwtOptions
{
    public const string Section = "JwtOptions";
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public string SecretKey { get; set; }
    public int AccessTokenLifeTimeFromDays { get; set; }
    public int RefreshTokenLifeTimeFromDays { get; set; }
    public SymmetricSecurityKey GetSymmetricSecurityKey() => new(Encoding.UTF8.GetBytes(SecretKey));
}