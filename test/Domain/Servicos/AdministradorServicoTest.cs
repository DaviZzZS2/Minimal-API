using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MinimalApi.Infraestrutura.Db;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.Entidades;

namespace Test.Domain.Entidades;

[TestClass]

public class AdministradorServicoTest
{

    private DbContexto CriarContextoTeste()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        var path = Path.GetFullPath(Path.Combine(assemblyPath ?? "", "..", "..", ".."));
       
        var options = new ConfigurationBuilder()
        .SetBasePath(path)
        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddEnvironmentVariables();


        return new DbContexto(options.Build());
    }
    

    [TestMethod]
    public void TestarSalvarAdministrador()
    {
        //Arrange

        var context = CriarContextoTeste();

        context.Database.ExecuteSqlRaw("TRUNCATE TABLE administradores");

        var adm = new Administrador();

        adm.Email = "teste@teste.com";
        adm.Senha = "teste";
        adm.Perfil = "Adm";
        

        //Act

        var administradorServico = new AdministradorServico(context);

        administradorServico.Incluir(adm);
        var admTeste = administradorServico.ConsultarPorId(1);
        //Assert
        if (admTeste != null)
        {
            Assert.AreEqual(1, admTeste.Id);
        }


    }
}