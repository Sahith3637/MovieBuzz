using Microsoft.EntityFrameworkCore;
using MovieBuzz.Core.Entities;
using MovieBuzz.Core.Exceptions;
using MovieBuzz.Repository.Context;
using MovieBuzz.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context) => _context = context;

        public async Task<User> AddUserAsync(User user)
        {
            await _context.Users.AddAsync(user);
            return user;
        }

        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id)
                   ?? throw MovieBuzzExceptions.NotFound($"User with ID {id} not found");
        }

        public async Task<IEnumerable<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User?> GetUserByPhoneAsync(string phoneNo)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.PhoneNo == phoneNo);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.EmailId == email); 
        }

        //public async Task<bool> ToggleUserActiveStatusAsync(int userId)
        //{
        //    var user = await _context.Users.FindAsync(userId);
        //    if (user == null) return false;

        //    user.IsActive = !user.IsActive;
        //    return true;
        //}
    }
}