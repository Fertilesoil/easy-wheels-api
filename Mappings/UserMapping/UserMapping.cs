using EasyWheelsApi.Models.Dtos.CarDtos;
using EasyWheelsApi.Models.Dtos.RentalDtos;
using EasyWheelsApi.Models.Dtos.UserDtos;
using EasyWheelsApi.Models.Dtos.ViaCep;
using EasyWheelsApi.Models.Entities;

namespace EasyWheelsApi.Models.Dtos.UserMapping
{
    public static class UserMapping
    {
        public static Lessor ToEntity(this AddUserDto lessor, AddressDto addressDto)
        {
            Lessor newLessor =
                new()
                {
                    UserName = lessor.Username,
                    FirstName = lessor.FirstName,
                    LastName = lessor.LastName,
                    Nationality = lessor.Nationality,
                    Profession = lessor.Profession,
                    Email = lessor.Email,
                    Rg = lessor.Rg,
                    Cpf = lessor.Cpf,
                    City = addressDto.Cidade,
                    State = addressDto.Estado,
                    Street = addressDto.Logradouro,
                    HouseNumber = lessor.HouseNumber,
                    PostalCode = lessor.PostalCode,
                    UserType = "Lessor"
                };
            return newLessor;
        }

        public static Lessor ToEntity(this AddUserDto lessor)
        {
            Lessor newLessor =
                new()
                {
                    UserName = lessor.Username,
                    FirstName = lessor.FirstName,
                    LastName = lessor.LastName,
                    Nationality = lessor.Nationality,
                    Profession = lessor.Profession,
                    Email = lessor.Email,
                    Rg = lessor.Rg,
                    Cpf = lessor.Cpf,
                    City = lessor.City,
                    State = lessor.State,
                    Street = lessor.Street,
                    HouseNumber = lessor.HouseNumber,
                    PostalCode = lessor.PostalCode,
                    UserType = "Lessor"
                };
            return newLessor;
        }

        public static Lessee ToEntityLessee(this AddUserDto lessee, AddressDto addressDto)
        {
            Lessee newLessee =
                new()
                {
                    UserName = lessee.Username,
                    FirstName = lessee.FirstName,
                    LastName = lessee.LastName,
                    Nationality = lessee.Nationality,
                    Profession = lessee.Profession,
                    Email = lessee.Email,
                    Rg = lessee.Rg,
                    Cpf = lessee.Cpf,
                    City = addressDto.Cidade,
                    State = addressDto.Estado,
                    Street = addressDto.Logradouro,
                    HouseNumber = lessee.HouseNumber,
                    PostalCode = lessee.PostalCode,
                    UserType = "Lessee"
                };
            return newLessee;
        }

        public static Lessee ToEntityLessee(this AddUserDto lessee)
        {
            Lessee newLessee =
                new()
                {
                    UserName = lessee.Username,
                    FirstName = lessee.FirstName,
                    LastName = lessee.LastName,
                    Nationality = lessee.Nationality,
                    Profession = lessee.Profession,
                    Email = lessee.Email,
                    Rg = lessee.Rg,
                    Cpf = lessee.Cpf,
                    City = lessee.City,
                    State = lessee.State,
                    Street = lessee.Street,
                    HouseNumber = lessee.HouseNumber,
                    PostalCode = lessee.PostalCode,
                    UserType = "Lessee"
                };
            return newLessee;
        }

        public static Lessor ToEntity(this UpdateUserDto lessor, Lessor oldLessor)
        {
            return new Lessor()
            {
                FirstName = lessor.FirstName,
                LastName = lessor.LastName,
                Nationality = lessor.Nationality,
                Profession = lessor.Profession,
                Email = lessor.Email,
                Rg = oldLessor.Rg,
                Cpf = oldLessor.Cpf,
                City = lessor.City,
                State = lessor.State,
                Street = lessor.Street,
                HouseNumber = lessor.HouseNumber,
                PostalCode = lessor.PostalCode,
                UserType = "Lessor"
            };
        }

