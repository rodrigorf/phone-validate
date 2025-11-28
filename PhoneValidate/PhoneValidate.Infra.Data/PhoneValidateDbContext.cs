using Microsoft.EntityFrameworkCore;
using PhoneValidate.Domain.Service.Models;

namespace PhoneValidate.Infra.Data
{
    public class PhoneValidateDbContext : DbContext
    {
        public PhoneValidateDbContext(DbContextOptions<PhoneValidateDbContext> options)
            : base(options)
        {
        }

        public DbSet<Recipients> Recipients { get; set; }
        public DbSet<History> Histories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Recipients>()
                .HasMany(r => r.Histories)
                .WithOne(h => h.Recipient)
                .HasForeignKey(h => h.RecipientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configurar índices
            modelBuilder.Entity<Recipients>()
                .HasIndex(r => r.PhoneNumber)
                .IsUnique();

            modelBuilder.Entity<History>()
                .HasIndex(h => h.RecipientId);

            modelBuilder.Entity<History>()
                .HasIndex(h => h.CreatedAt);
        }
    }
}