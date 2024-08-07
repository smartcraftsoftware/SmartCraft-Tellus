﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCraft.Core.Tellus.Domain.Utility;
public static class ClientHelpers
{
    public static HttpRequestMessage BuildRequestMessage(HttpMethod httpMethod, UriBuilder uri, Dictionary<string, string> headerKeyValues)
    {
        HttpRequestMessage requestMessage = new HttpRequestMessage(httpMethod, uri.Uri);
        foreach (var keyvalue in headerKeyValues)
        {
            requestMessage.Headers.Add(keyvalue.Key, keyvalue.Value);
        }
        return requestMessage;
    }

    public static UriBuilder BuildUri(string baseUrl, string path, string query = "")
    {
        UriBuilder uriBuilder = new UriBuilder(baseUrl);
        uriBuilder.Path = path;
        uriBuilder.Query = query;

        return uriBuilder;
    }

    public static UriBuilder BuildUri(string baseUrl, string path, Dictionary<string, string> parameters)
    {
        UriBuilder uriBuilder = new UriBuilder(baseUrl);
        uriBuilder.Path = path;
        StringBuilder query = new StringBuilder();
        foreach (var parameter in parameters)
        {
            query.Append($"{parameter.Key}={parameter.Value}&");
        }
        uriBuilder.Query = query.ToString().TrimEnd('&');

        return uriBuilder;
    }
}
