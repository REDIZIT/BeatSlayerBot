using BeatSlayerServer.Models.Configuration;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BeatSlayerServer.Services.Messaging.Discord.Commands
{
    public class ModerationCommands
    {
        private readonly DiscordBotService bot;
        private readonly ServerSettings settings;

        public ModerationCommands(SettingsWrapper wrapper, DiscordBotService botService)
        {
            settings = wrapper.settings;
            bot = botService;
        }

        [Command("maps-to-approve")]
        [Description("Get full list of maps to requested to approve")]
        public async Task GetMapsToApprove(CommandContext ctx)
        {
            WebClient c = new WebClient();

            DiscordMessage msg = await ctx.RespondAsync("loading...");

            string json = await c.DownloadStringTaskAsync(new Uri(settings.Host.ServerUrl + "/Moderation/GetOperations"));
            List<ModerateOperation> ls = JsonConvert.DeserializeObject<List<ModerateOperation>>(json);

            await msg.DeleteAsync();

            await ctx.RespondAsync("Maps to approve count " + ls.Count);
            try
            {
                await ctx.RespondAsync(string.Join("\n", ls.Select(o => o.trackname)));
            }
            catch (Exception err)
            {
                await ctx.RespondAsync("Exception: " + err);
            }
        }
    }

    public class ModerateOperation
    {
        public string trackname, nick;

        public enum State
        {
            Waiting, Rejected, Approved
        }
        public State state;

        public enum UploadType
        {
            Requested, Updated
        }
        public UploadType uploadType;


        public string moderatorNick;
        public string moderatorComment;
    }
}
