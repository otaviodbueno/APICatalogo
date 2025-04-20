using System.ComponentModel.DataAnnotations;

namespace APICatalogo.DTO;

public class ProdutoDTOUpdateRequest :IValidatableObject
{
    [Range(1, 9999, ErrorMessage = "Estoque tamanho inválido")]
    public float Estoque { get; set; }
    public DateTime DataCadastro { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if(DataCadastro.Date <= DateTime.Now.Date)
        {
            yield return new ValidationResult("Data de cadastro inválida", new[] { nameof(DataCadastro) });
        }
    }
}
