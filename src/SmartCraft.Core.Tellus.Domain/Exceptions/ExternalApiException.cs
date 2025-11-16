using System.Net;

namespace SmartCraft.Core.Tellus.Domain.Exceptions;

/// <summary>
/// Base exception for external API errors
/// </summary>
public class ExternalApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseContent { get; }

    public ExternalApiException(
        HttpStatusCode statusCode,
        string message,
        string? responseContent = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }

    public ExternalApiException(
        HttpStatusCode statusCode,
        string? responseContent = null,
        Exception? innerException = null)
        : base($"External API returned {(int)statusCode} ({statusCode})", innerException)
    {
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}

