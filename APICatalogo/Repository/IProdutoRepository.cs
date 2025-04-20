using APICatalogo.Models;
using APICatalogo.Pagination;
using Microsoft.EntityFrameworkCore.Update.Internal;

namespace APICatalogo.Repository;

public interface IProdutoRepository : IRepository<Produto>
{
    //IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters);
    Task<IEnumerable<Produto>> GetProdutosPorCategoria(int idCategoria);

    Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters);
    Task<PagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParameters);

}
