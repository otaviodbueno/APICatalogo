using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using APICatalogo.Context;
using APICatalogo.Models;
using Microsoft.EntityFrameworkCore;
using APICatalogo.Repository;
using APICatalogo.DTO;
using Microsoft.AspNetCore.Razor.TagHelpers;
using AutoMapper;

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

        [HttpGet]
        public ActionResult<IEnumerable<ProdutoDTO>> Get()
        {
            var produtos = _unitOfWork.ProdutoRepository.GetAll();
            if (produtos is null)
            {
                return NotFound("Produtos não encontrados!");
            }

            var produtosDto = _mapper.Map<IEnumerable<ProdutoDTO>>(produtos);

            return Ok(produtosDto);
        }


        [HttpGet("{id}", Name = "ObterProduto")]
        public ActionResult<ProdutoDTO> Get(int id)
        {
            var produto = _unitOfWork.ProdutoRepository.Get(c => c.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não encontrado! Verifique o Id");
            }

            var produtoDto = _mapper.Map<ProdutoDTO>(produto);

            return Ok(produtoDto);
        }

        [HttpPost]
        public ActionResult<ProdutoDTO> Post(ProdutoDTO produtoDto)
        {
            if (produtoDto is null)
            {
                return BadRequest("Produto inválido!");
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            var novoProduto = _unitOfWork.ProdutoRepository.Create(produto);
            _unitOfWork.Commit();


            var novoProdutoDto = _mapper.Map<ProdutoDTO>(novoProduto);

            return new CreatedAtRouteResult("ObterProduto", new
            { id = novoProdutoDto.ProdutoId }, novoProdutoDto);
        }

        [HttpPut("{id:int:min(1)}")] // Restrição de rota, apenas numeros maior ou igual que 1
        public ActionResult Put(int id, ProdutoDTO produtoDto)
        {
            if (id != produtoDto.ProdutoId)
            {
                return BadRequest();
            }

            var produto = _mapper.Map<Produto>(produtoDto);

            var produtoAtualizado = _unitOfWork.ProdutoRepository.Update(produto);

            var produtoAtualizadoDto = _mapper.Map<ProdutoDTO>(produtoAtualizado);
            _unitOfWork.Commit();


            return Ok(produtoAtualizadoDto);

        }

        [HttpDelete("{id:int}")]
        public ActionResult<ProdutoDTO> Delete(int id)
        {
            var produto = _unitOfWork.ProdutoRepository.Get(x => x.ProdutoId == id);

            if (produto is null)
            {
                return NotFound("Produto não localizado");
            }

            var produtoDeletado = _unitOfWork.ProdutoRepository.Delete(produto);
            _unitOfWork.Commit();

            var produtoDeletadoDto = _mapper.Map<ProdutoDTO>(produtoDeletado);

            return Ok(produtoDeletadoDto);
        }
    }
}
