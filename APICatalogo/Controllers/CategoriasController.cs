using APICatalogo.Context;
using APICatalogo.DTO;
using APICatalogo.DTO.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlTypes;

namespace APICatalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        public CategoriasController(IUnitOfWork uof, IConfiguration configuration
            , ILogger<CategoriasController> logger)
        {
            _unitOfWork = uof;
            _configuration = configuration;
            _logger = logger;
        }

        [HttpGet("LerArquivoConfiguracao")]
        public string GetValores()
        {
            var valor1 = _configuration["chave"];
            var valor2  =_configuration["chave2"];

            return $"Valor 1: {valor1} - Valor 2: {valor2}";
        }



        [HttpGet]
        [Authorize] // Adicionando o filtro de autenticação
        [ServiceFilter(typeof(ApiLoggingFilter))] // Adicionando o filtro de log
        public async Task<ActionResult<IEnumerable<CategoriaDTO>>> Get()
        {
            var categorias = await _unitOfWork.CategoriaRepository.GetAllAsync();

            var categoriasDto = categorias.ToCategoriaDTOList();
            return Ok(categoriasDto);

        }

        [HttpGet("{id}", Name = "ObterCategoria")]
        public async Task<ActionResult<CategoriaDTO>> Get(int id)
        {
            var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("Categoria não encontrado! Verifique o Id");
            }

            var categoriaDto = categoria.ToCategoriaDTO();

            return Ok(categoriaDto);
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDTO>> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
            {
                return BadRequest("Produto inválido!");
            }

            var categoria = categoriaDto.ToCategoria();

            _unitOfWork.CategoriaRepository.Create(categoria);
            await _unitOfWork.CommitAsync();

            return new CreatedAtRouteResult("ObterCategoria", new
            { id = categoria.CategoriaId }, categoria);
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Put(int id, CategoriaDTO categoriaDto)
        {
            if (id != categoriaDto.CategoriaId)
            {
                return BadRequest();
            }

            var categoria = new Categoria
            {
                CategoriaId = categoriaDto.CategoriaId,
                Nome = categoriaDto.Nome,
                ImageUrl = categoriaDto.ImageUrl
            };

            _unitOfWork.CategoriaRepository.Update(categoria);
             await _unitOfWork.CommitAsync();
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CategoriaDTO>> Delete(int id)
        {
            var categoria = await _unitOfWork.CategoriaRepository.GetAsync(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("Categoria não localizado");
            }

            _unitOfWork.CategoriaRepository.Delete(categoria);
            await _unitOfWork.CommitAsync();
            return Ok();
        }

    }
}
