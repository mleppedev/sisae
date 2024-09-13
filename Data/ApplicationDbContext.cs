using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using sisae.Models;

namespace sisae.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<AccesoProhibido> AccesosProhibidos { get; set; }

        public DbSet<Visitado> Visitados { get; set; }

        public DbSet<Visitante> Visitantes { get; set; }

        public DbSet<Visita> Visitas { get; set; }
    }
}