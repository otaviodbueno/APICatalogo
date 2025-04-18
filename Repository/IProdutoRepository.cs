using APICatalogo.Models;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace APICatalogo.Repository;

public interface IProdutoRepository : IRepository<Produto>
{
    IEnumerable<Produto> GetProdutosPorCategoria(int idCategoria);
}
