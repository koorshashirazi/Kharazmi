using Kharazmi.AspNetCore.Core.Exceptions;

namespace Kharazmi.EventSourcing.EfCore.XUnitTests.Specs.SpeechAggregate
{
    public readonly record struct UriValue
    {
        public string Value { get; }

        public UriValue(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new InvalidLenghtAggregateException("url should not be empty");

            Value = value;
        }

        public static bool ToUrl(string source, out Uri?  urlValue)
        {
            var result = Uri.TryCreate(source, UriKind.Absolute, out var uri) &&
                   (uri.Scheme == Uri.UriSchemeHttp
                    || uri.Scheme == Uri.UriSchemeHttps
                    || uri.Scheme == Uri.UriSchemeFile);

            urlValue = uri;
            return result;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}