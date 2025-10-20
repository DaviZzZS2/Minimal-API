using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Infraestrutura.Db;
using MinimalAPI;
using MinimalAPI.Dominio.DTOs;
using MinimalAPI.Dominio.Enuns;
using MinimalAPI.Dominio.Interfaces;
using MinimalAPI.Dominio.ModelViews;
using MinimalAPI.Dominio.Servicos;
using MinimalAPI.Entidades;


public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
        key = Configuration?["jwt"] ?? "";
    }

    private string key = "";

    public IConfiguration Configuration { get; set; } = default!;

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddAuthentication(option =>
        {
            option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(option =>
        {
            option.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        services.AddAuthorization();

        services.AddScoped<IAdministradorServico, AdministradorServico>();
        services.AddScoped<IVeiculoServico, VeiculoServico>();

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Insira o Token JWT aqui"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                new OpenApiSecurityScheme{
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[]{}
                }
            });
        });


        services.AddDbContext<DbContexto>(options =>
        {
            options.UseMySql(
                Configuration.GetConnectionString("MySql"),
                ServerVersion.AutoDetect(Configuration.GetConnectionString("MySql"))
                );
        });
    }
    
    public void Configure(IApplicationBuilder app,IWebHostEnvironment env)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        
        app.UseRouting();

        app.UseAuthentication();
        app.UseAuthorization();


        app.UseEndpoints(endpoint =>
        {
            #region Home
            endpoint.MapGet("/", () => Results.Json(new Home())).WithTags("home");

            #endregion


            #region Administradores

            string GerarTokenJwt(Administrador administrador)
            {
                if (string.IsNullOrEmpty(key))
                {
                    return string.Empty;
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>()
                {
                    new Claim("Email",administrador.Email),
                    new Claim("Perfil",administrador.Perfil),
                    new Claim(ClaimTypes.Role,administrador.Perfil)
                };

                var token = new JwtSecurityToken
                (
                    claims: claims,
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: credentials

                );

            return new JwtSecurityTokenHandler().WriteToken(token);
            }


            endpoint.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
            {
                var adm = administradorServico.Login(loginDTO);
                if (adm != null)
                {
                    string token = GerarTokenJwt(adm);
                    return Results.Ok(new AdmLogado
                    {
                        Email = adm.Email,
                        Perfil = adm.Perfil,
                        Token = token
                    });
                }
                else
                {
                    return Results.Unauthorized();
                }
            }
            ).AllowAnonymous().WithTags("Administradores");

            endpoint.MapGet("/administradores", ([FromQuery] int? pagina,IAdministradorServico administradorServico) =>
            {
                var adms = new List<AdministradorModelView>();
                var administradores = administradorServico.Todos(pagina);
                foreach(var adm in administradores){
                    
                    adms.Add(new AdministradorModelView{
                        Id = adm.Id,
                        Email = adm.Email,
                        Perfil = adm.Perfil    
                    });
                    
                }
                return Results.Ok(adms);
            }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Administradores");

            endpoint.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
            {
                var administrador = administradorServico.ConsultarPorId(id);
                if (administrador == null) return Results.NotFound();
                var adm = new AdministradorModelView
                {
                    Id = administrador.Id,
                    Email = administrador.Email,
                    Perfil = administrador.Perfil
                };
                return Results.Ok(adm);
            }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Administradores");

            endpoint.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
            {

                var validacao = new ErrosDeValidacao
                {
                    Mensagens = new List<string>()
                };
                if (string.IsNullOrEmpty(administradorDTO.Email))
                {
                    validacao.Mensagens.Add("Email não pode ser vazio");
                }
                if (string.IsNullOrEmpty(administradorDTO.Senha))
                {
                    validacao.Mensagens.Add("Senha não pode ser vazia");
                }
                if (administradorDTO.Perfil == null)
                {
                    validacao.Mensagens.Add("Perfil não pode ser vazio");
                }

                if (validacao.Mensagens.Count > 0)
                {
                    Results.BadRequest(validacao);
                }
                var administrador = new Administrador
                {
                    Email = administradorDTO.Email,
                    Senha = administradorDTO.Senha,
                    Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
                };
                administradorServico.Incluir(administrador);

                return Results.Created($"/administrador/{administrador.Id}", administrador);
            }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Administradores");



            #endregion

            #region Veiculos

            ErrosDeValidacao validaDTO(VeiculoDTO veiculoDTO)
            {
                var validacao = new ErrosDeValidacao
                {
                    Mensagens = new List<string>()
                };
                if (string.IsNullOrEmpty(veiculoDTO.Nome))
                {
                    validacao.Mensagens.Add("O nome não pode ser vazio");
                }
                if (string.IsNullOrEmpty(veiculoDTO.Marca))
                {
                    validacao.Mensagens.Add("A marca não pode ser vazio");
                }
                if (veiculoDTO.Ano < 1950)
                {
                    validacao.Mensagens.Add("Ano muito antigo,aceitamos apenas carros com ano superior a 1950");
                }

                return validacao;
            }
            endpoint.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
            {
                var validacao = validaDTO(veiculoDTO);
                if(validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }
                var veiculo = new Veiculo
                {
                    Nome = veiculoDTO.Nome,
                    Marca = veiculoDTO.Marca,
                    Ano = veiculoDTO.Ano
                };
                veiculoServico.Incluir(veiculo);

                return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
            }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"})
            .WithTags("Veiculos");

            endpoint.MapGet("/veiculos", (int? pagina, IVeiculoServico veiculoServico) =>
            {
                var veiculos = veiculoServico.Todos(pagina);

                return Results.Ok(veiculos);
            }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"})
            .WithTags("Veiculos");

            endpoint.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.ConsultarPorId(id);

                if (veiculo == null) return Results.NotFound();

                return Results.Ok(veiculo);
            }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"})
            .WithTags("Veiculos");

            endpoint.MapPut("/veiculos/{id}", ([FromRoute] int id,VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.ConsultarPorId(id);
                if (veiculo == null) return Results.NotFound();

                var validacao = validaDTO(veiculoDTO);
                if(validacao.Mensagens.Count > 0)
                {
                    return Results.BadRequest(validacao);
                }

                veiculo.Nome = veiculoDTO.Nome;
                veiculo.Marca = veiculoDTO.Marca;
                veiculo.Ano = veiculoDTO.Ano;

                veiculoServico.Atualizar(veiculo);

                return Results.Ok(veiculo);
            }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm"})
            .WithTags("Veiculos");

            endpoint.MapDelete("/veiculos/{id}", ([FromRoute]int id,IVeiculoServico veiculoServico) =>
            {
                var veiculo = veiculoServico.ConsultarPorId(id);
                if (veiculo == null) return Results.NotFound();

                veiculoServico.Apagar(veiculo);

                return Results.NoContent();
            }
            ).RequireAuthorization()
            .RequireAuthorization(new AuthorizeAttribute{Roles = "Adm,Editor"})
            .WithTags("Veiculos");
            #endregion
        });
    }
}
