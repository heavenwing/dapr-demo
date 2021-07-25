using Dapr.Client;
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
        private readonly DaprClient _daprClient;

        public DaprHelloService(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public async Task<string> HelloAsync(string name)
        {
            var model = await _daprClient.InvokeMethodAsync<HelloModel>(HttpMethod.Get, "daprdemo-helloapi", $"api/welcome?name={name}");
            return model.Msg;
        }
    }
}