using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace APICatalogo.Filters;

public class ApiExceptionFilter : IFilterMetadata
{
    private readonly ILogger<ApiExceptionFilter> _logger;

    public ApiExceptionFilter(ILogger<ApiExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {

        _logger.LogError(context.Exception, "Ocorreu uma eceção não tratada");

        context.Result = new ObjectResult("Ocorreu uma eceção não tratada")
        {
            StatusCode = StatusCodes.Status500InternalServerError,
        };

    }
}
