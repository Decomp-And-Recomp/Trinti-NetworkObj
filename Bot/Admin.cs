using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NetworkObj.Bot
{
    public class Admin
    {
        private static bool IsAdmin(SocketGuildUser guildUser)
        {
            var roleIds = guildUser.Roles.Select(r => r.Id).ToList();
            ulong adminRoleId = 1424408368239612114;
            return roleIds.Contains(adminRoleId);
        }

        public static async Task BanUser(SocketSlashCommand command)
        {
            if (command.GuildId == null || command.GuildId != 1355793353429487656)
            {
                await command.RespondAsync("This command can only be used in the `Mobile Games Revival` server");
                return;
            }

            var guildUser = command.User as SocketGuildUser;

            if (guildUser == null)
            {
                await command.RespondAsync("User unidentifiable");
                return;
            }

            string? ip = command.Data.Options.FirstOrDefault(o => o.Name == "ip")?.Value?.ToString();
            if (ip == null)
            {
                await command.RespondAsync("Something went wrong");
                return;
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                await command.RespondAsync("Blank input, invalid");
                return;
            }

            if (!System.Net.IPAddress.TryParse(ip, out var _))
            {
                await command.RespondAsync("Invalid IP");
                return;
            }

            if (IsAdmin(guildUser))
            {
                bool result = await Program.BanWrapper(ip);
                
                if (result)
                {
                    await command.RespondAsync("Successfully banned player");
                }
                else
                {
                    await command.RespondAsync("Player already baned");
                }

                    return;
            }
            else
            {
                await command.RespondAsync("You aren't an admin, fuck off.");
                return;
            }
        }

        public static async Task UnbanUser(SocketSlashCommand command)
        {
            if (command.GuildId == null || command.GuildId != 1355793353429487656)
            {
                await command.RespondAsync("This command can only be used in the `Mobile Games Revival` server");
                return;
            }

            var guildUser = command.User as SocketGuildUser;

            if (guildUser == null)
            {
                await command.RespondAsync("User unidentifiable");
                return;
            }

            string? ip = command.Data.Options.FirstOrDefault(o => o.Name == "ip")?.Value?.ToString();
            if (ip == null)
            {
                await command.RespondAsync("Something went wrong");
                return;
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                await command.RespondAsync("Blank input, invalid");
                return;
            }

            if (!System.Net.IPAddress.TryParse(ip, out var _))
            {
                await command.RespondAsync("Invalid IP");
                return;
            }

            if (IsAdmin(guildUser))
            {
                bool result = await Program.UnbanWrapper(ip);
                
                if (result)
                {
                    await command.RespondAsync("Successfully unbanned player");
                }
                else
                {
                    await command.RespondAsync("Player not banned");
                }

                return;
            }
            else
            {
                await command.RespondAsync("You aren't an admin, fuck off.");
                return;
            }
        }

        public static async Task IsBannedUser(SocketSlashCommand command)
        {
            if (command.GuildId == null || command.GuildId != 1355793353429487656)
            {
                await command.RespondAsync("This command can only be used in the `Mobile Games Revival` server");
                return;
            }

            var guildUser = command.User as SocketGuildUser;

            if (guildUser == null)
            {
                await command.RespondAsync("User unidentifiable");
                return;
            }

            string? ip = command.Data.Options.FirstOrDefault(o => o.Name == "ip")?.Value?.ToString();
            if (ip == null)
            {
                await command.RespondAsync("Something went wrong");
                return;
            }

            if (string.IsNullOrWhiteSpace(ip))
            {
                await command.RespondAsync("Blank input, invalid");
                return;
            }

            if (!System.Net.IPAddress.TryParse(ip, out var _))
            {
                await command.RespondAsync("Invalid IP");
                return;
            }

            if (IsAdmin(guildUser))
            {
                bool result = Program.IsBannedWrapper(ip);
                
                if (result)
                {
                    await command.RespondAsync("Player is banned");
                }
                else
                {
                    await command.RespondAsync("Player isn't banned");
                }

                return;
            }
            else
            {
                await command.RespondAsync("You aren't an admin, fuck off.");
                return;
            }
        }
    }
}
