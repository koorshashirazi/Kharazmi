using System.Collections;
using System.Collections.Generic;
using Kharazmi.AspNetCore.Core.XUnitTests.AggregateTest.SpeechAggregate;

namespace Kharazmi.AspNetCore.Core.XUnitTests.AggregateTest
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