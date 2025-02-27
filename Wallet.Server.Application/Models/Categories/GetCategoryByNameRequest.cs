namespace Wallet.Server.Application.Models.Categories;

public record GetCategoryByNameRequest(Guid UserId, string Name);