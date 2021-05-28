using CsvHelper.Configuration.Attributes;

namespace NightCredits.Models
{
    public class AirportModel
    {
        [Name("ident")]
        public string Code { get; set; }

        [Name("type")]
        public string Type { get; set; }

        [Name("name")]
        public string Name { get; set; }

        [Name("municipality")]
        public string Municipality { get; set; }

        [Name("latitude_deg")]
        public string Latitude { get; set; }

        [Name("longitude_deg")]
        public string Longitude { get; set; }
    }
}
