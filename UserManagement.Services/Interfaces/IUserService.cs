using System.Collections.Generic;
using System.Threading.Tasks;
using UserManagement.Data.DTO;
using UserManagement.Models;

namespace UserManagement.Services.Domain.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<User>> FilterByActiveAsync(bool isActive);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> CreateAsync(CreateUserDto userDto);
        Task DeleteAsync(long id);
        Task<User> UpdateAsync(long id, UpdateUserDto userDto);
    }
}
