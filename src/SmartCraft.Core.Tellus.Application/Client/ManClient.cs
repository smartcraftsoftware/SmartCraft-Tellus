using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Utility;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
using SmartCraft.Core.Tellus.Infrastructure.Client;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;
using System.Net;
using System.Text.Json;

namespace SmartCraft.Core.Tellus.Application.Client;
public class ManClient(HttpClient client) : IVehicleClient
{
    public string VehicleBrand => "man";

    public async Task<EsgVehicleReport> GetEsgReportAsync(string? vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        await Task.CompletedTask;
        UriBuilder uriBuilder = ClientHelpers.BuildUri("api.perform3.rio.cloud", $"api/assets/{vin}", $"from={startTime:yyyy-MM-dd}&to={stopTime:yyyy-MM-dd}");

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/json" },
            //{ "Authorization", "Basic " + CredentialsAsB64String() }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        #pragma warning disable CS8603 // Possible null reference return.
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603 // Possible null reference return.
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var jsonObject = JsonSerializer.Deserialize<ManPerformApiResponse>(result) ?? throw new JsonException();

        return jsonObject.ToDomainModel();
    }

    public async Task<List<Vehicle>> GetVehiclesAsync(Tenant tenant)
    {
        await Task.CompletedTask;
        UriBuilder uriBuilder = ClientHelpers.BuildUri("api.assets.rio.cloud", $"assets", $"identification_type=vin&status=active");

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/json" },
            //{ "Authorization", "Basic " + CredentialsAsB64String() }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);
        var response = await client.SendAsync(request);
        #pragma warning disable CS8603 // Possible null reference return.
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        #pragma warning restore CS8603 // Possible null reference return.

        var result = await response.Content.ReadAsStringAsync();

        var jsonObject = JsonSerializer.Deserialize<List<ManAssetApiResponse>>(result) ?? throw new JsonException();

        return jsonObject.Select(x => x.ToDomainModel()).ToList();
    }

    public async Task<StatusReport> GetVehicleStatusAsync(string id, Tenant tenant, DateTime startTime, DateTime? stopTime)
    {
        await Task.CompletedTask;
        UriBuilder uriBuilder = ClientHelpers.BuildUri($"assets", $"assets/{id}", $"identification_type=vin&status=active");

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/json" },
            //{ "Authorization", "Basic " + CredentialsAsB64String() }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        #pragma warning disable CS8603 // Possible null reference return.
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603 // Possible null reference return.
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadAsStringAsync();

        var jsonObject = JsonSerializer.Deserialize<ManTelematicsApiResponse>(result) ?? throw new JsonException();

        return jsonObject.ToDomainModel();
    }
}

