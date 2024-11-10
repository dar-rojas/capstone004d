using Api.Utils.Annotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Api.Models
{
    public class OptimizedData : GridData
    {
        [BsonElement("batteryPower")]
        [Required]
        public double BatteryPower { get; set; }

        [BsonElement("batteryEnergy")]
        [Required]
        [GreaterThanZero]
        public double BatteryEnergy { get; set; }
    }
}