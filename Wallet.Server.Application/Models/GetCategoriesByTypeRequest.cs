namespace Wallet.Server.Application.Models;

public record GetCategoriesByTypeRequest(Guid UserId, string Type);