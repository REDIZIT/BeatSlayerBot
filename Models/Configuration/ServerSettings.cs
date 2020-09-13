using BeatSlayerBot.Models.Configuration.Modules;
using BeatSlayerServer.Models.Configuration.Modules;

namespace BeatSlayerServer.Models.Configuration
{
    public class ServerSettings
    {
        public HostSettings Host { get; set; }
        public BotSettings Bot { get; set; }
        public EmailSettings Email { get; set; }
    }
}
