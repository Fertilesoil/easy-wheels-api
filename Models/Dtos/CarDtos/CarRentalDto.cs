
namespace EasyWheelsApi.Models.Dtos.CarDtos
{
    public record CarRentalDto(
        Guid Id,
        string Brand,
        string Model,
        decimal PricePerDay,
        bool IsRented
    );
}