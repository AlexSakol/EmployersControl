using Microsoft.EntityFrameworkCore;

namespace CRUDMVC.Models.Entities
{
    public class DataContext : DbContext
    {
        public DbSet<Employer> Employers { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            Department[] departments = {
                new Department { Id = 1, Name = "Логистики"}, new Department { Id = 2, Name = "Юр. отдел"},
                new Department { Id = 3, Name = "Контроля"}, new Department { Id = 4, Name = "АХО"},
                new Department { Id = 5, Name = "Приемная"}, new Department { Id = 6, Name = "Продаж"}
            };
            Employer[] employers = {
                new Employer() { Id = 1, Name = "Иван", Post = "Логист", Salary = 1000, DepartmentId = departments[0].Id},
                new Employer() { Id = 2, Name = "Петр", Post = "Юрист", Salary = 1200, DepartmentId =  departments[1].Id},
                new Employer() { Id = 3, Name = "Федр", Post = "Ревизор", Salary = 1100, DepartmentId = departments[2].Id}
            };
            Role[] roles =
            {
                new Role(){Id = 1, Name = "Admin"},
                new Role(){Id = 2, Name = "User"}
            };
            User[] users = {
                new User() { Id = 1, Login = "Admin", Password = Hasher.GetSha256Hash("95$@Admin"), RoleId = 1},
                new User() { Id = 2, Login = "User", Password = Hasher.GetSha256Hash("95$@User"), RoleId = 2}
            };
            modelBuilder.Entity<Department>().HasData(departments);
            modelBuilder.Entity<Employer>().HasData(employers);
            modelBuilder.Entity<Role>().HasData(roles);
            modelBuilder.Entity<User>().HasIndex(u => u.Login).IsUnique();
            modelBuilder.Entity<User>().HasData(users);
        }       
        
    }
}
