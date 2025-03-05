﻿using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models;

public class AddTransactionRequest(string userId, string categoryId, string? name, decimal amount, DateTime date, int type)
{
    [JsonPropertyName("userId")] public string UserId { get; set; } = userId;
    [JsonPropertyName("categoryId")] public string CategoryId { get; set; } = categoryId;
    [JsonPropertyName("name")] public string? Name { get; set; } = name;
    [JsonPropertyName("amount")] public decimal Amount { get; set; } = amount;
    [JsonPropertyName("date")] public DateTime Date { get; set; } = date;
    [JsonPropertyName("type")] public int Type { get; set; } = type;
}