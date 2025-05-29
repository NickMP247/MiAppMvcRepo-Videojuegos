using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RankingVideojuegos.Models
{
    public class Valoracion
    {
        [Key]
        public int Id { get; set; }

        [Range(1, 5)]
        public int Puntuacion { get; set; }

        public string? Comentario { get; set; }

        public DateTime Fecha { get; set; } = DateTime.UtcNow;

        public int VideojuegoId { get; set; }
        public Videojuego? Videojuego { get; set; }

        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}
