using MagicVilla_VillaAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace MagicVilla_VillaAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Villa> Villas { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Villa>().HasData(
                new Villa
                {
                    Id = 1,
                    Name = "Villa 1",
                    Details = "Villa 1 Description",
                    Rate = 1000,
                    ImageUrl = "https://via.placeholder.com/150"
                },
                new Villa
                {
                    Id = 2,
                    Name = "Villa 2",
                    Details = "Villa 2 Description",
                    Rate = 2000,
                    ImageUrl = "https://via.placeholder.com/150"
                }
                );
        }
    }
}
