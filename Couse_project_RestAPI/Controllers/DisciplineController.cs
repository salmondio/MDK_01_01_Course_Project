using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
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
    }
}
