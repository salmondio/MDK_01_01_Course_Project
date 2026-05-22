using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    /// <summary>
    /// Каждый пользователь имеет свою роль,
    /// от неё зависят контент, который может
    /// просматривать пользователь и действия,
    /// которые он может совершать в системе
    /// </summary>
    [Table("Role")]
    public class Role
    {
        [Key]
        public int Id { get; set; }
        /// <summary>
        /// Название роли
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Описание роли
        /// </summary>
        public string Description { get; set; }
    }
}
