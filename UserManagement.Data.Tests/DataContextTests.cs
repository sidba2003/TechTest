using System.Linq;
using FluentAssertions;
using UserManagement.Models;

namespace UserManagement.Data.Tests;

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
              .Which.Should().BeEquivalentTo(entity, options => options.Excluding(u => u.Id));
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

    private DataContext CreateContext() => new();
}
