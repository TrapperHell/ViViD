using Newtonsoft.Json;
using System.Collections.Generic;

namespace ViViD.VimeoPoco
{
    public class AudioVideo
    {
        [JsonProperty("base_url")]
        public string BaseUrl { get; set; }

        [JsonProperty("avg_bitrate")]
        public int AverageBitrate { get; set; }

        [JsonProperty("init_segment")]
        public string InitialSegment { get; set; }

        public List<Segment> Segments { get; set; }
    }
}