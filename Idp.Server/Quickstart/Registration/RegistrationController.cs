using IdentityModel;
using IdentityServer4;
using IdentityServer4.Services;
using Idp.Server.Entities;
using Idp.Server.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Idp.Server.Quickstart.Registration
{
    public class RegistrationController : Controller
    {
        private readonly IUserService _userService;
        private readonly IIdentityServerInteractionService _interactionService;

        public RegistrationController(IUserService userService, IIdentityServerInteractionService interactionService)
        {
            _userService = userService;
            _interactionService = interactionService;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userToCreate = new User
            {
                Username = model.UserName,
                Subject = Guid.NewGuid().ToString(),
                Email = model.Email
            };

            userToCreate.Claims.Add(new UserClaim()
            {
                Type = JwtClaimTypes.GivenName,
                Value = model.FirstName
            });

            userToCreate.Claims.Add(new UserClaim()
            {
                Type = JwtClaimTypes.FamilyName,
                Value = model.LastName
            });

            userToCreate.Claims.Add(new UserClaim()
            {
                Type = JwtClaimTypes.Email,
                Value = model.Email
            });

            await _userService.AddUserAsync(userToCreate, model.Password);

            var issuer = new IdentityServerUser(userToCreate.Subject);
            await HttpContext.SignInAsync(issuer);

            if (_interactionService.IsValidReturnUrl(model.ReturnUrl) || Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return Redirect("~/");
        }

        [HttpGet]
        public async Task<IActionResult> Register(string returnUrl)
        {
            var vm = new RegistrationViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(vm);
        }
    }
}
