using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    /// <summary>
    /// Таблица дисциплин нужна для того, чтобы
    /// указывать дисциплины, которым обучает преподаватель.
    /// </summary>
    [Table("Discipline")]
    public class Discipline
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Название дисциплины
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание дисциплины
        /// </summary>
        public string Description { get; set; }
    }
}
