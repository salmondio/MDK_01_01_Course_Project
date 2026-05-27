using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    [Table("Review")]
    public class Review
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Id студента, отправившего отзыв
        /// </summary>
        [Required]
        public int Id_student { get; set; }
        /// <summary>
        /// Id преподавателя, на которого оставлен отзыв
        /// </summary>
        [Required]
        public int Id_teacher { get; set; }
        /// <summary>
        /// Id статуса отзыва
        /// </summary>
        [Required]
        public int Id_status { get; set; }
        /// <summary>
        /// Id модератора, который проверит отзыв
        /// </summary>
        public int Id_inspector { get; set; }
        /// <summary>
        /// Дата и время отправки отзыва
        /// </summary>
        [Required]
        public DateTime Date_time { get; set; }
        /// <summary>
        /// Текст отзыва
        /// </summary>
        [Required]
        public string Text { get; set; }
        /// <summary>
        /// Флаг Is_active определяет активен ли отзыв на данный момент, или был удален отправителем
        /// </summary>
        [Required]
        public bool Is_active { get; set; }


        [ForeignKey(nameof(Id_student))]
        public virtual User? Student { get; set; }

        [ForeignKey(nameof(Id_teacher))]
        public virtual User? Teacher { get; set; }

        [ForeignKey(nameof(Id_status))]
        public virtual MessageStatus? Status { get; set; }

        [ForeignKey(nameof(Id_inspector))]
        public virtual User? Inspector { get; set; }
    }
}
