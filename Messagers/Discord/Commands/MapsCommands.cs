using BeatSlayerServer.Models.Configuration;
using BeatSlayerServer.Services;
using BeatSlayerServer.Services.Messaging.Discord;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BeatSlayerBot.Messagers.Discord.Commands
{
    public class MapsCommands
    {
        private readonly DiscordBotService bot;
        private readonly ServerSettings settings;

        public MapsCommands(DiscordBotService bot, SettingsWrapper wrapper)
        {
            this.bot = bot;
            settings = wrapper.settings;
        }

        [Command("random")]
        [Description("Get random map")]
        public async Task GetRandomGroup(CommandContext ctx)
        {
            string map = await bot.GetRandomGroup();

            await ctx.RespondAsync(map);
        }

        [Command("maps")]
        [Description("Get all player published maps")]
        public async Task GetPublishedMaps(CommandContext ctx, string nick)
        {
            Console.WriteLine("Get published maps with nick = " + nick);

            string url = "https://localhost:5011/Messagers/GetPublishedMaps?nick=" + nick;

            try
            {
                WebClient c = new WebClient();
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                ServicePointManager.ServerCertificateValidationCallback +=
                    (sender, cert, chain, error) =>
                    {
                        return true;
                    };

                string response = await c.DownloadStringTaskAsync(url);

                if (string.IsNullOrWhiteSpace(response))
                {
                    await ctx.RespondAsync("Can't get published maps for this player");
                    return;
                }

                List<string> maps = JsonConvert.DeserializeObject<List<string>>(response);


                await ctx.RespondAsync(string.Join("\n", maps));
            }
            catch(Exception err)
            {
                //logger.LogError(err.Message);
                Console.WriteLine(err.Message + " url was " + url);
                await ctx.RespondAsync("Sorry, but I can't respond you now ;(");
            }
        }


        [Command("maps-count")]
        [Description("Get maps count")]
        public async Task GetMapsCount(CommandContext ctx, [Description("Get maps from database or folders?")] bool isDb)
        {
            Console.WriteLine("Get maps count. IsDb? " + isDb);
            try
            {
                WebClient c = new WebClient();
                string response = await c.DownloadStringTaskAsync("http://localhost:5020/Messagers/GetMapsCount?isDb=" + isDb);

                await ctx.RespondAsync(response);
            }
            catch
            {
                await ctx.RespondAsync("Sorry, but I can't do this now ;(");
            }
        }

        [Command("map-info")]
        [Description("Get map info about map")]
        public async Task GetMapInfo(CommandContext ctx, [Description("Song name (syntax is Author-Name)")] string trackname, [Description("Player nick, who created map")] string mapper)
        {
            try
            {
                trackname = WebUtility.UrlEncode(trackname);
                mapper = WebUtility.UrlEncode(mapper);

                WebClient c = new WebClient();
                string response = await c.DownloadStringTaskAsync(settings.Host.ServerUrl + $"/Messagers/GetMapInfo?trackname={trackname}&mapper={mapper}");

                await ctx.RespondAsync(response);
            }
            catch(Exception err)
            {
                Console.WriteLine("GetMapInfo Error: " + err.Message);
                await ctx.RespondAsync("I can't handle your command >_<");
            }
        }

        [Command("get-skill")]
        [Description("Get preferred difficulty (raw calculation)")]
        public async Task GetSkill(CommandContext ctx, string nick)
        {
            try
            {
                nick = WebUtility.UrlEncode(nick);

                WebClient c = new WebClient();
                string response = await c.DownloadStringTaskAsync(settings.Host.ServerUrl + $"/Messagers/GetPreferredDifficulty?nick={nick}");

                await ctx.RespondAsync(response);
            }
            catch (Exception err)
            {
                Console.WriteLine("GetMapInfo Error: " + err.Message);
                await ctx.RespondAsync("I can't handle your command >_<");
            }
        }
    }
}
