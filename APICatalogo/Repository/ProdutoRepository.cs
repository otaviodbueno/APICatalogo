using APICatalogo.Context;
using APICatalogo.Models;
using APICatalogo.Pagination;

namespace APICatalogo.Repository;

public class ProdutoRepository : Repository<Produto>, IProdutoRepository
{
    public ProdutoRepository(AppDbContext context) : base(context)
    {
    }

    //public IEnumerable<Produto> GetProdutos(ProdutosParameters produtosParameters)
    //{
    //    return GetAll()
    //        .OrderBy(p => p.Nome)
    //        .Skip((produtosParameters.PageNumber - 1) * produtosParameters.PageSize)
    //        .Take(produtosParameters.PageSize).ToList();
    //}

    public async Task<PagedList<Produto>> GetProdutosAsync(ProdutosParameters produtosParameters)
    {
        var produtos = await GetAllAsync();

        var produtosOrdenados = produtos.OrderBy(p => p.ProdutoId).AsQueryable();

        var resultado = PagedList<Produto>.ToPagedList(produtosOrdenados, produtosParameters.PageNumber, produtosParameters.PageSize);


        return resultado;
    }

    public async Task<PagedList<Produto>> GetProdutosFiltroPrecoAsync(ProdutosFiltroPreco produtosFiltroParameters)
    {
        var produtos = await GetAllAsync();

        if (produtosFiltroParameters.Preco.HasValue && !string.IsNullOrEmpty(produtosFiltroParameters.PrecoCriterio))
        {
            if(produtosFiltroParameters.PrecoCriterio.ToLower() == "menor")
            {
                produtos = produtos.Where(p => p.Preco < produtosFiltroParameters.Preco.Value);
            }
            else if (produtosFiltroParameters.PrecoCriterio.ToLower() == "maior")
            {
                produtos = produtos.Where(p => p.Preco > produtosFiltroParameters.Preco.Value);
            }
            else if (produtosFiltroParameters.PrecoCriterio.ToLower() == "igual")
            {
                produtos = produtos.Where(p => p.Preco == produtosFiltroParameters.Preco.Value);
            }
        }

        var produtosFiltrados = PagedList<Produto>.ToPagedList(produtos.AsQueryable(), produtosFiltroParameters.PageNumber, produtosFiltroParameters.PageSize);

        return produtosFiltrados;
    }

    public async Task<IEnumerable<Produto>> GetProdutosPorCategoria(int idCategoria)
    {
        var produtos = await GetAllAsync();
        var produtosCategoria = produtos.Where(p => p.CategoriaId == idCategoria);
        return produtosCategoria;
    }
}
