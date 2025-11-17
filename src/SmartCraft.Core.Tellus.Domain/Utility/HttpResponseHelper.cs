using System.Net;
using SmartCraft.Core.Tellus.Domain.Exceptions;

namespace SmartCraft.Core.Tellus.Domain.Utility;

public static class HttpResponseHelper
{
    /// <summary>
    /// Ensures the HTTP response is successful, otherwise throws a VehicleApiException
    /// with the original status code and error message preserved
    /// </summary>
    public static async Task EnsureSuccessOrThrowVehicleApiException(
        this HttpResponseMessage response,
        string vehicleBrand)
    {
        if (response.IsSuccessStatusCode)
            return;

        var content = await response.Content.ReadAsStringAsync();
        var statusCode = response.StatusCode;

        var errorMessage = string.IsNullOrWhiteSpace(content)
            ? $"{vehicleBrand} API returned {(int)statusCode} ({statusCode}): {response.ReasonPhrase}"
            : $"{vehicleBrand} API returned {(int)statusCode} ({statusCode}): {content}";

        throw new VehicleApiException(
            vehicleBrand: vehicleBrand,
            statusCode: statusCode,
            message: errorMessage,
            responseContent: content);
    }

    /// <summary>
    /// Attempts to parse error message from response content
    /// </summary>
    public static string GetErrorMessageFromContent(string content, HttpStatusCode statusCode)
    {
        if (string.IsNullOrWhiteSpace(content))
            return $"Request failed with status code {(int)statusCode} ({statusCode})";

        try
        {
            if (content.Contains("\"message\""))
            {
                var messageIndex = content.IndexOf("\"message\"");
                var colonIndex = content.IndexOf(":", messageIndex);
                var start = content.IndexOf("\"", colonIndex) + 1;
                var end = content.IndexOf("\"", start);
                if (end > start && start > 0)
                {
                    return content.Substring(start, end - start);
                }
            }
            else if (content.Contains("\"error\""))
            {
                var errorIndex = content.IndexOf("\"error\"");
                var colonIndex = content.IndexOf(":", errorIndex);
                var start = content.IndexOf("\"", colonIndex) + 1;
                var end = content.IndexOf("\"", start);
                if (end > start && start > 0)
                {
                    return content.Substring(start, end - start);
                }
            }
        }
        catch
        {
        }

        return content;
    }
}

