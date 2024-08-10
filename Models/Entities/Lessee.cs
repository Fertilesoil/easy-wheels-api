namespace EasyWheelsApi.Models.Entities
{
    public class Lessee : User 
    {
        public List<Rental> Rentals { get; set; } = [];
    }
}