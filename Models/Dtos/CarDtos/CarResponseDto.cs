
namespace EasyWheelsApi.Models.Dtos.CarDtos
{
    public record CarResponseDto(
        Guid Id,
        string Brand,
        string Model,
        decimal PricePerDay,
        bool IsRented,
        string LessorId
    );
}
