using System.Net.Http.Json;
using SmartX.Shared.Models;

namespace SmartX.Client.Services
{
    public class ApiClient
    {
        private readonly HttpClient _httpClient;

        public ApiClient()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(
                    "https://localhost:44386/api/")
            };
        }

        public async Task<List<Sensor>> GetSensorsAsync()
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync("sensors");

            response.EnsureSuccessStatusCode();

            List<Sensor>? sensors =
                await response.Content
                    .ReadFromJsonAsync<List<Sensor>>();

            return sensors ?? new List<Sensor>();
        }

        public async Task<TelemetryRecord?> SendTemperatureAsync(
            TelemetryPacket<float> packet)
        {
            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "telemetry/temperature",
                    packet);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TelemetryRecord>();
        }

        public async Task<TelemetryRecord?> SendPowerAsync(
            TelemetryPacket<int> packet)
        {
            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "telemetry/power",
                    packet);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TelemetryRecord>();
        }

        public async Task<TelemetryRecord?> SendSwitchAsync(
            TelemetryPacket<bool> packet)
        {
            HttpResponseMessage response =
                await _httpClient.PostAsJsonAsync(
                    "telemetry/switch",
                    packet);

            response.EnsureSuccessStatusCode();

            return await response.Content
                .ReadFromJsonAsync<TelemetryRecord>();
        }

        public async Task<List<TelemetryRecord>>
            GetTelemetryAsync()
        {
            HttpResponseMessage response =
                await _httpClient.GetAsync("telemetry");

            response.EnsureSuccessStatusCode();

            List<TelemetryRecord>? telemetry =
                await response.Content
                    .ReadFromJsonAsync<List<TelemetryRecord>>();

            return telemetry ?? new List<TelemetryRecord>();
        }
    }
}