using System.Collections.Generic;
using System.Linq;
using Kharazmi.AspNetCore.Core.Dependency;
using Kharazmi.AspNetCore.Core.Functional;
using Kharazmi.AspNetCore.Core.Validation;

namespace Kharazmi.AspNetCore.Core.Application.Services
{
    public interface IApplicationService : IScopedDependency
    {
    }
    
    public abstract class ApplicationService : IApplicationService
    {
        protected static Result Ok() => Result.Ok();
        protected static Result Fail(string message) => Result.Fail(message);

        protected static Result Fail(string message, IReadOnlyCollection<ValidationFailure> failures) =>
            Result.Fail(message).WithValidationMessages(failures);

        protected static Result<T> Ok<T>(T value) => Result.Ok(value);
        protected static Result<T> Fail<T>(string message) => Result.Fail<T>(message);

        protected static Result<T> Fail<T>(string message, IReadOnlyCollection<ValidationFailure> failures) =>
            Result.Fail<T>(message).WithValidationMessages(failures);
    }
}