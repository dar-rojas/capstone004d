using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class GridData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("timestamp")]
        [Required]
        public DateTime Timestamp { get; set; }

        [BsonElement("gridId")]
        [BsonRepresentation(BsonType.ObjectId)]
        [Required]
        public string? GridId { get; set; }
    }
}