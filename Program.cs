using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if (string.IsNullOrEmpty(key)) key = "minimal";


builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT aqui"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
});


builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySql"))));


var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).AllowAnonymous().WithTags("Home");
#endregion

#region Administradores
const string AdministradoresTag = "Administradores";

string GerarTokenJwt(Administrador administrador)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;
    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

    var claims = new List<Claim>()
    {
        new Claim("Email", administrador.Email),
        new Claim("Perfil", administrador.Perfil)
    };
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: credentials
    );
    return new JwtSecurityTokenHandler().WriteToken(token);
}

// Helper methods for Administrador
static IResult ValidarAdministradorDTO(AdministradorDTO dto)
{
    var validacao = dto.ValidarDTO();
    if (validacao.Mensagens.Count != 0)
        return Results.BadRequest(validacao);
    return null;
}

static AdministradorModelView MapToModelView(Administrador administrador)
{
    return new AdministradorModelView
    {
        Id = administrador.Id,
        Email = administrador.Email,
        Perfil = administrador.Perfil
    };
}

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.Login(loginDTO);

    if (administrador != null)
    {
        string token = GerarTokenJwt(administrador);
        return Results.Ok(new AdministradorLogado
        {
            Token = token,
            Administrador = MapToModelView(administrador)
        });
    }

    return Results.Unauthorized();
    // return administrador != null
    //     ? Results.Ok("Login com sucesso!")
    //     : Results.Unauthorized();
}).AllowAnonymous().WithTags(AdministradoresTag).WithDescription("Realizar login de administrador");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var erro = ValidarAdministradorDTO(administradorDTO);
    if (erro != null) return erro;

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString()
    };

    administradorServico.Incluir(administrador);
    return Results.Created($"/administrador/{administrador.Id}", MapToModelView(administrador));
}).RequireAuthorization().WithTags(AdministradoresTag).WithDescription("Criar um novo administrador");

app.MapPut("/administradores/{id}", ([FromRoute] int id, [FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var erro = ValidarAdministradorDTO(administradorDTO);
    if (erro != null) return erro;

    var administrador = administradorServico.BuscarPorId(id);
    if (administrador == null)
        return Results.NotFound();

    administrador.Email = administradorDTO.Email;
    administrador.Senha = administradorDTO.Senha;
    administrador.Perfil = administradorDTO.Perfil.ToString() ?? Perfil.Editor.ToString();

    administradorServico.Atualizar(administrador);
    return Results.Ok(MapToModelView(administrador));
}).RequireAuthorization().WithTags(AdministradoresTag).WithDescription("Atualizar um administrador por ID");

app.MapGet("/administradores", (
    [FromQuery] int? pagina,
    [FromQuery] string? email,
    [FromQuery] string? perfil,
    IAdministradorServico administradorServico) =>
{
    var administradores = administradorServico.Todos(pagina, email, perfil)
        .Select(MapToModelView)
        .ToList();
    return Results.Ok(administradores);
}).RequireAuthorization().WithTags(AdministradoresTag).WithDescription("Listar todos os administradores");

app.MapGet("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);
    return administrador == null
        ? Results.NotFound()
        : Results.Ok(MapToModelView(administrador));
}).RequireAuthorization().WithTags(AdministradoresTag).WithDescription("Buscar um administrador por ID");

app.MapDelete("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);
    if (administrador == null)
        return Results.NotFound();

    administradorServico.Apagar(administrador);
    return Results.NoContent();
}).RequireAuthorization().WithTags(AdministradoresTag).WithDescription("Apagar um administrador por ID");
#endregion

#region Veiculos
const string VeiculosTag = "Veiculos";

// Helper methods for Veiculo
static IResult ValidarVeiculoDTO(VeiculoDTO dto)
{
    var validacao = dto.ValidarDTO();
    if (validacao.Mensagens.Count != 0)
        return Results.BadRequest(validacao);
    return null;
}

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var erro = ValidarVeiculoDTO(veiculoDTO);
    if (erro != null) return erro;

    var veiculo = new Veiculo
    {
        Nome = veiculoDTO.Nome,
        Marca = veiculoDTO.Marca,
        Ano = veiculoDTO.Ano
    };

    veiculoServico.Incluir(veiculo);
    return Results.Created($"/veiculo/{veiculo.Id}", veiculo);
}).RequireAuthorization().WithTags(VeiculosTag).WithDescription("Incluir um novo veículo");

app.MapGet("/veiculos", (
    [FromQuery] int? pagina,
    [FromQuery] string? nome,
    [FromQuery] string? marca,
    IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina, nome, marca);
    return Results.Ok(veiculos);
}).RequireAuthorization().WithTags(VeiculosTag).WithDescription("Listar todos os veículos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);
    return veiculo != null ? Results.Ok(veiculo) : Results.NotFound();
}).RequireAuthorization().WithTags(VeiculosTag).WithDescription("Buscar veículo por ID");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var erro = ValidarVeiculoDTO(veiculoDTO);
    if (erro != null) return erro;

    var veiculo = veiculoServico.BuscarPorId(id);
    if (veiculo == null)
        return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);
    return Results.Ok(veiculo);
}).RequireAuthorization().WithTags(VeiculosTag).WithDescription("Atualizar veículo por ID");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);
    if (veiculo == null)
        return Results.NotFound();

    veiculoServico.Apagar(veiculo);
    return Results.NoContent();
}).RequireAuthorization().WithTags(VeiculosTag).WithDescription("Apagar veículo por ID");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication(); // deve-se usar esta ordem, para evitar efeitos colaterais.
app.UseAuthorization();

app.Run();


#endregion



