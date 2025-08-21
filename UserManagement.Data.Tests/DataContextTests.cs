using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data.Tests
{
    public class DataContextTests
    {
        [Fact]
        public async Task GetAll_WhenNewEntityAdded_MustIncludeNewEntity()
        {
            // Arrange
            await using var context = CreateIsolatedContext();
            var entity = new User
            {
                Forename = "Brand New",
                Surname = "User",
                Email = "brandnewuser@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                IsActive = true
            };

            // Act
            await context.CreateAsync(entity);
            var result = await context.GetAll<User>().ToListAsync();

            // Assert
            result.Should().ContainSingle(u => u.Email == entity.Email)
                .Which.Should().BeEquivalentTo(entity, opt => opt.Excluding(u => u.Id));
        }

        [Fact]
        public async Task GetAll_WhenDeleted_MustNotIncludeDeletedEntity()
        {
            // Arrange
            await using var context = CreateIsolatedContext();
            var entity = new User
            {
                Forename = "ToDelete",
                Surname = "User",
                Email = "todelete@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                IsActive = true
            };
            await context.CreateAsync(entity);

            // Act
            await context.DeleteAsync(entity);
            var result = await context.GetAll<User>().ToListAsync();

            // Assert
            result.Should().NotContain(u => u.Email == entity.Email);
        }

        [Fact]
        public async Task GetAll_UserLogs_WhenNewLogAdded_MustIncludeNewLog()
        {
            // Arrange
            await using var context = CreateIsolatedContext();
            var user = new User
            {
                Forename = "Audited",
                Surname = "User",
                Email = "audited@example.com",
                DateOfBirth = new DateTime(1992, 2, 2),
                IsActive = true
            };
            await context.CreateAsync(user);

            var log = new UserLogs
            {
                UserId = user.Id,
                Operation = "CREATE",
                DataBefore = null,
                DataAfter = "{\"email\":\"audited@example.com\"}"
            };

            // Act
            await context.CreateAsync(log);
            var logs = await context.GetAll<UserLogs>()
                .Where(l => l.UserId == user.Id)
                .ToListAsync();

            // Assert
            logs.Should().ContainSingle(l => l.Operation == "CREATE")
                .Which.Should().BeEquivalentTo(log, opt => opt.Excluding(l => l.Id)
                                                            .Excluding(l => l.Timestamp));
        }

        [Fact]
        public async Task UpdateAsync_User_ModifiesPersistedValues()
        {
            await using var ctx = CreateIsolatedContext();
            var u = new User
            {
                Forename = "A",
                Surname = "B",
                Email = "u@example.com",
                IsActive = true,
                DateOfBirth = new(1990, 1, 1)
            };
            await ctx.CreateAsync(u);
            u.Forename = "Updated";

            await ctx.UpdateAsync(u);

            var reloaded = await ctx.GetAll<User>().SingleAsync(x => x.Id == u.Id);
            reloaded.Forename.Should().Be("Updated");
        }

        [Fact]
        public async Task GetAll_UserLogs_WhenDeleted_MustNotIncludeDeletedLog()
        {
            // Arrange
            await using var context = CreateIsolatedContext();
            var user = new User
            {
                Forename = "LogDel",
                Surname = "User",
                Email = "logdel@example.com",
                DateOfBirth = new DateTime(1993, 3, 3),
                IsActive = false
            };
            await context.CreateAsync(user);

            var log = new UserLogs
            {
                UserId = user.Id,
                Operation = "DELETE",
                DataBefore = "{\"email\":\"logdel@example.com\"}",
                DataAfter = null
            };
            await context.CreateAsync(log);

            (await context.GetAll<UserLogs>().AnyAsync(l => l.Id == log.Id)).Should().BeTrue();

            // Act
            await context.DeleteAsync(log);
            var logs = await context.GetAll<UserLogs>()
                .Where(l => l.UserId == user.Id)
                .ToListAsync();

            // Assert
            logs.Should().NotContain(l => l.Id == log.Id);
        }

        private static DataContext CreateIsolatedContext(bool ensureCreated = true)
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) 
                .Options;

            var ctx = new DataContext(options);
            if (ensureCreated) ctx.Database.EnsureCreated();
            return ctx;
        }
    }
}
