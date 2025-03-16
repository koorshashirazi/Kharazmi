using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Kharazmi.AspNetCore.Core.XUnitTests.EnumConverter
{
    [CollectionDefinition(nameof(EnumConverterTest), DisableParallelization = true)]
    public class EnumConverterTestCollection
    {
    }
    [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
    [Collection(nameof(EnumConverterTest))]
    public class EnumConverterTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EnumConverterTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void FromNameTest()
        {
            var employeeType = ExpirationType.FromName("Assignment");
            employeeType.Value.Should().Be(1);
        }

        [Fact]
        public void FromValueTest()
        {
            var employeeType = ExpirationType.FromValue(1);
            employeeType.Name.Should().Be("Assignment");
        }

        [Fact]
        public void FluentTest()
        {
            var employeeType = ExpirationType.FromValue(1);
            employeeType.When(ExpirationType.Assignment).Then(() => _testOutputHelper.WriteLine("Manager"))
                .When(ExpirationType.Fixed).Then(() => _testOutputHelper.WriteLine("Assistant"))
                .Default(() => _testOutputHelper.WriteLine("Manager"));
        }

        [Fact]
        public void JsonConverterTest()
        {
            var employeeTypeJsonConverter = new ExpirationTypeJsonConverter();
            _testOutputHelper.WriteLine(employeeTypeJsonConverter.ToString());
        }


        [Fact]
        public void GetEmployeeValidDateTest()
        {
            var employee = new Employee
            {
                Name = "Koorsha",
                ExpirationType = ExpirationType.Fixed,
                LastName = "Shirazi",
                BeginDate = DateTime.Today,
                DaysValid = 1
            };

            var employeeType = ExpirationType.FromValue(1);
            employeeType.GetEmployeeValidDate(employee).Should().Be(DateTime.Today.AddDays(employee.DaysValid));
            employeeType.Name.Should().Be(nameof(ExpirationType.Assignment));
        }
    }
}