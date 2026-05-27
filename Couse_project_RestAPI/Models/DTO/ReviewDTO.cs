namespace Couse_project_RestAPI.Models.DTO
{
    public class ReviewDTO
    {
        public int Id { get; set; }
        public DateTime Date_time { get; set; }
        public string Text { get; set; }
        public bool Is_active { get; set; }


        public UserDTO? Teacher { get; set; }
        public MessageStatus? Status { get; set; }
    }
}