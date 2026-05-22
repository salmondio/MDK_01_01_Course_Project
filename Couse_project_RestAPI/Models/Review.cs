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
        public int Id_student { get; set; }
        /// <summary>
        /// Id преподавателя, на которого оставлен отзыв
        /// </summary>
        public int Id_teacher { get; set; }
        /// <summary>
        /// Id статуса отзыва
        /// </summary>
        public int Id_status { get; set; }
        /// <summary>
        /// Id модератора, который проверит отзыв
        /// </summary>
        public int Id_inspector { get; set; }
        /// <summary>
        /// Текст отзыва
        /// </summary>
        public string Text { get; set; }
    }
}
