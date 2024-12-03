// DTO for receiving and validating request data
// from the /get_optimized_data endpoint.

using System.ComponentModel.DataAnnotations;

namespace api.Utils.DTOs
{
    public class OptimizedDataRequest
    {
        [Required]
        public string GridId { get; set; }

        [Timestamp]
        public DateTime? InitTimestamp { get; set; }

        [Timestamp]
        public DateTime? EndTimestamp { get; set; }
    }
}
