using System;
using System.Collections.Generic;
using System.Linq;
using UserManagement.Data;
using UserManagement.Models;
using UserManagement.Services.Domain.Implementations;

namespace UserManagement.Data.Tests
{
    public class UserLogsServiceTests
    {
        private readonly Mock<IDataContext> _dataContext = new();
        private UserLogsService CreateService() => new(_dataContext.Object);

        [Fact]
        public void GetAll_ReturnsLogs_OrderedByTimestampDescending()
        {
            // Arrange
            var service = CreateService();
            var logs = new List<UserLogs>
            {
                new UserLogs { Id = 1, UserId = 10, Operation = "CREATE", Timestamp = new DateTime(2024,1,1) },
                new UserLogs { Id = 2, UserId = 10, Operation = "UPDATE", Timestamp = new DateTime(2025,1,1) },
                new UserLogs { Id = 3, UserId = 11, Operation = "DELETE", Timestamp = new DateTime(2023,1,1) },
            }.AsQueryable();

            _dataContext.Setup(d => d.GetAll<UserLogs>()).Returns(logs);

            // Act
            var result = service.GetAll().ToList();

            // Assert
            result.Should().HaveCount(3);
            result.Select(r => r.Id).Should().Equal(2, 1, 3); // 2025, 2024, 2023
        }

        [Fact]
        public void GetAuditsForUser_FiltersAndOrdersDescending()
        {
            // Arrange
            var service = CreateService();
            var logs = new List<UserLogs>
            {
                new UserLogs { Id = 1, UserId = 10, Operation = "CREATE", Timestamp = new DateTime(2024,1,1) },
                new UserLogs { Id = 2, UserId = 10, Operation = "UPDATE", Timestamp = new DateTime(2025,1,1) },
                new UserLogs { Id = 3, UserId = 11, Operation = "DELETE", Timestamp = new DateTime(2023,1,1) },
            }.AsQueryable();

            _dataContext.Setup(d => d.GetAll<UserLogs>()).Returns(logs);

            // Act
            var result = service.GetAuditsForUser(10).ToList();

            // Assert
            result.Should().HaveCount(2);
            result.Select(r => r.Id).Should().Equal(2, 1);
            result.All(r => r.UserId == 10).Should().BeTrue();
        }

        [Fact]
        public void AddAudit_Null_Throws()
        {
            // Arrange
            var service = CreateService();

            // Act
            Action act = () => service.AddAudit(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("audit");
            _dataContext.Verify(d => d.Create(It.IsAny<UserLogs>()), Times.Never);
        }

        [Fact]
        public void AddAudit_PersistsAudit()
        {
            // Arrange
            var service = CreateService();
            UserLogs? captured = null;

            _dataContext.Setup(d => d.Create(It.IsAny<UserLogs>()))
                        .Callback<UserLogs>(a => captured = a);

            var audit = new UserLogs
            {
                UserId = 99,
                Operation = "UPDATE",
                DataBefore = "{\"x\":1}",
                DataAfter = "{\"x\":2}"
            };

            // Act
            service.AddAudit(audit);

            // Assert
            captured.Should().NotBeNull();
            captured!.UserId.Should().Be(99);
            captured.Operation.Should().Be("UPDATE");
            captured.DataBefore.Should().Contain("1");
            captured.DataAfter.Should().Contain("2");

            _dataContext.Verify(d => d.Create(It.IsAny<UserLogs>()), Times.Once);
        }
    }
}
