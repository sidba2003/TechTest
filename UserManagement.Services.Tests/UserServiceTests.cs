using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests
{
    public class UserServiceTests
    {
        private readonly Mock<IDataContext> _dataContext = new();
        private UserService CreateService() => new(_dataContext.Object);

        [Fact]
        public void GetAll_WhenContextReturnsEntities_MustReturnSameEntities()
        {
            // Arrange
            var service = CreateService();
            var users = SetupUsers();

            // Act
            var result = service.GetAll();

            // Assert
            result.Should().BeEquivalentTo(users);
        }

        [Fact]
        public void Create_ValidUser_MustCallContextCreateAndReturnUser()
        {
            // Arrange
            var service = CreateService();
            var dto = new CreateUserDto
            {
                Forename = "Alice",
                Surname = "Test",
                Email = "alice@test.com",
                DateOfBirth = new DateTime(1995, 1, 1),
                IsActive = true
            };

            User? capturedUser = null;
            _dataContext
                .Setup(d => d.Create(It.IsAny<User>()))
                .Callback<User>(u => capturedUser = u);

            // Act
            var result = service.Create(dto);

            // Assert
            capturedUser.Should().NotBeNull();
            capturedUser.Forename.Should().Be(dto.Forename);
            capturedUser.Surname.Should().Be(dto.Surname);
            capturedUser.Email.Should().Be(dto.Email);
            capturedUser.DateOfBirth.Should().Be(dto.DateOfBirth);
            capturedUser.IsActive.Should().Be(dto.IsActive);

            result.Should().BeEquivalentTo(capturedUser);
        }

        [Fact]
        public void Delete_ExistingUser_MustCallContextDelete()
        {
            // Arrange
            var service = CreateService();
            var users = SetupUsers();
            var idToDelete = users.First().Id;

            // Act
            service.Delete(idToDelete);

            // Assert
            _dataContext.Verify(d => d.Delete(It.Is<User>(u => u.Id == idToDelete)), Times.Once);
        }

        [Fact]
        public void Delete_NonExistingUser_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var service = CreateService();
            _dataContext.Setup(d => d.GetAll<User>()).Returns(new List<User>().AsQueryable());

            // Act
            Action act = () => service.Delete(999);

            // Assert
            act.Should().Throw<KeyNotFoundException>().WithMessage("User with Id 999 not found");
        }

        private IQueryable<User> SetupUsers()
        {
            var users = new List<User>
            {
                new User
                {
                    Id = 1,
                    Forename = "Johnny",
                    Surname = "User",
                    Email = "juser@example.com",
                    IsActive = true,
                    DateOfBirth = new DateTime(1990, 1, 1)
                }
            }.AsQueryable();

            _dataContext.Setup(d => d.GetAll<User>()).Returns(users);

            return users;
        }
    }
}
