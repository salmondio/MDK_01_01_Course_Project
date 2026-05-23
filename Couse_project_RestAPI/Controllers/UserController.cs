using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Couse_project_RestAPI.Models;
using Couse_project_RestAPI.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Couse_project_RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbContextMain _context;

        public UserController(DbContextMain context)
        {
            _context = context;
        }

        [HttpGet("List")]
        [ProducesResponseType(typeof(ICollection<User>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ICollection<User>>> UserList()
        {
            try
            {
                ICollection<User> userList = await _context.Users.ToListAsync();

                return Ok(userList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ICollection<User>>> GetUser(int id)
        {
            try
            {
                User user = await _context.Users.FindAsync(id);

                if (user == null)
                    return NotFound($"Пользователя с id = {id} не существует");

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("Add")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<User>> UserAdd([FromBody] User user)
        {
            try
            {
                if (user == null)
                    return BadRequest("Пользователь не может быть равен null-значению");
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    return BadRequest("Email пользователя должен быть уникальным");

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new {id = user.Id}, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UserDelete(int id)
        {

            try
            {
                User user = await _context.Users.FindAsync(id);

                if (user == null)
                    return NotFound($"Пользователя с id = {id} не существует");

                _context.Users.Remove(user);
                _context.SaveChanges();

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("ChangeActive/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UserChangeActive(int id)
        {
            try
            {
                User user = await _context.Users.FindAsync(id);

                if (user == null)
                    return StatusCode(404);

                user.Is_active = !user.Is_active;
                _context.SaveChanges();

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPut("Update")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult> UserUpdate([FromBody] User user)
        {
            try
            {
                if (user == null)
                    return BadRequest("Пользователь не может быть равен null-значению");
                if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != user.Id))
                    return BadRequest("Email пользователя должен быть уникальным");

                User oldUser = await _context.Users.FindAsync(user.Id);

                if (oldUser == null)
                    return NotFound($"Пользователя с id = {user.Id} не существует");

                oldUser.Id_role = user.Id_role;
                oldUser.Name = user.Name;
                oldUser.Lastname = user.Lastname;
                oldUser.Surname = user.Surname;
                oldUser.Email = user.Email;
                oldUser.Phone_number = user.Phone_number;
                oldUser.Password = user.Password;
                oldUser.Is_active = user.Is_active;

                await _context.SaveChangesAsync();

                return StatusCode(200);
            }
            catch(Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
