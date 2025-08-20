using System.Linq;
using UserManagement.Data;
using UserManagement.Models;

namespace UserManagement.Data.Tests
{
    public class DataContextTests
    {
        [Fact]
        public void GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
        {
            // Arrange
            using var context = CreateContext();
            var entity = new User
            {
                Forename = "Brand New",
                Surname = "User",
                Email = "brandnewuser@example.com",
                DateOfBirth = new System.DateTime(1990, 1, 1),
                IsActive = true
            };

            // Act
            context.Create(entity);
            var result = context.GetAll<User>().ToList();

            // Assert
            result.Should().ContainSingle(u => u.Email == entity.Email)
                  .Which.Should().BeEquivalentTo(entity, opt => opt.Excluding(u => u.Id));
        }

        [Fact]
        public void GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
        {
            // Arrange
            using var context = CreateContext();
            var entity = new User
            {
                Forename = "ToDelete",
                Surname = "User",
                Email = "todelete@example.com",
                DateOfBirth = new System.DateTime(1990, 1, 1),
                IsActive = true
            };
            context.Create(entity);

            // Act
            context.Delete(entity);
            var result = context.GetAll<User>().ToList();

            // Assert
            result.Should().NotContain(u => u.Email == entity.Email);
        }

        [Fact]
        public void GetAll_UserLogs_WhenNewLogAdded_MustIncludeNewLog()
        {
            // Arrange
            using var context = CreateContext();

            // create a user first (so we have a valid UserId to log against)
            var user = new User
            {
                Forename = "Audited",
                Surname = "User",
                Email = "audited@example.com",
                DateOfBirth = new System.DateTime(1992, 2, 2),
                IsActive = true
            };
            context.Create(user);

            var log = new UserLogs
            {
                UserId = user.Id,
                Operation = "CREATE",
                DataBefore = null,
                DataAfter = "{\"email\":\"audited@example.com\"}"
            };

            // Act
            context.Create(log);
            var logs = context.GetAll<UserLogs>().Where(l => l.UserId == user.Id).ToList();

            // Assert
            logs.Should().ContainSingle(l => l.Operation == "CREATE")
                .Which.Should().BeEquivalentTo(log, opt => opt.Excluding(l => l.Id)
                                                             .Excluding(l => l.Timestamp));
        }

        [Fact]
        public void GetAll_UserLogs_WhenDeleted_MustNotIncludeDeletedLog()
        {
            // Arrange
            using var context = CreateContext();

            var user = new User
            {
                Forename = "LogDel",
                Surname = "User",
                Email = "logdel@example.com",
                DateOfBirth = new System.DateTime(1993, 3, 3),
                IsActive = false
            };
            context.Create(user);

            var log = new UserLogs
            {
                UserId = user.Id,
                Operation = "DELETE",
                DataBefore = "{\"email\":\"logdel@example.com\"}",
                DataAfter = null
            };
            context.Create(log);

            // sanity: log exists
            context.GetAll<UserLogs>().Any(l => l.Id == log.Id).Should().BeTrue();

            // Act
            context.Delete(log);
            var logs = context.GetAll<UserLogs>().Where(l => l.UserId == user.Id).ToList();

            // Assert
            logs.Should().NotContain(l => l.Id == log.Id);
        }

        private DataContext CreateContext() => new();
    }
}
