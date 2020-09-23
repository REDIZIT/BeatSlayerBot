using BeatSlayerServer.Services.Messaging;
using DSharpPlus.Entities;

namespace BeatSlayerServer.Models.Messaging.Discord
{
    public class MapPublishMessage : IDiscordMessage
    {
        public string Message => "";
        public DiscordColor Color => new DiscordColor(0, 255, 128);
        public bool IsPublic => true;

        public string Trackname { get; }
        public string Mapper { get; }


        public MapPublishMessage(string trackname, string mapper)
        {
            Trackname = trackname;
            Mapper = mapper;
        }

        public DiscordEmbedBuilder ApplyFields(DiscordEmbedBuilder builder)
        {
            builder.Description = ":relaxed: New map published";
            builder.AddField("Map", Trackname);
            builder.AddField("Mapper", Mapper, true);

            return builder;
        }
    }
}
