using APICatalogo.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APICatalogo.DTO
{
    public class ProdutoDTO
    {
        public int ProdutoId { get; set; }

        //[Required(ErrorMessage = "O nome é obrigatório")]
        [StringLength(80)]
        //[PrimeiraLetraMaiusculaAttibute] // Pode usar essa, ou o que está implementado com IValidatableObject
        public string? Nome { get; set; }
        [Required]
        [StringLength(300)]
        public string? Descricao { get; set; }

        [Required]
        public decimal Preco { get; set; }

        [Required]
        [StringLength(80)]
        public string? ImageUrl { get; set; }
        public int CategoriaId { get; set; }
    }
}
