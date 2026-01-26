using Microsoft.EntityFrameworkCore;
using VisitorLogSystem.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VisitorLogSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Existing DbSets (already in your project)
        /// </summary>
        public DbSet<Visitor> Visitors { get; set; }
        public DbSet<User> Users { get; set; }

        /// <summary>
        /// ✨ NEW - DbSet for Room Visits
        /// This represents the "room_visits" table
        /// 
        /// Usage:
        /// - Query: _context.RoomVisits.Where(rv => rv.VisitorId == 1)
        /// - Add: _context.RoomVisits.Add(newRoomVisit)
        /// - Delete: _context.RoomVisits.Remove(roomVisit)
        /// </summary>
        public DbSet<RoomVisit> RoomVisits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ═══════════════════════════════════════════════════════════
            // Existing Configurations (already in your project)
            // ═══════════════════════════════════════════════════════════

            modelBuilder.Entity<Visitor>(entity =>
            {
                entity.Property(v => v.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.Property(v => v.UpdatedAt)
                    .HasDefaultValueSql("GETDATE()");

                entity.HasIndex(v => v.TimeIn);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Username).IsUnique();
                entity.Property(u => u.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");
            });

            // ═══════════════════════════════════════════════════════════
            // ✨ NEW - RoomVisit Configuration
            // ═══════════════════════════════════════════════════════════

            modelBuilder.Entity<RoomVisit>(entity =>
            {
                /// <summary>
                /// Set default value for CreatedAt
                /// When a new room visit is inserted, SQL Server automatically
                /// sets created_at to the current date/time using GETDATE()
                /// </summary>
                entity.Property(rv => rv.CreatedAt)
                    .HasDefaultValueSql("GETDATE()");

                /// <summary>
                /// Create index on visitor_id for faster queries
                /// 
                /// WHY? We often query: "Get all rooms visited by visitor X"
                /// Without index: Database scans EVERY row (slow)
                /// With index: Database jumps directly to visitor X's rows (fast)
                /// 
                /// Example query that benefits:
                /// SELECT * FROM room_visits WHERE visitor_id = 1
                /// </summary>
                entity.HasIndex(rv => rv.VisitorId);

                /// <summary>
                /// Create index on room_name for faster room-based queries
                /// 
                /// Example query that benefits:
                /// SELECT * FROM room_visits WHERE room_name = 'Room 101'
                /// (Find all visitors who entered Room 101)
                /// </summary>
                entity.HasIndex(rv => rv.RoomName);

                /// <summary>
                /// Create index on entered_at for date-based queries
                /// 
                /// Example query that benefits:
                /// SELECT * FROM room_visits WHERE entered_at >= '2025-01-01'
                /// (Find all room visits after January 1st)
                /// </summary>
                entity.HasIndex(rv => rv.EnteredAt);

                /// <summary>
                /// Define the relationship between Visitor and RoomVisit
                /// 
                /// RELATIONSHIP TYPE: One-to-Many
                /// - ONE Visitor can have MANY RoomVisits
                /// - EACH RoomVisit belongs to ONE Visitor
                /// 
                /// CASCADING DELETE: OnDelete(DeleteBehavior.Cascade)
                /// When a Visitor is deleted, all their RoomVisits are automatically deleted
                /// 
                /// Example:
                /// Delete Visitor (ID: 1)
                ///   → Automatically deletes all RoomVisits with visitor_id = 1
                /// 
                /// SQL Generated:
                /// ALTER TABLE room_visits
                /// ADD CONSTRAINT FK_RoomVisits_Visitors_VisitorId
                /// FOREIGN KEY (visitor_id) REFERENCES visitors(id)
                /// ON DELETE CASCADE
                /// </summary>
                entity.HasOne(rv => rv.Visitor)        // Each RoomVisit has ONE Visitor
                    .WithMany(v => v.RoomVisits)       // Each Visitor has MANY RoomVisits
                    .HasForeignKey(rv => rv.VisitorId) // Foreign key is visitor_id
                    .OnDelete(DeleteBehavior.Cascade); // Delete room visits when visitor is deleted
            });
        }
    }
}
