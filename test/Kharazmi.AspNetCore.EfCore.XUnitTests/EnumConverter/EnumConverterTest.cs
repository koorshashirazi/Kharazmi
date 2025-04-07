using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Kharazmi.AspNetCore.EfCore.XUnitTests.EnumConverter
{
    [CollectionDefinition(nameof(EnumConverterTest), DisableParallelization = true)]
    public class EnumConverterTestCollection
    {
    }

    [ExcludeFromCodeCoverage]
    [Collection(nameof(EnumConverterTest))]
    public class EnumConverterTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public EnumConverterTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void DbConventionTest()
        {
            using var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open(); 

            var optionsBuilder = new DbContextOptionsBuilder<DbContextTest>()
                .UseSqlite(connection)
                .EnableDetailedErrors()
                .EnableSensitiveDataLogging();

            using var context = new DbContextTest(optionsBuilder.Options);

            context.Database.EnsureCreated();

            context.Employees.Add(new Employee
            {
                Name = "Koorsha",
                DaysValid = 1,
                BeginDate = DateTime.Today,
                ExpirationType = ExpirationType.Assignment,
                LastName = "Shirazi"
            });

            context.Employees.Add(new Employee
            {
                Name = "Koorsha_2",
                DaysValid = 1,
                BeginDate = DateTime.Today,
                ExpirationType = ExpirationType.Fixed,
                LastName = "Shirazi_2"
            });

            context.SaveChanges();

            var employee_1 = context.Employees.FirstOrDefault(x => x.Id == 1);
            var employee_2 = context.Employees.FirstOrDefault(x => x.Id == 2);

            var employeeType = employee_1.ExpirationType;
            employeeType.GetEmployeeValidDate(employee_1).Should().Be(DateTime.Today.AddDays(employee_1.DaysValid));
            employeeType.Name.Should().Be(nameof(ExpirationType.Assignment));

            employeeType = employee_2.ExpirationType;
            employeeType.GetEmployeeValidDate(employee_2).Should()
                .Be(employee_2.BeginDate.Value.AddDays(employee_2.DaysValid));
            employeeType.Name.Should().Be(nameof(ExpirationType.Fixed));
        }
    }
}