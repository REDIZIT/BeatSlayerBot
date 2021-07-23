using BeatSlayerServer.Services.Messaging;
using DSharpPlus.Entities;

namespace BeatSlayerServer.Models.Messaging.Discord
{
    public class ModerationRequestMessage : IDiscordMessage
    {
        public string Message => ModeratorRole;
        public DiscordColor Color => new DiscordColor(0, 180, 255);
        public bool IsPublic => false;

        public string Trackname { get; }
        public string Mapper { get; }

        /// <summary>
        /// This role will be pinged
        /// </summary>
        public string ModeratorRole { get; }


        public ModerationRequestMessage(string trackname, string mapper, DiscordRole moderatorRole)
        {
            Trackname = trackname;
            Mapper = mapper;
            ModeratorRole = moderatorRole.Mention;
        }

        public DiscordEmbedBuilder ApplyFields(DiscordEmbedBuilder builder)
        {
            builder.Description = $":robot: Player want to approve his map";
            builder.AddField("Map", Trackname);
            builder.AddField("Mapper", Mapper);

            return builder;
        }
    }
}
