using System;
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
            if (failures == null) throw new ArgumentNullException(nameof(failures));
            var validationFailures = failures.ToArray();

            return validationFailures.Length <= 0 ? Result.Ok() : Result.Fail(string.Empty).WithValidationMessages(validationFailures);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="failures"></param>
        /// <returns></returns>
        public static Result ToResult(this List<ValidationFailure> failures)
        {
            if (failures == null) throw new ArgumentNullException(nameof(failures));
            var validationFailures = failures.ToArray();

            return validationFailures.Length <= 0 ? Result.Ok() : Result.Fail(string.Empty).WithValidationMessages(validationFailures);
        }
    }
}