using System.Collections.Generic;
using System.Linq;
using Kharazmi.AspNetCore.Core.Functional;

namespace Kharazmi.AspNetCore.Core.Validation
{
    public static class ModelValidationResultExtensions
    {
        public static Result ToResult(this IEnumerable<ValidationFailure> failures)
        {
            failures = failures as ValidationFailure[] ?? [.. failures];
            return !failures.Any() ? Result.Ok() : Result.Fail(string.Empty).WithValidationMessages([.. failures]);
        }
    }
}