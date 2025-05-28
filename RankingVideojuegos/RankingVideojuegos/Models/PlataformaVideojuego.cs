namespace RankingVideojuegos.Models
{
    public class PlataformaVideojuego
    {
        public int VideojuegoId { get; set; }
        public Videojuego? Videojuego { get; set; }

        public int PlataformaId { get; set; }
        public Plataforma? Plataforma { get; set; }
    }
}
