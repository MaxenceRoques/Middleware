using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Station
    {
        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Status { get; set; }

        [DataMember]
        public Availability MainStands { get; set; }

        [DataMember]
        public Position Position { get; set; }

        public bool IsAvailableForPickup()
        {
            return Status == "OPEN" && MainStands?.Availabilities?.Bikes > 0;
        }

        public bool IsAvailableForDropOff()
        {
            return Status == "OPEN" && MainStands?.Availabilities?.Stands > 0;
        }
    }
}
