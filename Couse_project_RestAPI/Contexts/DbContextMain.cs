using Microsoft.EntityFrameworkCore;

namespace Couse_project_RestAPI.Contexts
{
    public class DbContextMain
    {
        public DbSet<User> Users { get; set; }
        public DbContextMain()
        {
            Database.EnsureCreated();
            Users.Load();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=10.0.201.112;Trusted_Connection=False;Database=base1_ISP_23_2_8;User=ISP_23_2_8;Pwd=egW19je7D1_;Encrypt=false;");
        }
    }
}
}
