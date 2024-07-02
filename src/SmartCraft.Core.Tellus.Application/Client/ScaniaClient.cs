using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Utility;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;

namespace SmartCraft.Core.Tellus.Infrastructure.Client;

public class ScaniaClient(HttpClient client) : IVehicleClient
{
    public string VehicleBrand => "scania";
    string? token;

    public async Task<EsgVehicleReport> GetEsgReportAsync(string? vin, Tenant tenant, string startTime, string stopTime)
    {
        token ??= await AuthScania(tenant);

        var uriBuilder = string.IsNullOrEmpty(vin) ?
            ClientHelpers.BuildUri("https://dataaccess.scania.com", "/cs/vehicle/reports/VehicleEvaluationReport/v2", $"startDate={startTime}&endDate={stopTime}") :
            ClientHelpers.BuildUri("https://dataaccess.scania.com", "/cs/vehicle/reports/VehicleEvaluationReport/v2", $"startDate={startTime}&endDate={stopTime}&vinOfInterest={vin}");

        if (token == null)
            throw new HttpRequestException(HttpStatusCode.Unauthorized.ToString());

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Authorization", "Bearer " + token },
            { "accept", "application/json" }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        #pragma warning disable CS8603
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603

        response.EnsureSuccessStatusCode();
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
        var esgResponse = JsonSerializer.Deserialize<ScaniaVehicleEvaluationApiResponse>(await response.Content.ReadAsStringAsync(), options) ?? throw new JsonException();

        return esgResponse.ToDomainModel(startTime, stopTime);
    }

    public async Task<List<Vehicle>> GetVehiclesAsync(Tenant tenant)
    {
        var uriBuilder = ClientHelpers.BuildUri("https://dataaccess.scania.com", "rfms4/vehicles", "");
        token ??= await AuthScania(tenant);

        if (token == null)
            throw new HttpRequestException(HttpStatusCode.Unauthorized.ToString());

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Authorization", "Bearer " + token },
            { "accept", "application/json; rfms=vehicles.v4.0" }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        #pragma warning disable CS8603 // Possible null reference return.
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.TooManyRequests)
            throw new HttpRequestException("Too many requests");
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603 // Possible null reference return.

        response.EnsureSuccessStatusCode();

        var vehicleApiResponse = JsonSerializer.Deserialize<ScaniaVehiclesApiResponse>(await response.Content.ReadAsStringAsync()) ?? throw new JsonException();

        return vehicleApiResponse?.VehicleResponse?.Vehicles?.Select(x => x.ToDomainModel()).ToList() ?? new List<Vehicle>();
    }

    public async Task<StatusReport> GetVehicleStatusAsync(string vin, Tenant tenant, string startTime, string? stopTime)
    {
        token ??= await AuthScania(tenant); 

        if (token == null)
            throw new HttpRequestException(HttpStatusCode.Unauthorized.ToString());
        var param = new Dictionary<string, string>
        {
            { "vin", vin },
            { "starttime", startTime },
            { "stoptime", stopTime ?? DateTime.UtcNow.ToString() },
            { "triggerFilter", "TIMER" },
            { "contentFilter", "SNAPSHOT" },
            { "datetype", "received" }
        };
        var uriBuilder = ClientHelpers.BuildUri("https://dataaccess.scania.com", "rfms4/vehiclestatuses", param);

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Authorization", "Bearer " + token },
            { "accept", "application/json; rfms=vehiclestatuses.v4.0" }
        };
        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        #pragma warning disable CS8603 // Possible null reference return.
        var response = await client.SendAsync(request);
        if (response.StatusCode == HttpStatusCode.NotFound)
            return null;
        #pragma warning restore CS8603 // Possible null reference return.
        
        response.EnsureSuccessStatusCode();
        var statusResponse = JsonSerializer.Deserialize<ScaniaVehicleStatusResponse>(await response.Content.ReadAsStringAsync()) ?? throw new JsonException();

        return statusResponse.ToDomainModel();
    }
    
    private async Task<string> AuthScania(Tenant tenant)
    {
        //Build api request
        var uri = ClientHelpers.BuildUri("https://dataaccess.scania.com", "auth/clientid2challenge", $"");
        var request = new HttpRequestMessage(HttpMethod.Post, uri.Uri);

        request.Content = new StringContent("clientId=" + tenant.ScaniaClientId, null, "application/x-www-form-urlencoded");
        //Make api request and validate response code
        var response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        //Parse response
        var challenge = JsonSerializer.Deserialize<Dictionary<string, string>>(await response.Content.ReadAsStringAsync()) ?? throw new JsonException();

        var secretKey = tenant.ScaniaSecretKey ?? string.Empty;

        if (secretKey == string.Empty)
            throw new Exception("No secret key found");

        var sharedKeyArr = Base64Url.Decode(secretKey);
        var challengeArr = Base64Url.Decode(challenge.FirstOrDefault().Value);
        byte[]? challengeResponse = null;
        using (HMACSHA256 hmac = new HMACSHA256(sharedKeyArr))
        {
            challengeResponse = hmac.ComputeHash(challengeArr);
        }
        var encodedResponse = EncodeChallengeResponse(challengeResponse);

        uri = ClientHelpers.BuildUri("https://dataaccess.scania.com", "auth/response2token");

        request = new HttpRequestMessage(HttpMethod.Post, uri.Uri);
        var content = new StringContent("clientId=" + tenant.ScaniaClientId + "&Response=" + encodedResponse, null, "application/x-www-form-urlencoded");
        request.Content = content;
        response = await client.SendAsync(request);
        response.EnsureSuccessStatusCode();

        var token = JsonSerializer.Deserialize<Dictionary<string, string>>(await response.Content.ReadAsStringAsync()) ?? throw new JsonException();

        return token["token"];
    }

    private string EncodeChallengeResponse(byte[] challengeResponse)
    {
        return Base64Url.Encode(challengeResponse);
    }

    public Task<IntervalStatusReport> GetIntervalStatusReportAsync(string vin, Tenant tenant, string startTime, string stopTime)
    {
        throw new NotImplementedException();
    }
}

public class Base64Url
{
    public static string Encode(byte[] arg)
    {
        string s = Convert.ToBase64String(arg);
        s = s.Split('=')[0];
        s = s.Replace('+', '-');
        s = s.Replace('/', '_');
        return s;
    }
    public static byte[] Decode(string arg)
    {
        string s = arg;
        s = s.Replace('-', '+');
        s = s.Replace('_', '/');

        switch (s.Length % 4)
        {
            case 0: break;
            case 2: s += "=="; break;
            case 3: s += "="; break;
            default: throw new Exception("Illegal base64Url string");
        }
        return Convert.FromBase64String(s);
    }
}
