using System.Collections.Generic;
using System.Linq;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Validation;

namespace Kharazmi.AspNetCore.Core.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ValidationFailureExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        public static Result ToResult(this IEnumerable<ValidationFailure> failures)
        {
            failures = failures?.ToList();

            return failures != null && !failures.Any() ? Result.Ok() : Result.Fail(string.Empty, "").WithValidationMessages(failures);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        public static Result ToResult(this List<ValidationFailure> failures)
        {
            return failures != null && !failures.Any() ? Result.Ok() : Result.Fail(string.Empty).WithValidationMessages(failures);
        }
    }
}