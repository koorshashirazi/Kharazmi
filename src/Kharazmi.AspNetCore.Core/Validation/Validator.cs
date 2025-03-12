using System;
using System.Collections.Generic;
using System.Reflection;
using Kharazmi.AspNetCore.Core.GuardToolkit;

namespace Kharazmi.AspNetCore.Core.Validation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public interface IValidator<in TModel> : IValidator
    {
        /// <summary>
        /// Validate the specified instance synchronously.
        /// contains validation logic and business rules validation
        /// </summary>
        /// <param name="model">model to validate</param>
        /// <returns>
        /// A list of <see cref="ValidationFailure"/> indicating the results of validating the model value.
        /// </returns>
        IEnumerable<ValidationFailure> Validate(TModel model);
    }

    /// <summary>
    /// 
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validate the specified instance synchronously.
        /// contains validation logic and business rules validation
        /// </summary>
        /// <param name="model">model to validate</param>
        /// <returns>
        /// A list of <see cref="ValidationFailure"/> indicating the results of validating the model value.
        /// </returns>
        IEnumerable<ValidationFailure> Validate(object model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        bool CanValidateInstancesOfType(Type type);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public abstract class Validator<TModel> : IValidator<TModel>
    {
        IEnumerable<ValidationFailure> IValidator.Validate(object model)
        {
            Ensure.ArgumentIsNotNull(model, nameof(model));

            if (!((IValidator) this).CanValidateInstancesOfType(model.GetType()))
            {
                throw new InvalidOperationException(
                    $"Cannot validate instances of type '{model.GetType().GetTypeInfo().Name}'. This validator can only validate instances of type '{typeof(TModel).Name}'.");
            }

            return Validate((TModel) model);
        }

        bool IValidator.CanValidateInstancesOfType(Type type)
        {
            return typeof(TModel).GetTypeInfo().IsAssignableFrom(type.GetTypeInfo());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public abstract IEnumerable<ValidationFailure> Validate(TModel model);
    }
}