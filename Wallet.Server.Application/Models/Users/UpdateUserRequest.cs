namespace Wallet.Server.Application.Models.Users;

public record UpdateUserRequest(Guid UserId, string? Username, string? Password);