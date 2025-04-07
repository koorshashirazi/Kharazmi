using Kharazmi.AspNetCore.Core.Validation;

namespace Kharazmi.AspNetCore.Core.Domain
{
    public interface IDomainCommand
    {
        DomainCommandType CommandType { get; }
        DomainCommandId CommandId { get; }
    }

    /// <summary>
    /// 
    /// </summary>
    public abstract class DomainCommand : IDomainCommand
    {
        protected DomainCommand(DomainCommandId? commandId = null)
        {
            CommandType = DomainCommandType.From(GetType());
            CommandId = commandId ?? DomainCommandId.New();
        }

        public DomainCommandType CommandType { get; }
        public DomainCommandId CommandId { get; }

        /// <summary></summary>
        public ValidationResult ValidationResult { get; set; } = new ();

        /// <summary></summary>
        /// <summary> </summary>
        /// <returns></returns>
        public abstract bool IsValid();
    }
}