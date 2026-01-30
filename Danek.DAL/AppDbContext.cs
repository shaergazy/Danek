using Danek.DAL.Models.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Danek.Web.Models;

namespace Danek.DAL
{
    public class AppDbContext : IdentityDbContext<User, Role, string,
    IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>,
    IdentityRoleClaim<string>, IdentityUserToken<string>>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            builder.Entity<UserRole>()
                .HasOne(x => x.User)
                .WithMany(x => x.Roles)
                .HasForeignKey(x => x.UserId)
                .IsRequired();

            builder.Entity<UserRole>()
                .HasOne(x => x.Role)
                .WithMany(x => x.Users)
                .HasForeignKey(x => x.RoleId)
                .IsRequired();

            foreach (var x in builder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
                x.DeleteBehavior = DeleteBehavior.ClientCascade;

            // Добавьте Book seed data здесь
            builder.Entity<Book>().HasData(
                new Book
                {
                    Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                    Title = "Clean Code",
                    Author = "Robert C. Martin",
                    Intro = "A Handbook of Agile Software Craftsmanship.",
                    Description = "Classic book about writing clean, maintainable and readable code.",
                    Quantity = 5,
                    CreatedAt = new DateTime(2024, 1, 1)
                },
                new Book
                {
                    Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                    Title = "CLR via C#",
                    Author = "Jeffrey Richter",
                    Intro = "Deep dive into .NET CLR and C# internals.",
                    Description = "Advanced book for experienced .NET developers.",
                    Quantity = 3,
                    CreatedAt = new DateTime(2024, 1, 2)
                },
                new Book
                {
                    Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                    Title = "ASP.NET Core in Action",
                    Author = "Andrew Lock",
                    Intro = "Practical guide to building web apps with ASP.NET Core.",
                    Description = null,
                    Quantity = 0,
                    CreatedAt = new DateTime(2024, 1, 3)
                }
            );
        }

        public DbSet<Book> Books { get; set; }
    }
}