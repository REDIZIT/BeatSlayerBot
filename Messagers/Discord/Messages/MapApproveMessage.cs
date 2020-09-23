using BeatSlayerServer.Services.Messaging;
using DSharpPlus.Entities;

namespace BeatSlayerServer.Models.Messaging.Discord
{
    public class MapApproveMessage : IDiscordMessage
    {
        public string Message => "";
        public DiscordColor Color => new DiscordColor(0, 220, 0);
        public bool IsPublic => true;

        public string Trackname { get; }
        public string Mapper { get; }
        public string Moderator { get; }
        public string Comment { get; }


        public MapApproveMessage(string trackname, string mapper, string moderator, string comment)
        {
            Trackname = trackname;
            Mapper = mapper;
            Moderator = moderator;
            Comment = comment;
        }

        public DiscordEmbedBuilder ApplyFields(DiscordEmbedBuilder builder)
        {
            builder.Description = ":robot: Map was approved";
            builder.AddField("Map", Trackname, true);
            builder.AddField("Mapper", Mapper, true);
            builder.AddField("Comment", Comment + " -**" + Moderator + "**");

            return builder;
        }
    }
}
