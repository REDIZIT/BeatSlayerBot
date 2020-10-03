using BeatSlayerServer.Models.Configuration;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BeatSlayerServer.Services.Messaging.Discord.Commands
{
    public class BasicCommands
    {
        private readonly DiscordBotService discordBot;
        private readonly ServerSettings settings;

        public BasicCommands(DiscordBotService bot, SettingsWrapper wrapper)
        {
            discordBot = bot;
            settings = wrapper.settings;
        }


        [Command("ping")]
        public async Task Ping(CommandContext ctx)
        {
            Console.WriteLine("Pinged!!!");

            if (!discordBot.IsBotEnabled) return;

            var builder = new DiscordEmbedBuilder()
            {
                Color = new DiscordColor(32, 32, 32)
            };
            builder.AddField("Environment", $"Service");
            builder.AddField("Version", settings.Bot.Version);
            builder.AddField("Code Source", "Automated build");

            await ctx.Channel.SendMessageAsync("Hello guys", embed: builder.Build()).ConfigureAwait(false);
        }
    }
}
