using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EasyWheelsApi.Models.Dtos.CarDtos
{
    public record AddCarDto(
        string Brand,
        string Model,
        decimal PricePerDay,
        bool IsRented
    );
}