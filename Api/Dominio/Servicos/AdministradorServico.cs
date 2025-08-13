
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;
using MinimalApi.Dominio.DTOs;

namespace MinimalApi.Dominio.Servicos;

public class AdministradorServico : IAdministradorServico
{
    private readonly DbContexto _contexto;
    public AdministradorServico(DbContexto contexto)
    {
        _contexto = contexto;
    }
    public Administrador? Login(LoginDTO loginDTO)
    {
        return _contexto.Administradores
            .Where(a => a.Email == loginDTO.Email && a.Senha == loginDTO.Senha).FirstOrDefault();
    }

    public Administrador? Incluir(Administrador administrador)
    {
        _contexto.Administradores.Add(administrador);
        _contexto.SaveChanges();
        return administrador;
    }

    public List<Administrador> Todos(int? pagina = 1, string? email = null, string? perfil = null)
    {
        var query = _contexto.Administradores.AsQueryable();

        bool hasEmail = !string.IsNullOrEmpty(email);
        bool hasPerfil = !string.IsNullOrEmpty(perfil);
        bool hasBoth = hasEmail && hasPerfil;

        if (hasEmail)
        {
            query = query.Where(a => a.Email.Contains(email!, StringComparison.CurrentCultureIgnoreCase));
        }

        if (hasPerfil)
        {
            query = query.Where(a => a.Perfil.Contains(perfil!, StringComparison.CurrentCultureIgnoreCase));
        }

        if (hasBoth)
        {
            query = query.Where(
                a => a.Email.Contains(email!, StringComparison.CurrentCultureIgnoreCase) &&
                     a.Perfil.Contains(perfil!, StringComparison.CurrentCultureIgnoreCase)
                     ).OrderBy(a => a.Email);
        }

        int itensPorPagina = 10;

        if (pagina != null)
        { 
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        

        return query.ToList();

    }

    public Administrador? BuscarPorId(int id)
    {
        return _contexto.Administradores.Find(id);
    }

    public Administrador? Atualizar(Administrador administrador)
    {
        _contexto.Administradores.Update(administrador);
        _contexto.SaveChanges();
        return administrador;
    }

    public void Apagar(Administrador administrador)
    {
        _contexto.Administradores.Remove(administrador);
        _contexto.SaveChanges();
    }
}    
