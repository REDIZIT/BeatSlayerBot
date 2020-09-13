using BeatSlayerBot.Messagers.Discord.Commands;
using BeatSlayerServer.Models.Configuration;
using BeatSlayerServer.Models.Messaging.Discord;
using BeatSlayerServer.Services.Messaging.Discord.Commands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace BeatSlayerServer.Services.Messaging.Discord
{
    public class DiscordBotService
    {
        public bool IsConnected => !(string.IsNullOrWhiteSpace(client.GatewayUrl) && client.Ping == 0);
        public bool IsBotAlive => client != null && IsConnected;
        public bool IsBotEnabled => settings.Bot.IsDiscordBotEnabled && IsBotAlive;
        private bool IsChannelAvailable => ModerationChannel != null && PublicChannel != null;



        public DiscordClient client;
        private DiscordChannel ModerationChannel { get; set; }
        private DiscordChannel PublicChannel { get; set; }

        private string ModeratorRole { get; set; }


        public readonly IHostEnvironment env;
        private readonly ServerSettings settings;
        private readonly ILogger<DiscordBotService> logger;

        private DependencyCollection deps;






        public DiscordBotService(IHostEnvironment env, SettingsWrapper wrapper, ILogger<DiscordBotService> logger, TimerService timer)
        {
            settings = wrapper.settings;
            this.env = env;
            this.logger = logger;

            Task.Run(async () =>
            {
                try
                {
                    deps = new DependencyCollectionBuilder()
                        .AddInstance(wrapper)
                        .AddInstance(this)
                        .Build();
                    await BuildBot(deps);
                }
                catch (Exception err)
                {
                    logger.LogError("Can't build discord bot due to: " + err);
                }
            });

            timer.AddTimer(() =>
            {
                Task.Run(async () =>
                {
                    string map = await GetRandomGroup();
                    await SetStatus(map);
                });
            }, TimeSpan.FromSeconds(3 * 60));
        }
        

        public async Task Kill()
        {
            await client.DisconnectAsync();
            client = null;
        }
        public async Task Build()
        {
            await BuildBot(deps);
        }


        public async Task SendMessage(IDiscordMessage message)
        {
            if (!IsBotEnabled)
            {
                logger.LogError("I can't send message due to client isn't available");
                return;
            }
            if (!IsChannelAvailable)
            {
                logger.LogError("I can't send message due to channel isn't available");
                return;
            }



            DiscordEmbedBuilder builder = new DiscordEmbedBuilder()
            {
                Color = message.Color
            };
            DiscordEmbed embed = message.ApplyFields(builder).Build();



            // Send this message to channel, where only moderators live
            await client.SendMessageAsync(ModerationChannel, message.Message, false, embed);

            // Send this message to channel, where are can see
            if (message.IsPublic)
            {
                await client.SendMessageAsync(PublicChannel, message.Message, false, embed);
            }
        }






        public async Task SendModerationRequestMessage(string trackname, string mapper)
        {
            ModerationRequestMessage msg = new ModerationRequestMessage(trackname, mapper, ModeratorRole);
            await SendMessage(msg);
        }

        public async Task SendMapPublishedMessage(string trackname, string mapper)
        {
            MapPublishMessage msg = new MapPublishMessage(trackname, mapper);
            await SendMessage(msg);
        }

        public async Task SendMapApprovedMessage(string trackname, string mapper, string moderator, string comment)
        {
            MapApproveMessage msg = new MapApproveMessage(trackname, mapper, moderator, comment);
            await SendMessage(msg);
        }

        public async Task SendMapRejectedMessage(string trackname, string mapper, string moderator, string comment)
        {
            MapRejectMessage msg = new MapRejectMessage(trackname, mapper, moderator, comment);
            await SendMessage(msg);
        }

        public async Task<string> GetRandomGroup()
        {
            WebClient c = new WebClient();
            return await c.DownloadStringTaskAsync(settings.Host.ServerUrl + "/Messagers/GetRandomMap");
        }










        private async Task BuildBot(DependencyCollection deps)
        {
            client = new DiscordClient(new DiscordConfiguration
            {
                Token = settings.Bot.Discord_Token,
                TokenType = TokenType.Bot,
            });

            Console.WriteLine("Build bot");


            // Provide commands and their configuration
            CommandsNextConfiguration conf = new CommandsNextConfiguration()
            {
                StringPrefix = ">",
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = true,
                Dependencies = deps
            };

            CommandsNextModule commandsModule = client.UseCommandsNext(conf);
            commandsModule.RegisterCommands<BasicCommands>();
            commandsModule.RegisterCommands<ReportCommands>();
            commandsModule.RegisterCommands<MapsCommands>();

            client.MessageCreated += CheckAnotherPrefixes;
            client.MessageCreated += SudoPing;


            await client.ConnectAsync();


            // Get channels where bot will spam
            ModerationChannel = await client.GetChannelAsync(settings.Bot.Discord_ModerationChannelId);
            PublicChannel = await client.GetChannelAsync(settings.Bot.Discord_PublicChannelId);

            await client.UpdateStatusAsync(new DiscordGame(">play Beat Slayer"));


            await Task.Delay(1500);

            ModeratorRole = ModerationChannel.Guild.GetRole(settings.Bot.Discord_ModerationRoleId).Mention;
        }
        private async Task SetStatus(string status)
        {
            if (!IsBotAlive) return;

            await client.UpdateStatusAsync(new DiscordGame(status));
        }
        /// <summary>
        /// Tsundere mode: if user notified another bot not in special channel trigger xD
        /// </summary>
        private async Task CheckAnotherPrefixes(MessageCreateEventArgs e)
        {
            if (!IsBotEnabled) return;


            // If user invoked another bot in non-bot channel
            if (!settings.Bot.Discord_BotChannels.Contains(e.Channel.Id))
            {
                foreach (string prefix in settings.Bot.Discord_OtherPrefixes)
                {
                    if (e.Message.Content.StartsWith(prefix))
                    {
                        List<string> allowedChannels = new List<string>();
                        foreach (ulong channelId in settings.Bot.Discord_BotChannels)
                        {
                            var allowedChannel = await client.GetChannelAsync(channelId);
                            allowedChannels.Add(allowedChannel.Mention);
                        }


                        foreach (var message in await e.Channel.GetMessagesAsync(10, after: e.Message.Id))
                        {
                            if (message.Author.IsBot)
                            {
                                await e.Channel.DeleteMessageAsync(message);
                            }
                        }


                        string allowedChannelsStr = string.Join(',', allowedChannels);
                        await client.SendMessageAsync(e.Channel, e.Author.Mention + " please use bots on allowed channels (" + allowedChannelsStr + ")");

                        break;
                    }
                }
            }
        }

        private async Task SudoPing(MessageCreateEventArgs e)
        {
            if(e.Message.Content[1..] == "sudo ping")
            {
                var builder = new DiscordEmbedBuilder()
                {
                    Color = new DiscordColor(32, 32, 32)
                };
                builder.AddField("Environment", $"Service ({env.EnvironmentName})");

                await e.Channel.SendMessageAsync("SUDO!! PING!!!", embed: builder.Build());
            }
        }
    }
}
