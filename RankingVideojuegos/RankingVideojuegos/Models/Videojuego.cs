using System.ComponentModel.DataAnnotations;

namespace RankingVideojuegos.Models
{
    public class Videojuego
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Titulo { get; set; }

        public string? Descripcion { get; set; }

        public string? ImagenUrl { get; set; }

        public string? TrailerUrl { get; set; }
        [Required]
        public string? Disponibilidad { get; set; }
        [Required]
        public DateTime FechaLanzamiento { get; set; }

        public double PuntuacionPromedio { get; set; } = 0;

        public ICollection<Valoracion>? Valoraciones { get; set; }

        public ICollection<PlataformaVideojuego>? Plataformas { get; set; }
    }
}
