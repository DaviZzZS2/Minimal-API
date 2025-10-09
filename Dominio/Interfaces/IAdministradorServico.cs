using MinimalApi.Dominio.DTOs;
using MinimalAPI.Entidades;

namespace MinimalAPI.Dominio.Interfaces;

public interface IAdministradorServico
{
    Administrador Login(LoginDTO loginDTO);
}   