namespace Wallet.Server.Application.Models.Users;

public record TokensResponse(
    string AccessToken, 
    string RefreshToken);