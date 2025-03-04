using Wallet.Client.Web.Entities;

namespace Wallet.Client.Web.Interfaces;

public interface IUserService
{
    Task<IEnumerable<User>> GetAll();
}