using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using UserManagement.Data.DTO;
using UserManagement.Models;
using UserManagement.Services.Domain.Interfaces;

namespace UserManagement.WebMS.Controllers
{
    [ApiController]
    [Route("api/users/")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        public UsersController(IUserService userService) => _userService = userService;

        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<User> items = await _userService.GetAllAsync();
            return Ok(items);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateUserDto newUser)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var createdUser = await _userService.CreateAsync(newUser);
            return CreatedAtAction(nameof(List), new { id = createdUser.Id }, createdUser);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(long id)
        {
            try
            {
                await _userService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(long id, [FromBody] UpdateUserDto userDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            try
            {
                var updatedUser = await _userService.UpdateAsync(id, userDto);
                return Ok(updatedUser);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
