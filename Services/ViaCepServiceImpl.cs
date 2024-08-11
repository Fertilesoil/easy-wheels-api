using System.Reflection.Metadata;
using EasyWheelsApi.Models.Dtos.ViaCep;

namespace EasyWheelsApi.Services
{
    public class ViaCepServiceImpl : IViaCepService
    {
        private readonly HttpClient _client;

        private const string BASE_URL = "https://viacep.com.br/ws/";
        public ViaCepServiceImpl(HttpClient client)
        {
            client.BaseAddress = new Uri(BASE_URL);
            this._client = client;
        }

        public async Task<AddressDto> GetAddressByCepAsync(string cep)
        {
            var response = await _client.GetAsync($"{cep}/json/");
            Console.WriteLine(response);
            response.EnsureSuccessStatusCode();

            var viaCepResponse = await response.Content.ReadFromJsonAsync<ViaCepResponse>();

            return new AddressDto(
                viaCepResponse!.Cep ?? "",
                viaCepResponse.Logradouro ?? "",
                viaCepResponse.Bairro ?? "",
                viaCepResponse.Localidade ?? "",
                viaCepResponse.Uf ?? ""
            );
        }

        private sealed class ViaCepResponse
        {
            public string? Cep { get; set; }
            public string? Logradouro { get; set; } = string.Empty;
            public string? Bairro { get; set; }
            public string? Localidade { get; set; }
            public string? Uf { get; set; }
        }
    }
}
