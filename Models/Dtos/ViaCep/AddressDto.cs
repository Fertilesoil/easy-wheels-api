
namespace EasyWheelsApi.Models.Dtos.ViaCep
{
    public record AddressDto(
        string Cep,
        string Logradouro,
        string Bairro,
        string Cidade,
        string Estado
    );
}
