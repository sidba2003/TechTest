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
        public void Create_ValidUser_CreatesUser_And_WritesAudit()
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

            // Simulate EF setting an Id on insert
            User? capturedUser = null;
            _dataContext
                .Setup(d => d.Create(It.IsAny<User>()))
                .Callback<User>(u =>
                {
                    u.Id = 42; // simulate DB-generated Id
                    capturedUser = u;
                });

            UserLogs? capturedAudit = null;
            _dataContext
                .Setup(d => d.Create(It.IsAny<UserLogs>()))
                .Callback<object>(o =>
                {
                    capturedAudit = (UserLogs)o;
                });

            // Act
            var result = service.Create(dto);

            // Assert (user persisted)
            capturedUser.Should().NotBeNull();
            capturedUser!.Forename.Should().Be(dto.Forename);
            capturedUser.Surname.Should().Be(dto.Surname);
            capturedUser.Email.Should().Be(dto.Email);
            capturedUser.DateOfBirth.Should().Be(dto.DateOfBirth);
            capturedUser.IsActive.Should().Be(dto.IsActive);
            capturedUser.Id.Should().Be(42);

            result.Should().BeEquivalentTo(capturedUser);

            // Assert (audit persisted)
            capturedAudit.Should().NotBeNull();
            capturedAudit!.UserId.Should().Be(42);
            capturedAudit.Operation.Should().Be("CREATE");
            capturedAudit.DataBefore.Should().BeNull();
            capturedAudit.DataAfter.Should().NotBeNullOrEmpty();
            capturedAudit.DataAfter!.Should().Contain("alice@test.com");

            _dataContext.Verify(d => d.Create(It.IsAny<User>()), Times.Once);
            _dataContext.Verify(d => d.Create(It.IsAny<UserLogs>()), Times.Once);
        }

        [Fact]
        public void Update_ExistingUser_UpdatesEntity_And_WritesAudit_WithBeforeAndAfter()
        {
            // Arrange
            var service = CreateService();

            var existing = new User
            {
                Id = 7,
                Forename = "Old",
                Surname = "Name",
                Email = "old@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                IsActive = false
            };

            _dataContext.Setup(d => d.GetAll<User>())
                        .Returns(new List<User> { existing }.AsQueryable());

            User? updatedEntity = null;
            _dataContext.Setup(d => d.Update(It.IsAny<User>()))
                        .Callback<User>(u => updatedEntity = u);

            UserLogs? capturedAudit = null;
            _dataContext.Setup(d => d.Create(It.IsAny<UserLogs>()))
                        .Callback<object>(o => capturedAudit = (UserLogs)o);

            var dto = new UpdateUserDto
            {
                Forename = "New",
                Surname = "Name",
                Email = "new@example.com",
                DateOfBirth = new DateTime(1991, 2, 2),
                IsActive = true
            };

            // Act
            var result = service.Update(existing.Id, dto);

            // Assert entity updated
            updatedEntity.Should().NotBeNull();
            updatedEntity!.Id.Should().Be(existing.Id);
            updatedEntity.Forename.Should().Be(dto.Forename);
            updatedEntity.Surname.Should().Be(dto.Surname);
            updatedEntity.Email.Should().Be(dto.Email);
            updatedEntity.DateOfBirth.Should().Be(dto.DateOfBirth);
            updatedEntity.IsActive.Should().Be(dto.IsActive);

            result.Should().BeEquivalentTo(updatedEntity);

            // Assert audit
            capturedAudit.Should().NotBeNull();
            capturedAudit!.UserId.Should().Be(existing.Id);
            capturedAudit.Operation.Should().Be("UPDATE");
            capturedAudit.DataBefore.Should().NotBeNullOrEmpty();
            capturedAudit.DataAfter.Should().NotBeNullOrEmpty();
            capturedAudit.DataBefore!.Should().Contain("old@example.com");
            capturedAudit.DataAfter!.Should().Contain("new@example.com");

            _dataContext.Verify(d => d.Update(It.IsAny<User>()), Times.Once);
            _dataContext.Verify(d => d.Create(It.IsAny<UserLogs>()), Times.Once);
        }

        [Fact]
        public void Delete_ExistingUser_DeletesEntity_And_WritesAudit()
        {
            // Arrange
            var service = CreateService();
            var users = SetupUsers();
            var idToDelete = users.First().Id;

            User? deletedEntity = null;
            _dataContext.Setup(d => d.Delete(It.IsAny<User>()))
                        .Callback<User>(u => deletedEntity = u);

            UserLogs? capturedAudit = null;
            _dataContext.Setup(d => d.Create(It.IsAny<UserLogs>()))
                        .Callback<object>(o => capturedAudit = (UserLogs)o);

            // Act
            service.Delete(idToDelete);

            // Assert delete called
            deletedEntity.Should().NotBeNull();
            deletedEntity!.Id.Should().Be(idToDelete);

            // Assert audit called
            capturedAudit.Should().NotBeNull();
            capturedAudit!.UserId.Should().Be(idToDelete);
            capturedAudit.Operation.Should().Be("DELETE");
            capturedAudit.DataBefore.Should().NotBeNullOrEmpty();
            capturedAudit.DataAfter.Should().BeNull();

            _dataContext.Verify(d => d.Delete(It.Is < User > (u => u.Id == idToDelete)), Times.Once);
            _dataContext.Verify(d => d.Create(It.IsAny<UserLogs>()), Times.Once);
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
            _dataContext.Verify(d => d.Create(It.IsAny<UserLogs>()), Times.Never);
            _dataContext.Verify(d => d.Delete(It.IsAny<User>()), Times.Never);
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
