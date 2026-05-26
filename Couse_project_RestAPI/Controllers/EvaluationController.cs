using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Couse_project_RestAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

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
        [Authorize(Roles = "Admin")]
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


        [HttpGet("ListForTeacherOrStudent")]
        [ProducesResponseType(typeof(IEnumerable<EvaluationDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Teacher,Student")]
        public async Task<ActionResult<IEnumerable<EvaluationDTO>>> ListForTeacherOrStudent()
        {
            try
            {
                IEnumerable<EvaluationDTO> evaluationList;

                if (User.FindFirst("role")?.Value == "Teacher")
                {
                    evaluationList = await _context.Evaluations
                    .Where(e => e.Id_teacher == int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value ?? "0"))
                    .Select(e => new EvaluationDTO
                    {
                        Presentation = e.Presentation,
                        Attitude = e.Attitude,
                        Responsiveness = e.Responsiveness,
                        Date = DateOnly.FromDateTime(e.Date_time)
                    })
                    .ToArrayAsync();

                    if (!evaluationList.Any())
                        return NotFound($"Нет оценок, выставленных преподавателю с Id = {User.FindFirst(JwtRegisteredClaimNames.Sub).Value}");
                }
                else
                {
                    evaluationList = await _context.Evaluations
                    .Where(e => e.Id_student == int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value ?? "0"))
                    .Include(e => e.Teacher)
                    .Select(e => new EvaluationDTO
                    {
                        Presentation = e.Presentation,
                        Attitude = e.Attitude,
                        Responsiveness = e.Responsiveness,
                        Date = DateOnly.FromDateTime(e.Date_time),
                        Date_time = e.Date_time,
                        Teacher = new UserDTO
                        {
                            Id = e.Teacher.Id,
                            Name = e.Teacher.Name,
                            Lastname = e.Teacher.Lastname,
                            Surname = e.Teacher.Surname
                        }
                    })
                    .ToArrayAsync();

                    if (!evaluationList.Any())
                        return NotFound($"Нет оценок, выставленных студентом с Id = {User.FindFirst(JwtRegisteredClaimNames.Sub).Value}");
                }

                return Ok(evaluationList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("ListOfStudentEvaluations")]
        [ProducesResponseType(typeof(IEnumerable<EvaluationDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<EvaluationDTO>>> ListOfStudentEvaluations()
        {
            try
            {
                IEnumerable<EvaluationDTO> evaluationList = await _context.Evaluations
                    .Where(e => e.Id_student == int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value))
                    .Include(e => e.Teacher)
                    .Select(e => new EvaluationDTO
                    {
                        Presentation = e.Presentation,
                        Attitude = e.Attitude,
                        Responsiveness = e.Responsiveness,
                        Date = DateOnly.FromDateTime(e.Date_time),
                        Date_time = e.Date_time,
                        Teacher = new UserDTO
                        {
                            Id = e.Teacher.Id,
                            Name = e.Teacher.Name,
                            Lastname = e.Teacher.Lastname,
                            Surname = e.Teacher.Surname
                        }
                    })
                    .ToArrayAsync();

                if (!evaluationList.Any())
                    return NotFound($"Не существует оценок, поставленных студентом с id = {User.FindFirst(JwtRegisteredClaimNames.Sub).Value}");

                return Ok(evaluationList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        [HttpGet("ListOfTeacherEvaluations")]
        [ProducesResponseType(typeof(IEnumerable<EvaluationDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<EvaluationDTO>>> ListOfTeacherEvaluations()
        {
            try
            {
                IEnumerable<EvaluationDTO> evaluationList = await _context.Evaluations
                    .Where(e => e.Id_teacher == int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value))
                    .Select(e => new EvaluationDTO
                    {
                        Presentation = e.Presentation,
                        Attitude = e.Attitude,
                        Responsiveness = e.Responsiveness,
                        Date = DateOnly.FromDateTime(e.Date_time),
                    })
                    .ToArrayAsync();

                if (!evaluationList.Any())
                    return NotFound($"Не существует оценок, поставленных преподавателю с id = {User.FindFirst(JwtRegisteredClaimNames.Sub).Value}");

                return Ok(evaluationList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Возвращает информацию о конкретной оценке по id студента и преподавателя.
        /// В зависимости от роли будет выводиться разный объем данных
        /// </summary>
        /// <param name="idStudent"></param>
        /// <param name="idTeacher"></param>
        /// <returns></returns>
        [HttpGet("{idStudent}/{idTeacher}")]
        [ProducesResponseType(typeof(EvaluationDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student, Teacher")]
        public async Task<ActionResult<EvaluationDTO>> GetEvaluationForStudentOrTeacher(int idStudent, int idTeacher)
        {
            try
            {
                // Возвращаемый объект
                EvaluationDTO evaluationDTO;

                // Если преподаватель хочет посмотреть оценку
                if (User.FindFirst("role")?.Value == "Student")
                {
                    // Проверка полномочий
                    if (idStudent != int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value ?? "0"))
                        return Forbid("Вы не можете просматривать оценки, поставленные не вами");

                    // Поиск оценки с соответствующими id студента и препода
                    Evaluation evaluation = await _context.Evaluations
                        .Include(e => e.Teacher)
                        .FirstOrDefaultAsync(e => e.Id_student == idStudent && e.Id_teacher == idTeacher);
                    if (evaluation == null)
                        return NotFound($"Не существует оценки, выставленной студентом с Id = {idStudent} преподавателю с Id = {idTeacher}");

                    // Препобразую полноценную модель оценки в модель с обрезанными данными
                    evaluationDTO = new EvaluationDTO()
                    {
                        Presentation = evaluation.Presentation,
                        Attitude = evaluation.Attitude,
                        Responsiveness = evaluation.Responsiveness,
                        Date = DateOnly.FromDateTime(evaluation.Date_time),
                        Date_time = evaluation.Date_time,
                        Teacher = new UserDTO
                        {
                            Id = evaluation.Teacher.Id,
                            Name = evaluation.Teacher.Name,
                            Lastname = evaluation.Teacher.Lastname,
                            Surname = evaluation.Teacher.Surname
                        }
                    };
                }
                else // Если же студент хочет посмотреть оценку
                {
                    // Проверка полномочий
                    if (idTeacher != int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value ?? "0"))
                        return Forbid("Вы не можете просматривать оценки, выставленные не вам");

                    // Поиск оценки с соответствующими id студента и препода
                    Evaluation evaluation = await _context.Evaluations
                        .FirstOrDefaultAsync(e => e.Id_student == idStudent && e.Id_teacher == idTeacher);
                    if (evaluation == null)
                        return NotFound($"Не существует оценки, выставленной студентом с Id = {idStudent} преподавателю с Id = {idTeacher}");

                    // Препобразую полноценную модель оценки в модель с обрезанными данными
                    evaluationDTO = new EvaluationDTO()
                    {
                        Presentation = evaluation.Presentation,
                        Attitude = evaluation.Attitude,
                        Responsiveness = evaluation.Responsiveness,
                        Date = DateOnly.FromDateTime(evaluation.Date_time)
                    };
                }

                return Ok(evaluationDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Возвращает информацию о конкретной оценке по id студента и преподавателя.
        /// </summary>
        /// <param name="idStudent"></param>
        /// <param name="idTeacher"></param>
        /// <returns></returns>
        [HttpGet("{idStudent}/{idTeacher}")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<Evaluation>> GetEvaluationAdmin(int idStudent, int idTeacher)
        {
            try
            {
                // Поиск оценки с соответствующими id студента и препода
                Evaluation evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id_student == idStudent && e.Id_teacher == idTeacher);
                if (evaluation == null)
                    return NotFound($"Не существует оценки, выставленной студентом с Id = {idStudent} преподавателю с Id = {idTeacher}");

                return Ok(evaluation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Добавление оценки преподавателя студентом в БД
        /// </summary>
        /// <param name="evaluation"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        [ProducesResponseType(typeof(EvaluationDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<EvaluationDTO>> Add([FromBody] EvaluationDTO evaluationDTO)
        {
            try
            {
                // Проверка полномочий
                if (await _context.Evaluations.AnyAsync(e =>
                e.Id_student == int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value) &&
                e.Id_teacher == evaluationDTO.Teacher.Id))
                    return BadRequest($"Оценка студента с Id = {User.FindFirst(JwtRegisteredClaimNames.Sub).Value} " +
                        $"преподавателю с Id = {evaluationDTO.Teacher.Id} уже существует");

                // Создание объекта полноценной модели из урезанной
                Evaluation evaluation = new Evaluation()
                {
                    Id_student = int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value),
                    Id_teacher = evaluationDTO.Teacher.Id,
                    Date_time = DateTime.Now,
                    Presentation = evaluationDTO.Presentation,
                    Attitude = evaluationDTO.Attitude,
                    Responsiveness = evaluationDTO.Responsiveness
                };

                // Добавляем и сохраняем в БД оценку
                await _context.Evaluations.AddAsync(evaluation);
                await _context.SaveChangesAsync();

                return Ok(evaluationDTO);
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
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<Evaluation>> Update([FromBody] EvaluationDTO evaluationDTO)
        {
            try
            {
                // Ищем оценку выставленную этим студентом этому преподу
                Evaluation updatedEvaluation = await _context.Evaluations
                    .FirstOrDefaultAsync(e => e.Id_student == int.Parse(User.FindFirst(JwtRegisteredClaimNames.Sub).Value) && 
                    e.Id_teacher == evaluationDTO.Teacher.Id);

                // Если такой оценки нет
                if (updatedEvaluation == null)
                    return NotFound($"Не существует оценки, поставленной студентом с id = {User.FindFirst(JwtRegisteredClaimNames.Sub).Value} преподавателю с id = {evaluationDTO.Teacher.Id}");

                // Обновляем данные и сохраняем в БД
                updatedEvaluation.Presentation = evaluationDTO.Presentation;
                updatedEvaluation.Attitude = evaluationDTO.Attitude;
                updatedEvaluation.Responsiveness = evaluationDTO.Responsiveness;
                updatedEvaluation.Date_time = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(evaluationDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        //[HttpDelete("Delete")]
        //[ProducesResponseType(typeof(Evaluation), 200)]
        //[ProducesResponseType(404)]
        //[ProducesResponseType(500)]
        //public async Task<ActionResult<Evaluation>> Delete(int idStudent, int idTeacher)
        //{
        //    try
        //    {
        //        Evaluation evaluation = await _context.Evaluations.FirstOrDefaultAsync(e => e.Id_student == idStudent && e.Id_teacher == idTeacher);

        //        if (evaluation == null)
        //            return NotFound($"Не существует оценки, поставленной студентом с id = {evaluation.Id_student} преподавателю с id = {evaluation.Id_teacher}");

        //        _context.Evaluations.Remove(evaluation);
        //        await _context.SaveChangesAsync();

        //        return Ok(evaluation);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(500, ex.Message);
        //    }
        //}
    }
}
