using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain.ValueObjects;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.SpeechAggregate
{
    public class Title : ValueObject
    {
        private const int MinLenght = 10;
        private const int MaxLenght = 60;
        public string Value { get; }

        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return Value;
            }
        }

        public Title(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (value.Length < MinLenght)
                throw new InvalidLenghtAggregateException("Value is too short");

            if (value.Length > MaxLenght)
                throw new InvalidLenghtAggregateException("Value is too long");

            Value = value;
        }
    }
}