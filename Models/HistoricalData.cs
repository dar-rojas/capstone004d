using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using Api.Utils.Annotations;

namespace Api.Models
{
    public class HistoricalData
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

        [BsonElement("demandPower")]
        [GreaterThanZero]
        [Required]
        public double DemandPower { get; set; }
    }
}