namespace Wallet.Server.Application.Models;

public record UpdateUserRequest(Guid UserId, string? Username, string? Password);