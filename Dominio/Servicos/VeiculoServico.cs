
using MinimalApi.Dominio.Entidades;
using MinimalApi.Dominio.Interfaces;
using MinimalApi.Infraestrutura.Db;

namespace MinimalApi.Dominio.Servicos;

public class VeiculoServico : IVeiculoServico
{
    private readonly DbContexto _contexto;
    public VeiculoServico(DbContexto contexto)
    {
        _contexto = contexto;
    }

    public Veiculo? BuscarPorId(int id)
    {
        return _contexto.Veiculos.Find(id);
    }

    public List<Veiculo> Todos(int? pagina = 1, string? nome = null, string? marca = null)
    {
        var query = _contexto.Veiculos.AsQueryable();

        bool hasNome = !string.IsNullOrEmpty(nome);
        bool hasMarca = !string.IsNullOrEmpty(marca);
        bool hasBoth = hasNome && hasMarca;

        if (hasNome)
        {
            query = query.Where(v => v.Nome.Contains(nome!, StringComparison.CurrentCultureIgnoreCase));
        }

        if (hasMarca)
        {
            query = query.Where(v => v.Marca.Contains(marca!, StringComparison.CurrentCultureIgnoreCase));
        }

        if (hasBoth)
        {
            query = query.Where(
                v => v.Nome.Contains(nome!, StringComparison.CurrentCultureIgnoreCase) &&
                     v.Marca.Contains(marca!, StringComparison.CurrentCultureIgnoreCase)
                     ).OrderBy(v => v.Nome);
        }

        int itensPorPagina = 10;

        if (pagina != null)
        { 
            query = query.Skip(((int)pagina - 1) * itensPorPagina).Take(itensPorPagina);
        }
        

        return query.ToList();

        // The commented code below is an alternative way to implement the same logic.
        // var query = _contexto.Veiculos.AsQueryable();

        // if (!string.IsNullOrEmpty(nome))
        // {
        //     query = query.Where(v => v.Nome.Contains(nome));
        // }

        // if (!string.IsNullOrEmpty(marca))
        // {
        //     query = query.Where(v => v.Marca.Contains(marca));
        // }

        // return query.Skip((pagina - 1) * 10).Take(10).ToList();
    }

    public void Incluir(Veiculo veiculo)
    {
        _contexto.Veiculos.Add(veiculo);
        _contexto.SaveChanges();
    }

    public void Atualizar(Veiculo veiculo)
    {
        _contexto.Veiculos.Update(veiculo);
        _contexto.SaveChanges();
    }

    public void Apagar(Veiculo veiculo)
    {
        _contexto.Veiculos.Remove(veiculo);
        _contexto.SaveChanges();
    }
}
