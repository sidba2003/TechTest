using System;
using System.Collections.Generic;
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

        return newUser;
    }
}
