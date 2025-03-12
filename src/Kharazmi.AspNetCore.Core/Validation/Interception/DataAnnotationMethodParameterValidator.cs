using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Kharazmi.AspNetCore.Core.Extensions;

namespace Kharazmi.AspNetCore.Core.Validation.Interception
{
    internal sealed class DataAnnotationMethodParameterValidator : IMethodParameterValidator
    {
        private readonly IServiceProvider _provider;

        public DataAnnotationMethodParameterValidator(IServiceProvider provider)
        {
            _provider = provider ?? throw new ArgumentNullException();
        }

        public IEnumerable<ValidationFailure> Validate(object parameter)
        {
            var properties = TypeDescriptor.GetProperties(parameter).Cast<PropertyDescriptor>();
            foreach (var property in properties)
            {
                var validationAttributes = property.Attributes.OfType<ValidationAttribute>().ToArray();
                if (validationAttributes.IsNullOrEmpty())
                {
                    continue;
                }

                var validationContext = new ValidationContext(parameter,
                    _provider,
                    null)
                {
                    DisplayName = property.DisplayName,
                    MemberName = property.Name
                };

                foreach (var attribute in validationAttributes)
                {
                    var failures = new List<ValidationFailure>();

                    var result = attribute.GetValidationResult(property.GetValue(parameter), validationContext);

                    if (result == System.ComponentModel.DataAnnotations.ValidationResult.Success) continue;

                    var message = result?.ErrorMessage;

                    if (result?.MemberNames != null)
                    {
                        failures.AddRange(result.MemberNames.Select(memberName =>  ValidationFailure.For(memberName, message)));
                    }

                    if (failures.Count == 0)
                    {
                        // result.MemberNames was null or empty.
                        failures.Add( ValidationFailure.For(string.Empty, message));
                    }

                    return failures;
                }
            }

            return Enumerable.Empty<ValidationFailure>();
        }
    }
}