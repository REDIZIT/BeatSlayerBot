using BeatSlayerServer.Services.Messaging;
using DSharpPlus.Entities;

namespace BeatSlayerServer.Models.Messaging.Discord
{
    public class ModerationRequestMessage : IDiscordMessage
    {
        public string Message => $"{ModeratorRole}\n:robot: Player want to approve his map";
        public DiscordColor Color => new DiscordColor(0, 180, 255);
        public bool IsPublic => false;

        public string Trackname { get; }
        public string Mapper { get; }

        /// <summary>
        /// This role will be pinged
        /// </summary>
        public string ModeratorRole { get; }


        public ModerationRequestMessage(string trackname, string mapper, string moderatorRole)
        {
            Trackname = trackname;
            Mapper = mapper;
            ModeratorRole = moderatorRole;
        }

        public DiscordEmbedBuilder ApplyFields(DiscordEmbedBuilder builder)
        {
            builder.AddField("Map", Trackname);
            builder.AddField("Mapper", Mapper);

            return builder;
        }
    }
}
