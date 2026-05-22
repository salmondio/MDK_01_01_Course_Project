using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        [Column("Id")]
        public int Id { get; set; }

        /// <summary>
        /// Роль пользователя
        /// </summary>
        [Column("Id_role")]
        public int Id_role { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        [Column("Name")]
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        [Column("Lastname")]
        [Required]
        [MaxLength(100)]
        public string Lastname { get; set; } = string.Empty;

        /// <summary>
        /// Отчество пользователя
        /// </summary>
        [Column("Surname")]
        [MaxLength(100)]
        public string? Surname { get; set; }

        /// <summary>
        /// Адрес почты пользователя
        /// </summary>
        [Column("Email")]
        [Required]
        [MaxLength(100)]
        [EmailAddress]  // Доп. валидация
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        [Column("Phone_number")]
        [MaxLength(20)]
        public string? Phone_number { get; set; }

        /// <summary>
        /// Зашифрованный пароль пользователя
        /// </summary>
        [Column("Password")]
        [Required]
        [MaxLength(256)]
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Токен доступа пользователя
        /// </summary>
        [Column("Token")]
        [MaxLength(256)]
        public string? Token { get; set; }
        /// <summary>
        /// Флаг Is_active определяет, есть ли у пользователя
        /// полномочия, соответсвующие его роли. Если пользователь
        /// не активен, он будет считаться неавторизованным пользователем.
        /// Активировать и дизактивировать пользователей может администратор.
        /// </summary>
        [Column("Is_active")]
        public bool Is_active { get; set; } = true;

        
        [ForeignKey(nameof(Id_role))]
        public virtual Role? Role { get; set; }
    }
}
