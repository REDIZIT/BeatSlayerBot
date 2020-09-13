using System;
using System.Threading.Tasks;
using BeatSlayerServer.Models.Configuration;
using Microsoft.Extensions.Logging;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace BeatSlayerServer.Services.Messaging
{
    public class VkBotService
    {
        public bool IsBotAlive => vkapi != null;
        public bool IsBotEnabled => true;

        private readonly VkApi vkapi;
        private readonly ServerSettings settings;
        private readonly ILogger<VkBotService> logger;

        public VkBotService(SettingsWrapper wrapper, ILogger<VkBotService> logger)
        {
            settings = wrapper.settings;
            this.logger = logger;

            vkapi = new VkApi();

            vkapi.Authorize(new ApiAuthParams()
            {
                AccessToken = settings.Bot.Vk_AcessToken
            });
        }

        public async Task SendMessage(string message)
        {
            if(!IsBotAlive)
            {
                logger.LogError("Can't send message due to bot is dead");
                return;
            }
            if (!IsBotEnabled)
            {
                logger.LogError("Can't send message due to bot is disabled");
                return;
            }

            await vkapi.Messages.SendAsync(new MessagesSendParams
            {
                RandomId = new Random().Next(),
                Message = message,
                UserId = settings.Bot.Vk_UserId
            });
        }
       


        public async Task ModeratorCheat(string trackname, string moderator)
        {
            string msg = $"😱 Модератор {moderator} нарушил правило 😱\nОн перенёс свою же карту ({trackname}) в список одобренных!";
            await SendMessage(msg);
        }

        public async Task SendModerationRequest(string trackname, string mapper, bool update)
        {
            string msg = $"🤖 Игрок запросил модерацию 🤖\n{mapper} хочет добавить {trackname} в approved карты. Update? " + update;
            await SendMessage(msg);
        }

        public async Task SendMapApprovedMessage(string trackname, string mapper, string moderator, string comment)
        {
            string msg = $"👾 Модератор {moderator} подтвердил запрос на получение approved статуса для карты {trackname} от игрока {mapper}.\nКомментарий: {comment}";
            await SendMessage(msg);
        }
        public async Task SendMapRejectedMessage(string trackname, string mapper, string moderator, string comment)
        {
            string msg = $"👾 Модератор {moderator} отклонил запрос на получение approved статуса для карты {trackname} от игрока {mapper}.\nКомментарий: {comment}";
            await SendMessage(msg);
        }



        public async Task ErrorLogging(string errorMessage)
        {
            string msg = $"Произошла ошибка: " + errorMessage;
            await SendMessage(msg);
        }
        public async Task CoinsSyncLimit(string nick, int coins)
        {
            string msg = $"Так лять, тут происходит какая та чернуха.\nАккаунт: {nick}\nCoins: {coins}";
            await SendMessage(msg);
        }
    }
}
