using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Availabilities
    {
        [DataMember]
        public int Bikes { get; set; }

        [DataMember]
        public int Stands { get; set; }
    }
}
