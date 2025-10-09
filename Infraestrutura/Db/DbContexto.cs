using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MinimalAPI.Entidades;

namespace MinimalApi.Infraestrutura.Db;

public class DbContexto : DbContext
{
    private readonly IConfiguration _configuracaoAppSettings;
    public DbSet<Administrador> Administradores { get; set; } = default!;
    public DbContexto(IConfiguration configuracaoAppSettings)
    {
        _configuracaoAppSettings = configuracaoAppSettings;
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Administrador>().HasData(
            new Administrador
            {
                Id = 1,
                Email = "administador@teste.com",
                Senha = "123456",
                Perfil = "Adm"

            }
        );
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var stringConexao = _configuracaoAppSettings.GetConnectionString("mySql")?.ToString();
            if (!string.IsNullOrEmpty(stringConexao))
            {
                optionsBuilder.UseMySql(stringConexao,
                ServerVersion.AutoDetect(stringConexao));
            }
        }
    }
}