using SmartCraft.Core.Tellus.Domain.Models;
using SmartCraft.Core.Tellus.Domain.Utility;
using SmartCraft.Core.Tellus.Infrastructure.ApiResponse;
using SmartCraft.Core.Tellus.Infrastructure.Mappers;
using System;
using System.Net;
using System.Security.Cryptography;
using System.Text.Json;
using Serilog;

namespace SmartCraft.Core.Tellus.Infrastructure.Client;

public class ScaniaClient : IVehicleClient
{
    public string VehicleBrand => "scania";
    string? token;
    private readonly HttpClient _client;
    private readonly ILogger _logger;
    public ScaniaClient(HttpClient client, ILogger logger)
    {
        _client = client;
        _logger = logger.ForContext<ScaniaClient>();
    }
     
    public async Task<VehicleEvaluationReport> GetVehicleEvaluationReportAsync(string? vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        token ??= await AuthScania(tenant);

        var uriBuilder = string.IsNullOrEmpty(vin) ?
            ClientHelpers.BuildUri("https://dataaccess.scania.com", "/cs/vehicle/reports/VehicleEvaluationReport/v2", $"startDate={DateOnly.FromDateTime(startTime)}&endDate={DateOnly.FromDateTime(stopTime)}") :
            ClientHelpers.BuildUri("https://dataaccess.scania.com", "/cs/vehicle/reports/VehicleEvaluationReport/v2", $"startDate={DateOnly.FromDateTime(startTime)}&endDate={DateOnly.FromDateTime(stopTime)}&vinOfInterest={vin}");

        if (token == null)
            throw new InvalidOperationException("Invalid credentials");

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Authorization", "Bearer " + token },
            { "accept", "application/json" }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        try
        {
            var response = await _client.SendAsync(request);
#pragma warning disable CS8603
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
#pragma warning restore CS8603

            response.EnsureSuccessStatusCode();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            var vehicleEvaluationResponse = JsonSerializer.Deserialize<ScaniaVehicleEvaluationApiResponse>(await response.Content.ReadAsStringAsync(), options);
            return vehicleEvaluationResponse.ToDomainModel(startTime, stopTime);
        }
        catch(HttpRequestException ex)
        {
            _logger.Error("Making request to /cs/vehicle/reports/VehicleEvaluationReport/v2 failed with {ErrorMessage} and {StatusCode}", ex.Message, ex.StatusCode);
            throw new HttpRequestException("Something went wrong when accessing external resource, Statuscode: " +  ex.StatusCode);
        }
        catch(JsonException ex)
        {
            _logger.Error("Unable to deserialize json to {ItemType} with {ErrorMessage}", "ScaniaVehicleEvaluationApiResponse", ex.Message);
            throw new JsonException("Unable to deserialize json");
        }
    }

    public async Task<List<Vehicle>> GetVehiclesAsync(Tenant tenant)
    {
        var uriBuilder = ClientHelpers.BuildUri("https://dataaccess.scania.com", "rfms4/vehicles", "");
        token ??= await AuthScania(tenant);

        if (token == null)
            throw new InvalidOperationException("Invalid credentials");

        Dictionary<string, string> headerKeyValues = new Dictionary<string, string>
        {
            { "Authorization", "Bearer " + token },
            { "accept", "application/json; rfms=vehicles.v4.0" }
        };

        var request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);

