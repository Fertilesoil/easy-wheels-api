using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyWheelsApi.Models.Dtos.RentalDtos
{
    public record RentalInfo(
        Guid Id,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalPrice
    );
}