using Wallet.Client.Web.Entities;
using Wallet.Client.Web.Interfaces;

namespace Wallet.Client.Web.Services;

public class UserService(IHttpService httpService) : IUserService
{
    public async Task<IEnumerable<User>> GetAll()
    {
        return await httpService.Get<IEnumerable<User>>("/users");
    }
}