﻿using Idp.Server.DbContexts;
using Idp.Server.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Idp.Server.Services
{
    public class UserService : IUserService
    {
        private readonly IdentityDbContext _context;

        public UserService(IdentityDbContext context)
        {
            _context = context;
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

        public async Task<bool> ValidateCredentials(string username, string password)
        {
            //TODO: use password hasher
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

            return (user.Password == password);
        }
    }
}