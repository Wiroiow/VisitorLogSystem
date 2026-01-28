using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Models;

namespace VisitorLogSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<RoomVisit> RoomVisits { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<PreRegisteredVisitor> PreRegisteredVisitors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Visitor configuration
            modelBuilder.Entity<Visitor>()
                .HasIndex(v => v.FullName);

            // RoomVisit configuration
            modelBuilder.Entity<RoomVisit>()
                .HasOne(rv => rv.Visitor)
                .WithMany(v => v.RoomVisits)
                .HasForeignKey(rv => rv.VisitorId)
                .OnDelete(DeleteBehavior.Restrict);

            // User configuration
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // PreRegisteredVisitor configuration
            modelBuilder.Entity<PreRegisteredVisitor>()
                .HasOne(p => p.HostUser)
                .WithMany()
                .HasForeignKey(p => p.HostUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PreRegisteredVisitor>()
                .HasOne(p => p.CheckedInByUser)
                .WithMany()
                .HasForeignKey(p => p.CheckedInByUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PreRegisteredVisitor>()
                .HasOne(p => p.RoomVisit)
                .WithMany()
                .HasForeignKey(p => p.RoomVisitId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PreRegisteredVisitor>()
                .HasIndex(p => p.FullName);

            modelBuilder.Entity<PreRegisteredVisitor>()
                .HasIndex(p => p.ExpectedVisitDate);

            modelBuilder.Entity<PreRegisteredVisitor>()
                .HasIndex(p => p.IsCheckedIn);
        }
    }
}