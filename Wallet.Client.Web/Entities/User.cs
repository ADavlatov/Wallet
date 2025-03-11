namespace Wallet.Client.Web.Entities;

public class User
{
    public string UserId { get; set; }
    public string Username { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
}