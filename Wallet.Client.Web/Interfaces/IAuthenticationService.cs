using Wallet.Client.Web.Entities;

namespace Wallet.Client.Web.Interfaces;

public interface IAuthenticationService
{
    User User { get; }
    Task Initialize();
    Task SignIn(string username, string password);
    Task SignUp(string username, string password);
    Task SignOut();
}