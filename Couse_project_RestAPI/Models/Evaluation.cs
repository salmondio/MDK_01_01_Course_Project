using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    [Table("Evaluation")]
    [PrimaryKey(nameof(Id_student), nameof(Id_teacher))]
    public class Evaluation
    {
        /// <summary>
        /// Id студента, поставившего оценку
        /// </summary>
        [Column(Order = 0)]
        public int Id_student { get; set; }
        /// <summary>
        /// Id преподавателя, которому поставлена оценка
        /// </summary>
        [Column(Order = 1)]
        public int Id_teacher { get; set; }
        /// <summary>
        /// Дата и время выставления оценки
        /// </summary>
        [Required]
        public DateTime Date_time { get; set; }
        /// <summary>
        /// Оценка способности подачи материала преподаателя
        /// </summary>
        [Required]
        [Range(1, 9)]
        public byte Presentation { get; set; }
        /// <summary>
        /// Оценка отношения преподавателя к студентам
        /// </summary>
        [Required]
        [Range(1, 9)]
        public byte Attitude { get; set; }
        /// <summary>
        /// Оценка отзывчивости преподавателя к студентам
        /// </summary>
        [Required]
        [Range(1, 9)]
        public byte Responsiveness { get; set; }


        [NotMapped]
        public double AverageScore => (Presentation + Attitude + Responsiveness) / 3.0;

        [ForeignKey(nameof(Id_student))]
        public virtual User? Student { get; set; }

        [ForeignKey(nameof(Id_teacher))]
        public virtual User? Teacher { get; set; }

    }
}
