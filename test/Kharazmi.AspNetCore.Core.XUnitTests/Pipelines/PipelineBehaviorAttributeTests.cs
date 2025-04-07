using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Kharazmi.AspNetCore.Core.Pipelines;
using Xunit;

namespace Kharazmi.AspNetCore.Core.XUnitTests.Pipelines
{
    [Collection("PipelineBehaviorAttributeTests")]
    [ExcludeFromCodeCoverage]
    public class PipelineBehaviorAttributeTests
    {
        [Fact]
        public void PipelineBehaviorAttribute_Constructor_WithBehaviorTypes_ShouldSetBehaviorTypesProperty()
        {
            // Arrange
            Type[] behaviorTypes = { typeof(string), typeof(int) };

            // Act
            var attribute = new PipelineBehaviorAttribute(behaviorTypes);

            // Assert
            attribute.BehaviorTypes.Should().BeEquivalentTo(behaviorTypes, "because the constructor should initialize the BehaviorTypes property with the provided types");
        }

        [Fact]
        public void PipelineBehaviorAttribute_BehaviorTypes_ShouldBeInitializedWithEmptyArray_WhenNoBehaviorTypesProvided()
        {
            // Arrange

            // Act
            var attribute = new PipelineBehaviorAttribute();

            // Assert
            attribute.BehaviorTypes.Should().BeEquivalentTo(Array.Empty<Type>(), "because the constructor should initialize the BehaviorTypes property with the provided types");
        }

        [Fact]
        public void PipelineBehaviorAttribute_Constructor_WithNullBehaviorTypes_ShouldSetBehaviorTypesPropertyToEmptyArray()
        {
            // Arrange
            Type[] behaviorTypes = [];

            // Act
            var attribute = new PipelineBehaviorAttribute(behaviorTypes);

            // Assert
            attribute.BehaviorTypes.Should().BeEmpty("because the constructor should handle null behaviorTypes by initializing with empty array.");
        }

        [Fact]
        public void PipelineBehaviorAttribute_BehaviorTypes_ShouldReturnEmptyArray_WhenNoTypesPassedInConstructor()
        {
            // Arrange

            // Act
            var attribute = new PipelineBehaviorAttribute();

            // Assert
            attribute.BehaviorTypes.Should().BeEmpty();
        }

        [Fact]
        public void PipelineBehaviorAttribute_BehaviorTypes_ShouldReturnCorrectTypeArray_WhenValidTypesPassedInConstructor()
        {
            // Arrange
            var types = new Type[] { typeof(string), typeof(int) };

            // Act
            var attribute = new PipelineBehaviorAttribute(types);

            // Assert
            attribute.BehaviorTypes.Should().BeEquivalentTo(types);
        }

        [Fact]
        public void PipelineBehaviorAttribute_MultipleBehaviorTypes_ShouldStoreAllTypes()
        {
            // Arrange
            var behaviorTypes = new Type[] { typeof(string), typeof(int), typeof(object) };

            // Act
            var attribute = new PipelineBehaviorAttribute(behaviorTypes);

            // Assert
            attribute.BehaviorTypes.Should().BeEquivalentTo(behaviorTypes, "because the constructor should store all provided behavior types");
        }
    }
}