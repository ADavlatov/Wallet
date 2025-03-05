using Wallet.Client.Web.Entities;
using Wallet.Client.Web.Interfaces;

namespace Wallet.Client.Web.Services;

public class UserService : IUserService
{
    private IHttpService _httpService;

    public UserService(IHttpService httpService)
    {
        _httpService = httpService;
    }

    public async Task<IEnumerable<User>> GetAll()
    {
        return await _httpService.Get<IEnumerable<User>>("/users");
    }
}