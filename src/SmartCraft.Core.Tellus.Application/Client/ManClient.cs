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
        UriBuilder uriBuilder = ClientHelpers.BuildUri("api.perform3.rio.cloud", $"api/assets/{vin}", $"from={startTime:yyyy-MM-dd}&to={stopTime:yyyy-MM-dd}");
        var credentials = tenant.ManToken ?? "";

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/json" },
            { "Authorization", "Basic " + CredentialsAsB64String(credentials) }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        #pragma warning disable CS8603
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603
        response.EnsureSuccessStatusCode();

        var jsonObject = JsonSerializer.Deserialize<ManPerformApiResponse>(await response.Content.ReadAsStringAsync()) ?? throw new JsonException();

        return jsonObject.ToDomainModel();
    }

    public Task<IntervalStatusReport> GetVehicleStatusAsync(string vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Vehicle>> GetVehiclesAsync(Tenant tenant, string? vin)
    {
        UriBuilder uriBuilder = ClientHelpers.BuildUri("api.assets.rio.cloud", $"assets", $"identification_type=vin&status=active");
        var credentials = tenant.ManToken ?? "";

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Accept", $"application/json" },
            { "Authorization", "Basic " + CredentialsAsB64String(credentials) }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);
        var response = await client.SendAsync(request);
        #pragma warning disable CS8603
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        response.EnsureSuccessStatusCode();
        #pragma warning restore CS8603

        var jsonObject = JsonSerializer.Deserialize<List<ManAssetApiResponse>>(await response.Content.ReadAsStringAsync()) ?? throw new JsonException();

        return jsonObject.Select(x => x.ToDomainModel()).ToList();
    }

    //public async Task<StatusReport> GetVehicleStatusAsync(string id, Tenant tenant, DateTime startTime, DateTime? stopTime)
    //{
    //    
    //    UriBuilder uriBuilder = ClientHelpers.BuildUri($"assets", $"assets/{id}", $"identification_type=vin&status=active");
    //    var credentials = tenant.ManToken ?? "";
    //
    //    Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
    //    {
    //        { "Accept", $"application/json" },
    //        { "Authorization", "Basic " + CredentialsAsB64String(credentials) }
    //    };
    //    var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);
    //
    //    #pragma warning disable CS8603
    //    var response = await client.SendAsync(request);
    //    if (response.StatusCode == HttpStatusCode.NotFound)
    //        return null;
    //    #pragma warning restore CS8603
    //    response.EnsureSuccessStatusCode();
    //
    //    var jsonObject = JsonSerializer.Deserialize<ManTelematicsApiResponse>(await response.Content.ReadAsStringAsync()) ?? throw new JsonException();
    //
    //    return jsonObject.ToDomainModel();
    //}

    private string CredentialsAsB64String(string credentials)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(credentials);
        return Convert.ToBase64String(bytes);
    }
}