        try
        {
#pragma warning disable CS8603 // Possible null reference return.
            var response = await _client.SendAsync(request);
#pragma warning restore CS8603 // Possible null reference return.

            response.EnsureSuccessStatusCode();

            var vehicleApiResponse = JsonSerializer.Deserialize<ScaniaVehiclesApiResponse>(await response.Content.ReadAsStringAsync());

            return vehicleApiResponse?.VehicleResponse?.Vehicles?.Select(x => x.ToDomainModel()).ToList() ?? new List<Vehicle>();
        }
        catch (HttpRequestException ex) 
        {
            _logger.Error("Making request to rfms4/vehicles failed with {ErrorMessage} and {StatusCode}", ex.Message, ex.StatusCode);
            throw new HttpRequestException("Something went wrong when accessing external resource, Statuscode: " + ex.StatusCode);
        }
        catch (JsonException ex)
        {
            _logger.Error("Unable to deserialize json to {ItemType} with {ErrorMessage}", "ScaniaVehicleApiResponse", ex.Message);
            throw new JsonException("Unable to deserialize json");
        }
    }

    public async Task<IntervalStatusReport> GetVehicleStatusAsync(string vin, Tenant tenant, DateTime startTime, DateTime stopTime)
    {
        token = await AuthScania(tenant);

        if (token == null)
            throw new InvalidOperationException("Invalid credentials");

        var param = new Dictionary<string, string>
        {
            { "vin", vin },
            { "starttime", startTime.ToIso8601() },
            { "stoptime", stopTime.ToIso8601() },
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


        try
        {
            var response = await _client.SendAsync(request);
            #pragma warning disable CS8603 // Possible null reference return.
            if (response.StatusCode == HttpStatusCode.NotFound)
                return null;
            #pragma warning restore CS8603 // Possible null reference return.

            response.EnsureSuccessStatusCode();
            string responseContent = await response.Content.ReadAsStringAsync();
            var scaniaVehicleStatusResponse = JsonSerializer.Deserialize<ScaniaVehicleStatusResponse>(responseContent);

            if (scaniaVehicleStatusResponse.MoreDataAvailable && scaniaVehicleStatusResponse?.VehicleStatusResponse?.VehicleStatuses?.Length > 0)
            {
                bool moreDataAvailable = true;
                while (moreDataAvailable)
                {
                    var latest = scaniaVehicleStatusResponse.VehicleStatusResponse.VehicleStatuses[^1];
                    #pragma warning disable CS8629 // Nullable value type may be null.
                    DateTime latestDate = (DateTime)latest.ReceivedDateTime;
                    #pragma warning restore CS8629 // Nullable value type may be null.
                    latestDate = latestDate.AddSeconds(1);
                    param["starttime"] = latestDate.ToIso8601();
                    uriBuilder = ClientHelpers.BuildUri("https://api.volvotrucks.com", $"/rfms/vehiclestatuses", param);
                    request = ClientHelpers.BuildRequestMessage(HttpMethod.Get, uriBuilder, headerKeyValues);
                    response = await _client.SendAsync(request);
                    responseContent = await response.Content.ReadAsStringAsync();
                    var newVehicleStatusResponse = JsonSerializer.Deserialize<ScaniaVehicleStatusResponse>(responseContent);
                    scaniaVehicleStatusResponse.VehicleStatusResponse.VehicleStatuses = scaniaVehicleStatusResponse.VehicleStatusResponse.VehicleStatuses.Concat(newVehicleStatusResponse.VehicleStatusResponse.VehicleStatuses).ToArray();
                    moreDataAvailable = newVehicleStatusResponse.MoreDataAvailable;
                }
            }

            return scaniaVehicleStatusResponse.ToIntervalDomainModel();
        }
        catch(HttpRequestException ex)
        {
            _logger.Error("Making request to rfms4/vehiclestatuses failed with {ErrorMessage} and {StatusCode}", ex.Message, ex.StatusCode);
            throw new HttpRequestException("Something went wrong when accessing external resource, Statuscode: " + ex.StatusCode);
        }
        catch(JsonException ex)
        {
            _logger.Error("Unable to deserialize json to {ItemType} with {ErrorMessage}", "ScaniaVehicleStatusResponse", ex.Message);
            throw new JsonException("Unable to deserialize json");
        }

    }

    private async Task<string> AuthScania(Tenant tenant)
    {
        if (tenant.ScaniaClientId == null || tenant.ScaniaSecretKey == null)
            throw new InvalidOperationException("Invalid credentials");

        //Build api request
        var uri = ClientHelpers.BuildUri("https://dataaccess.scania.com", "auth/clientid2challenge", $"");
        var request = new HttpRequestMessage(HttpMethod.Post, uri.Uri);

        request.Content = new StringContent("clientId=" + tenant.ScaniaClientId, null, "application/x-www-form-urlencoded");
        //Make api request and validate response code
        try
        {
            var response = await _client.SendAsync(request);
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
            response = await _client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var token = JsonSerializer.Deserialize<Dictionary<string, string>>(await response.Content.ReadAsStringAsync());

            return token["token"];
        }
        catch(HttpRequestException ex)
        {
            _logger.Error("Making request to auth/clientid2challenge failed with {ErrorMessage} and {StatusCode}", ex.Message, ex.StatusCode);
            throw new HttpRequestException("Something went wrong when accessing external resource, Statuscode: " + ex.StatusCode);
        }
        catch(JsonException ex)
        {
            _logger.Error("Unable to deserialize json to {ItemType} with {ErrorMessage}", "Dictionary<string, string>", ex.Message);
            throw new JsonException("Unable to deserialize json");
        }
    }

    private string EncodeChallengeResponse(byte[] challengeResponse)
    {
        return Base64Url.Encode(challengeResponse);
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
