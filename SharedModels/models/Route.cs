using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Route
    {
        [DataMember(Name = "instruction")]
        public string Instruction { get; set; }

        [DataMember(Name = "duration")]
        public double Duration { get; set; }

        [DataMember(Name = "distance")]
        public double Distance { get; set; }

        public Route(string instruction, double duration, double distance)
        {
            Instruction = instruction;
            Duration = duration;
            Distance = distance;
        }
    }
}
