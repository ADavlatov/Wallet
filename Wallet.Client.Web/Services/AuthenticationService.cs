using Microsoft.AspNetCore.Components;
using Wallet.Client.Web.Entities;
using Wallet.Client.Web.Interfaces;

namespace Wallet.Client.Web.Services;

public class AuthenticationService : IAuthenticationService
{
    private IHttpService _httpService;
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;

    public User User { get; private set; }

    public AuthenticationService(
        IHttpService httpService,
        NavigationManager navigationManager,
        ILocalStorageService localStorageService
    ) {
        _httpService = httpService;
        _navigationManager = navigationManager;
        _localStorageService = localStorageService;
    }

    public async Task Initialize()
    {
        User = await _localStorageService.GetItem<User>("user");
    }

    public async Task SignIn(string username, string password)
    {
        User = await _httpService.Post<User>("/api/v1/users/SignIn", new { username, password });
        await _localStorageService.SetItem("user", User);
    }

    public async Task SignUp(string username, string password)
    {
        User = await _httpService.Post<User>("/api/v1/users/SignUp", new { username, password });
        await _localStorageService.SetItem("user", User);
    }

    public async Task SignOut()
    {
        User = null;
        await _localStorageService.RemoveItem("user");
        _navigationManager.NavigateTo("", true);
    }
}