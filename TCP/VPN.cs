using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

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
                _httpClient = new HttpClient
                {
                    BaseAddress = new Uri("https://v2.api.iphub.info/"),
                    Timeout = TimeSpan.FromSeconds(5)
                };
                _httpClient.DefaultRequestHeaders.Add("X-Key", _apiKey);
            }

            public async Task<IPHubResponse?> GetIpInfoAsync(string ip)
            {
                try
                {
                    //Console.WriteLine($"[IPHub] Checking {ip}...");

                    var response = await _httpClient.GetAsync($"ip/{ip}");

                    if (!response.IsSuccessStatusCode)
                    {
                        //Console.WriteLine($"[IPHub] HTTP {(int)response.StatusCode} ({response.ReasonPhrase}) for {ip}");

                        if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
                        {
                            //Console.WriteLine("[IPHub] Rate limit hit, assuming non-VPN for now.");
                            return new IPHubResponse { Ip = ip, Block = 0, Isp = "Rate-Limited", Asn = 0 };
                        }

                        return null;
                    }

                    string json = await response.Content.ReadAsStringAsync();

                    var result = JsonSerializer.Deserialize<IPHubResponse>(
                        json,
                        new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                            TypeInfoResolver = new System.Text.Json.Serialization.Metadata.DefaultJsonTypeInfoResolver()
                        });

                    if (result == null)
                    {
                        //Console.WriteLine($"[IPHub] Could not parse response for {ip}: {json}");
                        return null;
                    }

                    return result;
                }
                catch (TaskCanceledException)
                {
                    //Console.WriteLine($"[IPHub] Timeout while checking {ip}");
                    return null;
                }
                catch (Exception ex)
                {
                    //Console.WriteLine($"[IPHub] Error checking {ip}: {ex.Message}");
                    return null;
                }
            }

            public async Task<bool> IsVpnIpAsync(string ip)
            {
                var info = await GetIpInfoAsync(ip);

                if (info == null)
                {
                    //Console.WriteLine($"[IPHub] Could not verify {ip}, defaulting to non-VPN.");
                    return false;
                }

                bool isVpn = info.Block == 1 || info.Block == 2;

                //Console.WriteLine($"[IPHub] {ip} | VPN: {isVpn} | ISP: {info.Isp} | ASN: {info.Asn}");

                return isVpn;
            }
        }

        public class IPHubResponse
        {
            public string Ip { get; set; } = "";
            public string CountryCode { get; set; } = "";
            public string CountryName { get; set; } = "";
            public int Block { get; set; }       // 0 = residential, 1 = VPN/proxy, 2 = mixed
            public string Isp { get; set; } = "";
            public int Asn { get; set; }
        }
    }
}
