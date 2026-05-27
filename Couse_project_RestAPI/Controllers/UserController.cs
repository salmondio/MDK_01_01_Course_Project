using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Helpers;
using Couse_project_RestAPI.Models;
using Couse_project_RestAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Couse_project_RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DbContextMain _context;
        private IConfiguration _configuration;

        public UserController(DbContextMain context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }



        [HttpGet("Admin/List")]
        [ProducesResponseType(typeof(IEnumerable<User>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<User>>> List()
        {
            try
            {
                IEnumerable<User> userList = await _context.Users.ToArrayAsync();

                return Ok(userList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("Admin/{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> GetUser(int id)
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


        [HttpGet("ListTeacher")]
        [ProducesResponseType(typeof(IEnumerable<UserDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<UserDTO>>> ListTeacher()
        {
            try
            {
                IEnumerable<UserDTO> teacherList = await _context.Users
                    .Include(u => u.Role)
                    .Where(u => u.Role.Name == "Teacher")
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Lastname = u.Lastname,
                        Surname = u.Surname,
                        RoleName = u.Role.Name
                    })
                    .ToArrayAsync();
                if (!teacherList.Any())
                    return NotFound("Преподавателей.net");

                return Ok(teacherList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("ListTeacher/{id}")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<UserDTO>> GetTeacher(int id)
        {
            try
            {
                UserDTO teacher = await _context.Users
                    .Where(u => u.Id == id)
                    .Include(u => u.Role)
                    .Where(u => u.Role.Name == "Teacher")
                    .Select(u => new UserDTO
                    {
                        Id = u.Id,
                        Name = u.Name,
                        Lastname = u.Lastname,
                        Surname = u.Surname,
                        RoleName = u.Role.Name
                    })
                    .FirstAsync();
                if (teacher == null)
                    return NotFound($"Не существует преподавателя с id = {id}");

                return Ok(teacher);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Login")]
        [ProducesResponseType(typeof(object), 200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // Ищем пользователя в БД
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Email == request.Email);

            // Проверяем пароль (надо сделать хэширование)
            if (user == null || user.Password != request.Password)
                return Unauthorized(new { message = "Неверный email или пароль" });

            // Создание токена
            TokenHelper tokenHelper = new TokenHelper(_configuration);
            JwtSecurityToken token = tokenHelper.CreateToken(user);
            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            // Возвращаем токен и необходимую информацию о пользователе
            return Ok(new
            {
                Token = tokenString,
                ValidTo = token.ValidTo,
                User = new
                {
                    user.Id,
                    user.Email,
                    user.Name,
                    user.Lastname,
                    user.Surname,
                    user.Id_role,
                    user.Is_active,
                    Role = user.Role?.Name
                }
            });
        }


        [HttpPost("Add")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> Add([FromBody] User user)
        {
            try
            {
                if (user == null)
                    return BadRequest("Пользователь не может быть равен null-значению");
                if (await _context.Users.AnyAsync(u => u.Email == user.Email))
                    return BadRequest("Email пользователя должен быть уникальным");

                await _context.Users.AddAsync(user);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetUser), new {id = user.Id}, user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPatch("ChangeActive/{id}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ChangeActive(int id)
        {
            try
            {
                User user = await _context.Users.FindAsync(id);

                if (user == null)
                    return NotFound($"Пользователя с id = {id} не существует");

                user.Is_active = !user.Is_active;
                _context.SaveChanges();

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPatch("ChangePassword")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult> UpdateForUser([FromBody] string password)
        {
            try
            {
                if (String.IsNullOrEmpty(password))
                    return BadRequest("Пароль не может быть пустым");

                User oldUser = await _context.Users.FindAsync(int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value));

                // Если тебя не существует..?
                if (oldUser == null)
                    return NotFound($"Вы не существуете...");

                oldUser.Password = password;
                await _context.SaveChangesAsync();

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("Update")]
        [ProducesResponseType(typeof(UserDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize]
        public async Task<ActionResult<UserDTO>> UpdateForUser([FromBody] UserDTO user)
        {
            try
            {
                // Проверяем наличие полномочий для выполнения запроса
                if (User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value != user.Id.ToString())
                    return Forbid("Доступ на обновление пользователя запрещен");

                if (user == null)
                    return BadRequest("Пользователь не может быть равен null-значению");
                if (await _context.Users.AnyAsync(u => u.Email == user.Email && u.Id != user.Id))
                    return BadRequest("Такой Email-адрес уже используется");

                User oldUser = await _context.Users.FindAsync(user.Id);

                if (oldUser == null)
                    return NotFound($"Пользователя с id = {user.Id} не существует");

                oldUser.Name = user.Name;
                oldUser.Lastname = user.Lastname;
                oldUser.Surname = user.Surname;
                oldUser.Email = user.Email;
                oldUser.Phone_number = user.Phone_number;

                await _context.SaveChangesAsync();

                return StatusCode(200);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("Admin/Update")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> Update([FromBody] User user)
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

                return Ok(oldUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(User), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<User>> Delete(int id)
        {

            try
            {
                User user = await _context.Users.FindAsync(id);

                if (user == null)
                    return NotFound($"Пользователя с id = {id} не существует");

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
