using System.Net;

namespace SmartCraft.Core.Tellus.Domain.Exceptions;

/// <summary>
/// Exception that wraps errors from third-party vehicle manufacturer APIs
/// </summary>
public class VehicleApiException : Exception
{
    public HttpStatusCode StatusCode { get; }
    public string? ResponseContent { get; }
    public string VehicleBrand { get; }

    /// <summary>
    /// Creates a VehicleApiException with a custom message
    /// </summary>
    public VehicleApiException(
        string vehicleBrand,
        HttpStatusCode statusCode,
        string message,
        string? responseContent = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        VehicleBrand = vehicleBrand;
        StatusCode = statusCode;
        ResponseContent = responseContent;
    }
}

