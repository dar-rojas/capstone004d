using System.ComponentModel.DataAnnotations;

namespace Api.Utils.DTOs
{
    public class HistoricalDataRequest
    {
        [Required]
        public string GridId { get; set; }

        [Required]
        public List<DateTime> Timestamps { get; set; }

        [Required]
        public List<double> DemandPowers { get; set; }
    }
}