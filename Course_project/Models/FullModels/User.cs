using System.Text.Json.Serialization;

namespace Course_project_wpf.Models.FullModels
{
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("id_role")]
        public int Id_role { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("lastname")]
        public string Lastname { get; set; }

        [JsonPropertyName("surname")]
        public string? Surname { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("phone_number")]
        public string? Phone_number { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        public string? Token { get; set; }

        [JsonPropertyName("is_active")]
        public bool Is_active { get; set; }
        [JsonPropertyName("role")]
        public string Role { get; set; }
    }
}