using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyWheelsApi.Models.Dtos.RentalDtos;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Mappings.RentalMapping
{
    public static class RentalMapping
    {
        public static Rental ToEntity(this AddRentalDto dto)
        {
            return new()
            {
                LessorId = dto.LessorId,
                LesseeId = dto.LesseeId,
                CarId = dto.CarId,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                TotalPrice = dto.TotalPrice
            };
        }

        // public static RentalResponseDto ToResponse(this object result)
        // {
        //     return new(
                
        //     );
        // }
    }
}