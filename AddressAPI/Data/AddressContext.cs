using System;
using AddressAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AddressAPI
{
    public class AddressContext : DbContext
    {
        public AddressContext(DbContextOptions<AddressContext> options)
            : base(options)
        {
        }

        public DbSet<Address> Address { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Address>().ToTable("Address");
        }
    }

}
