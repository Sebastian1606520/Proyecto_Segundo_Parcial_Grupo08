using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoldeMVC_Core.Models
{
    [BsonIgnoreExtraElements]
    public class Catalogo
    {
        [Key]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string _id { get; set; } = default!;
        public string tipo { get; set; } = default!;
        public string codigo { get; set; } = default!;
        public string descripcion { get; set; } = default!;
        public bool? ocupaInventario { get; set; }
    }
}
