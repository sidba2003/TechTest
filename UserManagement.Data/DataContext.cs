﻿using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UserManagement.Models;

namespace UserManagement.Data
{
    public class DataContext : DbContext, IDataContext
    {
        public DataContext() => Database.EnsureCreated();

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                options.UseInMemoryDatabase("UserManagement.Data.DataContext");
            }
        }

        public DbSet<User>? Users { get; set; }
        public DbSet<UserLogs>? UserAudits { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
            => model.Entity<User>().HasData(new[]
            {
                new User { Id = 1, Forename = "Peter", Surname = "Loew", Email = "ploew@example.com", IsActive = true, DateOfBirth = new System.DateTime(1991, 10, 11) },
                new User { Id = 2, Forename = "Benjamin Franklin", Surname = "Gates", Email = "bfgates@example.com", IsActive = true, DateOfBirth = new System.DateTime(2001, 11, 12) },
                new User { Id = 3, Forename = "Castor", Surname = "Troy", Email = "ctroy@example.com", IsActive = false, DateOfBirth = new System.DateTime(2002, 8, 22) },
                new User { Id = 4, Forename = "Memphis", Surname = "Raines", Email = "mraines@example.com", IsActive = true, DateOfBirth = new System.DateTime(1992, 5, 2) },
                new User { Id = 5, Forename = "Stanley", Surname = "Goodspeed", Email = "sgodspeed@example.com", IsActive = true, DateOfBirth = new System.DateTime(1989, 1, 5) },
                new User { Id = 6, Forename = "H.I.", Surname = "McDunnough", Email = "himcdunnough@example.com", IsActive = true, DateOfBirth = new System.DateTime(2001, 11, 8) },
                new User { Id = 7, Forename = "Cameron", Surname = "Poe", Email = "cpoe@example.com", IsActive = false, DateOfBirth = new System.DateTime(2004, 4, 1) },
                new User { Id = 8, Forename = "Edward", Surname = "Malus", Email = "emalus@example.com", IsActive = false, DateOfBirth = new System.DateTime(2000, 10, 14) },
                new User { Id = 9, Forename = "Damon", Surname = "Macready", Email = "dmacready@example.com", IsActive = false, DateOfBirth = new System.DateTime(2003, 2, 17) },
                new User { Id = 10, Forename = "Johnny", Surname = "Blaze", Email = "jblaze@example.com", IsActive = true, DateOfBirth = new System.DateTime(1991, 6, 18) },
                new User { Id = 11, Forename = "Robin", Surname = "Feld", Email = "rfeld@example.com", IsActive = true, DateOfBirth = new System.DateTime(1988, 9, 2) },
            });

        public IQueryable<TEntity> GetAll<TEntity>() where TEntity : class => Set<TEntity>();

        public async Task CreateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            await AddAsync(entity);
            await SaveChangesAsync();
        }

        public async Task UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            Update(entity);
            await SaveChangesAsync();
        }

        public async Task DeleteAsync<TEntity>(TEntity entity) where TEntity : class
        {
            Remove(entity);
            await SaveChangesAsync();
        }
    }
}
