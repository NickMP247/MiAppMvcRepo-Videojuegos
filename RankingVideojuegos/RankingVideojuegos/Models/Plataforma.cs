using System.ComponentModel.DataAnnotations;

namespace RankingVideojuegos.Models
{
    public class Plataforma
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Nombre { get; set; }

        public ICollection<PlataformaVideojuego>? Videojuegos { get; set; }
    }
}
