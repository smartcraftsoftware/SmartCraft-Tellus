using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Utility;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Infrastructure.Client;

public class VolvoClient : IVehicleClient
{
    public string VehicleBrand => "volvo";
    private readonly HttpClient _client;
    private readonly ILogger _logger;

    public VolvoClient(HttpClient client, ILogger logger)
    {
        _client = client;
        _logger = logger.ForContext<VolvoClient>();
    }

    public async Task<VehicleEvaluationReport> GetVehicleEvaluationReportAsync(string? vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        var uriBuilder = !string.IsNullOrEmpty(vin) ?
            ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/score/scores", $"vin={vin}&starttime={startTime:yyyy-MM-dd}&stoptime={stopTime:yyyy-MM-dd}") :
            ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/score/scores", $"?starttime={startTime:yyyy-MM-dd}&stoptime={stopTime:yyyy-MM-dd}");
        
        var credentials = tenant.VolvoCredentials ?? "";
        if (string.IsNullOrEmpty(credentials))
            throw new InvalidOperationException("Invalid credentials");

        var baseEncoded = CredentialsAsB64String(credentials);
        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/x.volvogroup.com.scores.v2.0+json; UTF-8" },
            { "Authorization", "Basic " + baseEncoded }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        try
        {
            var response = await _client.SendAsync(request);

            #pragma warning disable CS8603 // Possible null reference return.
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            #pragma warning restore CS8603 // Possible null reference return.
            response.EnsureSuccessStatusCode();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var jsonObject = JsonSerializer.Deserialize<VolvoUtilizationScoreApiResponse>(await response.Content.ReadAsStringAsync(), options);

            return jsonObject.ToDomainModel();
        }
        catch (HttpRequestException ex)
        {
            _logger.Error("Making request to score/scores failed with {ErrorMessage} and {StatusCode}", ex.Message, ex.StatusCode);
            throw new HttpRequestException("Something went wrong when accessing external resource " + ex.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.Error("Unable to deserialize json to {ItemType} with {ErrorMessage}", "VolvoUtilizationScoreApiResponse", ex.Message);
            throw new JsonException("Something went wrong when deserializing the object");
        }
    }

    public async Task<List<Vehicle>> GetVehiclesAsync(Tenant tenant)
    {
        var uriBuilder = ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/rfms/vehicles");
        var credentials = tenant?.VolvoCredentials ?? "";
        if (string.IsNullOrEmpty(credentials))
            throw new InvalidOperationException("Invalid credentials");

        var baseEncoded = CredentialsAsB64String(credentials);

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/vnd.fmsstandard.com.vehicles.v2.1+json" },
            { "Authorization", "Basic " + baseEncoded }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        try
        {
            var response = await _client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            #pragma warning disable CS8603 // Possible null reference return.
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            #pragma warning restore CS8603 // Possible null reference return.

            response.EnsureSuccessStatusCode();

            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var jsonObject = JsonSerializer.Deserialize<VolvoVehiclesApiResponse>(await response.Content.ReadAsStringAsync(), options);
            var vehicleList = jsonObject?.Vehicle?.Select(x => x.ToDomainModel()).ToList();
            return vehicleList ?? new List<Vehicle>();
        }
        catch (HttpRequestException ex) 
        {
            _logger.Error("Making request to /rfms/vehicles failed with {ErrorMessage} and {StatusCode}", ex.Message, ex.StatusCode);
            throw new HttpRequestException("Something went wrong when accessing external resource " + ex.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.Error("Unable to deserialize json to {ItemType} with {ErrorMessage}", "VolvoVehiclesApiResponse", ex.Message);
            throw new JsonException("Something went wrong when deserializing the object");
        }


    }

    public async Task<IntervalStatusReport> GetVehicleStatusAsync(string vin, Tenant tenant, DateTime startTime, DateTime stopTime) {

        TimeSpan ts = DateTime.UtcNow - startTime;
        if (ts.TotalDays >= 14)
            throw new InvalidOperationException("Only the last 14 days are available");

        var param = new Dictionary<string, string>
        {
            { "vin", vin },
            { "starttime", startTime.ToIso8601() },
            { "stoptime", stopTime.ToIso8601() },
            { "triggerFilter", "TIMER" },
            { "contentFilter", "SNAPSHOT" },
            { "datetype", "received" }
        };

        var uriBuilder = ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/rfms/vehiclestatuses", param);
        var credentials = tenant?.VolvoCredentials ?? "";

        if (string.IsNullOrEmpty(credentials))
            throw new InvalidOperationException("Invalid credentials");

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", "application/vnd.fmsstandard.com.Vehiclestatuses.v2.1+json" },
            { "Authorization", "Basic " + CredentialsAsB64String(credentials) }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);
        try
        {
            var response = await _client.SendAsync(request);
            #pragma warning disable CS8603 // Possible null reference return.
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            #pragma warning restore CS8603 // Possible null reference return.

            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            var vehicleStatusResponse = JsonSerializer.Deserialize<VolvoVehicleStatusResponse>(responseContent);
            if (vehicleStatusResponse.MoreDataAvailable && vehicleStatusResponse?.VehicleStatus?.Length > 0)
            {
                bool moreDataAvailable = true;
                while (moreDataAvailable)
                {
                    var latest = vehicleStatusResponse.VehicleStatus[^1];
                    #pragma warning disable CS8629 // Nullable value type may be null.
                    DateTime latestDate = (DateTime)latest.ReceivedDateTime;
                    #pragma warning restore CS8629 // Nullable value type may be null.
                    latestDate = latestDate.AddSeconds(1);
                    param["starttime"] = latestDate.ToIso8601();
                    uriBuilder = ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/rfms/vehiclestatuses", param);
                    request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);
                    response = await _client.SendAsync(request);
                    responseContent = await response.Content.ReadAsStringAsync();
                    var newVehicleStatusResponse = JsonSerializer.Deserialize<VolvoVehicleStatusResponse>(responseContent);
                    vehicleStatusResponse.VehicleStatus = vehicleStatusResponse.VehicleStatus.Concat(newVehicleStatusResponse.VehicleStatus).ToArray();
                    moreDataAvailable = newVehicleStatusResponse.MoreDataAvailable;
                }
            }

            return vehicleStatusResponse.ToIntervalDomainModel();   
        }
        catch(HttpRequestException ex)
        {
            _logger.Error("Making request to /rfms/vehiclestatuses failed with {ErrorMessage} and {StatusCode}", ex, ex.StatusCode);
            throw new HttpRequestException("Something went wrong when accessing external resource, " + ex.StatusCode);
        }
        catch(JsonException ex)
        {
            _logger.Error("Unable to deserialize json to {ItemType} with {ErrorMessage}", "VolvoVehicleStatusResponse", ex.Message);
            throw new JsonException("Something went wrong when deserializing the object");
        }

    }

    private string CredentialsAsB64String(string credentials)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(credentials);
        return Convert.ToBase64String(bytes);
    }
}
