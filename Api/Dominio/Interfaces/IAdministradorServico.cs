
using MinimalApi.Dominio.DTOs;
using MinimalApi.Dominio.Entidades;

namespace MinimalApi.Dominio.Interfaces;

public interface IAdministradorServico
{
    public Administrador? Login(LoginDTO loginDTO);

    public Administrador? Incluir(Administrador administrador);
    public List<Administrador> Todos(int? pagina = 1, string? email = null, string? perfil = null);

    public Administrador? BuscarPorId(int id);

    public Administrador? Atualizar(Administrador administrador);

    public void Apagar(Administrador administrador);
}


