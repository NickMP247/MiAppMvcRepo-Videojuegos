using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RankingVideojuegos.Data;
using RankingVideojuegos.Models;
using RankingVideojuegos.Services;

namespace RankingVideojuegos.Controllers
{
    public class VideojuegosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly S3Service _s3Service;
        public VideojuegosController(ApplicationDbContext context, S3Service s3Service)
        {
            _context = context;
            _s3Service = s3Service;
        }

        public async Task<IActionResult> Index(string search, string disponibilidad, List<int> plataformas)
        {
            var juegosQuery = _context.Videojuegos!
                .Include(v => v.Plataformas)!
                    .ThenInclude(pv => pv.Plataforma)
                .Include(v => v.Valoraciones)
                .AsQueryable();

            if (!string.IsNullOrEmpty(search))
            {
                juegosQuery = juegosQuery.Where(v => v.Titulo!.Contains(search));
            }

            if (!string.IsNullOrEmpty(disponibilidad))
            {
                juegosQuery = juegosQuery.Where(v => v.Disponibilidad == disponibilidad);
            }

            if (plataformas != null && plataformas.Any())
            {
                juegosQuery = juegosQuery.Where(v => v.Plataformas!.Any(p => plataformas.Contains(p.PlataformaId)));
            }

            ViewBag.Plataformas = await _context.Plataformas.ToListAsync();
            ViewBag.DisponibilidadActual = disponibilidad;
            ViewBag.BusquedaActual = search;
            ViewBag.PlataformasSeleccionadas = plataformas;

            var juegos = await juegosQuery.ToListAsync();
            return View(juegos);
        }


        public async Task<IActionResult> Details(int id)
        {
            var juego = await _context.Videojuegos
                .Include(v => v.Plataformas)
                    .ThenInclude(pv => pv.Plataforma)
                .Include(v => v.Valoraciones)
                    .ThenInclude(v => v.Usuario)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (juego == null) return NotFound();
            return View(juego);
        }
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Plataformas = new MultiSelectList(await _context.Plataformas.ToListAsync(), "Id", "Nombre");
            return View();
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Create(Videojuego videojuego, IFormFile Imagen, List<int> PlataformasSeleccionadas)
        {
            if (Imagen != null)
            {
                videojuego.ImagenUrl = await _s3Service.UploadToS3Async(Imagen, S3Folder.Videojuegos);
            }

            videojuego.PuntuacionPromedio = 0;
            videojuego.FechaLanzamiento = videojuego.FechaLanzamiento.ToUniversalTime();

            _context.Videojuegos.Add(videojuego);
            await _context.SaveChangesAsync();

            foreach (var plataformaId in PlataformasSeleccionadas)
            {
                _context.PlataformaVideojuego.Add(new PlataformaVideojuego
                {
                    VideojuegoId = videojuego.Id,
                    PlataformaId = plataformaId
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Edit(int id)
        {
            var juego = await _context.Videojuegos
                .Include(v => v.Plataformas)
                    .ThenInclude(pv => pv.Plataforma)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (juego == null) return NotFound();

            var plataformasSeleccionadas = juego.Plataformas.Select(p => p.PlataformaId).ToList();

            ViewBag.Plataformas = new MultiSelectList(
                await _context.Plataformas.ToListAsync(),
                "Id", "Nombre",
                plataformasSeleccionadas
            );

            return View(juego);
        }

        [Authorize(Roles = "admin")]
        [HttpPost]
        public async Task<IActionResult> Edit(Videojuego videojuego, IFormFile Imagen, List<int> PlataformasSeleccionadas)
        {
            var juegoExistente = await _context.Videojuegos
                .Include(p => p.Plataformas)
                .FirstOrDefaultAsync(v => v.Id == videojuego.Id);

            if (juegoExistente == null) return NotFound();

            juegoExistente.Titulo = videojuego.Titulo;
            juegoExistente.Descripcion = videojuego.Descripcion;
            juegoExistente.TrailerUrl = videojuego.TrailerUrl;
            juegoExistente.Disponibilidad = videojuego.Disponibilidad;
            juegoExistente.FechaLanzamiento = videojuego.FechaLanzamiento;

            if (Imagen != null)
            {
                juegoExistente.ImagenUrl = await _s3Service.UploadToS3Async(Imagen, S3Folder.Videojuegos);
            }

            // Actualizar plataformas
            _context.PlataformaVideojuego.RemoveRange(juegoExistente.Plataformas);
            await _context.SaveChangesAsync();

            foreach (var id in PlataformasSeleccionadas)
            {
                _context.PlataformaVideojuego.Add(new PlataformaVideojuego
                {
                    VideojuegoId = videojuego.Id,
                    PlataformaId = id
                });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var juego = await _context.Videojuegos.FindAsync(id);
            if (juego == null) return NotFound();

            _context.Videojuegos.Remove(juego);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
