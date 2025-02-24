namespace Wallet.Server.Application.Models;

public record UpdateCategoryRequest(Guid CategoryId, string? Name, string? Type);