using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Models.Entities
{
    public class Rental
    {
        public Guid Id { get; set; }
        public required string LessorId { get; set; }
        public Lessor? Lessor { get; set; }
        public required string LesseeId { get; set; }
        public Lessee? Lessee { get; set; }
        public Guid CarId { get; set; }
        public Car? Car { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        [Range(2, 7), Precision(7, 2)]
        public decimal TotalPrice { get; set; }
    }
}
