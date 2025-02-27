namespace Wallet.Server.Application.Models.Categories;

public record GetCategoriesByTypeRequest(Guid UserId, string Type);