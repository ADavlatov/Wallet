namespace Wallet.Server.Application.Models.Users;

public record ValidateApiKeyRequest(string ApiKey, long telegramUserId);