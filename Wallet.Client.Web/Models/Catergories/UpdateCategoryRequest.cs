using System.Text.Json.Serialization;

namespace Wallet.Client.Web.Models.Catergories;

public class UpdateCategoryRequest(string categoryId, string name)
{
    [JsonPropertyName("categoryId")] public string CategoryId { get; set; } = categoryId;
    [JsonPropertyName("name")] public string Name { get; set; } = name;
}