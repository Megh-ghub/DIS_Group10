using System;
using Microsoft.EntityFrameworkCore;
using DIS_Group10.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DIS_Group10.DataAccess
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        public DbSet<Park> Parks { get; set; }
        public DbSet<Activity> Activities { get; set; }
        public DbSet<ParkActivity> ParkActivities { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<StatePark> StateParks { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ParkActivity>()
            .Property(f => f.ID)
            .ValueGeneratedOnAdd();
            
            modelBuilder.Entity<Park>()
            .Property(f => f.ID)
            .ValueGeneratedOnAdd();

            base.OnModelCreating(modelBuilder);
        }
    }
}