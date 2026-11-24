using MoldeMVC_Core.Models;
using MongoDB.Driver;

namespace MoldeMVC_Core.Data
{
    public class AerolineaMongoContext
    {
        public IMongoDatabase Database { get; }
        public IMongoCollection<Vuelo> Vuelos { get; }
        public IMongoCollection<Pasajero> Pasajeros { get; }
        public IMongoCollection<Aeronave> Aeronaves { get; }
        public IMongoCollection<Asignacion> Asignaciones { get; }
        public IMongoCollection<Catalogo> Catalogos { get; }

        public AerolineaMongoContext(IMongoDatabase database)
        {
            Database = database;
            Vuelos = database.GetCollection<Vuelo>("vuelos");
            Pasajeros = database.GetCollection<Pasajero>("pasajeros");
            Aeronaves = database.GetCollection<Aeronave>("aeronaves");
            Asignaciones = database.GetCollection<Asignacion>("asignaciones");
            Catalogos = database.GetCollection<Catalogo>("catalogos");
        }

        // Estados de asignacion que ocupan un asiento (PENDIENTE y CONFIRMADA en el catalogo)
        public async Task<List<Asignacion>> ObtenerAsignacionesQueOcupanAsync(FilterDefinition<Asignacion> filtro)
        {
            var estadosQueOcupan = await Catalogos
                .Find(c => c.tipo == "EstadoAsignacion" && c.ocupaInventario == true)
                .Project(c => c.codigo)
                .ToListAsync();

            var filtroOcupan = Builders<Asignacion>.Filter.In(a => a.estado, estadosQueOcupan);

            return await Asignaciones
                .Find(Builders<Asignacion>.Filter.And(filtro, filtroOcupan))
                .ToListAsync();
        }
    }
}