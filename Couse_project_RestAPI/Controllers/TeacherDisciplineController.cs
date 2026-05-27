using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Couse_project_RestAPI.Controllers
{
    public class TeacherDisciplineController : Controller
    {
        private readonly DbContextMain _context;

        public TeacherDisciplineController(DbContextMain context)
        {
            _context = context;
        }



        [HttpGet("List")]
        [ProducesResponseType(typeof(IEnumerable<TeacherDiscipline>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<TeacherDiscipline>>> List()
        {
            try
            {
                IEnumerable<TeacherDiscipline> teacherDisciplineList = await _context.TeacherDisciplines
                    .Include(td => td.Discipline)
                    .Include(tc => tc.Teacher)
                    .ToArrayAsync();

                return Ok(teacherDisciplineList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("ListByTeacher/{idTeacher}")]
        [ProducesResponseType(typeof(IEnumerable<TeacherDiscipline>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ICollection<TeacherDiscipline>>> ListByTeacher(int idTeacher)
        {
            try
            {
                IEnumerable<TeacherDiscipline> teacherDisciplineList = await _context.TeacherDisciplines
                    .Where(td => td.Id_teacher == idTeacher)
                    .Include(td => td.Discipline)
                    .ToArrayAsync();

                if (teacherDisciplineList == null)
                    return NotFound($"У преподавателя с id = {idTeacher} не указан ни одна дисциплина, которую он преподает");

                return Ok(teacherDisciplineList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Add")]
        [ProducesResponseType(typeof(TeacherDiscipline), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeacherDiscipline>> Add([FromBody] TeacherDiscipline teacherDiscipline)
        {
            try
            {
                await _context.TeacherDisciplines.AddAsync(teacherDiscipline);
                await _context.SaveChangesAsync();

                return Ok(teacherDiscipline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPut("Update")]
        [ProducesResponseType(typeof(TeacherDiscipline), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeacherDiscipline>> Update([FromBody] TeacherDiscipline teacherDiscipline)
        {
            try
            {
                if (_context.TeacherDisciplines.Any(td =>
                td.Id_teacher == teacherDiscipline.Id_teacher && td.Id_discipline == teacherDiscipline.Id_discipline))
                    return BadRequest("Такая запись уже существует");

                TeacherDiscipline updatedTeacherDiscipline = await _context.TeacherDisciplines
                    .FirstOrDefaultAsync(td => td.Id == teacherDiscipline.Id);

                if (updatedTeacherDiscipline == null)
                    return NotFound($"Не существует преподавателя с id = {teacherDiscipline.Id_teacher}, " +
                        $"преподающего дисциплину с id = {teacherDiscipline.Id_discipline}");

                updatedTeacherDiscipline.Id_teacher = teacherDiscipline.Id_teacher;
                updatedTeacherDiscipline.Id_discipline = teacherDiscipline.Id_discipline;

                await _context.SaveChangesAsync();

                return Ok(updatedTeacherDiscipline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("Delete/{id}")]
        [ProducesResponseType(typeof(TeacherDiscipline), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<TeacherDiscipline>> Delete(int id)
        {

            try
            {
                TeacherDiscipline teacherDiscipline = await _context.TeacherDisciplines.FindAsync(id);

                if (teacherDiscipline == null)
                    return NotFound($"Записи с id = {id} не существует");

                _context.TeacherDisciplines.Remove(teacherDiscipline);
                await _context.SaveChangesAsync();

                return Ok(teacherDiscipline);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}