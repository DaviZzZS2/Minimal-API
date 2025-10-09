using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Infraestrutura.Db;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.Servicos;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mySql"))
        );
});

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", ([FromBody] LoginDTO loginDTO ,IAdministradorServico administradorServico) => 
{
    if (administradorServico.Login(loginDTO) != null)
    {
        return Results.Ok("Login autorizado");
    }
    else
    {
        return Results.Unauthorized();
    }
}
);

app.Run();
