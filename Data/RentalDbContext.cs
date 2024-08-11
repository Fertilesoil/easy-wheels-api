using EasyWheelsApi.Models.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace EasyWheelsApi.Data
{
    public class RentalDbContext(DbContextOptions<RentalDbContext> options)
        : IdentityDbContext<User>(options)
    {
        public DbSet<Car> Cars { get; set; }
        public DbSet<Rental> Rentals { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            builder
                .Entity<User>()
                .HasDiscriminator<string>("UserType")
                .HasValue<Lessor>("Lessor")
                .HasValue<Lessee>("Lessee");

            builder.Entity<User>().HasIndex(c => c.Email).IsUnique(true);

            // Configuração do relacionamento entre Rental e Lessor
            builder
                .Entity<Rental>()
                .HasOne(r => r.Lessor)
                .WithMany(l => l.Rentals)
                .HasForeignKey(r => r.LessorId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento entre Rental e Lessee
            builder
                .Entity<Rental>()
                .HasOne(r => r.Lessee)
                .WithMany(l => l.Rentals)
                .HasForeignKey(r => r.LesseeId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento entre Rental e Car
            builder
                .Entity<Rental>()
                .HasOne(r => r.Car)
                .WithMany(c => c.Rentals)
                .HasForeignKey(r => r.CarId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuração do relacionamento entre Car e Lessor
            builder
                .Entity<Car>()
                .HasOne(c => c.Lessor)
                .WithMany(l => l.Cars)
                .HasForeignKey(c => c.LessorId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
