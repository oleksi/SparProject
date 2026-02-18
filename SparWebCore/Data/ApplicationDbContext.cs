using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SparWebCore.Models;

namespace SparWebCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Fighter> Fighters => Set<Fighter>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Fighter>(entity =>
            {
                entity.ToTable("Fighters");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasColumnName("FighterId");

                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Sex).IsRequired();
                entity.Property(e => e.DateOfBirth).IsRequired();
                entity.Property(e => e.City).IsRequired();
                entity.Property(e => e.State).IsRequired();
                entity.Property(e => e.Height).IsRequired();
                entity.Property(e => e.Weight).IsRequired();
                entity.Property(e => e.IsSouthpaw).IsRequired();
                entity.Property(e => e.NumberOfAmateurFights).IsRequired();
                entity.Property(e => e.NumberOfProFights).IsRequired();
                entity.Property(e => e.ProfilePictureUploaded).IsRequired();
                entity.Property(e => e.InsertDate).IsRequired();
                entity.Property(e => e.UpdateDate).IsRequired();
                entity.Property(e => e.IsDemo).IsRequired();

                entity.Property(e => e.GymId).HasColumnName("GymId");
                entity.Property(e => e.TrainerId).HasColumnName("TrainerId");
                entity.Property(e => e.AspNetUserId).HasColumnName("AspNetUserId");

                entity.HasOne(e => e.SparIdentityUser)
                    .WithMany()
                    .HasForeignKey(e => e.AspNetUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
