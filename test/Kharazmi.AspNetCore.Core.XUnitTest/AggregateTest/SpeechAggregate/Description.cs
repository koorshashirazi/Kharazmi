using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain.ValueObjects;
using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.SpeechAggregate
{
    public class Description : ValueObject
    {
        private const int MinLenght = 100;
        private const int MaxLenght = 500;
        public string Value { get; }

        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return Value;
            }
        }

        public Description(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(Value));
            }
            if (value.Length < MinLenght)
                throw new InvalidLenghtAggregateException("Value is too short");

            if (value.Length > MaxLenght)
                throw new InvalidLenghtAggregateException("Value is too long");

            Value = value;
        }

       
    }
}