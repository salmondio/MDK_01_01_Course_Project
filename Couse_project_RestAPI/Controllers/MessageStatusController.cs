using Couse_project_RestAPI.Contexts;
using Couse_project_RestAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Couse_project_RestAPI.Controllers
{
    /// <summary>
    /// Контроллер для управления статусами сообщений (отзывов и жалоб)
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class MessageStatusController : ControllerBase
    {
        private readonly DbContextMain _context;

        public MessageStatusController(DbContextMain context)
        {
            _context = context;
        }


        /// <summary>
        /// Получить список всех статусов (доступно всем)
        /// </summary>
        /// <returns>Список статусов</returns>
        [HttpGet("List")]
        [ProducesResponseType(typeof(IEnumerable<MessageStatus>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<IEnumerable<MessageStatus>>> List()
        {
            try
            {
                var statusList = await _context.MessageStatuses.ToArrayAsync();

                if (!statusList.Any())
                    return NotFound("Статусы не найдены");

                return Ok(statusList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Получить статус по ID (доступно всем)
        /// </summary>
        /// <param name="id">ID статуса</param>
        /// <returns>Статус</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MessageStatus), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<MessageStatus>> GetStatus(int id)
        {
            try
            {
                var status = await _context.MessageStatuses.FindAsync(id);

                if (status == null)
                    return NotFound($"Статуса с id = {id} не существует");

                return Ok(status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Добавить новый статус (только для Owner)
        /// </summary>
        /// <param name="status">Модель статуса</param>
        /// <returns>Созданный статус</returns>
        [HttpPost("Owner/Add")]
        [ProducesResponseType(typeof(MessageStatus), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<MessageStatus>> Add([FromBody] MessageStatus status)
        {
            try
            {
                if (status == null)
                    return BadRequest("Статус не может быть null");

                if (await _context.MessageStatuses.AnyAsync(s => s.Name == status.Name))
                    return BadRequest($"Статус с именем '{status.Name}' уже существует");

                status.Id = 0;
                await _context.MessageStatuses.AddAsync(status);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetStatus), new { id = status.Id }, status);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Обновить существующий статус (только для Owner)
        /// </summary>
        /// <param name="status">Модель статуса с обновлёнными данными</param>
        /// <returns>Обновлённый статус</returns>
        [HttpPut("Owner/Update")]
        [ProducesResponseType(typeof(MessageStatus), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<MessageStatus>> Update([FromBody] MessageStatus status)
        {
            try
            {
                if (status == null)
                    return BadRequest("Статус не может быть null");

                var updatedStatus = await _context.MessageStatuses.FindAsync(status.Id);

                if (updatedStatus == null)
                    return NotFound($"Статуса с id = {status.Id} не существует");

                // Проверяем, что новое имя не занято другим статусом
                if (await _context.MessageStatuses.AnyAsync(s => s.Name == status.Name && s.Id != status.Id))
                    return BadRequest($"Статус с именем '{status.Name}' уже существует");

                updatedStatus.Name = status.Name;
                updatedStatus.Description = status.Description;

                await _context.SaveChangesAsync();

                return Ok(updatedStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }


        /// <summary>
        /// Удалить статус по ID (только для Owner)
        /// </summary>
        /// <param name="id">ID статуса</param>
        /// <returns>Удалённый статус</returns>
        [HttpDelete("Owner/Delete/{id}")]
        [ProducesResponseType(typeof(MessageStatus), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [Authorize(Roles = "Owner")]
        public async Task<ActionResult<MessageStatus>> Delete(int id)
        {
            try
            {
                var deletedStatus = await _context.MessageStatuses.FindAsync(id);

                if (deletedStatus == null)
                    return NotFound($"Статуса с id = {id} не существует");

                // Проверяем, не используется ли статус в отзывах или жалобах
                var isUsedInReviews = await _context.Reviews.AnyAsync(r => r.Id_status == id);
                var isUsedInReports = await _context.Reports.AnyAsync(r => r.Id_status == id);

                if (isUsedInReviews || isUsedInReports)
                    return BadRequest("Невозможно удалить статус, так как он используется в отзывах или жалобах");

                _context.MessageStatuses.Remove(deletedStatus);
                await _context.SaveChangesAsync();

                return Ok(deletedStatus);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}