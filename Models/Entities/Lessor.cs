using System.Text.Json.Serialization;

namespace EasyWheelsApi.Models.Entities
{
    public class Lessor : User
    {
        [JsonIgnore]
        public List<Car> Cars { get; set; } = [];
        [JsonIgnore]
        public List<Rental> Rentals { get; set; } = [];
    }
}
