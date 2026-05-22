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
        public int Id_student { get; set; }
        /// <summary>
        /// Id преподавателя, на которого оставлена жалоба
        /// </summary>
        public int Id_teacher { get; set; }
        /// <summary>
        /// Id статуса жалобы
        /// </summary>
        public int Id_status { get; set; }
        /// <summary>
        /// Id модератора/администратора, который проверит жалобу
        /// </summary>
        public int Id_inspector { get; set; }
        /// <summary>
        /// Текст жалобы
        /// </summary>
        public string Text { get; set; }
    }
}
