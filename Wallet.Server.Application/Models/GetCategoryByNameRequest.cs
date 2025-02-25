namespace Wallet.Server.Application.Models;

public record GetCategoryByNameRequest(Guid UserId, string Name);