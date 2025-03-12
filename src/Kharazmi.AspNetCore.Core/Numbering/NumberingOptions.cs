using System;
using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.Numbering
{
    public class NumberingOptions
    {
        public IDictionary<Type, NumberedEntityOption> NumberedEntityMap { get; } =
            new Dictionary<Type, NumberedEntityOption>();
    }
}