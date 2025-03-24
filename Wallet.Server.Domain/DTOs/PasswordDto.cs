namespace Wallet.Server.Domain.DTOs;

public record PasswordDto(byte[] PasswordHash, byte[] PasswordSalt);