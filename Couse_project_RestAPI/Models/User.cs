using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Couse_project_RestAPI.Models
{
    [Table("User")]
    public class User
    {
        [Key]
        public int Id {  get; set; }
        /// <summary>
        /// Роль пользователя
        /// </summary>
        public int Id_role { get; set; }
        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Фамилия пользователя
        /// </summary>
        public string Lastname { get; set; }
        /// <summary>
        /// Отчество пользователя
        /// </summary>
        public string Surname { get; set; }
        /// <summary>
        /// Адрес почты пользователя
        /// </summary>
        public string Email { get; set; }
        /// <summary>
        /// Номер телефона пользователя
        /// </summary>
        public string Phone_number { get; set; }
        /// <summary>
        /// Зашифрованный пароль пользователя
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// Токен доступа пользователя
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Флаг Is_active определяет, есть ли у пользователя
        /// полномочия, соответсвующие его роли. Если пользователь
        /// не активен, он будет считаться неавторизованным пользователем.
        /// Активировать и дизактивировать пользователей может администратор.
        /// </summary>
        public bool Is_active { get; set; }
    }
}
