using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Dominio.Servicos;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using MinimalApi.Dominio.ModelViews;
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministradorServico, AdministradorServico>();
builder.Services.AddScoped<IVeiculoServico, VeiculoServico>();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<DbContexto>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("mysql"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("mysql"))));


var app = builder.Build();
#endregion

#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Administradores
const string AdministradoresTag = "Administradores";

app.MapPost("/administradores/login", ([FromBody] LoginDTO loginDTO, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.Login(loginDTO);

    if (administrador != null)
        return Results.Ok("Login com sucesso!");
    else
        return Results.Unauthorized();
}).WithTags(AdministradoresTag).WithDescription("Realizar login de administrador");

app.MapPost("/administradores", ([FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = administradorDTO.ValidarDTO();

    if (validacao.Mensagens.Count != 0)
    {
        return Results.BadRequest(validacao);
    }

    var administrador = new Administrador
    {
        Email = administradorDTO.Email,
        Senha = administradorDTO.Senha,
        Perfil = administradorDTO.Perfil
    };

    administradorServico.Incluir(administrador);
    return Results.Created($"/administrador/{administrador.Id}", administrador);
}).WithTags(AdministradoresTag).WithDescription("Criar um novo administrador");

app.MapPut("/administradores/{id}", ([FromRoute] int id, [FromBody] AdministradorDTO administradorDTO, IAdministradorServico administradorServico) =>
{
    var validacao = administradorDTO.ValidarDTO();

    if (validacao.Mensagens.Count != 0)
    {
        return Results.BadRequest(validacao);
    }

    var administrador = administradorServico.BuscarPorId(id);

    if (administrador == null)
        return Results.NotFound();

    administrador.Email = administradorDTO.Email;
    administrador.Senha = administradorDTO.Senha;
    administrador.Perfil = administradorDTO.Perfil;

    administradorServico.Atualizar(administrador);
    return Results.Ok(administrador);
}).WithTags(AdministradoresTag).WithDescription("Atualizar um administrador por ID");

app.MapGet("/administradores", (IAdministradorServico administradorServico) =>
{
    var administradores = administradorServico.Todos();
    return Results.Ok(administradores);
}).WithTags(AdministradoresTag).WithDescription("Listar todos os administradores");

app.MapDelete("/administradores/{id}", ([FromRoute] int id, IAdministradorServico administradorServico) =>
{
    var administrador = administradorServico.BuscarPorId(id);

    if (administrador == null)
        return Results.NotFound();

    administradorServico.Apagar(administrador);

    return Results.NoContent();
}).WithTags(AdministradoresTag).WithDescription("Apagar um administrador por ID");
#endregion

#region Veiculos
const string VeiculosTag = "Veiculos";

app.MapPost("/veiculos", ([FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{

    var validacao = veiculoDTO.ValidarDTO();

    if (validacao.Mensagens.Count != 0)
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
}).WithTags(VeiculosTag).WithDescription("Incluir um novo veículo");

app.MapGet("/veiculos", (
    [FromQuery] int? pagina,
    [FromQuery] string? nome,
    [FromQuery] string? marca,
    IVeiculoServico veiculoServico) =>
{
    var veiculos = veiculoServico.Todos(pagina, nome, marca);
    return Results.Ok(veiculos);
}).WithTags(VeiculosTag).WithDescription("Listar todos os veículos");

app.MapGet("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);

    return veiculo != null ? Results.Ok(veiculo) : Results.NotFound();
}).WithTags(VeiculosTag).WithDescription("Buscar veículo por ID");

app.MapPut("/veiculos/{id}", ([FromRoute] int id, [FromBody] VeiculoDTO veiculoDTO, IVeiculoServico veiculoServico) =>
{
    var validacao = veiculoDTO.ValidarDTO();

    if (validacao.Mensagens.Count != 0)
    {
        return Results.BadRequest(validacao);
    }

    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null)
        return Results.NotFound();

    veiculo.Nome = veiculoDTO.Nome;
    veiculo.Marca = veiculoDTO.Marca;
    veiculo.Ano = veiculoDTO.Ano;

    veiculoServico.Atualizar(veiculo);
    return Results.Ok(veiculo);
}).WithTags(VeiculosTag).WithDescription("Atualizar veículo por ID");

app.MapDelete("/veiculos/{id}", ([FromRoute] int id, IVeiculoServico veiculoServico) =>
{
    var veiculo = veiculoServico.BuscarPorId(id);

    if (veiculo == null)
        return Results.NotFound();

    veiculoServico.Apagar(veiculo);

    return Results.NoContent();
}).WithTags(VeiculosTag).WithDescription("Apagar veículo por ID");

#endregion

#region App
app.UseSwagger();
app.UseSwaggerUI();

await app.RunAsync();
#endregion



