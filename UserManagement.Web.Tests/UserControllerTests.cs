using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.WebMS.Controllers;

namespace UserManagement.Data.Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userService = new();
        private UsersController CreateController() => new(_userService.Object);

        [Fact]
        public async Task List_WhenServiceReturnsUsers_ReturnsOkWithUsers()
        {
            // Arrange
            var controller = CreateController();
            var users = new[]
            {
                new User { Id = 1, Forename = "A", Surname = "B", Email = "a@b.com", IsActive = true, DateOfBirth = new DateTime(2000,1,1) }
            };
            _userService.Setup(s => s.GetAllAsync()).ReturnsAsync(users);

            // Act
            var result = await controller.List();

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(users);
        }

        [Fact]
        public async Task Create_ValidDto_ReturnsCreatedAtActionWithEntity()
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
            _userService.Setup(s => s.CreateAsync(It.IsAny<CreateUserDto>())).ReturnsAsync(created);

            // Act
            var result = await controller.Create(dto);

            // Assert
            var createdAt = result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdAt.ActionName.Should().Be(nameof(controller.List));
            createdAt.RouteValues.Should().ContainKey("id").WhoseValue.Should().Be(42);
            createdAt.Value.Should().BeEquivalentTo(created);
        }

        [Fact]
        public async Task Create_InvalidModel_ReturnsBadRequest()
        {
            // Arrange
            var controller = CreateController();
            controller.ModelState.AddModelError("Email", "Required");
            var dto = new CreateUserDto();

            // Act
            var result = await controller.Create(dto);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
            _userService.Verify(s => s.CreateAsync(It.IsAny<CreateUserDto>()), Times.Never);
        }

        [Fact]
        public async Task Delete_ExistingUser_ReturnsNoContent()
        {
            // Arrange
            var controller = CreateController();
            var id = 1L;
            _userService.Setup(s => s.DeleteAsync(id)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.Delete(id);

            // Assert
            result.Should().BeOfType<NoContentResult>();
            _userService.Verify(s => s.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task Delete_MissingUser_ReturnsNotFound()
        {
            // Arrange
            var controller = CreateController();
            var id = 999L;
            _userService.Setup(s => s.DeleteAsync(id)).ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await controller.Delete(id);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
            _userService.Verify(s => s.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task Update_ValidDto_ReturnsOkWithUpdatedUser()
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
            _userService.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateUserDto>())).ReturnsAsync(updated);

            // Act
            var result = await controller.Update(id, dto);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(updated);
        }

        [Fact]
        public async Task Update_MissingUser_ReturnsNotFound()
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
            _userService.Setup(s => s.UpdateAsync(id, It.IsAny<UpdateUserDto>()))
                        .ThrowsAsync(new KeyNotFoundException());

            // Act
            var result = await controller.Update(id, dto);

            // Assert
            result.Should().BeOfType<NotFoundResult>();
        }
    }
}
