using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Position
    {
        [DataMember]
        public double Latitude { get; set; }

        [DataMember]
        public double Longitude { get; set; }

        public Position(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }

        public Position()
        {
        }
    }
}