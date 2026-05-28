using System.ComponentModel.DataAnnotations;

namespace Couse_project_RestAPI.Models.DTO
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
