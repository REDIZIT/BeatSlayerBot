using BeatSlayerServer.Models.Configuration;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.IO;
using System.Net;
using System.Threading.Tasks;

namespace BeatSlayerServer.Services.Messaging.Discord.Commands
{
    public class ReportCommands
    {
        private readonly ServerSettings settings;
        private readonly DiscordBotService botService;

        public ReportCommands(SettingsWrapper wrapper, DiscordBotService botService)
        {
            settings = wrapper.settings;
            this.botService = botService;
        }


        [Command("suggestion")]
        [Description("This command allows you to put your suggestion into suggestion channel. If this idea will be good, then it will be implemented in next updates :D")]
        public async Task Suggest(CommandContext ctx)
        {
            var suggestionChannel = ctx.Guild.GetChannel(settings.Bot.Discord_SuggestionChannelId);
            var developerRole = ctx.Guild.GetRole(settings.Bot.Discord_DeveloperRoleId);

            var parts = ctx.Message.Content.Split(' ');
            if (parts == null || parts.Length == 1)
            {
                await ctx.RespondAsync($"This command allows you to put your suggestion into {suggestionChannel.Mention} channel. If this idea will be good, then it will be implemented in next updates :D");
                return;
            }
            else if (parts.Length <= 2)
            {
                await ctx.RespondAsync($"Minimum words count is 2");
                return;
            }



            string message = string.Join(" ", parts[1..]);


            var builder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    IconUrl = ctx.Message.Author.AvatarUrl,
                    Name = ctx.Message.Author.Username
                },
                Color = new DiscordColor(0, 128, 255),
                Description = message,
                Title = "Suggestion"
            };




            DiscordMessage msg = null;

            if(ctx.Message.Attachments.Count > 0)
            {
                WebClient c = new WebClient();

                string path = "UploadData/" + ctx.Message.Attachments[0].FileName;
                c.DownloadFile(ctx.Message.Attachments[0].Url, path);

                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        msg = await suggestionChannel.SendFileAsync(stream, developerRole.Mention, embed: builder.Build());
                    }
                }
                finally
                {
                    File.Delete(path);
                }
            }
            else
            {
                msg = await suggestionChannel.SendMessageAsync(developerRole.Mention, embed: builder.Build());
            }
            

            await msg.CreateReactionAsync(DiscordEmoji.FromName(botService.client, ":+1:"));
            await msg.CreateReactionAsync(DiscordEmoji.FromName(botService.client, ":-1:"));
            await msg.CreateReactionAsync(DiscordEmoji.FromName(botService.client, ":thinking:"));
        }

        [Command("bug")]
        [Description("Report about bug, message will appear in bugs channel")]
        public async Task Bug(CommandContext ctx)
        {

            var bugsChannel = ctx.Guild.GetChannel(settings.Bot.Discord_BugsChannelId);
            var developerRole = ctx.Guild.GetRole(settings.Bot.Discord_DeveloperRoleId);

            var parts = ctx.Message.Content.Split(' ');
            if (parts == null || parts.Length == 1)
            {
                await ctx.RespondAsync($"Report about bug, message will appear in {bugsChannel.Mention} channel");
                return;
            }
            else if (parts.Length <= 2)
            {
                await ctx.RespondAsync($"Minimum words count is 2");
                return;
            }



            string message = string.Join(" ", parts[1..]);


            var builder = new DiscordEmbedBuilder()
            {
                Author = new DiscordEmbedBuilder.EmbedAuthor()
                {
                    IconUrl = ctx.Message.Author.AvatarUrl,
                    Name = ctx.Message.Author.Username
                },
                Color = new DiscordColor(255, 128, 0),
                Description = message,
                Title = "Bug"
            };


            var attachments = ctx.Message.Attachments;
            

            string messageContent = developerRole.Mention;

            if(attachments.Count > 0)
            {
                WebClient c = new WebClient();

                string path = "UploadData/" + attachments[0].FileName;
                c.DownloadFile(attachments[0].Url, path);

                try
                {
                    using (FileStream stream = new FileStream(path, FileMode.Open))
                    {
                        await bugsChannel.SendFileAsync(stream, messageContent, embed: builder.Build());
                    }
                }
                finally
                {
                    File.Delete(path);
                }
            }
            else
            {
                await bugsChannel.SendMessageAsync(messageContent, embed: builder.Build());
            }

            //await msg.CreateReactionAsync(DiscordEmoji.FromName(botService.client, ":+1:"));
            //await msg.CreateReactionAsync(DiscordEmoji.FromName(botService.client, ":-1:"));
            //await msg.CreateReactionAsync(DiscordEmoji.FromName(botService.client, ":thinking:"));
        }
    }
}
