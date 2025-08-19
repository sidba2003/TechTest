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

    [HttpDelete("{id}")]
    public IActionResult Delete(long id)
    {
        try
        {
            _userService.Delete(id);
            return NoContent(); // HTTP 204 – standard for successful DELETE
        }
        catch (KeyNotFoundException)
        {
            return NotFound(); // HTTP 404 if user does not exist
        }
    }

    [HttpPut("{id}")]
    public IActionResult Update(long id, [FromBody] UpdateUserDto userDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var updatedUser = _userService.Update(id, userDto);
            return Ok(updatedUser);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
