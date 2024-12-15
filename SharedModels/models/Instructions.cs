using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Instructions
    {

        [DataMember(Name = "profile")]
        public string Profile { get; set; }

        [DataMember(Name = "routes")]
        public List<Route> Routes { get; set; }

        public Instructions(string profile)
        {
            Profile = profile;
            Routes = new List<Route>();
        }
    }
}
