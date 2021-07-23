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
            DiscordMessage msg = await ctx.RespondAsync("loading...");

            var ls = await GetOperations();

            await msg.DeleteAsync();

            await ctx.RespondAsync("Maps to approve count " + ls.Count());
            try
            {
                await ctx.RespondAsync(string.Join("\n", ls.Select(o => "**" + o.trackname + "** by **" + o.nick + "**")));
            }
            catch (Exception err)
            {
                await ctx.RespondAsync("Exception: " + err);
            }
        }

        [Command("approve")]
        [Description("Approve map")]
        public async Task ApproveMap(CommandContext ctx, [Description("Song name (syntax is Author-Name)")] string trackname, [Description("Player nick, who created map")] string mapper, [Description("Comment")] string comment)
        {
            if (ctx.Member.Roles.All(r => r.Name != bot.ModeratorRole.Name))
            {
                await ctx.RespondAsync("You're not moderator");
                return;
            }


            DiscordMessage msg = await ctx.RespondAsync("loading...");

            ModerateOperation op = (await GetOperations()).FirstOrDefault(o => o.trackname == trackname && o.nick == mapper);
            if (op == null)
            {
                await msg.DeleteAsync();
                await ctx.RespondAsync("Map not found");
                return;
            }

            op.moderatorNick = "[Discord] " + ctx.Member.DisplayName;
            op.moderatorComment = comment;
            op.state = ModerateOperation.State.Approved;

            await SendModerationResponse(op);

            await msg.DeleteAsync();
            await ctx.RespondAsync("Approved");
        }
        [Command("reject")]
        [Description("Reject map")]
        public async Task RejectMap(CommandContext ctx, [Description("Song name (syntax is Author-Name)")] string trackname, [Description("Player nick, who created map")] string mapper, [Description("Comment")] string comment)
        {
            if (ctx.Member.Roles.All(r => r.Name != bot.ModeratorRole.Name))
            {
                await ctx.RespondAsync("You're not moderator");
                return;
            }


            DiscordMessage msg = await ctx.RespondAsync("loading...");

            ModerateOperation op = (await GetOperations()).FirstOrDefault(o => o.trackname == trackname && o.nick == mapper);
            if (op == null)
            {
                await msg.DeleteAsync();
                await ctx.RespondAsync("Map not found");
                return;
            }

            op.moderatorNick = "[Discord] " + ctx.Member.DisplayName;
            op.moderatorComment = comment;
            op.state = ModerateOperation.State.Rejected;

            await SendModerationResponse(op);

            await msg.DeleteAsync();
            await ctx.RespondAsync("Rejected");
        }

        private async Task<IEnumerable<ModerateOperation>> GetOperations()
        {
            WebClient c = new WebClient();

            string json = await c.DownloadStringTaskAsync(new Uri(settings.Host.ServerUrl + "/Moderation/GetOperations"));
            List<ModerateOperation> ls = JsonConvert.DeserializeObject<List<ModerateOperation>>(json);

            return ls.Where(o => o.state == ModerateOperation.State.Waiting).ToList();
        }
        private async Task SendModerationResponse(ModerateOperation op)
        {
            WebClient c = new WebClient();
            string json = WebUtility.UrlEncode(JsonConvert.SerializeObject(op));
            string url = string.Format(settings.Host.ServerUrl + "/Moderation/SendResponse?opJson={0}", json);

            await c.DownloadStringTaskAsync(new Uri(url));
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
