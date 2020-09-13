using System.Collections.Generic;

namespace BeatSlayerBot.Models.Configuration.Modules
{
    public class HostSettings
    {
        public string StartUrl { get; set; } = "http://locahost:4020";
        public string ServerUrl { get; set; } = "http://localhost:5020";
        public List<string> AllowedIPs { get; set; } = new List<string>();
    }
}
