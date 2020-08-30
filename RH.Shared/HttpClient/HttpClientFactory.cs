using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace RH.Shared.HttpClient
{
    public class HttpClientFactory : IHttpClientFactory
    {
        private readonly Dictionary<string, System.Net.Http.HttpClient> _clients=new Dictionary<string, System.Net.Http.HttpClient>();
        private readonly ILogger<HttpClientFactory> _logger;

        public HttpClientFactory(ILogger<HttpClientFactory> logger)
        {
            _logger = logger;
        }

        public System.Net.Http.HttpClient GetHttpClient(string baseAddress)
        {
            if (!_clients.ContainsKey(baseAddress))
                _clients.Add(baseAddress, new System.Net.Http.HttpClient()
                {
                    BaseAddress = new Uri(baseAddress)
                });
            return _clients[baseAddress];

        }
    }
}
