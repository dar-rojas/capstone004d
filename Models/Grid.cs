using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using Api.Utils.Annotations;
namespace Api.Models
{
    public class Grid
    {
        //Id
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get;set; }

        //Name
        [Required(ErrorMessage = "Value is required.")]
        [StringLength(100, ErrorMessage = "Name can't be longer than 100 characters.")]
        public string? Name { get;set; }

        //Energy cost
        [Required(ErrorMessage = "Value is required.")]
        [GreaterThanZero]
        public double EnergyCost { get;set; }

        //Power cost
        [Required(ErrorMessage = "Value is required.")]
        [GreaterThanZero]
        public double PowerCost { get;set; }

        //Power cost peak hour
        [Required(ErrorMessage = "Value is required.")]
        [GreaterThanZero]
        public double PowerCostPH{ get;set; }

        //Maximum battery capacity
        [Required(ErrorMessage = "Value is required.")]
        [GreaterThanZero]
        public int MaxEnergy { get;set; }

        //Maximum SOC allowed
        [Required(ErrorMessage = "Value is required.")]
        [DoublePercentage]
        public double MaxSOC { get;set; }

        //Minimum SOC allowed
        [Required(ErrorMessage = "Value is required.")]
        [DoublePercentage]
        public double MinSOC { get;set; }

        // Grid power limit
        [Required(ErrorMessage = "Value is required.")]
        [GreaterThanZero]
        public int PowerLimit{ get;set; }

        //Max battery power
        [Required(ErrorMessage = "Value is required.")]
        [GreaterThanZero]
        public int MaxBatteryPower { get;set; }

        // Min battery power
        [Required(ErrorMessage = "Value is required.")]
        [Range(int.MinValue, 0, ErrorMessage = "Value must be less than or equal to 0")]
        public int MinBatteryPower { get;set; }

        // Charge efficiency
        [Required(ErrorMessage = "Value is required.")]
        [DoublePercentage]
        public double ChargeEfficiency { get;set; }

        // Discharge efficiency
        [Required(ErrorMessage = "Value is required.")]
        [DoublePercentage]
        public double DischargeEfficiency { get;set; }
    }
}