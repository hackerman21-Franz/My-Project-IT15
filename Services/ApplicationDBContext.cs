using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyProjectIT15.Models;
using System.Reflection.Emit;

namespace MyProjectIT15.Services
{
    public class ApplicationDBContext : IdentityDbContext<User>
    {
        public ApplicationDBContext(DbContextOptions options) : base(options) 
        {

        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }
        public DbSet<Meter> Meters { get; set; }
        public DbSet<MeterReading> MeterReadings { get; set; }



        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserRoom>()
                .HasOne(ur => ur.Room)
                .WithMany(r => r.UserRooms)
                .HasForeignKey(ur => ur.RoomId);

            builder.Entity<UserRoom>()
                .HasOne(ur => ur.Admin)
                .WithMany(u => u.AssignedAsAdmin)
                .HasForeignKey(ur => ur.AdminId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<UserRoom>()
                .HasOne(ur => ur.Tenant)
                .WithMany(u => u.AssignedAsTenant)
                .HasForeignKey(ur => ur.TenantId)
                .OnDelete(DeleteBehavior.Restrict);

            var admin = new IdentityRole("admin");
            admin.NormalizedName = "admin";

            var tenant = new IdentityRole("tenant");
            tenant.NormalizedName = "tenant";

            var owner = new IdentityRole("owner");
            owner.NormalizedName = "owner";

            builder.Entity<IdentityRole>().HasData(admin, tenant, owner);
        }
    }

} 
