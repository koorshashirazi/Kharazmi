using System;

namespace Kharazmi.AspNetCore.Core.XUnitTests.EnumConverter
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public int DaysValid { get; set; }
        public DateTime? BeginDate { get; set; }

        // public ExpirationType_ ExpirationType { get; set; }
        public ExpirationType ExpirationType { get; set; }

        public DateTime ValidateExpirationType()
            => ExpirationType.GetEmployeeValidDate(this);
    }
}