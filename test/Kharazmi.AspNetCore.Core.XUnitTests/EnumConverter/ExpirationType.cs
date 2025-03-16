using System;
using Kharazmi.AspNetCore.Core.Enumerations;

namespace Kharazmi.AspNetCore.Core.XUnitTests.EnumConverter
{
    public abstract class ExpirationType : Enumeration<ExpirationType>
    {
        public static readonly ExpirationType Assignment = new AssignmentType();
        public static readonly ExpirationType Fixed = new FixedType();

        private ExpirationType(string name, int value) : base(name, value)
        {
        }

        public abstract DateTime GetEmployeeValidDate(Employee employee);

        private sealed class AssignmentType : ExpirationType
        {
            public AssignmentType() : base(nameof(Assignment), 1)
            {
            }

            public override DateTime GetEmployeeValidDate(Employee employee) => DateTime.Today.AddDays(employee.DaysValid);
        }

        private sealed class FixedType : ExpirationType
        {
            public FixedType() : base(nameof(Fixed), 2)
            {
            }

            public override DateTime GetEmployeeValidDate(Employee employee) =>
                employee.BeginDate?.AddDays(employee.DaysValid) ?? throw new InvalidOperationException();
        }
    }

    public enum ExpirationType_
    {
        Assistant = 1,
        Fixed = 2,
    }
}