using Newtonsoft.Json;

namespace ViViD.VimeoPoco
{
    public class Player
    {
        [JsonProperty("config_url")]
        public string ConfigUrl { get; set; }

        [JsonProperty("player_url")]
        public string PlayerUrl { get; set; }
    }
}