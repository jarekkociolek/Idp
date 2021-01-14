using Ardalis.GuardClauses;
using Idp.Server.DbContexts;
using Idp.Server.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Idp.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IdentityDbContext _context;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IdentityDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task AddUserAsync(User user, string password)
        {
            Guard.Against.Null(user, nameof(user));
            Guard.Against.Null(password, nameof(password));

            if (await _context.Users.AnyAsync(q => q.Username == user.Username))
            {
                throw new Exception("Username already in use");
            }

            if (await _context.Users.AnyAsync(q => q.Email == user.Email))
            {
                throw new Exception("Email already in use");
            }

            user.Password = _passwordHasher.HashPassword(user, password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User> FindByUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username))
            {
                throw new ArgumentNullException(nameof(username));
            }

            return await _context.Users
                 .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<UserClaim>> GetClaimBySubjectId(string subjectId)
        {
            if (string.IsNullOrWhiteSpace(subjectId))
            {
                throw new ArgumentNullException(nameof(subjectId));
            }

            var user = await _context.Users
                .Include(q => q.Claims)
                .FirstOrDefaultAsync(u => u.Subject == subjectId);

            return user.Claims;
        }

        public async Task<bool> ValidateCredentials(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) ||
                            string.IsNullOrWhiteSpace(password))
            {
                return false;
            }

            var user = await FindByUsername(username);

            if (user == null)
            {
                return false;
            }

            var result = _passwordHasher.VerifyHashedPassword(user, user.Password, password);

            return result == PasswordVerificationResult.Success;
        }
    }
}
