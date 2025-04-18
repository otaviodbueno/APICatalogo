using APICatalogo.Context;
using APICatalogo.Models;

namespace APICatalogo.Repository;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    public IEnumerable<Produto> GetProdutosPorCategoria(int idCategoria)
    {
        return GetAll().Where(p => p.CategoriaId == idCategoria);
    }
}
