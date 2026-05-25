using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Couse_project_RestAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluationController : ControllerBase
    {
        private readonly DbContextMain _context;

        public EvaluationController(DbContextMain context)
        {
            _context = context;
        }



        [HttpGet("List")]
        [ProducesResponseType(typeof(IEnumerable<Evaluation>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Evaluation>>> List()
        {
            try
            {
                IEnumerable<Evaluation> evaluationList = await _context.Evaluations.ToArrayAsync();

                return Ok(evaluationList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("ListOfStudentReports")]
        [ProducesResponseType(typeof(IEnumerable<Evaluation>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Evaluation>>> ListOfStudentReports([FromQuery] int idStudent)
        {
            try
            {
                IEnumerable<Evaluation> evaluationList = await _context.Evaluations.Where(e => e.Id_student == idStudent).ToArrayAsync();

                if (evaluationList == null)
                    return NotFound($"Не существует оценок, поставленных студентом с id = {idStudent}");

                return Ok(evaluationList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("ListOfTeacherReports")]
        [ProducesResponseType(typeof(IEnumerable<Evaluation>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<Evaluation>>> ListOfTeacherReports([FromQuery] int idTeacher)
        {
            try
            {
                IEnumerable<Evaluation> evaluationList = await _context.Evaluations.Where(e => e.Id_teacher == idTeacher).ToArrayAsync();

                if (evaluationList == null)
                    return NotFound($"Не существует оценок, поставленных преподавателю с id = {idTeacher}");

                return Ok(evaluationList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("{idStudent}/{idTeacher}")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Evaluation>> GetEvaluation(int idStudent, int idTeacher)
        {
            try
            {
                Evaluation evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id_student == idStudent && e.Id_teacher == idTeacher);

                if (evaluation == null)
                    return NotFound($"Не существует оценки, поставленной студентом с id = {idStudent} преподавателю с id = {idTeacher}");

                return Ok(evaluation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Add")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Evaluation>> Add([FromBody] Evaluation evaluation)
        {
            try
            {
                await _context.Evaluations.AddAsync(evaluation);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetEvaluation), new { idStudent = evaluation.Id_student, idTeacher = evaluation.Id_teacher }, evaluation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpPost("Update")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Evaluation>> Update([FromBody] Evaluation evaluation)
        {
            try
            {
                Evaluation updatedEvaluation = await _context.Evaluations
                    .FirstOrDefaultAsync(e => e.Id_student == evaluation.Id_student && e.Id_teacher == evaluation.Id_teacher);

                if (updatedEvaluation == null)
                    return NotFound($"Не существует оценки, поставленной студентом с id = {evaluation.Id_student} преподавателю с id = {evaluation.Id_teacher}");

                updatedEvaluation.Presentation = evaluation.Presentation;
                updatedEvaluation.Attitude = evaluation.Attitude;
                updatedEvaluation.Responsiveness = evaluation.Responsiveness;

                await _context.SaveChangesAsync();

                return Ok(updatedEvaluation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpDelete("Delete")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<Evaluation>> Delete(int idStudent, int idTeacher)
        {
            try
            {
                Evaluation evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id_student == idStudent && e.Id_teacher == idTeacher);

                if (evaluation == null)
                    return NotFound($"Не существует оценки, поставленной студентом с id = {evaluation.Id_student} преподавателю с id = {evaluation.Id_teacher}");

                _context.Evaluations.Remove(evaluation);
                await _context.SaveChangesAsync();

                return Ok(evaluation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
