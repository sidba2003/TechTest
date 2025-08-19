//using System;
//using System.Collections.Generic;
//using Microsoft.AspNetCore.Mvc;
//using UserManagement.Data.DTO;
//using UserManagement.Models;
//using UserManagement.Services.Domain.Interfaces;
//using UserManagement.WebMS.Controllers;

//namespace UserManagement.Data.Tests;

//public class UserControllerTests
//{
//    private readonly Mock<IUserService> _userService = new();
//    private UsersController CreateController() => new(_userService.Object);

//    [Fact]
//    public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
//    {
//        // Arrange
//        var controller = CreateController();
//        var users = SetupUsers();

//        // Act
//        var result = controller.List();

//        // Assert
//        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
//        var value = okResult.Value.Should().BeAssignableTo<IEnumerable<User>>().Subject;

//        value.Should().BeEquivalentTo(users);
//    }

//    [Fact]
//    public void Create_ValidUser_ReturnsCreatedUser()
//    {
//        // Arrange
//        var controller = CreateController();
//        var dto = new CreateUserDto
//        {
//            Forename = "Alice",
//            Surname = "Test",
//            Email = "alice@test.com",
//            DateOfBirth = new DateTime(1995, 1, 1),
//            IsActive = true
//        };
//        var createdUser = new User
//        {
//            Id = 1,
//            Forename = dto.Forename,
//            Surname = dto.Surname,
//            Email = dto.Email,
//            DateOfBirth = dto.DateOfBirth,
//            IsActive = dto.IsActive
//        };

//        _userService.Setup(s => s.Create(dto)).Returns(createdUser);

//        // Act
//        var result = controller.Create(dto);

//        // Assert
//        var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
//        createdAtActionResult.Value.Should().BeEquivalentTo(createdUser);
//        createdAtActionResult.ActionName.Should().Be(nameof(controller.List));
//        createdAtActionResult.RouteValues["id"].Should().Be(createdUser.Id);
//    }

//    [Fact]
//    public void Delete_ExistingUser_ReturnsNoContent()
//    {
//        // Arrange
//        var controller = CreateController();
//        long userId = 1;

//        _userService.Setup(s => s.Delete(userId));

//        // Act
//        var result = controller.Delete(userId);

//        // Assert
//        result.Should().BeOfType<NoContentResult>();
//        _userService.Verify(s => s.Delete(userId), Times.Once);
//    }

//    [Fact]
//    public void Delete_NonExistingUser_ReturnsNotFound()
//    {
//        // Arrange
//        var controller = CreateController();
//        long userId = 999;

//        _userService.Setup(s => s.Delete(userId)).Throws<KeyNotFoundException>();

//        // Act
//        var result = controller.Delete(userId);

//        // Assert
//        result.Should().BeOfType<NotFoundResult>();
//        _userService.Verify(s => s.Delete(userId), Times.Once);
//    }

//    private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true, DateTime DateOfBirth = default)
//    {
//        DateOfBirth = DateOfBirth.Date;

//        var users = new[]
//        {
//            new User
//            {
//                Forename = forename,
//                Surname = surname,
//                Email = email,
//                IsActive = isActive,
//                DateOfBirth = DateOfBirth
//            }
//        };

//        _userService
//            .Setup(s => s.GetAll())
//            .Returns(users);

//        return users;
//    }
//}

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests
{
    public class UserControllerTests
    {
        private readonly Mock<IUserService> _userService = new();
        private UsersController CreateController() => new(_userService.Object);

        [Fact]
        public void List_WhenServiceReturnsUsers_ModelMustContainUsers()
        {
            // Arrange
            var controller = CreateController();
            var users = SetupUsers();

            // Act
            var result = controller.List();

            // Assert
            var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
            var value = okResult.Value.Should().BeAssignableTo<IEnumerable<User>>().Subject;
            value.Should().BeEquivalentTo(users);
        }

        [Fact]
        public void Create_ValidUser_ReturnsCreatedUser()
        {
            // Arrange
            var controller = CreateController();
            controller.ModelState.Clear(); // important if using [ApiController]

            var dto = new CreateUserDto
            {
                Forename = "Alice",
                Surname = "Test",
                Email = "alice@test.com",
                DateOfBirth = new DateTime(1995, 1, 1),
                IsActive = true
            };
            var createdUser = new User
            {
                Id = 1,
                Forename = dto.Forename,
                Surname = dto.Surname,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                IsActive = dto.IsActive
            };

            _userService.Setup(s => s.Create(It.IsAny<CreateUserDto>())).Returns(createdUser);

            // Act
            var result = controller.Create(dto);

            // Assert
            var createdAtActionResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAtActionResult.Value.Should().BeEquivalentTo(createdUser);
            createdAtActionResult.ActionName.Should().Be(nameof(controller.List));
            createdAtActionResult.RouteValues["id"].Should().Be(createdUser.Id);
        }

        [Fact]
        public void Delete_ExistingUser_ReturnsNoContent()
        {
            // Arrange
            var controller = CreateController();
            long userId = 1;

            _userService.Setup(s => s.Delete(userId));

            // Act
            var result = controller.Delete(userId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _userService.Verify(s => s.Delete(userId), Times.Once);
        }

        [Fact]
        public void Delete_NonExistingUser_ReturnsNotFound()
        {
            // Arrange
            var controller = CreateController();
            long userId = 999;

            _userService.Setup(s => s.Delete(userId)).Throws<KeyNotFoundException>();

            // Act
            var result = controller.Delete(userId);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _userService.Verify(s => s.Delete(userId), Times.Once);
        }

        private User[] SetupUsers(string forename = "Johnny", string surname = "User", string email = "juser@example.com", bool isActive = true, DateTime DateOfBirth = default)
        {
            DateOfBirth = DateOfBirth.Date;

            var users = new[]
            {
                new User
                {
                    Forename = forename,
                    Surname = surname,
                    Email = email,
                    IsActive = isActive,
                    DateOfBirth = DateOfBirth
                }
            };

            _userService.Setup(s => s.GetAll()).Returns(users);

            return users;
        }
    }
}

