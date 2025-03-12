using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Kharazmi.AspNetCore.Core.Linq
{
    public class Group : Sort
    {
        [DataMember(Name = "aggregates")]
        public IEnumerable<Aggregator> Aggregates { get; set; }
    }
}
