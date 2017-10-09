using Newtonsoft.Json;

namespace ViViD.VimeoPoco
{
    public class Cdns
    {
        [JsonProperty("fastly_skyfire")]
        public FastlySkyfire FastlySkyfire { get; set; }
    }
}