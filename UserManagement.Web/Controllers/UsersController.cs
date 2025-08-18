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
}
