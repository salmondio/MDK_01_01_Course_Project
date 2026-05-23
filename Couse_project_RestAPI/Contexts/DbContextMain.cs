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

        public DbContextMain(DbContextOptions<DbContextMain> options) : base(options) { }
    }
}