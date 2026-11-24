using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Controllers
{
    public class HomeController : Controller
    {
        private readonly AerolineaMongoContext _context;

        public HomeController(AerolineaMongoContext context)
        {
            _context = context;
        }

        // GET: Home/Index
        public async Task<IActionResult> Index()
        {
            // 1. Obtener la lista de vuelos desde MongoDB
            var vuelos = await _context.Vuelos
                .Find(v => v.estado == "PROGRAMADO" || v.estado == "DISPONIBLE")
                .SortBy(v => v.fechaOperacion)
                .ToListAsync();

            // 2. Extraer aeropuertos de origen y destino únicos de las rutas embebidas
            var aeropuertosOrigen = vuelos
                .Select(v => new {
                    Id = v.ruta.origen.codigoIATA,
                    Texto = $"{v.ruta.origen.localidad} ({v.ruta.origen.codigoIATA})"
                })
                .GroupBy(a => a.Id)
                .Select(g => g.First())
                .OrderBy(a => a.Texto)
                .ToList();

            var aeropuertosDestino = vuelos
                .Select(v => new {
                    Id = v.ruta.destino.codigoIATA,
                    Texto = $"{v.ruta.destino.localidad} ({v.ruta.destino.codigoIATA})"
                })
                .GroupBy(a => a.Id)
                .Select(g => g.First())
                .OrderBy(a => a.Texto)
                .ToList();

            // 3. Preselección de "UIO" tal como lo hacías en el proyecto de SQL
            var quito = aeropuertosOrigen.FirstOrDefault(a => a.Id == "UIO");

            // 4. Asignación directa a ViewBag con SelectList (Sin ViewModels)
            ViewBag.Origenes = new SelectList(aeropuertosOrigen, "Id", "Texto", quito != null ? quito.Id : null);
            ViewBag.Destinos = new SelectList(aeropuertosDestino, "Id", "Texto");

            // 5. Ocupación de asientos y envío directo del modelo de dominio List<Vuelo>
            var asignaciones = await _context.ObtenerAsignacionesQueOcupanAsync(Builders<Asignacion>.Filter.Empty);
            Vuelo.CalcularOcupacion(vuelos, asignaciones);

            return View(vuelos.Take(4).ToList());
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}