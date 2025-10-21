using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Entidades;

namespace Test.Mocks;

public class VeiculoServicoMock : IVeiculoServico
{

     private static List<Veiculo> veiculos = new List<Veiculo>()
    {
        new Veiculo
        {
            Id = 1,
            Marca = "teste",
            Nome = "teste",
            Ano = 2000
        }
    };
    public void Apagar(Veiculo veiculo)
    {
        veiculos.RemoveAll(v => v.Id == veiculo.Id);
    }

    public void Atualizar(Veiculo veiculo)
    {
        int index = veiculos.FindIndex(v => v.Id == veiculo.Id);

        if (index >= 0)
        {
            veiculos[index] = veiculo;
        }

    }

    public Veiculo? ConsultarPorId(int id)
    {
        return veiculos.Find(a => a.Id == id);
    }

    public void Incluir(Veiculo veiculo)
    {
        veiculo.Id = veiculos.Count() + 1;
        veiculos.Add(veiculo);
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        return veiculos;
    }
}