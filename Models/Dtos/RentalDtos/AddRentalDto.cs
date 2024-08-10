using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasyWheelsApi.Models.Dtos.RentalDtos
{
    public record AddRentalDto(
        string LessorId,
        string LesseeId,
        Guid CarId,
        DateTime StartDate,
        DateTime EndDate,
        decimal TotalPrice
    );
}