namespace Wallet.Server.Application.Models.Users;

public record AuthRequest(
    string Username, 
    string Password);