using Newtonsoft.Json;
using System.Linq;

namespace NightCredits.Models
{
    public class AirportModel
    {
        [JsonProperty("ident")]
        public string Code { get; set; }
        [JsonProperty("coordinates")]
        public string Coordinates { get; set; }
        public string Latitude => Coordinates.Split(',').Last().Trim();
        public string Longitude => Coordinates.Split(',').First().Trim();
    }
}
