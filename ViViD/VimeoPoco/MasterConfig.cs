using Newtonsoft.Json;
using System.Collections.Generic;

namespace ViViD.VimeoPoco
{
    public class MasterConfig
    {
        [JsonProperty("base_url")]
        public string BaseUrl { get; set; }

        public List<AudioVideo> Video { get; set; }

        public List<AudioVideo> Audio { get; set; }
    }
}