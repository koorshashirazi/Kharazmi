using System;
using System.Collections.Generic;
using System.Linq;

namespace Kharazmi.AspNetCore.Core.Validation
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TModel"></typeparam>
    public class NullValidator<TModel> : IValidator<TModel>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool CanValidateInstancesOfType(Type type)
        {
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<ValidationFailure> Validate(TModel model)
        {
            return Enumerable.Empty<ValidationFailure>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public IEnumerable<ValidationFailure> Validate(object model)
        {
            return Enumerable.Empty<ValidationFailure>();
        }
    }
}
