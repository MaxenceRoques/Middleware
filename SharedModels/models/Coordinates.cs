using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Coordinates
    {
        [DataMember(Name = "coordinates")]
        public List<List<double>> Coord { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }
    }
}
