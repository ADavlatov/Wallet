namespace Wallet.Server.Domain.DTOs;

public record AuthDto(string AccessToken, string RefreshToken, string UserId);