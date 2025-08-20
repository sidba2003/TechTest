using System;
using System.Collections.Generic;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.WebMS.Controllers;
using Xunit;

namespace UserManagement.Data.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userService = new();
        private UsersController CreateController() => new(_userService.Object);

        [Fact]
        public void List_WhenServiceReturnsUsers_ReturnsOkWithUsers()
        {
            // Arrange
            var controller = CreateController();
            var users = new[]
            {
                new User { Id = 1, Forename = "A", Surname = "B", Email = "a@b.com", IsActive = true, DateOfBirth = new DateTime(2000,1,1) }
            };
            _userService.Setup(s => s.GetAll()).Returns(users);

            // Act
            var result = controller.List();

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(users);
        }

        [Fact]
        public void Create_ValidDto_ReturnsCreatedAtActionWithEntity()
        {
            // Arrange
            var controller = CreateController();
            var dto = new CreateUserDto
            {
                Forename = "Alice",
                Surname = "Test",
                Email = "alice@test.com",
                DateOfBirth = new DateTime(1995, 1, 1),
                IsActive = true
            };
            var created = new User
            {
                Id = 42,
                Forename = dto.Forename,
                Surname = dto.Surname,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                IsActive = dto.IsActive
            };
            _userService.Setup(s => s.Create(It.IsAny<CreateUserDto>())).Returns(created);

            // Act
            var result = controller.Create(dto);

            // Assert
            var createdAt = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAt.ActionName.Should().Be(nameof(controller.List));
            createdAt.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(42);
            createdAt.Value.Should().BeEquivalentTo(created);
        }

        [Fact]
        public void Create_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();
            controller.ModelState.AddModelError("Email", "Required");
            var dto = new CreateUserDto();

            // Act
            var result = controller.Create(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _userService.Verify(s => s.Create(It.IsAny<CreateUserDto>()), Times.Never);
        }

        [Fact]
        public void Delete_ExistingUser_ReturnsNoContent()
        {
            // Arrange
            var controller = CreateController();
            var id = 1L;
            _userService.Setup(s => s.Delete(id));

            // Act
            var result = controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _userService.Verify(s => s.Delete(id), Times.Once);
        }

        [Fact]
        public void Delete_MissingUser_ReturnsNotFound()
        {
            // Arrange
            var controller = CreateController();
            var id = 999L;
            _userService.Setup(s => s.Delete(id)).Throws<KeyNotFoundException>();

            // Act
            var result = controller.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _userService.Verify(s => s.Delete(id), Times.Once);
        }

        [Fact]
        public void Update_ValidDto_ReturnsOkWithUpdatedUser()
        {
            // Arrange
            var controller = CreateController();
            var id = 5L;
            var dto = new UpdateUserDto
            {
                Forename = "New",
                Surname = "Name",
                Email = "new@example.com",
                DateOfBirth = new DateTime(1990, 2, 2),
                IsActive = true
            };
            var updated = new User
            {
                Id = id,
                Forename = dto.Forename,
                Surname = dto.Surname,
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                IsActive = dto.IsActive
            };
            _userService.Setup(s => s.Update(id, It.IsAny<UpdateUserDto>())).Returns(updated);

            // Act
            var result = controller.Update(id, dto);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(updated);
        }

        [Fact]
        public void Update_MissingUser_ReturnsNotFound()
        {
            // Arrange
            var controller = CreateController();
            var id = 404L;
            var dto = new UpdateUserDto
            {
                Forename = "X",
                Surname = "Y",
                Email = "x@y.com",
                DateOfBirth = new DateTime(2001, 1, 1),
                IsActive = false
            };
            _userService.Setup(s => s.Update(id, It.IsAny < UpdateUserDto > ()))
                        .Throws<KeyNotFoundException>();

            // Act
            var result = controller.Update(id, dto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
