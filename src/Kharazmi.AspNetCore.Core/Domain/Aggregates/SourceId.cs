using System;
using Kharazmi.AspNetCore.Core.ValueObjects;

namespace Kharazmi.AspNetCore.Core.Domain.Aggregates
{
    public readonly record struct SourceId
    {
        public SourceId()
        {
            Value = Id.Default<string>();
        }

        public SourceId(string value)
        {
            if (string.IsNullOrEmpty(value)) throw new ArgumentNullException(nameof(value));
            Value = value;
        }

        public string Value { get; }
        public static SourceId New() => new(Id.New<string>());
    }
}