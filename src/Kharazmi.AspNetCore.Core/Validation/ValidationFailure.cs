using System;
using System.Collections.Generic;

namespace Kharazmi.AspNetCore.Core.Validation
{
    /// <summary>Specifies the severity of a rule.</summary>
    public enum Severity
    {
        /// <summary>Error</summary>
        Error,

        /// <summary>Warning</summary>
        Warning,

        /// <summary>Info</summary>
        Info,
    }

    /// <summary>Defines a validation failure</summary>
    [Serializable]
    public sealed record ValidationFailure
    {
        internal ValidationFailure()
        {
        }

        /// <summary>Creates a new ValidationFailure.</summary>
        public ValidationFailure(string propertyName, string errorMessage, object? attemptedValue = null)
        {
            PropertyName = propertyName;
            ErrorMessage = errorMessage;
            AttemptedValue = attemptedValue;
        }

        /// <summary>The name of the property.</summary>
        public string PropertyName { get; set; }

        /// <summary>The error message</summary>
        public string ErrorMessage { get; set; }

        /// <summary>The property value that caused the failure.</summary>
        public object AttemptedValue { get; set; }

        /// <summary>Custom state associated with the failure.</summary>
        public object CustomState { get; set; }

        /// <summary>Custom severity level associated with the failure.</summary>
        public Severity Severity { get; set; }

        /// <summary>Gets or sets the error code.</summary>
        public string ErrorCode { get; set; }

        /// <summary>
        /// Gets or sets the formatted message arguments.
        /// These are values for custom formatted message in validator resource files
        /// Same formatted message can be reused in UI and with same number of format placeholders
        /// Like "Value {0} that you entered should be {1}"
        /// </summary>
        public object[] FormattedMessageArguments { get; set; }

        /// <summary>Gets or sets the formatted message placeholder values.</summary>
        public Dictionary<string, object> FormattedMessagePlaceholderValues { get; set; }

        /// <summary>The resource name used for building the message</summary>
        public string ResourceName { get; set; }

        /// <summary>Creates a textual representation of the failure.</summary>
        public override string ToString()
        {
#if DEBUG
            return $$"""
                     { 
                        "PropertyName": "{{PropertyName}}",
                        "ErrorMessage": "{{ErrorMessage}}"
                     }
                     """;
#else
        return $$"""
                 { "PropertyName": "{{PropertyName}}", "ErrorMessage": "{{ErrorMessage}}" }
                 """;
#endif
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="errorMessage"></param>
        /// <param name="attemptedValue"></param>
        /// <returns></returns>
        public static ValidationFailure For(string propertyName, string errorMessage, object? attemptedValue = null)
        {
            return new ValidationFailure(propertyName, errorMessage, attemptedValue);
        }
    }
}