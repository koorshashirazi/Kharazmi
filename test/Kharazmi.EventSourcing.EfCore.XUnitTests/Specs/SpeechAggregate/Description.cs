﻿using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.SpeechAggregate
{
    public class Description : IEquatable<Description>
    {
        private const int MinLenght = 100;
        private const int MaxLenght = 500;
        public string Value { get; }

        public Description(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidLenghtAggregateException("Value is null or whitespace");
            }
            if (value?.Length < MinLenght)
                throw new InvalidLenghtAggregateException("Value is too short");

            if (value?.Length > MaxLenght)
                throw new InvalidLenghtAggregateException("Value is too long");

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Description);
        }

        public bool Equals(Description other)
        {
            return other != null &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Value);
        }

        public static bool operator ==(Description description1, Description description2)
        {
            return EqualityComparer<Description>.Default.Equals(description1, description2);
        }

        public static bool operator !=(Description description1, Description description2)
        {
            return !(description1 == description2);
        }
    }
}