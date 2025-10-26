using Discord;
using Discord.Net;
using Discord.WebSocket;
using NetworkObj.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;


namespace NetworkObj.Bot
{
    internal class Bot
    {
        private static DiscordSocketClient _client;
        private static ulong channel = 1424404773763022982;
        private static IMessageChannel chan;

        public static async Task Start()
        {
            _client = new DiscordSocketClient();

            var token = "GetAJob";
            _client.SlashCommandExecuted += HandleSlashCommand;
            _client.Ready += OnReady;

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            chan = (IMessageChannel)_client.GetChannel(channel);

            Logger.Info("Discord bot started");

            await Task.Delay(-1);
        }

        private static async Task OnReady()
        {
            Logger.Info($"Bot connected as {_client.CurrentUser.Username}");

            chan = (IMessageChannel)_client.GetChannel(channel);

            if (chan != null)
            {
                await SendMessage("```ansi\r\n\u001b[2;32m\u001b[2;37m\u001b[2;34mInfo\u001b[0m\u001b[2;37m\u001b[0m\u001b[2;32m\u001b[0m: BossRaid Control Panel Online```");
                var guild = _client.GetGuild(1355793353429487656);

                var banCommand = new SlashCommandBuilder()
                    .WithName("ban")
                    .WithDescription("Ban a player from the game by IP")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("ip")
                        .WithDescription("The IP address to ban")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.String));

                var unbanCommand = new SlashCommandBuilder()
                    .WithName("unban")
                    .WithDescription("Unban a player from the game by IP")
                    .AddOption(new SlashCommandOptionBuilder()
                        .WithName("ip")
                        .WithDescription("The IP address to unban")
                        .WithRequired(true)
                        .WithType(ApplicationCommandOptionType.String));

                var isBanCommand = new SlashCommandBuilder()
                        .WithName("isbanned")
                        .WithDescription("Checks if a player is banned by IP")
                        .AddOption(new SlashCommandOptionBuilder()
                            .WithName("ip")
                            .WithDescription("The IP address to check")
                            .WithRequired(true)
                            .WithType(ApplicationCommandOptionType.String));

                await guild.DeleteApplicationCommandsAsync();
                await guild.CreateApplicationCommandAsync(unbanCommand.Build());
                await guild.CreateApplicationCommandAsync(isBanCommand.Build());
                await guild.CreateApplicationCommandAsync(banCommand.Build());
            }
            else
            {
                Logger.Error("Could not find the channel! Check the channel ID or bot permissions.");
            }
        }

        private static async Task HandleSlashCommand(SocketSlashCommand command)
        {
            switch (command.Data.Name)
            {
                case "ban":
                    await Admin.BanUser(command);
                    break;
                case "unban":
                    await Admin.UnbanUser(command);
                    break;
                case "isbanned":
                    await Admin.IsBannedUser(command);
                    break;
            }
        }

        public static async Task SendMessage(string Message)
        {
            await chan.SendMessageAsync(Message);
        }
    }
}
