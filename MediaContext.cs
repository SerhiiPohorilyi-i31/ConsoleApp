using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ConsoleApp.DB
{


    // Контекст бази даних для роботи з EF Core
    public class MediaContext : DbContext
    {
        // DbSet для доступу до всіх медіа матеріалів (аудіо та відео)
        public DbSet<Media> Media { get; set; }

        // Конфігурація контексту для використання SQLite
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=media.db");
        }

        // Налаштування моделі для TPH (Table Per Hierarchy)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Media>()
                .HasDiscriminator<string>("MediaType")
                .HasValue<Audio>("Audio")
                .HasValue<Video>("Video");
        }
    }
}
