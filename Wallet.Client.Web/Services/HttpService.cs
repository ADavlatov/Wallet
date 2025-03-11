using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Wallet.Client.Web.Entities;
using Wallet.Client.Web.Interfaces;

namespace Wallet.Client.Web.Services;

public class HttpService : IHttpService
{
    private HttpClient _httpClient;
    private NavigationManager _navigationManager;
    private ILocalStorageService _localStorageService;
    private IConfiguration _configuration;

    public HttpService(
        HttpClient httpClient,
        NavigationManager navigationManager,
        ILocalStorageService localStorageService,
        IConfiguration configuration
    )
    {
        _httpClient = httpClient;
        _navigationManager = navigationManager;
        _localStorageService = localStorageService;
        _configuration = configuration;
    }

    public async Task<T> Get<T>(string uri)
    {
        var request = new HttpRequestMessage(HttpMethod.Get, uri);
        return await SendRequest<T>(request);
    }

    public async Task<T> Post<T>(string uri, object value)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, uri);
        request.Content = new StringContent(JsonSerializer.Serialize(value), Encoding.UTF8, "application/json");
        return await SendRequest<T>(request);
    }

    // helper methods

    private async Task<T> SendRequest<T>(HttpRequestMessage request)
    {
        var user = await _localStorageService.GetItem<User>("user");
        var isApiUrl = !request.RequestUri.IsAbsoluteUri;
        if (user != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer ", user.AccessToken);

        using var response = await _httpClient.SendAsync(request);

        Console.WriteLine(response.StatusCode);
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            await RefreshToken(user);
        }

        if (!response.IsSuccessStatusCode)
        {
            var error = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
            throw new Exception(error["message"]);
        }

        return await response.Content.ReadFromJsonAsync<T>();
    }
    
    private async Task RefreshToken(User user)
    {
        var response = await _httpClient.PostAsJsonAsync("/api/v1/users/RefreshToken", user.RefreshToken);

        if (response.IsSuccessStatusCode)
        {
            user = await response.Content.ReadFromJsonAsync<User>();
            await _localStorageService.SetItem("user", user);
        }
        else
        {
            _navigationManager.NavigateTo("signout");
        }
    }
}