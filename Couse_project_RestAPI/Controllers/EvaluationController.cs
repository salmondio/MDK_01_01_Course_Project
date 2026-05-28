using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Helpers;
using Couse_project_RestAPI.Models;
using Couse_project_RestAPI.Models.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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


        /// <summary>
        /// Позволяет админу получить одну/список оценок
        /// </summary>
        /// <returns></returns>
        [HttpGet("Admin/List")]
        [ProducesResponseType(typeof(IEnumerable<Evaluation>), 200)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Owner")]
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


        /// <summary>
        /// Возвращает информацию о конкретной оценке по id студента и преподавателя.
        /// </summary>
        /// <param name="idStudent"></param>
        /// <param name="idTeacher"></param>
        /// <returns></returns>
        [HttpGet("Admin/{idStudent}/{idTeacher}")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Admin,Owner")]
        public async Task<ActionResult<Evaluation>> GetEvaluationAdmin(int idStudent, int idTeacher)
        {
            try
            {
                // Поиск оценки с соответствующими id студента и препода
                Evaluation evaluation = await _context.Evaluations.FirstOrDefaultAsync(e =>
                e.Id_student == idStudent && e.Id_teacher == idTeacher);
                // Если таковой нет
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
        /// Позволяет получить студенту список оценок, выставленных им или список оценок преподу, выставленных ему
        /// </summary>
        /// <returns></returns>
        [HttpGet("List")]
        [ProducesResponseType(typeof(IEnumerable<EvaluationDTO>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Teacher,Student")]
        public async Task<ActionResult<IEnumerable<EvaluationDTO>>> ListForTeacherOrStudent()
        {
            try
            {
                // Id и роль пользователя, отправившего запрос
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role).Value;

                // Берем таблицу оценок для запроса
                var query = _context.Evaluations.AsQueryable();

                // Если запрос отправил преподаватель, то отбираем оценки, поставленные ему
                if (role == "Teacher")
                    query = query.Where(e => e.Id_teacher == userId);
                else // Иначе отбираем оценки, поставленные студентом
                    query = query.Where(e => e.Id_student == userId);

                // Преобразуем выборку оценок в выборку оценок с обрезанной информацией,
                // в зависимости от того, кто отправил запрос возвращаемая информация разная
                var evaluationList = await query
                    .Select(e => new EvaluationDTO
                    {
                        Presentation = e.Presentation,
                        Attitude = e.Attitude,
                        Responsiveness = e.Responsiveness,
                        Date = DateOnly.FromDateTime(e.Date_time),
                        Teacher = role == "Student" ? new UserDTO
                        {
                            Id = e.Teacher.Id,
                            Name = e.Teacher.Name,
                            Lastname = e.Teacher.Lastname,
                            Surname = e.Teacher.Surname
                        } : null
                    })
                    .ToArrayAsync();

                // Если нужных оценок нет
                if (!evaluationList.Any())
                    return NotFound($"Нет оценок для пользователя {userId}");

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
        [ProducesResponseType(403)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student, Teacher")]
        public async Task<ActionResult<EvaluationDTO>> GetEvaluationForStudentOrTeacher(int idStudent, int idTeacher)
        {
            try
            {
                // Id и роль пользователя, отправившего запрос
                var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
                var role = User.FindFirst(ClaimTypes.Role).Value;

                // Проверка прав
                if (role == "Student" && idStudent != userId)
                    return Forbid("Вы можете просматривать только свои оценки");
                if (role == "Teacher" && idTeacher != userId)
                    return Forbid("Вы можете просматривать только оценки, выставленные вам");

                // Ищем нужную оценку
                var evaluation = await _context.Evaluations
                    .Include(e => e.Teacher)
                    .FirstOrDefaultAsync(e => e.Id_student == idStudent && e.Id_teacher == idTeacher);
                if (evaluation == null)
                    return NotFound($"Оценка не найдена");

                // Преобразуем в упрощенную модель
                var evaluationDTO = new EvaluationDTO
                {
                    Presentation = evaluation.Presentation,
                    Attitude = evaluation.Attitude,
                    Responsiveness = evaluation.Responsiveness,
                    Date = DateOnly.FromDateTime(evaluation.Date_time),
                    Date_time = evaluation.Date_time
                };

                // Для студента добавляем информацию о преподе
                if (role == "Student")
                {
                    evaluationDTO.Teacher = new UserDTO
                    {
                        Id = evaluation.Teacher.Id,
                        Name = evaluation.Teacher.Name,
                        Lastname = evaluation.Teacher.Lastname,
                        Surname = evaluation.Teacher.Surname
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
        /// Добавление оценки преподавателя студентом в БД
        /// </summary>
        /// <param name="evaluationCreateDTO"></param>
        /// <returns></returns>
        [HttpPost("Add")]
        [ProducesResponseType(typeof(EvaluationCreateDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<EvaluationCreateDTO>> Add([FromBody] EvaluationCreateDTO evaluationCreateDTO)
        {
            try
            {
                // Если студент уже поставил оценку этому преподу
                if (await _context.Evaluations.AnyAsync(e =>
                e.Id_student == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) &&
                e.Id_teacher == evaluationCreateDTO.Id_teacher))
                    return BadRequest($"Оценка студента с Id = {User.FindFirst(ClaimTypes.NameIdentifier).Value} " +
                        $"преподавателю с Id = {evaluationCreateDTO.Id_teacher} уже существует");

                //if (!await _context.Evaluations.AnyAsync(e => e.Id_teacher == evaluationCreateDTO.Id_teacher))
                //    return NotFound($"Вы уже поставили оценку этому преподавателю");

                // Создание объекта оценки полноценной модели из урезанной
                Evaluation evaluation = new Evaluation()
                {
                    Id_student = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value),
                    Id_teacher = evaluationCreateDTO.Id_teacher,
                    Date_time = DateTime.Now,
                    Presentation = evaluationCreateDTO.Presentation,
                    Attitude = evaluationCreateDTO.Attitude,
                    Responsiveness = evaluationCreateDTO.Responsiveness
                };

                // Добавляем и сохраняем в БД оценку
                await _context.Evaluations.AddAsync(evaluation);
                await _context.SaveChangesAsync();

                return Ok(evaluationCreateDTO);
            }
            catch (Exception ex)
            {
                await LogHelper.Log("Error: Не удалось добавить оценку Add. " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Добавление оценки преподавателя студентом в БД
        /// </summary>
        /// <param name="evaluation"></param>
        /// <returns></returns>
        [HttpPost("Owner/Add")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<EvaluationCreateDTO>> OwnerAdd([FromBody] Evaluation evaluation)
        {
            try
            {
                // Если студент уже поставил оценку этому преподу
                if (await _context.Evaluations.AnyAsync(e =>
                e.Id_student == evaluation.Id_student &&
                e.Id_teacher == evaluation.Id_teacher))
                    return BadRequest($"Оценка студента с Id = {evaluation.Id_student} " +
                        $"преподавателю с Id = {evaluation.Id_teacher} уже существует");

                // Добавляем и сохраняем в БД оценку
                await _context.Evaluations.AddAsync(evaluation);
                await _context.SaveChangesAsync();

                return Ok(evaluation);
            }
            catch (Exception ex)
            {
                await LogHelper.Log("Error: Не удалось добавить оценку OwnerAdd. " + ex.Message);
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Позволяет обновить оценку студенту, который её выставил
        /// </summary>
        /// <param name="evaluationCreateDTO"></param>
        /// <returns></returns>
        [HttpPut("Update")]
        [ProducesResponseType(typeof(EvaluationCreateDTO), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<EvaluationCreateDTO>> Update([FromBody] EvaluationCreateDTO evaluationCreateDTO)
        {
            try
            {
                // Ищем оценку выставленную этим студентом этому преподу
                Evaluation updatedEvaluation = await _context.Evaluations
                    .FirstOrDefaultAsync(e => e.Id_student == int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value) && 
                    e.Id_teacher == evaluationCreateDTO.Id_teacher);

                // Если такой оценки нет
                if (updatedEvaluation == null)
                    return NotFound($"Не существует оценки, поставленной студентом с id = {User.FindFirst(ClaimTypes.NameIdentifier).Value} преподавателю с id = {evaluationCreateDTO.Id_teacher}");

                // Обновляем данные и сохраняем в БД
                updatedEvaluation.Presentation = evaluationCreateDTO.Presentation;
                updatedEvaluation.Attitude = evaluationCreateDTO.Attitude;
                updatedEvaluation.Responsiveness = evaluationCreateDTO.Responsiveness;
                updatedEvaluation.Date_time = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(evaluationCreateDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Позволяет обновить оценку студенту, который её выставил
        /// </summary>
        /// <param name="evaluation"></param>
        /// <returns></returns>
        [HttpPut("Owner/Update")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Evaluation>> OwnerUpdate([FromBody] Evaluation evaluation)
        {
            try
            {
                // Ищем оценку выставленную этим студентом этому преподу
                Evaluation updatedEvaluation = await _context.Evaluations
                    .FirstOrDefaultAsync(e => e.Id_student == evaluation.Id_student &&
                    e.Id_teacher == evaluation.Id_teacher);

                // Если такой оценки нет
                if (updatedEvaluation == null)
                    return NotFound($"Не существует оценки, поставленной студентом с id = {evaluation.Id_student} преподавателю с id = {evaluation.Id_teacher}");

                // Обновляем данные и сохраняем в БД
                updatedEvaluation.Id_student = evaluation.Id_student;
                updatedEvaluation.Id_teacher = evaluation.Id_teacher;
                updatedEvaluation.Presentation = evaluation.Presentation;
                updatedEvaluation.Attitude = evaluation.Attitude;
                updatedEvaluation.Responsiveness = evaluation.Responsiveness;
                updatedEvaluation.Date_time = DateTime.Now;
                await _context.SaveChangesAsync();

                return Ok(updatedEvaluation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Позволяет обновить оценку студенту, который её выставил
        /// </summary>
        /// <param name="idStudent"></param>
        /// <param name="idTeacher"></param>
        /// <returns></returns>
        [HttpDelete("Owner/Delete/{idStudent}/{idTeacher}")]
        [ProducesResponseType(typeof(Evaluation), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<Evaluation>> Delete(int idStudent, int idTeacher)
        {
            try
            {
                // Ищем оценку выставленную этим студентом этому преподу
                Evaluation deletedEvaluation = await _context.Evaluations
                    .FirstOrDefaultAsync(e => e.Id_student == idStudent &&
                    e.Id_teacher == idTeacher);

                // Если такой оценки нет
                if (deletedEvaluation == null)
                    return NotFound($"Не существует оценки, поставленной студентом с id = {idStudent} преподавателю с id = {idTeacher}");

                // Обновляем данные и сохраняем в БД
                _context.Remove(deletedEvaluation);
                await _context.SaveChangesAsync();

                return Ok(deletedEvaluation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
