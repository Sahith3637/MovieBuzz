using MovieBuzz.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MovieBuzz.Repository.Interfaces
{
    //public interface IUserRepository
    //{
    //    Task<User> AddUserAsync(User user);
    //    Task<User?> GetUserByUsernameAsync(string username);  // extra
    //    Task<User> GetUserByIdAsync(int userId);
    //    Task<IEnumerable<User>> GetAllUsersAsync();
    //}

    public interface IUserRepository
    {
        Task<User> AddUserAsync(User user);
        Task<User?> GetUserByUsernameAsync(string username);
        Task<User?> GetUserByPhoneAsync(string phoneNo);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(int id);
        Task<IEnumerable<User>> GetAllUsersAsync();
    }
}
