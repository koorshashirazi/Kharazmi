using System;
using FluentValidation;
using Kharazmi.AspNetCore.Core.Dependency;

namespace Kharazmi.AspNetCore.Validation
{
    /// <summary>
    /// Validator factory implementation that uses the asp.net service provider to construct validators.
    /// </summary>
    internal class ServiceProviderValidatorFactory : ValidatorFactoryBase, ITransientDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public ServiceProviderValidatorFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public override IValidator CreateInstance(Type validatorType)
        {
            return _serviceProvider.GetService(validatorType) as IValidator;
        }
    }
}