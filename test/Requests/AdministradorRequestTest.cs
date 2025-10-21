using System.Net;
using System.Text;
using System.Text.Json;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.ModelViews;
using MinimalAPI.Entidades;
using Test.Helpers;

namespace Test.Domain.Requests;

[TestClass]

public class AdministradorRequestTest
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
    public async Task TestarGetSetPropriedades()
    {

        //Arrange
        var loginDTO = new LoginDTO
        {
            Email = "adm@teste.com",
            Senha = "123456"
        };

        var content = new StringContent(JsonSerializer.Serialize(loginDTO), Encoding.UTF8, "Application/json");
        //Act

        var response = await Setup.client.PostAsync("/administradores/login", content);

        //Assert

        Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadAsStringAsync();

        var admLogado = JsonSerializer.Deserialize<AdmLogado>(result, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        Assert.IsNotNull(admLogado?.Email);
        Assert.IsNotNull(admLogado.Perfil);
        Assert.IsNotNull(admLogado.Token);
    }
}