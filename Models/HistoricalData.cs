using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using Api.Utils.Annotations;

namespace Api.Models
{
    public class HistoricalData : GridData
    {
        [BsonElement("demandPower")]
        [GreaterThanZero]
        [Required]
        public double DemandPower { get; set; }
    }
}