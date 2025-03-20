using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Wallet.Client.Web.Entities;
using Wallet.Client.Web.Interfaces;

namespace Wallet.Client.Web.Services;

public class HttpService(
    HttpClient httpClient,
    NavigationManager navigationManager,
    ILocalStorageService localStorageService,
    IConfiguration configuration) : IHttpService
{
    private IConfiguration _configuration = configuration;

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
        var user = await localStorageService.GetItem<User>("user");
        var isApiUrl = !request.RequestUri.IsAbsoluteUri;
        if (user != null && isApiUrl)
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer ", user.AccessToken);

        using var response = await httpClient.SendAsync(request);

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
        var response = await httpClient.PostAsJsonAsync("/api/v1/users/RefreshToken", user.RefreshToken);

        if (response.IsSuccessStatusCode)
        {
            user = await response.Content.ReadFromJsonAsync<User>();
            await localStorageService.SetItem("user", user);
        }
        else
        {
            navigationManager.NavigateTo("signout");
        }
    }
}