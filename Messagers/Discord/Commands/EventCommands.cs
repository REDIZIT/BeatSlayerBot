using BeatSlayerServer.Services.Messaging.Discord;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;
using System.Text;
using System.Threading.Tasks;

namespace BeatSlayerBot.Messagers.Discord.Commands
{
    public class EventCommands
    {
        private readonly DiscordBotService bot;

        public EventCommands(DiscordBotService bot)
        {
            this.bot = bot;
        }

        [Command("event")]
        public async Task GetResults(CommandContext ctx)
        {
            StringBuilder b = new StringBuilder();

            DateTime[] startAndEndTimes = await bot.GetStartAndEndEventTimes();
            TimeSpan duration = startAndEndTimes[1] - startAndEndTimes[0];


            bool isExpired = DateTime.UtcNow > startAndEndTimes[1];

            if (!isExpired)
            {
                TimeSpan leftTime = DateTime.UtcNow - startAndEndTimes[1];
                string leftTimeStr = leftTime.ToString("%d") + " days " + leftTime.ToString("hh") + ":" + leftTime.ToString("%m");

                b.AppendLine(":diamond_shape_with_a_dot_inside: The event is still going on :diamond_shape_with_a_dot_inside:");
                b.AppendLine($":small_blue_diamond: Duration {duration.TotalDays} days **(left {leftTimeStr})**");
            }
            else
            {
                b.AppendLine(":white_check_mark: Event completed :white_check_mark: ");
            }
            b.AppendLine("\n:small_orange_diamond: You can only take part if you have less than 50k RP");
            b.AppendLine(":small_orange_diamond: Play in multiplayer and get 1 score for each game\n\nNickname          Games count");




            string[] results = await bot.GetMultiplayerEventResults();

            for (int i = 0; i < results.Length - 1; i++)
            {
                string medal = (i == 0 ? ":first" : i == 1 ? ":second" : ":third") + "_place:";

                string nick = results[i].Split(':')[0];
                string score = results[i].Split(':')[1];

                string resultLine;
                if (i < 3)
                {
                    resultLine = medal + "  **" + SetStringLenght(nick, 10) + $"{score}**";
                }
                else
                {
                    resultLine = "**" + SetStringLenght(nick, 10) + $"**{score}";
                }

                if (i == 3) resultLine = "\n" + resultLine;

                b.AppendLine(resultLine.Replace("\r", ""));
            }



            await ctx.Channel.SendMessageAsync(b.ToString()).ConfigureAwait(false);
        }

        private string SetStringLenght(string sourceText, int lenght)
        {
            int originalLenght = sourceText.Length;

            for (int i = 0; i < lenght - originalLenght; i++)
            {
                sourceText += "   ";
            }

            return sourceText;
        }
    }
}
