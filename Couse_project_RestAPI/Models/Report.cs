using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    [Table("Report")]
    public class Report
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Id студента, отправившего жалобу
        /// </summary>
        [Required]
        public int Id_student { get; set; }
        /// <summary>
        /// Id преподавателя, на которого оставлена жалоба
        /// </summary>
        [Required]
        public int Id_teacher { get; set; }
        /// <summary>
        /// Id статуса жалобы
        /// </summary>
        [Required]
        public int Id_status { get; set; }
        /// <summary>
        /// Id модератора/администратора, который проверит жалобу
        /// </summary>
        public int Id_inspector { get; set; }
        /// <summary>
        /// Дата и время отправки жалобы
        /// </summary>
        [Required]
        public DateTime Date_time { get; set; }
        /// <summary>
        /// Текст жалобы
        /// </summary>
        [Required]
        public string Text { get; set; }


        [ForeignKey(nameof(Id_student))]
        public virtual User? Student { get; set; }

        [ForeignKey(nameof(Id_teacher))]
        public virtual User? Teacher { get; set; }

        [ForeignKey(nameof(Id_status))]
        public virtual User? Status { get; set; }

        [ForeignKey(nameof(Id_inspector))]
        public virtual User? Inspector { get; set; }
    }
}
