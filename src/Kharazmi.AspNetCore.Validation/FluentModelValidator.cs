using FluentValidation;

namespace Kharazmi.AspNetCore.Validation
{
    public abstract class FluentModelValidator<TModel> : AbstractValidator<TModel>
    {
    }
}