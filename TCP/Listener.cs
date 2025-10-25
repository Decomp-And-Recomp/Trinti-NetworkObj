using System.Net;
using System.Net.Sockets;
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

                Logger.Info($"Client {client.Client.RemoteEndPoint} connected");

                _ = Task.Run(async () =>
                {
                    try
                    {
                        await new Responder().Respond(client);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Error with client {client.Client.RemoteEndPoint}: {ex.Message}");
                    }
                });
            }
        }
    }
}