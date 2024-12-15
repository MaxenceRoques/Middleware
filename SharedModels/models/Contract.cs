using System.Collections.Generic;
using System.Runtime.Serialization;

namespace SharedModels.models
{
    [DataContract]
    public class Contract
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CommercialName { get; set; }

        [DataMember]
        public string CountryCode { get; set; }

        [DataMember]
        public List<string> Cities { get; set; }
    }
}