using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Infraestrutura.Db;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.ModelViews;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.Entidades;
var builder = WebApplication.CreateBuilder(args);

#region Builder

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DbContexto>(options =>
{
    options.UseMySql(
        builder.Configuration.GetConnectionString("mySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mySql"))
        );
});

var app = builder.Build();

#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("home");

#endregion


#region Administradores
app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
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
).WithTags("Administrador");

#endregion

#region Veiculos
app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };
    veiculoServico.Incluir(veiculo);

    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}
).WithTags("Veiculos");;

app.MapGet("/veiculos", (int? pagina, IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina);

    return Results.Ok(veiculos);
}
).WithTags("Veiculos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.ConsultarPorId(id);

    if (veiculo == null) return Results.NotFound();

    return Results.Ok(veiculo);
}
).WithTags("Veiculos");

app.MapPut("/veiculos/{id}", ([FromRoute] int id,VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.ConsultarPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);

    return Results.Ok(veiculo);
}
).WithTags("Veiculos");

app.MapDelete("/veiculos/{id}", ([FromRoute]int id,IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.ConsultarPorId(id);

    if (veiculo == null) return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}
).WithTags("Veiculos");
#endregion

#region AppSwagger
app.UseSwagger();
app.UseSwaggerUI();
#endregion
app.Run();
