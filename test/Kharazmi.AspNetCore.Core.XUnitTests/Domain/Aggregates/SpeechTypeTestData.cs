using System.Collections;
using Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates.SpeechAggregate;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Domain.Aggregates
{
    public class SpeechTypeTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { SpeechType.Conferences };
            yield return new object[] { SpeechType.SelfPacedLabs };
            yield return new object[] { SpeechType.TraingVideo };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}