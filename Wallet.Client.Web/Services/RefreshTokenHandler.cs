using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Wallet.Client.Web.Entities;
using Wallet.Client.Web.Interfaces;
using Wallet.Client.Web.Models.Users;

namespace Wallet.Client.Web.Services;

public class RefreshTokenHandler(
    ILocalStorageService localStorageService,
    IHttpClientFactory httpClientFactory,
    NavigationManager navigationManager) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            var user = await localStorageService.GetItem<User>("user");
            var httpClient = httpClientFactory.CreateClient("MainClient");
            var refreshTokenRequest = new HttpRequestMessage(HttpMethod.Post, "api/v1/users/RefreshTokens");
            refreshTokenRequest.Content = new StringContent(
                JsonSerializer.Serialize(new RefreshTokenRequest(user.RefreshToken)),
                System.Text.Encoding.UTF8, "application/json");

            var refreshTokenResponse = await httpClient.SendAsync(refreshTokenRequest, cancellationToken);
            if (refreshTokenResponse.IsSuccessStatusCode)
            {
                var newTokens = await refreshTokenResponse.Content
                    .ReadFromJsonAsync<User>(cancellationToken: cancellationToken);
                if (newTokens != null)
                {
                    user.AccessToken = newTokens.AccessToken;
                    user.RefreshToken = newTokens.RefreshToken;
                    await localStorageService.SetItem("user", user);

                    var newRequest = new HttpRequestMessage(request.Method, request.RequestUri);
                    foreach (var header in request.Headers)
                    {
                        newRequest.Headers.TryAddWithoutValidation(header.Key, header.Value);
                    }

                    if (request.Content != null)
                    {
                        var contentBytes = await request.Content.ReadAsByteArrayAsync(cancellationToken);
                        newRequest.Content = new ByteArrayContent(contentBytes);
                        newRequest.Content.Headers.ContentType = request.Content.Headers.ContentType;
                    }

                    newRequest.Headers.Authorization =
                        new AuthenticationHeaderValue("Bearer", newTokens.AccessToken);

                    return await base.SendAsync(newRequest, cancellationToken);
                }
            }

            navigationManager.NavigateTo("/signout");
        }

        return response;
    }
}