        using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Functional;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Functional
{
    [Collection("NullOptionTests")]
    [ExcludeFromCodeCoverage]
    public class NullOptionTests
    {
        [Fact]
        public void GetEnumerator_WithValue_ReturnsEnumeratorWithSingleValue()
        {
            // Arrange
            var value = "testValue";
            var nullOption = NullOption<string>.Value(value);

            // Act
            var enumerator = nullOption.GetEnumerator();
            var list = new List<string>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }

            // Assert
            list.Should().HaveCount(1);
            list.Should().Contain(value);
        }

        [Fact]
        public void GetEnumerator_WithEmpty_ReturnsEmptyEnumerator()
        {
            // Arrange
            var nullOption = NullOption<string>.Empty();

            // Act
            var enumerator = nullOption.GetEnumerator();
            var list = new List<string>();
            while (enumerator.MoveNext())
            {
                list.Add(enumerator.Current);
            }

            // Assert
            list.Should().BeEmpty();
        }

        [Fact]
        public void GetEnumerator_IEnumerable_WithValue_ReturnsEnumeratorWithSingleValue()
        {
            // Arrange
            var value = 123;
            IEnumerable nullOption = NullOption<int>.Value(value);

            // Act
            var enumerator = nullOption.GetEnumerator();
            var list = new List<int>();
            while (enumerator.MoveNext())
            {
                list.Add((int)enumerator.Current);
            }

            // Assert
            list.Should().HaveCount(1);
            list.Should().Contain(value);
        }

        [Fact]
        public void GetEnumerator_IEnumerable_WithEmpty_ReturnsEmptyEnumerator()
        {
            // Arrange
            IEnumerable nullOption = NullOption<int>.Empty();

            // Act
            var enumerator = nullOption.GetEnumerator();
            var list = new List<int>();
            while (enumerator.MoveNext())
            {
                if (enumerator.Current != null)
                {
                    list.Add((int)enumerator.Current);
                }
               
            }

            // Assert
            list.Should().BeEmpty();
        }
    }
}