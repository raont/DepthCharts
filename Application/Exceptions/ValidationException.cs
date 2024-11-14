using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Application.Exceptions;

[ExcludeFromCodeCoverage]
public class ValidationException : Exception, IGameApiException 
{
    public int StatusCode { get; set; }

    public ValidationException(string message) : base (message)
    {
        StatusCode = StatusCodes.Status403Forbidden;
    }

}
