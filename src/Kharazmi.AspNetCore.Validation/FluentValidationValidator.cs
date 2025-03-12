using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Kharazmi.AspNetCore.Core.Validation;

namespace Kharazmi.AspNetCore.Validation
{
    internal class FluentValidationValidator<T> : Validator<T>
    {
        private readonly IValidatorFactory _factory;

        public FluentValidationValidator(IValidatorFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public override IEnumerable<ValidationFailure> Validate(T model)
        {
            var fvValidator = _factory.GetValidator(model.GetType());

            if (fvValidator == null) return Enumerable.Empty<ValidationFailure>();

            var validationResult = fvValidator.Validate((IValidationContext) model);
            var failures = validationResult.Errors
                .Select(e => ValidationFailure.For(e.PropertyName, e.ErrorMessage))
                .ToList();

            return failures;
        }
    }
}