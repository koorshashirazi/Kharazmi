using Kharazmi.AspNetCore.Core.Validation;

namespace Kharazmi.AspNetCore.Core.Domain.Commands
{
    /// <summary>
    /// 
    /// </summary>
    public abstract class Command : Message, ICommand
    {
        /// <summary></summary>
        public ValidationResult ValidationResult { get; set; }

        /// <summary></summary>
        /// <summary> </summary>
        /// <returns></returns>
        public abstract bool IsValid();
    }
}