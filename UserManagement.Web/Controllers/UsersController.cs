using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.WebMS.Controllers;

[ApiController]
[Route("api/users/")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService) => _userService = userService;

    [HttpGet]
    public IActionResult List()
    {
        var items = _userService.GetAll();

        return Ok(items);
    }

    [HttpPost]
    public IActionResult Create([FromBody] CreateUserDto newUser)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Call service to create user
        var createdUser = _userService.Create(newUser);

        return CreatedAtAction(nameof(List), new { id = createdUser.Id }, createdUser);
    }
}
