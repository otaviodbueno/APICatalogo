using APICatalogo.Context;
using APICatalogo.DTO;
using APICatalogo.DTO.Mappings;
using APICatalogo.Filters;
using APICatalogo.Models;
using APICatalogo.Repository;
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


        //[HttpGet("Produtos")] // Apenas definição de rota, note que nao está entre {}
        //public ActionResult<IEnumerable<Categoria>> GetCategoriasProdutos()
        //{
        //    _logger.LogInformation("### Executando Logger -> GetCategoriasProdutos");

        //    return _categoriaRepository.
        //        Include(p => p.Produtos).ToList();
        //}


        [HttpGet]
        [ServiceFilter(typeof(ApiLoggingFilter))] // Adicionando o filtro de log
        public ActionResult<IEnumerable<CategoriaDTO>> Get()
        {
            var categorias = _unitOfWork.CategoriaRepository.GetAll();

            //var categoriasDto = new List<CategoriaDTO>();
            //foreach (var categoria in categorias)
            //{
            //    CategoriaDTO categoriaDto = new CategoriaDTO
            //    {
            //        CategoriaId = categoria.CategoriaId,
            //        Nome = categoria.Nome,
            //        ImageUrl = categoria.ImageUrl
            //    };
            //    categoriasDto.Add(categoriaDto);
            //}

            var categoriasDto = categorias.ToCategoriaDTOList();
            return Ok(categoriasDto);

        }

        [HttpGet("{id}", Name = "ObterCategoria")]
        public ActionResult<CategoriaDTO> Get(int id)
        {
            var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("Categoria não encontrado! Verifique o Id");
            }

            //var categoriaDto = new CategoriaDTO
            //{
            //    CategoriaId = categoria.CategoriaId,
            //    Nome = categoria.Nome,
            //    ImageUrl = categoria.ImageUrl
            //};

            var categoriaDto = categoria.ToCategoriaDTO();

            return Ok(categoriaDto);
        }

        [HttpPost]
        public ActionResult<CategoriaDTO> Post(CategoriaDTO categoriaDto)
        {
            if (categoriaDto is null)
            {
                return BadRequest("Produto inválido!");
            }

            //var categoria = new Categoria
            //{
            //    CategoriaId = categoriaDto.CategoriaId,
            //    Nome = categoriaDto.Nome,
            //    ImageUrl = categoriaDto.ImageUrl
            //};

            var categoria = categoriaDto.ToCategoria();

            _unitOfWork.CategoriaRepository.Create(categoria);
            _unitOfWork.Commit();

            return new CreatedAtRouteResult("ObterCategoria", new
            { id = categoria.CategoriaId }, categoria);
        }


        [HttpPut("{id:int}")]
        public ActionResult<CategoriaDTO> Put(int id, CategoriaDTO categoriaDto)
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
            _unitOfWork.Commit();
            return Ok(categoria);
        }

        [HttpDelete("{id:int}")]
        public ActionResult<CategoriaDTO> Delete(int id)
        {
            var categoria = _unitOfWork.CategoriaRepository.Get(c => c.CategoriaId == id);

            if (categoria is null)
            {
                return NotFound("Categoria não localizado");
            }

            _unitOfWork.CategoriaRepository.Delete(categoria);
            _unitOfWork.Commit();
            return Ok();
        }

    }
}
