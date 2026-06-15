using System.Text.Json.Serialization;

namespace Course_project_wpf.Models.DTO
{
    public class UserDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("lastname")]
        public string? Lastname { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("email")]
        public string? Email { get; set; }

        [JsonPropertyName("phone_number")]
        public string? Phone_number { get; set; }

        [JsonPropertyName("roleName")]
        public string? RoleName { get; set; }
    }
}