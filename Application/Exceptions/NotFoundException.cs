using Microsoft.AspNetCore.Http;
using System.Diagnostics.CodeAnalysis;

namespace Application.Exceptions;

[ExcludeFromCodeCoverage]
public class NotFoundException : Exception, IGameApiException 
{
    public int StatusCode { get; set; }

    public NotFoundException(string message) : base (message)
    {
        StatusCode = StatusCodes.Status404NotFound;
    }

}
