using Idp.Server.Entities;
using System.Threading.Tasks;

namespace Idp.Server.Services
{
    public interface IUserService
    {
        Task<bool> ValidateCredentials(string username, string password);
        Task<User> FindByUsername(string username);
    }
}
