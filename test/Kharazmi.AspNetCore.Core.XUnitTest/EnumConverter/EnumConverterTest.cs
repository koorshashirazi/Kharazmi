using System;
using System.Linq;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Abstractions;

namespace Kharazmi.AspNetCore.Core.XUnitTest.EnumConverter
{
    [CollectionDefinition(nameof(EnumConverterTest), DisableParallelization = true)]
    public class EnumConverterTestCollection { }

    [Collection(nameof(EnumConverterTest))]

    /// <summary>
    /// https://github.com/ardalis/SmartEnum/blob/master/README.md
    /// </summary>
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
            IServiceCollection services = new ServiceCollection();

            services.AddEntityFrameworkInMemoryDatabase()
                .AddDbContext<DbContextTest>(
                    (_, optionsBuilder) =>
                    {
                        optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString("N"));
                    },
                    ServiceLifetime.Singleton);

            services.AddTransient<DbContextTest>();
            
            var db = services.BuildServiceProvider().GetService<DbContextTest>();
            
            db.Employees.Add(new Employee
            {
                Name = "Koorsha",
                DaysValid = 1,
                BeginDate = DateTime.Today,
                ExpirationType = ExpirationType.Assignment,
                LastName = "Shirazi"
            });

            db.Employees.Add(new Employee
            {
                Name = "Koorsha_2",
                DaysValid = 1,
                BeginDate = DateTime.Today,
                ExpirationType = ExpirationType.Fixed,
                LastName = "Shirazi_2"
            });

            db.SaveChanges();

            var employee_1 = db.Employees.FirstOrDefault(x => x.Id == 1);
            var employee_2 = db.Employees.FirstOrDefault(x => x.Id == 2);
            
            var employeeType = employee_1.ExpirationType;
            employeeType.GetEmployeeValidDate(employee_1).Should().Be(DateTime.Today.AddDays(employee_1.DaysValid));
            employeeType.Name.Should().Be(nameof(ExpirationType.Assignment));
            
            employeeType = employee_2.ExpirationType;
            employeeType.GetEmployeeValidDate(employee_2).Should().Be(employee_2.BeginDate.Value.AddDays(employee_2.DaysValid));
            employeeType.Name.Should().Be(nameof(ExpirationType.Fixed));
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