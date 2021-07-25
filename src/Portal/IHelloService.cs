using Dapr.Client;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace Portal
{
    public interface IHelloService
    {
        Task<string> HelloAsync(string name);
    }

    public class LocalHelloService : IHelloService
    {
        public Task<string> HelloAsync(string name)
        {
            return Task.FromResult($"Hello {name}!");
        }
    }

    public class RemoteHelloService : IHelloService
    {
        private readonly HttpClient _client;

        public RemoteHelloService(HttpClient client)
        {
            _client = client;
        }

        private readonly JsonSerializerOptions jsonOptions = new JsonSerializerOptions()
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        };

        public async Task<string> HelloAsync(string name)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"api/welcome?name={name}");
            var response = await _client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();
            var model = JsonSerializer.Deserialize<HelloModel>(json, jsonOptions);
            return model.Msg;
        }
    }

    public class DaprHelloService : IHelloService
    {
        private readonly DaprClient _dapr;
        private readonly ILogger<DaprHelloService> _logger;

        public DaprHelloService(DaprClient daprClient, ILogger<DaprHelloService> logger)
        {
            _dapr = daprClient;
            _logger = logger;
        }

        public async Task<string> HelloAsync(string name)
        {
            var model = await _dapr.InvokeMethodAsync<HelloModel>(HttpMethod.Get, "daprdemo-helloapi", $"api/welcome?name={name}");
            _logger.LogInformation("Invoked welcome method {Name}", name);
            return model.Msg;
        }
    }
}