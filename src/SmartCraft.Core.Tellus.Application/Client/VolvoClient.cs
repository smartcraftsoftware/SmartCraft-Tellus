using System.Net;
using System.Text.Json;
using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Utility;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;

namespace SmartCraft.Core.Tellus.Infrastructure.Client;

public class VolvoClient(HttpClient client) : IVehicleClient
{
    public string VehicleBrand => "volvo";

    public async Task<Domain.Models.EsgVehicleReport> GetEsgReportAsync(string? vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        var uriBuilder = string.IsNullOrEmpty(vin) ?
            ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/score/scores", $"vin={vin}&starttime={startTime:yyyy-MM-dd}&stoptime={stopTime:yyyy-MM-dd}") :
            ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/score/scores", $"?starttime={startTime:yyyy-MM-dd}&stoptime={stopTime:yyyy-MM-dd}");
        
        var credentials = tenant.VolvoCredentials ?? "";
        if (string.IsNullOrEmpty(credentials))
            throw new HttpRequestException(HttpStatusCode.Unauthorized.ToString());

        var baseEncoded = CredentialsAsB64String(credentials);
        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/x.volvogroup.com.scores.v2.0+json; UTF-8" },
            { "Authorization", "Basic " + baseEncoded }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);
        var response = await client.SendAsync(request);
       
        #pragma warning disable CS8603 // Possible null reference return.
        if(response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603 // Possible null reference return.
        response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var jsonObject = JsonSerializer.Deserialize<VolvoUtilizationScoreApiResponse>(await response.Content.ReadAsStringAsync(), options) 
            ?? throw new JsonException();

        return jsonObject.ToDomainModel();
    }

    public async Task<List<Vehicle>> GetVehiclesAsync(Tenant tenant)
    {
        var uriBuilder = ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/rfms/vehicles");
        var credentials = tenant?.VolvoCredentials ?? "";
        if (string.IsNullOrEmpty(credentials))
            throw new HttpRequestException(HttpStatusCode.Unauthorized.ToString());

        var baseEncoded = CredentialsAsB64String(credentials);

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/vnd.fmsstandard.com.vehicles.v2.1+json" },
            { "Authorization", "Basic " + baseEncoded }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        #pragma warning disable CS8603 // Possible null reference return.
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603 // Possible null reference return.

        response.EnsureSuccessStatusCode();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var jsonObject = JsonSerializer.Deserialize<VolvoVehiclesApiResponse>(await response.Content.ReadAsStringAsync(), options) ?? throw new JsonException();
        var vehicleList = jsonObject?.Vehicle?.Select(x => x.ToDomainModel()).ToList();
        return vehicleList ?? new List<Vehicle>();
    }

    public async Task<StatusReport> GetVehicleStatusAsync(string? vin, Tenant tenant, DateTime startTime, DateTime? stopTime)
    {
        var uriBuilder = ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/rfms/vehiclestatuses", $"vin={vin}&starttime={startTime.ToIso8601()}&stoptime={stopTime.ToIso8601()}&latestonly=true");
        var credentials = tenant?.VolvoCredentials ?? "";
        if (string.IsNullOrEmpty(credentials))
            throw new HttpRequestException(HttpStatusCode.Unauthorized.ToString());

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/vnd.fmsstandard.com.vehiclestatuses.v2.1+json" },
            { "Authorization", "Basic " + credentials }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        #pragma warning disable CS8603 // Possible null reference return.
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603 // Possible null reference return.

        response.EnsureSuccessStatusCode();

        var jsonObject = JsonSerializer.Deserialize<VolvoVehicleStatusResponse>(await response.Content.ReadAsStringAsync()) ?? throw new JsonException("Could not serialize the object");

        return jsonObject.ToDomainModel();
    }

    private string CredentialsAsB64String(string credentials)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(credentials);
        return Convert.ToBase64String(bytes);
    }
}
