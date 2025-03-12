using System;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.Domain.ValueObjects;
using Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.Exceptions;

namespace Kharazmi.AspNetCore.Core.XUnitTest.AggregateTest.SpeechAggregate
{
    public class UrlValue : ValueObject
    {
        public string Value { get; }

        public UrlValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentNullException(nameof(value));

            if (!CheckUrlValid(value))
                throw new InvalidUrlAggregateException("url is invalid");
            Value = value;
        }
        protected override IEnumerable<object> EqualityValues
        {
            get
            {
                yield return Value;
            }
        }

        private static bool CheckUrlValid(string source) =>
            Uri.TryCreate(source, UriKind.Absolute, out Uri uriResult) &&
            (uriResult.Scheme == Uri.UriSchemeHttp
             || uriResult.Scheme == Uri.UriSchemeHttps
             || uriResult.Scheme == Uri.UriSchemeFile);


    }
}