using BeatSlayerServer.Services.Messaging;
using DSharpPlus.Entities;

namespace BeatSlayerServer.Models.Messaging.Discord
{
    public class MapRejectMessage : IDiscordMessage
    {
        public string Message => ":robot: Map was rejected";
        public DiscordColor Color => new DiscordColor(255, 0, 0);
        public bool IsPublic => false;

        public string Trackname { get; }
        public string Mapper { get; }
        public string Moderator { get; }
        public string Comment { get; }


        public MapRejectMessage(string trackname, string mapper, string moderator, string comment)
        {
            Trackname = trackname;
            Mapper = mapper;
            Moderator = moderator;
            Comment = comment;
        }

        public DiscordEmbedBuilder ApplyFields(DiscordEmbedBuilder builder)
        {
            builder.AddField("Map", Trackname, true);
            builder.AddField("Mapper", Mapper, true);
            builder.AddField("Comment", Comment + " -**" + Moderator + "**");

            return builder;
        }
    }
}
