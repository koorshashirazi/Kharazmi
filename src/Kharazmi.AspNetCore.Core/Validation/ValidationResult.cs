using System;
using System.Collections.Generic;
using System.Linq;

namespace Kharazmi.AspNetCore.Core.Validation
{
    /// <summary>The result of running a validator</summary>
    [Serializable]
    public class ValidationResult
    {
        /// <summary>Whether validation succeeded</summary>
        public virtual bool IsValid => Errors.Count == 0;

        /// <summary>A collection of errors</summary>
        public IList<ValidationFailure> Errors { get; }

        /// <summary>
        /// 
        /// </summary>
        public string[] RuleSetsExecuted { get; internal set; }

        /// <summary>Creates a new validationResult</summary>
        public ValidationResult()
        {
            Errors = new List<ValidationFailure>();
        }

        /// <summary>
        /// Creates a new ValidationResult from a collection of failures
        /// </summary>
        /// <param name="failures">List of <see cref="T:FluentValidation.Results.ValidationFailure" /> which is later available through <see cref="P:FluentValidation.Results.ValidationResult.Messages" />. This list get's copied.</param>
        /// <remarks>
        /// Every caller is responsible for not adding <c>null</c> to the list.
        /// </remarks>
        public ValidationResult(IEnumerable<ValidationFailure> failures)
        {
            Errors = failures.Where(failure => failure != null).ToList();
        }

        /// <summary>
        /// Generates a string representation of the error messages separated by new lines.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.ToString(Environment.NewLine);
        }

        /// <summary>
        /// Generates a string representation of the error messages separated by the specified character.
        /// </summary>
        /// <param name="separator">The character to separate the error messages.</param>
        /// <returns></returns>
        public string ToString(string separator)
        {
            return string.Join(separator, Errors.Select(failure => failure.ErrorMessage));
        }
    
    }
}