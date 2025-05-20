using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Repository;
using APICatalogo.DTO;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using APICatalogo.Pagination;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutoController : ControllerBase
    {

        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProdutoController(IUnitOfWork uof, IMapper mapper)
        {
            _unitOfWork = uof;
            _mapper = mapper;

        }

        [HttpGet("produtos/{id}")]
        public ActionResult<IEnumerable<ProdutoDTO>> GetProdutosPorCategoria(int id)
        {
            var produtos = _unitOfWork.ProdutoRepository.GetProdutosPorCategoria(id);
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!");
            }

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }

        [Authorize(Policy = "UserOnly")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get()
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetAllAsync();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!");
            }

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }


        [HttpGet("{id}", Name = "ObterProduto")]
        public async Task<ActionResult<ProdutoDTO>> Get(int id)
        {
            if(id == null || id <= 0)
            {
                return BadRequest("Id inválido!");
            }

            var produto = await _unitOfWork.ProdutoRepository.GetAsync(c => c.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não encontrado! Verifique o Id");
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }

        [HttpGet("Pagination")]
        public async Task<ActionResult<IEnumerable<ProdutoDTO>>> Get([FromQuery] ProdutosParameters produtosParameters)
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetProdutosAsync(produtosParameters);

            return ObterProdutos(produtos);

        }

        [HttpGet("FiltroPreco")]
        public async Task <ActionResult<IEnumerable<ProdutoDTO>>> GetProdutosFilterPreco([FromQuery] ProdutosFiltroPreco produtosFiltroPreco)
        {
            var produtos = await _unitOfWork.ProdutoRepository.GetProdutosFiltroPrecoAsync(produtosFiltroPreco); ;
            return ObterProdutos(produtos);
        }

        private ActionResult<IEnumerable<ProdutoDTO>> ObterProdutos(PagedList<Produto> produtos)
        {
            var metadata = new
            {
                produtos.TotalCount,
                produtos.PageSize,
                produtos.CurrentPage,
                produtos.HasNext,
                produtos.HasPrevious
            };

            Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));
            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);
            return Ok(produtosDto);
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDTO>> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
            {
                return BadRequest("Produto inválido!");
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
            await _unitOfWork.CommitAsync();

            var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

            return new CreatedAtRouteResult("ObterProduto", new
            { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
        }

        [HttpPatch("{id}/UpdatePartial")]
        public async Task<ActionResult<ProdutoDTO>> UpdatePartial(int id, JsonPatchDocument<ProdutoDTOUpdateRequest> patchProdutoDto)
        {
            if (patchProdutoDto is null)
            {
                return BadRequest("Produto inválido!");
            }

            var produto = await _unitOfWork.ProdutoRepository.GetAsync(c => c.ProdutoId == id);
            if (produto is null)
            {
                return NotFound("Produto não encontrado! Verifique o Id");
            }

            var produtoUpdateRequest = _mapper.Map<ProdutoDTOUpdateRequest>(produto);

            patchProdutoDto.ApplyTo(produtoUpdateRequest, ModelState);

            if (!ModelState.IsValid || TryValidateModel(produtoUpdateRequest))
            {
                return BadRequest(ModelState);
            }

            _mapper.Map(produtoUpdateRequest, produto);

            _unitOfWork.ProdutoRepository.Update(produto);
            await _unitOfWork.CommitAsync();

            return Ok(_mapper.Map<ProdutoDTOUpdateResponse>(produto));
        }

        [HttpPut("{id:int:min(1)}")] // Restrição de rota, apenas numeros maior ou igual que 1
        public async Task<ActionResult> Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);

            var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
            await _unitOfWork.CommitAsync();


            return Ok(produtoAtualizadoDto);

        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<ProdutoDTO>> Delete(int id)
        {
            var produto = await _unitOfWork.ProdutoRepository.GetAsync(x => x.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não localizado");
            }

            var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
            await _unitOfWork.CommitAsync();

            var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

            return Ok(produtoDeletadoDto);
        }
    }
}
