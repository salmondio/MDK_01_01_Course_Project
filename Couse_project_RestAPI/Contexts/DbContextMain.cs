using Microsoft.EntityFrameworkCore;
using Couse_project_RestAPI.Models;

namespace Couse_project_RestAPI.Contexts
{
    public class DbContextMain : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Discipline> Disciplines { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<MessageStatus> MessageStatuses { get; set; }
        public DbSet<Report> Reports { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<TeacherDiscipline> TeacherDisciplines { get; set; }
        public DbContextMain()
        {
            Database.EnsureCreated();
            Users.Load();
            Disciplines.Load();
            Evaluations.Load();
            MessageStatuses.Load();
            Reports.Load();
            Reviews.Load();
            Roles.Load();
            TeacherDisciplines.Load();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=10.0.201.112;Trusted_Connection=False;Database=base1_ISP_23_2_8;User=ISP_23_2_8;Pwd=egW19je7D1_;Encrypt=false;");
        }
    }
}