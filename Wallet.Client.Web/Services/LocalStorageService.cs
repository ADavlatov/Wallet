﻿using System.Text.Json;
using Microsoft.JSInterop;
using Wallet.Client.Web.Interfaces;

namespace Wallet.Client.Web.Services;

public class LocalStorageService : ILocalStorageService
{
    private IJSRuntime _jsRuntime;

    public LocalStorageService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task<T> GetItem<T>(string key)
    {
        var json = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", key);

        if (json == null)
            return default;

        return JsonSerializer.Deserialize<T>(json);
    }

    public async Task SetItem<T>(string key, T value)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.setItem", key, JsonSerializer.Serialize(value));
    }

    public async Task RemoveItem(string key)
    {
        await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", key);
    }
}