using Idp.Server.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Idp.Server.Services
{
    public interface IUserService
    {
        Task<bool> ValidateCredentials(string username, string password);
        Task<User> FindByUsername(string username);
        Task<IEnumerable<UserClaim>> GetClaimBySubjectId(string subjectId);
        Task AddUserAsync(User user, string password);
    }
}
