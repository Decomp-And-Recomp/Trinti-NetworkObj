using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkObj.Utils;

namespace NetworkObj.TCP
{
    class Listener
    {
        public static List<(TcpClient client, string IP)> IPData = new List<(TcpClient client, string IP)>();
        private static VPN.IPHubClient VPN = new VPN.IPHubClient("MzAxMjU6OHA5T1MyWlNQcGxSc05MSmZEVk1jQXBDRlVXMHVZNEQ=");
        
        public static void DisconnectClient(string IP)
        {
            lock (IPData)
            {
                for (int i = IPData.Count - 1; i >= 0; i--)
                {
                    if (IPData[i].IP == IP)
                    {
                        Logger.Info($"Disconnecting banned IP: {IP}");
                        Clients.RemoveClient(IPData[i].client);
                        IPData[i].client.Close();
                        IPData.RemoveAt(i);
                    }
                }
            }
        }

        public static string GetIP(TcpClient client)
        {
            lock (IPData)
            {
                for (int i = IPData.Count - 1; i >= 0; i--)
                {
                    if (IPData[i].client == client)
                    {
                        return IPData[i].IP;
                    }
                }
            }

            return "";
        }

        public async Task Start(IPAddress IP, int Port)
        {
            TcpListener listener = new TcpListener(IP, Port);
            listener.Start();

            Logger.Info($"Server started at {IP}:{Port}");
            _ = Task.Run(CheckConnectionsLoop);

            while (true)
            {
                TcpClient client = await listener.AcceptTcpClientAsync();
                _ = Task.Run(async () =>
                {
                    try
                    {
                        NetworkStream stream = client.GetStream();

                        IPEndPoint? real = await Proxy.TryReadProxyHeaderAsync(stream);
                        IPEndPoint displayed = real ?? (IPEndPoint)client.Client.RemoteEndPoint!;

                        if (Program.BannedIPs.Contains(displayed.Address.ToString()))
                        {
                            client.Close();
                            return;
                        }

                        bool isVPN = await VPN.IsVpnIpAsync(displayed.Address.ToString());
                        if (isVPN)
                        {
                            client.Close();
                            return;
                        }

                        Logger.Info($"Client {displayed.Address} connected");
                        IPData.Add((client, displayed.Address.ToString()));

                        await new Responder().Respond(client, displayed.Address.ToString());
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Error($"Error with client: {ex.Message}");
                    }
                });
            }
        }

        private static async Task CheckConnectionsLoop()
        {
            while (true)
            {
                lock (IPData)
                {
                    for (int i = IPData.Count - 1; i >= 0; i--)
                    {
                        var (client, ip) = IPData[i];

                        if (!IsClientConnected(client))
                        {
                            Logger.Info($"Client {ip} disconnected or unresponsive — removing");
                            try { client.Close(); } catch { }
                            IPData.RemoveAt(i);
                            Clients.RemoveClient(client);
                        }
                    }
                }

                await Task.Delay(5000);
            }
        }

        private static bool IsClientConnected(TcpClient client)
        {
            try
            {
                if (client == null || client.Client == null)
                    return false;

                bool part1 = client.Client.Poll(1, SelectMode.SelectRead);
                bool part2 = (client.Client.Available == 0);
                return !(part1 && part2);
            }
            catch
            {
                return false;
            }
        }
    }
}
