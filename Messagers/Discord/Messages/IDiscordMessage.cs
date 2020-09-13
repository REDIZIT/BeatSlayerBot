using DSharpPlus.Entities;

namespace BeatSlayerServer.Services.Messaging
{
    public interface IDiscordMessage
    {
        /// <summary>
        /// Main message where you can ping someone
        /// </summary>
        string Message { get; }

        /// <summary>
        /// Color of embed
        /// </summary>
        DiscordColor Color { get; }

        /// <summary>
        /// Add fields to embed
        /// </summary>
        DiscordEmbedBuilder ApplyFields(DiscordEmbedBuilder builder);

        /// <summary>
        /// Should this message appear on public channel?
        /// </summary>
        bool IsPublic { get; }
    }
}
