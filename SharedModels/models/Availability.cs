using SharedModels.models;
using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Availability
    {
        [DataMember]
        public Availabilities Availabilities { get; set; }

        [DataMember]
        public int Capacity { get; set; }
    }
}
