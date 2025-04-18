using APICatalogo.Validations;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APICatalogo.Models;


[Table("Produtos")]
public class Produto// : IValidatableObject
{
    [Key]
    public int ProdutoId { get; set; }

    //[Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(80)]
    //[PrimeiraLetraMaiusculaAttibute] // Pode usar essa, ou o que está implementado com IValidatableObject
    public string? Nome { get; set; }
    [Required]
    [StringLength(300)]
    public string? Descricao { get; set; }

    [Required]
    [Column(TypeName = "decimal(10, 2)")]
    public decimal Preco { get; set; }

    [Required]
    [StringLength(80)]
    public string? ImageUrl { get; set; }
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }
    public int CategoriaId { get; set; }

    [JsonIgnore] //Apenas propriedade de navegação, não deve ser uma propriedade realmente do objeto
    public Categoria? Categoria { get; set; }

    //public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    //{
    //    if (this.Nome != null || !string.IsNullOrEmpty(this.Nome.ToString()))
    //    {
    //        var primeiraLetra = this.Nome.ToString()[0].ToString();

    //        if (primeiraLetra != primeiraLetra.ToUpper())
    //        {
    //            yield return new ValidationResult("A primeira letra não é maiúscula");
    //        }
    //    }
    //}
}
