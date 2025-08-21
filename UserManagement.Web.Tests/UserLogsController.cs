using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;
using UserManagement.WebMS.Controllers;
namespace UserManagement.Data.Tests
{
    public class UserLogsControllerTests
    {
        private readonly Mock<IUserLogsService> _logsService = new();
        private UserLogsController CreateController() => new(_logsService.Object);

        [Fact]
        public async Task GetAuditsByUserId_WhenNone_ReturnsOkWithEmptyArray()
        {
            // Arrange
            var controller = CreateController();
            long userId = 10;
            _logsService.Setup(s => s.GetAuditsForUserAsync(userId))
                        .ReturnsAsync(Enumerable.Empty<UserLogs>());

            // Act
            var result = await controller.GetAuditsByUserId(userId);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeAssignableTo<IEnumerable<object>>()
               .Which.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAuditsByUserId_WhenSomeExist_ReturnsOkWithItems()
        {
            // Arrange
            var controller = CreateController();
            long userId = 10;
            var audits = new[]
            {
                new UserLogs { Id = 1, UserId = userId, Operation = "CREATE", Timestamp = new DateTime(2024,1,1) },
                new UserLogs { Id = 2, UserId = userId, Operation = "UPDATE", Timestamp = new DateTime(2025,1,1) },
            };
            _logsService.Setup(s => s.GetAuditsForUserAsync(userId)).ReturnsAsync(audits);

            // Act
            var result = await controller.GetAuditsByUserId(userId);

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(audits);
        }

        [Fact]
        public async Task List_ReturnsOkWithAllLogs()
        {
            // Arrange
            var controller = CreateController();
            var all = new[]
            {
                new UserLogs { Id = 1, UserId = 10, Operation = "CREATE", Timestamp = new DateTime(2024,1,1) },
                new UserLogs { Id = 2, UserId = 11, Operation = "DELETE", Timestamp = new DateTime(2024,2,1) },
            };
            _logsService.Setup(s => s.GetAllAsync()).ReturnsAsync(all);

            // Act
            var result = await controller.List();

            // Assert
            var ok = result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(all);
        }
    }
}
