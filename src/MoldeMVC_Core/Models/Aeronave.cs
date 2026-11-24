using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Aeronave
    {
        [Key]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = default!;
        public int idSql { get; set; } = default!;
        public string matricula { get; set; } = default!;
        public string estado { get; set; } = default!;
        public List<AsientoAeronave> asientos { get; set; } = new();
    }

    [BsonIgnoreExtraElements]
    public class AsientoAeronave
    {
        public int numeroAsiento { get; set; } = default!;
        public string codigo { get; set; } = default!;
        public string ubicacion { get; set; } = default!;
    }
}
