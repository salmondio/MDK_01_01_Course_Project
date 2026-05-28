using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Couse_project_RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DisciplineController : Controller
    {
        private readonly DbContextMain _context;

        public DisciplineController(DbContextMain context)
        {
            _context = context;
        }



        [HttpGet("List")]
        [ProducesResponseType(typeof(IEnumerable<Discipline>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Discipline>>> List()
        {
            try
            {
                IEnumerable<Discipline> disciplineList = await _context.Disciplines.ToArrayAsync();

                return Ok(disciplineList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Discipline), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Discipline>> GetDiscipline(int id)
        {
            try
            {
                Discipline discipline = await _context.Disciplines.FindAsync(id);

                if (discipline == null)
                    return NotFound($"Пользователя с id = {id} не существует");

                return Ok(discipline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Owner/Add")]
        [ProducesResponseType(typeof(Discipline), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Discipline>> Add([FromBody] Discipline discipline)
        {
            try
            {
                if (await _context.Disciplines.AnyAsync(d => d.Name == discipline.Name))
                    return BadRequest("Дисциплина с таким имененм уже существует");

                await _context.Disciplines.AddAsync(discipline);
                await _context.SaveChangesAsync();

                return Ok(discipline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("Owner/Update")]
        [ProducesResponseType(typeof(Discipline), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Discipline>> Update([FromBody] Discipline discipline)
        {
            try
            {
                Discipline updatedDiscipline = await _context.Disciplines.FindAsync(discipline.Id);
                if (updatedDiscipline == null)
                    return BadRequest($"Не существует дисциплины с Id = {discipline.Id}");

                updatedDiscipline.Name = discipline.Name;
                updatedDiscipline.Description = discipline.Description;

                await _context.Disciplines.AddAsync(discipline);
                await _context.SaveChangesAsync();

                return Ok(discipline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("Owner/Delete/{id}")]
        [ProducesResponseType(typeof(Discipline), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Discipline>> Delete(int id)
        {
            try
            {
                Discipline deletedDiscipline = await _context.Disciplines.FindAsync(id);
                if (deletedDiscipline == null)
                    return BadRequest($"Не существует дисциплины с Id = {id}");

                _context.Disciplines.Remove(deletedDiscipline);
                await _context.SaveChangesAsync();

                return Ok(deletedDiscipline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
