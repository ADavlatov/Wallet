namespace Wallet.Server.Application.Models;

public record AddCategoryRequest(Guid UserId, string Name, string Type);