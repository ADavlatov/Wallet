using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Application.Models.Categories;

public record GetCategoriesByTypeRequest(Guid UserId, TransactionTypes Type);