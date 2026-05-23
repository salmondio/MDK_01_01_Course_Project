using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    /// <summary>
    /// Таблица хранит статусы отправки Отзывово и Жалоб.
    /// Прошли отзывы или жалобы модерацию, дошли ли они до адресата.
    /// </summary>
    [Table("Message_status")]
    public class MessageStatus
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Название статуса
        /// </summary>
        [MaxLength(100)]
        public string Name { get; set; }
        /// <summary>
        /// Описание статуса
        /// </summary>
        [MaxLength(1000)]
        public string Description { get; set; }
    }
}
