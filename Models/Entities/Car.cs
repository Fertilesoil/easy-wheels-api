using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Models.Entities
{
    public class Car
    {
        public Guid Id { get; set; }
        [StringLength(50), MaxLength(50)]
        public required string Brand { get; set; }
        [StringLength(50), MaxLength(50)]
        public required string Model { get; set; }
        [Range(2, 5), Precision(5, 2)]
        public required decimal PricePerDay { get; set; }
        public bool IsRented { get; set; } = false;
        public required string LessorId { get; set; }
        [JsonIgnore]
        public Lessor? Lessor { get; set; }
        public List<Rental>? Rentals { get; set; }
    }
}
