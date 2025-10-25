using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using NetworkObj.Utils;

namespace NetworkObj.TCP
{
    class Listener
    {
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

                        Logger.Info($"Client {displayed.Address}:{displayed.Port} connected");

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
