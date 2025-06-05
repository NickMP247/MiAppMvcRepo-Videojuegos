using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.WebUtilities;

namespace RankingVideojuegos.Models
{
    public class Videojuego
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? Titulo { get; set; }
        [Required]
        public string? Descripcion { get; set; }
        [Required]
        public string? ImagenUrl { get; set; }
        [Required]
        public string? TrailerUrl { get; set; }

        [Required]
        public string? Disponibilidad { get; set; }

        [Required]
        public DateTime FechaLanzamiento { get; set; }

        public double PuntuacionPromedio { get; set; } = 0;

        public ICollection<Valoracion>? Valoraciones { get; set; }

        public ICollection<PlataformaVideojuego>? Plataformas { get; set; }

        [NotMapped]
        public string? TrailerEmbedUrl
        {
            get
            {
                if (string.IsNullOrEmpty(TrailerUrl)) return null;

                try
                {
                    var uri = new Uri(TrailerUrl);

                    if (uri.Host.Contains("youtube.com"))
                    {
                        var query = QueryHelpers.ParseQuery(uri.Query);
                        var videoId = query["v"].ToString();
                        return $"https://www.youtube.com/embed/{videoId}";
                    }

                    // Para enlaces tipo youtu.be
                    if (uri.Host.Contains("youtu.be"))
                    {
                        var videoId = uri.AbsolutePath.Trim('/');
                        return $"https://www.youtube.com/embed/{videoId}";
                    }

                    return null;
                }
                catch
                {
                    return null;
                }
            }
        }
    }
}
