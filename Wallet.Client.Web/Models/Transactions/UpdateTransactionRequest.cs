using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Transactions;

public class UpdateTransactionRequest(string transactionId, string categoryId, string? name, decimal amount, DateOnly date)
{
    [JsonPropertyName("transactionId")] public string TransactionId { get; set; } = transactionId;
    [JsonPropertyName("categoryId")] public string CategoryId { get; set; } = categoryId;
    [JsonPropertyName("name")] public string? Name { get; set; } = name;
    [JsonPropertyName("amount")] public decimal Amount { get; set; } = amount;
    [JsonPropertyName("date")] public DateOnly Date { get; set; } = date;
}