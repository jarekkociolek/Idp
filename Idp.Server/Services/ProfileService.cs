using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Idp.Server.Services
{
    public class ProfileService : IProfileService
    {
        private readonly IUserService _userService;

        public ProfileService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var subjectId = context.Subject.GetSubjectId();
            var claimsForUser = (await _userService.GetClaimBySubjectId(subjectId)).ToList();

            context.IssuedClaims.AddRange(claimsForUser.Select(c => new Claim(c.Type, c.Value)).ToList());
        }

        public Task IsActiveAsync(IsActiveContext context)
        {
            return Task.FromResult(true);
        }
    }
}
