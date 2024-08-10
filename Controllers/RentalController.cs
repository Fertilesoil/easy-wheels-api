using EasyWheelsApi.Data;
using EasyWheelsApi.Models.Dtos.RentalDtos;
using EasyWheelsApi.Models.Entities;
using EasyWheelsApi.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace EasyWheelsApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RentalController(
        IRentalService service,
        RentalDbContext dbContext,
        UserManager<User> userManager
    ) : ControllerBase
    {
        private readonly IRentalService _service = service;
        private readonly RentalDbContext _dbContext = dbContext;
        private readonly UserManager<User> _userManager = userManager;

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateRental([FromBody] AddRentalDto rental)
        {
            var lessor = await _userManager
                .Users.OfType<Lessor>()
                .FirstOrDefaultAsync(l => l.Id == rental.LessorId);
            var lessee = await _userManager
                .Users.OfType<Lessee>()
                .FirstOrDefaultAsync(l => l.Id == rental.LesseeId);

            if (lessor is null || lessee is null)
                return NotFound(
                    new
                    {
                        message = "One of the users in the contract doesn't have an registered account"
                    }
                );

            var newRental = await _service.CreateRentalAsync(rental);
            return Ok(newRental);
        }

        [Authorize]
        [HttpGet("{rentalId}")]
        public async Task<IActionResult> GetRentalById(Guid rentalId)
        {
            var contract = await _dbContext.Rentals.FirstOrDefaultAsync(r => r.Id == rentalId);

            if (contract is null)
                return NotFound(new { message = "None rental was found with this Id" });

            var result = await _service.GetRentalByIdAsync(rentalId);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{rentalId}/pdf")]
        public IActionResult GetRentalDemo(Guid rentalId)
        {
            string nome = "Fernando Dias Costa";
            string nacionalidade = "Brasileira";
            string profissao = "Açougueiro";
            string cpf = "99999999999";
            string rg = "233454443";
            string city = "Diadema";
            string state = "SP";
            string street = "Rua Moxotó";
            int houseNumber = 81;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.MarginTop(0.9f, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(t => t.FontSize(21));
                    page.DefaultTextStyle(t => t.FontFamily("Inter"));

                    page.Header()
                        .Text("CONTRATO DE LOCAÇÃO DE VEÍCULO ENTRE PARTICULARES")
                        .Bold()
                        .AlignCenter()
                        .FontSize(20);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Item().Text("Entre:").LineHeight(1.8f).FontSize(14);

                            x.Item()
                                .Text(
                                    $"{nome}, nacionalidade: {nacionalidade[..1].ToLower() + nacionalidade[1..]}, profissão: {profissao[..1].ToLower() + profissao[1..]}, carteira de identidade número: {rg}, CPF número: {cpf}, residente em: {city} {state}, {street} , {houseNumber}, doravante denominada LOCADORA(Lessor)"
                                )
                                .LineHeight(1.8f)
                                .FontSize(14);

                            x.Item().Text("e:").LineHeight(1.8f).FontSize(14);

                            x.Item()
                                .Text(
                                    $"{nome}, nacionalidade: {nacionalidade[..1].ToLower() + nacionalidade[1..]}, profissão: {profissao[..1].ToLower() + profissao[1..]}, carteira de identidade número: {rg}, CPF número: {cpf}, residente em: {city} {state}, {street} , {houseNumber}, doravante denominada LOCATÁRIA(Lesser)."
                                )
                                .LineHeight(1.8f)
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(1f, Unit.Centimetre)
                                .Text(
                                    "As partes acima identificadas têm entre si, justo e acertado, o presente contrato de locação de veículo que será regulado pelas cláusulas e condições abaixo descritas."
                                )
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(1f, Unit.Centimetre)
                                .Text("1. Objeto:")
                                .ExtraBold()
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(0.3f, Unit.Centimetre)
                                .Text(
                                    "O presente contrato tem por objeto a locação do veículo [Marca], [Modelo], ano [Ano], placa [Placa], cor [Cor], de propriedade do Locador, ao Locatário, pelo período de [Data de início] a [Data de fim]."
                                )
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(1f, Unit.Centimetre)
                                .Text("2. Valor e Forma de Pagamento:")
                                .ExtraBold()
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(0.3f, Unit.Centimetre)
                                .Text(
                                    "O valor total do aluguel é de R$ [Valor total], a ser pago da seguinte forma: [Forma de pagamento, por exemplo: à vista, em [número] parcelas de R$ [valor da parcela], vencíveis nos dias [dias do vencimento]]."
                                )
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(1f, Unit.Centimetre)
                                .Text("3. Condições de Uso:")
                                .ExtraBold()
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(0.3f, Unit.Centimetre)
                                .Text(
                                    "3.1. O veículo será entregue ao Locatário em perfeitas condições de uso, com tanque de combusvel cheio e todos os documentos em ordem."
                                )
                                .FontSize(14);
                            x.Item()
                                .Text(
                                    "3.2. O Locatário se compromete a ulizar o veículo de forma cuidadosa e responsável, observando as normas de trânsito e as leis vigentes."
                                )
                                .FontSize(14);
                            x.Item().Text("3.3. É proibida a sublocação do veículo.").FontSize(14);
                            x.Item()
                                .Text(
                                    "3.4. O Locatário será responsável por todas as multas e infrações comedas durante o período de locação."
                                )
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(1f, Unit.Centimetre)
                                .Text("4. Devolução do Veículo:")
                                .ExtraBold()
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(0.3f, Unit.Centimetre)
                                .Text(
                                    "O Locatário se compromete a devolver o veículo ao Locador no dia [Data de devolução], no mesmo local da entrega, nas mesmas condições em que o recebeu, salvo o desgaste natural pelo uso."
                                )
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(1f, Unit.Centimetre)
                                .Text("5. Responsabilidades:")
                                .ExtraBold()
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(0.3f, Unit.Centimetre)
                                .Text(
                                    "5.1. O Locador garante ser o proprietário legímo do veículo e que o mesmo se encontra em condições de uso."
                                )
                                .FontSize(14);
                            x.Item()
                                .Text(
                                    "5.2. O Locatário é responsável por qualquer dano causado ao veículo, exceto por desgaste natural."
                                )
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(1f, Unit.Centimetre)
                                .Text("6. Resolução:")
                                .ExtraBold()
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(0.3f, Unit.Centimetre)
                                .Text(
                                    "Qualquer das partes poderá rescindir o presente contrato mediante noficação prévia com [número] dias de antecedência."
                                )
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(1f, Unit.Centimetre)
                                .Text("7. Foro:")
                                .ExtraBold()
                                .FontSize(14);

                            x.Item()
                                .PaddingTop(0.3f, Unit.Centimetre)
                                .Text(
                                    "Eleito o foro da comarca de [Cidade] para dirimir quaisquer dúvidas ou ligios decorrentes deste contrato."
                                )
                                .FontSize(14);
                            x.Item()
                                .Text(
                                    "E por estarem assim justos e acordados, as partes firmam o presente instrumento em [Cidade], no dia [Dia] de [Mês] de [Ano]."
                                )
                                .FontSize(14);

                            x.Spacing(0.12f, Unit.Centimetre);
                            x.Item()
                                .PaddingTop(0.5f, Unit.Centimetre)
                                .PaddingBottom(0.3f, Unit.Centimetre)
                                .BorderTop(0.03f, Unit.Inch);

                            x.Item()
                                .Row(r =>
                                {
                                    r.Spacing(20);

                                    r.RelativeItem()
                                        .Column(c =>
                                        {
                                            c.Item()
                                                .Text("[Nome do Locadora]")
                                                .AlignLeft()
                                                .FontSize(13)
                                                .SemiBold();
                                            c.Item().Text("[CPF]").AlignLeft().SemiBold();
                                            c.Item().Text("").AlignLeft();
                                            c.Item()
                                                .Text("____________________________________")
                                                .AlignLeft();
                                            c.Item()
                                                .Text("Assinatura do Locador")
                                                .AlignLeft()
                                                .Bold();
                                        });

                                    r.RelativeItem()
                                        .Column(c =>
                                        {
                                            c.Item()
                                                .Text("[Nome do Locatária]")
                                                .AlignRight()
                                                .FontSize(13)
                                                .SemiBold();
                                            c.Item().Text("[CPF]").AlignRight().SemiBold();
                                            c.Item().Text("").AlignRight();
                                            c.Item()
                                                .Text("____________________________________")
                                                .AlignRight();
                                            c.Item()
                                                .Text("Assinatura do Locatário")
                                                .AlignRight()
                                                .Bold();
                                        });
                                });
                        });

                    page.Footer()
                        .AlignLeft()
                        .Text(t =>
                        {
                            t.Span("Página ");
                            t.CurrentPageNumber();
                        });

                    page.Background();
                });
            });

            var dataPdf = document.GeneratePdf();
            return File(dataPdf, "application/pdf", "contratomodelo.pdf");
        }
    }
}