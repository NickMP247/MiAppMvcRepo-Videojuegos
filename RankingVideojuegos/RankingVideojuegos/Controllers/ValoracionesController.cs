using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RankingVideojuegos.Data;
using RankingVideojuegos.Models;

namespace RankingVideojuegos.Controllers
{
    [Authorize]
    public class ValoracionesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ValoracionesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> Crear(int videojuegoId, int puntuacion, string comentario)
        {
            var userId = _userManager.GetUserId(User);

            // Evitar valoraciones duplicadas del mismo usuario
            var yaValoro = await _context.Valoraciones
                .AnyAsync(v => v.VideojuegoId == videojuegoId && v.UsuarioId == userId);

            if (yaValoro)
            {
                TempData["Error"] = "Ya has valorado este videojuego.";
                return RedirectToAction("Details", "Videojuegos", new { id = videojuegoId });
            }

            // Crear nueva valoración
            var valoracion = new Valoracion
            {
                VideojuegoId = videojuegoId,
                Puntuacion = puntuacion,
                Comentario = comentario,
                UsuarioId = userId
            };

            _context.Valoraciones.Add(valoracion);
            await _context.SaveChangesAsync();

            // Recalcular puntuación promedio del videojuego
            var valoraciones = await _context.Valoraciones
                .Where(v => v.VideojuegoId == videojuegoId)
                .ToListAsync();

            var promedio = valoraciones.Average(v => v.Puntuacion);

            var videojuego = await _context.Videojuegos.FindAsync(videojuegoId);
            if (videojuego != null)
            {
                videojuego.PuntuacionPromedio = Math.Round(promedio, 2);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Details", "Videojuegos", new { id = videojuegoId });
        }
    }
}
