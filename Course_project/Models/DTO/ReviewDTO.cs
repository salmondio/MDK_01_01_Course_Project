using System.Text.Json.Serialization;
using Couse_project_RestAPI.Models;

namespace Course_project_wpf.Models.DTO
{
    public class ReviewDTO
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("date_time")]
        public DateTime Date_time { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("is_active")]
        public bool Is_active { get; set; }

        [JsonPropertyName("teacher")]
        public UserDTO? Teacher { get; set; }

        [JsonPropertyName("status")]
        public MessageStatus? Status { get; set; }
    }
}