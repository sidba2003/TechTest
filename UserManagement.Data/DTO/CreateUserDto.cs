using System;

namespace UserManagement.Data.DTO;

public class CreateUserDto
{
    public string Email { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Forename { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public bool IsActive { get; set; }
}
