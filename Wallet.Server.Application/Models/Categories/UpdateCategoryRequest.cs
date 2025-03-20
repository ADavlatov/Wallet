namespace Wallet.Server.Application.Models.Categories;

public record UpdateCategoryRequest(Guid CategoryId, string? Name);