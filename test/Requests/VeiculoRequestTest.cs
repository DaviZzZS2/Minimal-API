using System.Net;
using System.Text;
using System.Text.Json;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Entidades;
using Test.Helpers;

namespace Test.Domain.Requests;

[TestClass]
public class VeiculoRequestTest
{
    [ClassInitialize]
    public static void ClassInit(TestContext testContext)
    {
        Setup.ClassInit(testContext);
    }

    [ClassCleanup]
    public static void ClassCleanUp()
    {
        Setup.ClassCleanUp();
    }

    [TestMethod]
    public async Task TestarIncluirVeiculo()
    {
        // Arrange: criar um objeto VeiculoDTO ou modelo similar
        var veiculoDTO = new VeiculoDTO
        {
            Marca = "Fiat",
            Nome = "Palio",
            Ano = 2010
        };

        var content = new StringContent(
            JsonSerializer.Serialize(veiculoDTO), 
            Encoding.UTF8, 
            "application/json"
        );

        // Act: enviar POST para criar veículo
        var response = await Setup.client.PostAsync("/veiculos", content);

        // Assert: verificar status HTTP
        Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();

        // Desserializar resposta
        var veiculoCriado = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculoCriado);
        Assert.IsNotNull(veiculoCriado.Id); 
        Assert.AreEqual(veiculoDTO.Marca, veiculoCriado.Marca);
        Assert.AreEqual(veiculoDTO.Nome, veiculoCriado.Nome);
        Assert.AreEqual(veiculoDTO.Ano, veiculoCriado.Ano);
    }

    [TestMethod]
    public async Task TestarConsultarVeiculoPorId()
    {
        // Arrange: Id do veículo que você quer consultar (criado anteriormente ou mock)
        int idVeiculo = 1;

        // Act
        var response = await Setup.client.GetAsync($"/veiculos/{idVeiculo}");

        // Assert
        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();
        var veiculo = JsonSerializer.Deserialize<Veiculo>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(veiculo);
        Assert.AreEqual(idVeiculo, veiculo.Id);
    }
}
