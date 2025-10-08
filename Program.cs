using MinimalApi.DTOs;
var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapPost("/login", (LoginDTO login) => 
{
    if (login.Email == "adm@teste.com" && login.Senha == "123456")
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
