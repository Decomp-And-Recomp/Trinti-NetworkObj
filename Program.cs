using System.Net;
using System.Threading.Tasks;
using NetworkObj.TCP;
using NetworkObj.Utils;

namespace NetworkObj
{
    class Program
    {
        public static List<string> BannedIPs = new List<string>();
        public static List<string> Logs = new List<string>();
        static string banFile = "banlist.txt";
        static string logFile = "log.txt";
        private static int CurrentArg = 0;
        private static int Indexed = 0;
        private static bool indexedServer = false;
        private static bool indexedPort = false;
        private static int port = 4201;
        private static int server = 1;

        static async Task Main(string[] args)
        {
            LoadBanList();
            LoadLog();

            foreach (string arg in args)
            {
                if ((arg == "-s" || arg == "--server") && !indexedServer)
                {
                    indexedServer = true;
                    Indexed++;

                    if (int.TryParse(args[CurrentArg + 1], out int serverVersion))
                    {
                        if (serverVersion == 1)
                        {
                            // no functionality yet to change server.
                            server = serverVersion;
                            Logger.Info("Running 4.3+ Server");
                        }
                        else
                        {
                            Environment.Exit(0);
                            break;
                        }
                    }
                    else
                    {
                        Environment.Exit(0);
                        break;
                    }
                }
                else if ((arg == "-p" || arg == "--port") && !indexedPort)
                {
                    indexedPort = true;
                    Indexed++;

                    if (int.TryParse(args[CurrentArg + 1], out int servPort))
                    {
                        port = servPort;
                    }
                    else
                    {
                        Environment.Exit(0);
                        break;
                    }
                }

                if (Indexed == 2) break;

                CurrentArg++;
            }

            Thread commandThread = new Thread(CommandLine);
            commandThread.Start();

            Listener Server = new Listener();
            IPAddress IP = IPAddress.Any;

            await Server.Start(IP, port);
        }

        static void LoadBanList()
        {
            if (File.Exists(banFile))
            {
                BannedIPs = new List<string>(File.ReadAllLines(banFile));
            }
        }

        static void SaveBanList()
        {
            File.WriteAllLines(banFile, BannedIPs);
        }

        static void LoadLog()
        {
            if (File.Exists(logFile))
            {
                Logs = new List<string>(File.ReadAllLines(logFile));
            }
        }

        public static void SaveLog()
        {
            File.WriteAllLines(logFile, Logs);
        }

        static void CommandLine()
        {
            while (true)
            {
                string input = Console.ReadLine();
                string[] parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length < 2) continue;

                string command = parts[0].ToLower();
                string ip = parts[1];

                if (command == "ban")
                {
                    if (!BannedIPs.Contains(ip))
                    {
                        BannedIPs.Add(ip);
                        SaveBanList();
                        Logger.Info($"Banned IP: {ip}");
                        Listener.DisconnectClient(ip);
                    }
                    else
                    {
                        Logger.Info($"IP {ip} is already banned.");
                    }
                }
                else if (command == "unban")
                {
                    if (BannedIPs.Contains(ip))
                    {
                        BannedIPs.Remove(ip);
                        SaveBanList();
                        Logger.Info($"Unbanned IP: {ip}");
                    }
                    else
                    {
                        Logger.Info($"IP {ip} is not banned.");
                    }
                }
            }
        }
    }
}