using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests
{
    public class UserLogsServiceTest
    {
        private static DataContext NewCleanContext()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString()) // unique per test
                .Options;

            return new DataContext(options);
        }

        [Fact]
        public async Task GetAllAsync_Returns_DescendingByTimestamp()
        {
            await using var context = NewCleanContext();
            var service = new UserLogsService(context);

            await context.CreateAsync(new UserLogs { UserId = 10, Operation = "CREATE", Timestamp = new DateTime(2024, 1, 1) });
            await context.CreateAsync(new UserLogs { UserId = 10, Operation = "UPDATE", Timestamp = new DateTime(2025, 1, 1) });
            await context.CreateAsync(new UserLogs { UserId = 11, Operation = "DELETE", Timestamp = new DateTime(2023, 1, 1) });

            var all = (await service.GetAllAsync()).ToList();

            all.Select(l => l.Timestamp).Should().BeInDescendingOrder();
            all.Select(l => l.Operation).Should().Equal("UPDATE", "CREATE", "DELETE");
        }

        [Fact]
        public async Task GetAuditsForUserAsync_FiltersToUser_And_OrdersDescending()
        {
            await using var context = NewCleanContext();

            await context.CreateAsync(new UserLogs { UserId = 10, Operation = "CREATE", Timestamp = new DateTime(2024, 1, 1) });
            await context.CreateAsync(new UserLogs { UserId = 10, Operation = "UPDATE", Timestamp = new DateTime(2025, 1, 1) });
            await context.CreateAsync(new UserLogs { UserId = 11, Operation = "DELETE", Timestamp = new DateTime(2023, 1, 1) });

            (await context.UserAudits!.CountAsync()).Should().Be(3);

            var service = new UserLogsService(context);

            var user10 = (await service.GetAuditsForUserAsync(10)).ToList();

            user10.Should().HaveCount(2);
            user10.All(l => l.UserId == 10).Should().BeTrue();
            user10.Select(l => l.Operation).Should().Equal("UPDATE", "CREATE");
        }

        [Fact]
        public async Task AddAuditAsync_Persists()
        {
            await using var context = NewCleanContext();
            var service = new UserLogsService(context);

            var audit = new UserLogs
            {
                UserId = 99,
                Operation = "UPDATE",
                DataBefore = "{\"x\":1}",
                DataAfter = "{\"x\":2}"
            };

            await service.AddAuditAsync(audit);

            var fromDb = await context.UserAudits!.SingleAsync();
            fromDb.UserId.Should().Be(99);
            fromDb.Operation.Should().Be("UPDATE");
            fromDb.DataBefore.Should().Contain("\"x\":1");
            fromDb.DataAfter.Should().Contain("\"x\":2");
        }

        [Fact]
        public async Task AddAuditAsync_Null_Throws()
        {
            await using var context = NewCleanContext();
            var service = new UserLogsService(context);

            var act = async () => await service.AddAuditAsync(null!);

            await act.Should().ThrowAsync<ArgumentNullException>()
                     .WithParameterName("audit");
        }

        [Fact]
        public async Task GetAllAsync_WhenNoLogs_ReturnsEmpty()
        {
            await using var context = NewCleanContext();
            var svc = new UserLogsService(context);

            var all = await svc.GetAllAsync();
            all.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAuditsForUserAsync_WhenNoLogsForUser_ReturnsEmpty()
        {
            await using var context = NewCleanContext();
            await context.CreateAsync(new UserLogs { UserId = 42, Operation = "CREATE", Timestamp = new DateTime(2024, 1, 1) });

            var svc = new UserLogsService(context);
            var logs = await svc.GetAuditsForUserAsync(99);

            logs.Should().BeEmpty();
        }
    }
}
