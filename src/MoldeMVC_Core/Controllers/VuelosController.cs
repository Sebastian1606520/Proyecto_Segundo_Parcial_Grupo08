using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using MoldeMVC_Core.Data;
using MoldeMVC_Core.Models;

namespace MoldeMVC_Core.Controllers
{
    [Authorize]
    public class VuelosController : Controller
    {
        private readonly AerolineaMongoContext _context;

        public VuelosController(AerolineaMongoContext context)
        {
            _context = context;
        }

        // GET: Vuelos
        public async Task<IActionResult> Index()
        {
            var vuelos = await _context.Vuelos
                .Find(Builders<Vuelo>.Filter.Empty)
                .SortBy(v => v.fechaOperacion)
                .ToListAsync();

            var asignaciones = await _context.ObtenerAsignacionesQueOcupanAsync(Builders<Asignacion>.Filter.Empty);
            Vuelo.CalcularOcupacion(vuelos, asignaciones);

            return View(vuelos);
        }

        // GET: Vuelos/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (string.IsNullOrEmpty(id) || !ObjectId.TryParse(id, out _))
            {
                return NotFound();
            }

            var vuelo = await _context.Vuelos
                .Find(v => v._id == id)
                .FirstOrDefaultAsync();

            if (vuelo == null)
            {
                return NotFound();
            }

            var asignaciones = await _context.ObtenerAsignacionesQueOcupanAsync(
                Builders<Asignacion>.Filter.Eq(a => a.vueloId, id));

            Aeronave? aeronave = null;
            if (vuelo.aeronave != null)
            {
                aeronave = await _context.Aeronaves
                    .Find(a => a._id == vuelo.aeronave.aeronaveId)
                    .FirstOrDefaultAsync();
            }

            vuelo.CalcularAsientos(aeronave, asignaciones);

            return View(vuelo);
        }
    }
}
