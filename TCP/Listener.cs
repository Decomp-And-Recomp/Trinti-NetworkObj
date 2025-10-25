using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkObj.Utils;

namespace NetworkObj.TCP
{
    class Listener
    {
        public static List<(TcpClient client, string IP)> IPData = new List<(TcpClient client, string IP)>();

        public static void DisconnectClient(string IP)
        {
            lock (IPData)
            {
                for (int i = IPData.Count - 1; i >= 0; i--)
                {
                    if (IPData[i].IP == IP)
                    {
                        Logger.Info($"Disconnecting banned IP: {IP}");
                        IPData[i].client.Close();
                        IPData.RemoveAt(i);
                    }
                }
            }
        }

        public async Task Start(IPAddress IP, int Port)
        {
            TcpListener listener = new TcpListener(IP, Port);
            listener.Start();

            Logger.Info($"Server started at {IP}:{Port}");

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

                        Logger.Info($"Client {displayed.Address} connected");
                        IPData.Add((client, displayed.Address.ToString()));

                        await new Responder().Respond(client);
                    }
                    catch (System.Exception ex)
                    {
                        Logger.Error($"Error with client: {ex.Message}");
                    }
                });
            }
        }
    }
}
