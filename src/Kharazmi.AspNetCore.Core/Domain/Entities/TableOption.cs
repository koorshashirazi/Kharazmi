using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain.ValueObjects;

namespace Kharazmi.AspNetCore.Core.Domain.Entities
{
    public class TableOption: ValueObject
    {
        public TableOption() 
        {

        }
        public TableOption(string name) : this(name, "dbo")
        {
        }
        public TableOption(string name, string schema)
        {
            Name = name;
            Schema = schema;
        }

        public string Name { get;  }
        public string Schema { get;  }

        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return Name;
                yield return Schema;
            }
        }
    }
}