using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.Numbering
{
    public class NumberedEntityOption
    {
        public int Start { get; set; } = 1;
        public int IncrementBy { get; set; } = 1;
        public string Prefix { get; set; }

        /// <summary>
        /// AggregateType of fields that used for reset next-number
        /// </summary>
        public IEnumerable<string> FieldNames { get; set; } = new List<string>();
    }
}