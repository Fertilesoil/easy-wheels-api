using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EasyWheelsApi.Models.Dtos.ViaCep;

namespace EasyWheelsApi.Validation.AddressValidation
{
    public static class AddressValidation
    {
        public static bool IsValid(this AddressDto address)
        {
            return string.IsNullOrEmpty(address.Bairro)
                || string.IsNullOrEmpty(address.Logradouro)
                || string.IsNullOrEmpty(address.Cidade)
                || string.IsNullOrEmpty(address.Estado);
        }
    }
}
