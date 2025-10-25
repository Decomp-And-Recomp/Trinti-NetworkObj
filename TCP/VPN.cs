using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Text.Json;

namespace NetworkObj.TCP
{
    class VPN
    {
        public class IPHubClient
        {
            private readonly HttpClient _httpClient;
            private readonly string _apiKey;

            public IPHubClient(string apiKey)
            {
                _apiKey = apiKey;
                _httpClient = new HttpClient();
                _httpClient.DefaultRequestHeaders.Add("X-Key", _apiKey);
                _httpClient.BaseAddress = new Uri("https://v2.api.iphub.info/");
            }

            public async Task<IPHubResponse?> GetIpInfoAsync(string ip)
            {
                try
                {
                    var response = await _httpClient.GetAsync($"ip/{ip}");

                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"[IPHub] HTTP {response.StatusCode} for {ip}");
                        return null;
                    }

                    string json = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<IPHubResponse>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[IPHub] Error checking {ip}: {ex.Message}");
                    return null;
                }
            }

            public async Task<bool> IsVpnIpAsync(string ip)
            {
                var info = await GetIpInfoAsync(ip);

                if (info == null)
                {
                    Console.WriteLine($"[IPHub] Could not verify {ip}, defaulting to non-VPN.");
                    return false;
                }

                bool isVpn = info.Block == 1;

                Console.WriteLine($"[IPHub] {ip} | VPN: {isVpn} | ISP: {info.Isp} | ASN: {info.Asn}");

                return isVpn;
            }
        }

        public class IPHubResponse
        {
            public string Ip { get; set; }
            public string CountryCode { get; set; }
            public int Block { get; set; }  // 0 = residential, 1 = VPN/proxy, 2 = mixed
            public string Isp { get; set; }
            public string Asn { get; set; }
        }
    }
}