        public static Lessee ToEntityLessee(this UpdateUserDto lessor, Lessee oldLessor)
        {
            return new()
            {
                FirstName = lessor.FirstName,
                LastName = lessor.LastName,
                Nationality = lessor.Nationality,
                Profession = lessor.Profession,
                Email = lessor.Email,
                Rg = oldLessor.Rg,
                Cpf = oldLessor.Cpf,
                City = lessor.City,
                State = lessor.State,
                Street = lessor.Street,
                HouseNumber = lessor.HouseNumber,
                PostalCode = lessor.PostalCode,
                UserType = "Lessee"
            };
        }

        public static UserResponseDto ToResponse(this AddUserDto lessor, Lessor newLessor)
        {
            return new UserResponseDto(
                newLessor.Id,
                lessor.FirstName,
                lessor.LastName,
                lessor.Nationality,
                lessor.Profession,
                lessor.Email,
                newLessor.UserType,
                newLessor.Rentals.Select(r => new RentalInfo (
                    r.Id,
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice
                )).ToList(),
                newLessor.Cars.Select(c => new CarResponseDto (
                    c.Id,
                    c.Brand,
                    c.Model,
                    c.PricePerDay,
                    c.IsRented,
                    c.LessorId
                )).ToList()
            );
        }

        public static LesseeResponseDto ToResponseLessee(this AddUserDto lessee, Lessee newLessee)
        {
            return new(
                newLessee.Id,
                lessee.FirstName,
                lessee.LastName,
                lessee.Nationality,
                lessee.Profession,
                lessee.Email,
                newLessee.UserType,
                newLessee.Rentals.Select(r => new RentalInfo (
                    r.Id,
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice
                )).ToList()
            );
        }

        public static LesseeResponseDto ToResponseLessee(this Lessee lessee)
        {
            return new(
                lessee.Id,
                lessee.FirstName,
                lessee.LastName,
                lessee.Nationality,
                lessee.Profession,
                lessee.Email!,
                lessee.UserType,
                lessee.Rentals.Select(r => new RentalInfo (
                    r.Id,
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice
                )).ToList()
            );
        }

        public static UserResponseDto ToResponse(this User lessor)
        {
            var lessor1 = (Lessor)lessor;
            return new(
                lessor.Id,
                lessor.FirstName,
                lessor.LastName,
                lessor.Nationality,
                lessor.Profession,
                lessor.Email!,
                lessor.UserType,
                lessor1.Rentals.Select(r => new RentalInfo (
                    r.Id,
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice
                )).ToList(),
                lessor1.Cars.Select(c => new CarResponseDto (
                    c.Id,
                    c.Brand,
                    c.Model,
                    c.PricePerDay,
                    c.IsRented,
                    c.LessorId
                )).ToList()
            );
        }

        public static UserSearchDto ToSearchResponse(this User lessor)
        {
            var lessor1 = (Lessor)lessor;
            return new(
                lessor.Id,
                lessor.FirstName,
                lessor.LastName,
                lessor.Nationality,
                lessor.Profession,
                lessor.Email!,
                lessor.City!,
                lessor.State!,
                lessor.Street!,
                lessor.HouseNumber,
                lessor.PostalCode,
                lessor.UserType,
                lessor1.Rentals.Select(r => new RentalInfo (
                    r.Id,
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice
                )).ToList(),
                lessor1.Cars.Select(c => new CarResponseDto (
                    c.Id,
                    c.Brand,
                    c.Model,
                    c.PricePerDay,
                    c.IsRented,
                    c.LessorId
                )).ToList()
            );
        }

        public static LesseeSearchDto ToSearchResponse(this Lessee lessee)
        {
            return new(
                lessee.Id,
                lessee.FirstName,
                lessee.LastName,
                lessee.Nationality,
                lessee.Profession,
                lessee.Email!,
                lessee.City!,
                lessee.State!,
                lessee.Street!,
                lessee.HouseNumber,
                lessee.PostalCode,
                lessee.UserType,
                lessee.Rentals.Select(r => new RentalInfo (
                    r.Id,
                    r.StartDate,
                    r.EndDate,
                    r.TotalPrice
                )).ToList()
            );
        }
    }
}
