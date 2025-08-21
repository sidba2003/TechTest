using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations
{
    public class UserService : IUserService
    {
        private readonly IDataContext _dataAccess;
        public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

        public async Task<IEnumerable<User>> FilterByActiveAsync(bool isActive)
            => await _dataAccess.GetAll<User>().Where(u => u.IsActive == isActive).ToListAsync();

        public async Task<IEnumerable<User>> GetAllAsync()
            => await _dataAccess.GetAll<User>().ToListAsync();

        public async Task<User> CreateAsync(CreateUserDto userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));

            var newUser = new User
            {
                Email = userDto.Email,
                Surname = userDto.Surname,
                Forename = userDto.Forename,
                DateOfBirth = userDto.DateOfBirth,
                IsActive = userDto.IsActive
            };

            await _dataAccess.CreateAsync(newUser);

            await LogAuditAsync(newUser, "CREATE", null, JsonSerializer.Serialize(newUser));

            return newUser;
        }

        public async Task DeleteAsync(long id)
        {
            var user = await _dataAccess.GetAll<User>().FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) throw new KeyNotFoundException($"User with Id {id} not found");

            await LogAuditAsync(user, "DELETE", JsonSerializer.Serialize(user), null);

            await _dataAccess.DeleteAsync(user);
        }

        public async Task<User> UpdateAsync(long id, UpdateUserDto userDto)
        {
            if (userDto == null) throw new ArgumentNullException(nameof(userDto));

            var existingUser = await _dataAccess.GetAll<User>().FirstOrDefaultAsync(u => u.Id == id);
            if (existingUser == null) throw new KeyNotFoundException($"User with Id {id} not found");

            var before = JsonSerializer.Serialize(existingUser);

            existingUser.Email = userDto.Email;
            existingUser.Surname = userDto.Surname;
            existingUser.Forename = userDto.Forename;
            existingUser.DateOfBirth = userDto.DateOfBirth;
            existingUser.IsActive = userDto.IsActive;

            await _dataAccess.UpdateAsync(existingUser);

            var after = JsonSerializer.Serialize(existingUser);
            await LogAuditAsync(existingUser, "UPDATE", before, after);

            return existingUser;
        }

        private async Task LogAuditAsync(User user, string operation, string? before, string? after)
        {
            var audit = new UserLogs
            {
                UserId = user.Id,
                Operation = operation,
                DataBefore = before,
                DataAfter = after
            };

            await _dataAccess.CreateAsync(audit);
        }
    }
}
