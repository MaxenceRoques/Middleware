using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Itinerary
    {
        [DataMember(Name = "profile")]
        public string Profile { get; set; }

        [DataMember(Name = "total_duration")]
        public double TotalDuration { get; set; }

        [DataMember(Name = "total_distance")]
        public double TotalDistance { get; set; }

        [DataMember(Name = "geometry")]
        public Coordinates Geometry { get; set; }

        public Itinerary(string profile)
        {
            Profile = profile;
            TotalDuration = 0;
            TotalDistance = 0;
            Geometry = new Coordinates();
        }
    }
}
