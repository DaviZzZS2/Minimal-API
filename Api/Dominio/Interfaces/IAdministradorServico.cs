using MinimalApi.Dominio.DTOs;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Entidades;

namespace MinimalAPI.Dominio.Interfaces;

public interface IAdministradorServico
{
    Administrador? Login(LoginDTO loginDTO);
    Administrador? ConsultarPorId(int id);
    Administrador Incluir(Administrador administradorDTO);
    List<Administrador> Todos(int? pagina);
}   