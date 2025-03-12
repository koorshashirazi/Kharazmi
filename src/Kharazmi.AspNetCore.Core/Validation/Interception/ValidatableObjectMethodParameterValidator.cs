using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Kharazmi.AspNetCore.Core.Validation.Interception
{
    internal sealed class ValidatableObjectMethodParameterValidator : IMethodParameterValidator
    {
        public IEnumerable<ValidationFailure> Validate(object parameter)
        {
            if (parameter == null || !(parameter is IValidatableObject validatable))
            {
                return Enumerable.Empty<ValidationFailure>();
            }

            var failures = validatable.Validate(new ValidationContext(parameter));

            return ToModelValidationResult(failures);
        }

        private static IEnumerable<ValidationFailure> ToModelValidationResult(
            IEnumerable<System.ComponentModel.DataAnnotations.ValidationResult> failures)
        {
            foreach (var result in failures)
            {
                if (result == System.ComponentModel.DataAnnotations.ValidationResult.Success) continue;

                if (result.MemberNames == null || !result.MemberNames.Any())
                {
                    yield return ValidationFailure.For(propertyName: null, errorMessage: result.ErrorMessage);
                }
                else
                {
                    foreach (var memberName in result.MemberNames)
                    {
                        yield return ValidationFailure.For(memberName, result.ErrorMessage);
                    }
                }
            }
        }
    }
}