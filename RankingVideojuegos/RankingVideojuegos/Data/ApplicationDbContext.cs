using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RankingVideojuegos.Models;

namespace RankingVideojuegos.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Videojuego> Videojuegos { get; set; }
        public DbSet<Valoracion> Valoraciones { get; set; }
        public DbSet<Plataforma> Plataformas { get; set; }
        public DbSet<PlataformaVideojuego> PlataformaVideojuego { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configuración relación muchos a muchos
            modelBuilder.Entity<PlataformaVideojuego>()
                .HasKey(pv => new { pv.VideojuegoId, pv.PlataformaId });

            modelBuilder.Entity<PlataformaVideojuego>()
                .HasOne(pv => pv.Videojuego)
                .WithMany(v => v.Plataformas)
                .HasForeignKey(pv => pv.VideojuegoId);

            modelBuilder.Entity<PlataformaVideojuego>()
                .HasOne(pv => pv.Plataforma)
                .WithMany(p => p.Videojuegos)
                .HasForeignKey(pv => pv.PlataformaId);
        }
    }
}
