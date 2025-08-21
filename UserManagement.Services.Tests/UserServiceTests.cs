using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests
{
    public class UserServiceTest
    {
        private static DataContext NewCleanContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new DataContext(options);
        }

        [Fact]
        public async Task GetAllAsync_WhenNewUserAdded_ContainsIt()
        {
            await using var context = NewCleanContext();
            var service = new UserService(context);

            var dto = new CreateUserDto
            {
                Forename = "Alice",
                Surname = "Test",
                Email = "alice@test.com",
                DateOfBirth = new DateTime(1995, 1, 1),
                IsActive = true
            };
            await service.CreateAsync(dto);

            var all = await service.GetAllAsync();

            all.Should().Contain(u => u.Email == "alice@test.com");
        }

        [Fact]
        public async Task CreateAsync_PersistsUser_And_WritesCreateAudit()
        {
            await using var context = NewCleanContext();
            var service = new UserService(context);

            var dto = new CreateUserDto
            {
                Forename = "Bob",
                Surname = "Builder",
                Email = "bob@example.com",
                DateOfBirth = new DateTime(1990, 2, 2),
                IsActive = true
            };

            var created = await service.CreateAsync(dto);

            var fromDb = await context.Users!.FirstOrDefaultAsync(u => u.Id == created.Id);
            fromDb.Should().NotBeNull();
            fromDb!.Email.Should().Be("bob@example.com");

            var audit = await context.UserAudits!.Where(a => a.UserId == created.Id).SingleAsync();
            audit.Operation.Should().Be("CREATE");
            audit.DataBefore.Should().BeNull();
            audit.DataAfter.Should().Contain("bob@example.com");
        }

        [Fact]
        public async Task UpdateAsync_ExistingUser_Updates_And_WritesUpdateAudit()
        {
            await using var context = NewCleanContext();
            var service = new UserService(context);

            var created = await service.CreateAsync(new CreateUserDto
            {
                Forename = "Old",
                Surname = "Name",
                Email = "old@example.com",
                DateOfBirth = new DateTime(1990, 1, 1),
                IsActive = false
            });

            var updated = await service.UpdateAsync(created.Id, new UpdateUserDto
            {
                Forename = "New",
                Surname = "Name",
                Email = "new@example.com",
                DateOfBirth = new DateTime(1991, 2, 2),
                IsActive = true
            });

            updated.Email.Should().Be("new@example.com");

            var audit = await context.UserAudits!
                .Where(a => a.UserId == created.Id && a.Operation == "UPDATE")
                .SingleAsync();

            audit.DataBefore.Should().Contain("old@example.com");
            audit.DataAfter.Should().Contain("new@example.com");
        }

        [Fact]
        public async Task DeleteAsync_ExistingUser_Deletes_And_WritesDeleteAudit()
        {
            await using var context = NewCleanContext();
            var service = new UserService(context);

            var created = await service.CreateAsync(new CreateUserDto
            {
                Forename = "Del",
                Surname = "User",
                Email = "del@example.com",
                DateOfBirth = new DateTime(1980, 1, 1),
                IsActive = true
            });

            await service.DeleteAsync(created.Id);

            // user gone
            (await context.Users!.AnyAsync(u => u.Id == created.Id)).Should().BeFalse();

            var audit = await context.UserAudits!
                .SingleAsync(a => a.UserId == created.Id && a.Operation == "DELETE");

            audit.Operation.Should().Be("DELETE");
            audit.DataBefore.Should().Contain("del@example.com");
            audit.DataAfter.Should().BeNull();
        }


        [Fact]
        public async Task DeleteAsync_NonExisting_Throws_KeyNotFoundException()
        {
            await using var context = NewCleanContext();
            var service = new UserService(context);

            var act = async () => await service.DeleteAsync(999);
            await act.Should().ThrowAsync<KeyNotFoundException>()
                     .WithMessage("User with Id 999 not found");
        }

        [Fact]
        public async Task FilterByActiveAsync_ReturnsOnlyRequestedState()
        {
            await using var context = NewCleanContext();
            var svc = new UserService(context);

            await svc.CreateAsync(new CreateUserDto { Forename = "A", Surname = "X", Email = "a@x.com", DateOfBirth = new(1990, 1, 1), IsActive = true });
            await svc.CreateAsync(new CreateUserDto { Forename = "B", Surname = "Y", Email = "b@y.com", DateOfBirth = new(1990, 1, 1), IsActive = false });

            var actives = (await svc.FilterByActiveAsync(true)).Select(u => u.Email).ToList();
            var inactives = (await svc.FilterByActiveAsync(false)).Select(u => u.Email).ToList();

            actives.Should().Contain("a@x.com").And.NotContain("b@y.com");
            inactives.Should().Contain("b@y.com").And.NotContain("a@x.com");
        }

        [Fact]
        public async Task CreateAsync_NullDto_Throws()
        {
            await using var context = NewCleanContext();
            var svc = new UserService(context);

            var act = () => svc.CreateAsync(null!);
            await act.Should().ThrowAsync<ArgumentNullException>()
                     .WithParameterName("userDto");
        }

        [Fact]
        public async Task UpdateAsync_NullDto_Throws()
        {
            await using var context = NewCleanContext();
            var svc = new UserService(context);

            var act = () => svc.UpdateAsync(123, null!);
            await act.Should().ThrowAsync<ArgumentNullException>()
                     .WithParameterName("userDto");
        }

        [Fact]
        public async Task UpdateAsync_NonExisting_Throws_KeyNotFoundException()
        {
            await using var context = NewCleanContext();
            var svc = new UserService(context);

            var act = () => svc.UpdateAsync(999, new UpdateUserDto
            {
                Forename = "X",
                Surname = "Y",
                Email = "x@y.com",
                DateOfBirth = new(1990, 1, 1),
                IsActive = true
            });

            await act.Should().ThrowAsync<KeyNotFoundException>()
                     .WithMessage("User with Id 999 not found");
        }

        [Fact]
        public async Task CreateAsync_SetsId_And_AuditLinksToCreatedUser()
        {
            await using var context = NewCleanContext();
            var svc = new UserService(context);

            var created = await svc.CreateAsync(new CreateUserDto
            {
                Forename = "C",
                Surname = "Z",
                Email = "c@z.com",
                DateOfBirth = new(1990, 1, 1),
                IsActive = true
            });

            created.Id.Should().BeGreaterThan(0);

            var audit = await context.UserAudits!.SingleAsync(a => a.Operation == "CREATE");
            audit.UserId.Should().Be(created.Id);
            audit.DataBefore.Should().BeNull();

            var after = System.Text.Json.JsonSerializer.Deserialize<User>(audit.DataAfter!);
            after.Should().NotBeNull();
            after!.Email.Should().Be("c@z.com");
        }

        [Fact]
        public async Task UpdateAsync_WritesAccurateBeforeAfterSnapshots()
        {
            await using var context = NewCleanContext();
            var svc = new UserService(context);

            var u = await svc.CreateAsync(new CreateUserDto
            {
                Forename = "Old",
                Surname = "Name",
                Email = "old@ex.com",
                DateOfBirth = new(1990, 1, 1),
                IsActive = false
            });

            await svc.UpdateAsync(u.Id, new UpdateUserDto
            {
                Forename = "New",
                Surname = "Name",
                Email = "new@ex.com",
                DateOfBirth = new(1991, 2, 2),
                IsActive = true
            });

            var audit = await context.UserAudits!.SingleAsync(a => a.UserId == u.Id && a.Operation == "UPDATE");

            var before = JsonSerializer.Deserialize<User>(audit.DataBefore!);
            var after = JsonSerializer.Deserialize<User>(audit.DataAfter!);

            before!.Email.Should().Be("old@ex.com");
            before.IsActive.Should().BeFalse();

            after!.Email.Should().Be("new@ex.com");
            after.IsActive.Should().BeTrue();
            after.Forename.Should().Be("New");
        }
    }
}
