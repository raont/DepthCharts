using Microsoft.AspNetCore.Http;

namespace Application.Exceptions;

public interface IGameApiException
{
    public int StatusCode { get; set; }
}
