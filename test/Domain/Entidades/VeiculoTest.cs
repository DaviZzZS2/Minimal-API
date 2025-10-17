using MinimalAPI.Entidades;

namespace Test.Domain.Entidades;

[TestClass]

public class VeiculoTest
{
    [TestMethod]

    public void TestarGetSetPropriedades()
    {
        //Arrange
        var veiculo = new Veiculo();

        //Act
        veiculo.Id = 1;
        veiculo.Ano = 2012;
        veiculo.Marca = "BMW";
        veiculo.Nome = "A7";

        //Assert
        Assert.AreEqual(1, veiculo.Id);
        Assert.AreEqual(2012, veiculo.Ano);
        Assert.AreEqual("BMW",  veiculo.Marca);
        Assert.AreEqual("A7", veiculo.Nome);


    }
}