using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Couse_project_RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoleController : Controller
    {
        private readonly DbContextMain _context;

        public RoleController(DbContextMain context)
        {
            _context = context;
        }



        [HttpGet("Admin/List")]
        [ProducesResponseType(typeof(IEnumerable<Role>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult<IEnumerable<Role>>> List()
        {
            try
            {
                IEnumerable<Role> roleList = await _context.Roles.ToArrayAsync();

                return Ok(roleList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("Admin/{id}")]
        [ProducesResponseType(typeof(Role), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult<Role>> GetRole(int id)
        {
            try
            {
                Role role = await _context.Roles.FindAsync(id);

                if (role == null)
                    return NotFound($"Роли с id = {id} не существует");

                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("Owner/Add")]
        [ProducesResponseType(typeof(Role), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Role>> Add([FromBody] Role role)
        {
            try
            {
                if (await _context.Roles.AnyAsync(d => d.Name == role.Name))
                    return BadRequest("Роль с таким имененм уже существует");

                await _context.Roles.AddAsync(role);
                await _context.SaveChangesAsync();

                return Ok(role);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("Owner/Update")]
        [ProducesResponseType(typeof(Role), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Role>> Update([FromBody] Role role)
        {
            try
            {
                Role updatedRole = await _context.Roles.FindAsync(role.Id);
                if (updatedRole == null)
                    return BadRequest($"Не существует роли с Id = {role.Id}");

                updatedRole.Name = role.Name;
                updatedRole.Description = role.Description;

                await _context.Roles.AddAsync(updatedRole);
                await _context.SaveChangesAsync();

                return Ok(updatedRole);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("Owner/Delete/{id}")]
        [ProducesResponseType(typeof(Role), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Role>> Delete(int id)
        {
            try
            {
                Role deletedRole = await _context.Roles.FindAsync(id);
                if (deletedRole == null)
                    return BadRequest($"Не существует роли с Id = {id}");

                _context.Roles.Remove(deletedRole);
                await _context.SaveChangesAsync();

                return Ok(deletedRole);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
