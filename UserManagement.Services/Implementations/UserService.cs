using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using UserManagement.Data;
using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.Services.Domain.Implementations;

public class UserService : IUserService
{
    private readonly IDataContext _dataAccess;
    public UserService(IDataContext dataAccess) => _dataAccess = dataAccess;

    /// <summary>
    /// Return users by active state
    /// </summary>
    /// <param name="isActive"></param>
    /// <returns></returns>
    public IEnumerable<User> FilterByActive(bool isActive)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<User> GetAll() => _dataAccess.GetAll<User>();

    public User Create(CreateUserDto userDto)
    {
        if (userDto == null)
            throw new ArgumentNullException(nameof(userDto));

        var newUser = new User
        {
            Email = userDto.Email,
            Surname = userDto.Surname,
            Forename = userDto.Forename,
            DateOfBirth = userDto.DateOfBirth,
            IsActive = userDto.IsActive
        };

        _dataAccess.Create(newUser);

        LogAudit(newUser, "CREATE", null, JsonSerializer.Serialize(newUser));

        return newUser;
    }

    public void Delete(long id)
    {
        var user = _dataAccess.GetAll<User>().FirstOrDefault(u => u.Id == id);
        if (user == null) throw new KeyNotFoundException($"User with Id {id} not found");

        LogAudit(user, "DELETE", JsonSerializer.Serialize(user), null);

        _dataAccess.Delete(user);
    }

    public User Update(long id, UpdateUserDto userDto)
    {
        if (userDto == null)
            throw new ArgumentNullException(nameof(userDto));

        var existingUser = _dataAccess.GetAll<User>().FirstOrDefault(u => u.Id == id);
        if (existingUser == null)
            throw new KeyNotFoundException($"User with Id {id} not found");

        var dataBefore = JsonSerializer.Serialize(existingUser);

        // Update fields
        existingUser.Email = userDto.Email;
        existingUser.Surname = userDto.Surname;
        existingUser.Forename = userDto.Forename;
        existingUser.DateOfBirth = userDto.DateOfBirth;
        existingUser.IsActive = userDto.IsActive;

        _dataAccess.Update(existingUser);  // assuming IDataContext has Update

        var dataAfter = JsonSerializer.Serialize(existingUser);

        LogAudit(
            existingUser,
            "UPDATE",
            dataBefore,
            dataAfter
        );

        return existingUser;
    }

    private void LogAudit(User user, string operation, string? dataBefore, string? dataAfter)
    {
        var audit = new UserLogs
        {
            UserId = user.Id,
            Operation = operation,
            DataBefore = dataBefore,
            DataAfter = dataAfter
        };

        _dataAccess.Create(audit);
    }
}
