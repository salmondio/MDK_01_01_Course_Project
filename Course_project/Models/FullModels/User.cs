namespace Course_project_wpf.Models.FullModels
{
    public class User
    {
        public int Id { get; set; }

        public int Id_role { get; set; }

        public string Name { get; set; }

        public string Lastname { get; set; }

        public string? Surname { get; set; }

        public string Email { get; set; }

        public string? Phone_number { get; set; }

        public string Password { get; set; }

        public string? Token { get; set; }
        
        public bool Is_active { get; set; }
        public string Role { get; set; }
    }
}