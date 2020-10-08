using BeatSlayerServer.Services.Messaging;
using BeatSlayerServer.Services.Messaging.Discord;
using BeatSlayerServer.Utils.Email;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Threading.Tasks;

namespace BeatSlayerBot.Controllers
{
    public class BotController : Controller
    {
        private readonly DiscordBotService dsBot;
        private readonly VkBotService vkBot;
        private readonly EmailService emailService;

        public BotController(DiscordBotService dsBot, VkBotService vkBot, EmailService emailService)
        {
            this.dsBot = dsBot;
            this.vkBot = vkBot;
            this.emailService = emailService;
        }

        public IActionResult Index()
        {
            return Content("I'm Messager :D");
        }
        public IActionResult Status()
        {
            var builder = new StringBuilder();

            builder.AppendLine("Discord Bot");
            builder.AppendLine("Alive? " + dsBot.IsBotAlive);
            builder.AppendLine("Enabled? " + dsBot.IsBotEnabled);

            builder.AppendLine("\nVk Bot");
            builder.AppendLine("Alive? " + vkBot.IsBotAlive);
            builder.AppendLine("Enabled? " + vkBot.IsBotEnabled);

            builder.AppendLine("\nEmail service");
            builder.AppendLine("Alive? " + emailService.IsServiceAlive);
            builder.AppendLine("Enabled? " + emailService.IsServiceEnabled);

            builder.AppendLine("\nEnvironment: Service");

            return Content(builder.ToString());
        }



        public bool IsDiscordAlive()
        {
            try { return dsBot.IsBotAlive; }
            catch { return false; }
        }
        public bool IsDiscordEnabled()
        {
            try { return dsBot.IsBotEnabled; }
            catch { return false; }
        }
        public bool IsVkAlive()
        {
            try { return vkBot.IsBotAlive; }
            catch { return false; }
        }
        public bool IsVkEnabled()
        {
            try { return vkBot.IsBotEnabled; }
            catch { return false; }
        }
        public bool IsEmailAlive()
        {
            try { return emailService.IsServiceAlive; }
            catch { return false; }
        }
        public bool IsEmailEnabled()
        {
            try { return emailService.IsServiceEnabled; }
            catch { return false; }
        }


        public async Task KillDiscord() { await dsBot.Kill(); }
        public void KillEmail() { emailService.Kill(); }

        public async Task BuildDiscord() { await dsBot.Build(); }
        public void BuildEmail() { emailService.Build(); }








        public async Task<IActionResult> SendMapPublished(string trackname, string mapper)
        {
            await dsBot.SendMapPublishedMessage(trackname, mapper);

            return Content("Done");
        }
        public async Task<IActionResult> SendModerationRequestMessage(string trackname, string mapper)
        {
            await dsBot.SendModerationRequestMessage(trackname, mapper);
            await vkBot.SendModerationRequest(trackname, mapper, false);
            return Content("Done");
        }
        public async Task<IActionResult> SendMapApprovedMessage(string trackname, string mapper, string moderator, string comment)
        {
            await dsBot.SendMapApprovedMessage(trackname, mapper, moderator, comment);
            await vkBot.SendMapApprovedMessage(trackname, mapper, moderator, comment);
            return Content("Done");
        }
        public async Task<IActionResult> SendMapRejectedMessage(string trackname, string mapper, string moderator, string comment)
        {
            await dsBot.SendMapRejectedMessage(trackname, mapper, moderator, comment);
            await vkBot.SendMapRejectedMessage(trackname, mapper, moderator, comment);
            return Content("Done");
        }


        public async Task SendMessageToVk(string message)
        {
            await vkBot.SendMessage(message);
        }
        public async Task SendCheat(string trackname, string moderator)
        {
            await vkBot.ModeratorCheat(trackname, moderator);
        }
        public async Task SendCoinsSyncLimit(string nick, int coins)
        {
            await vkBot.CoinsSyncLimit(nick, coins);
        }
        public async Task SendScore(string nick, string grade, string trackname, string mods, string accuracy, string rp)
        {
            // [player name] got [rank] [Map+mods] [accuracy%] [Number of RP]
            await dsBot.SendScoreMessage(nick, grade, trackname, mods, accuracy, rp);
        }



        public void SendRestorePasswordCode(string nick, string email, string code)
        {
            emailService.SendRestorePasswordCode(nick, email, code);
        }
        public void SendEmailChangeCode(string nick, string email, string code)
        {
            emailService.SendEmailChangeCode(nick, email, code);
        }
        public void SendApprove(string nick, string email, string trackname, string moderator, string reason)
        {
            emailService.SendApprove(nick, email, trackname, moderator, reason);
        }
        public void SendReject(string nick, string email, string trackname, string moderator, string reason)
        {
            emailService.SendReject(nick, email, trackname, moderator, reason);
        }
    }
}