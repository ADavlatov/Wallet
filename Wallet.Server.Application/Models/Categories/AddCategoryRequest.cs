using Wallet.Server.Domain.Enums;

namespace Wallet.Server.Application.Models.Categories;

public record AddCategoryRequest(
    Guid UserId, 
    string Name, 
    TransactionTypes Type);