using System;

namespace UserManagement.Data.DTO;

public class UpdateUserDto
{
    public required string Email { get; set; }
    public required string Surname { get; set; }
    public required string Forename { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }
}
