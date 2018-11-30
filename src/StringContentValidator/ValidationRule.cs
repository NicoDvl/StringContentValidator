using System;
using System.Collections.Generic;
using System.Text;

namespace StringContentValidator
{
    /// <summary>
    /// Define a rule to validate a property.
    /// </summary>
    /// <typeparam name="TRow">Type to validate.</typeparam>
    public class ValidationRule<TRow> : IValidationRuleError<TRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationRule{TRow}"/> class.
        /// </summary>
        /// <param name="value">delegate to get the value to validate.</param>
        public ValidationRule(Func<TRow, string> value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets a method returning the value to validate.
        /// </summary>
        public Func<TRow, string> Value { get; private set; }

        /// <summary>
        /// Gets or sets a pre condition prior to check if property value is valid.
        /// Usefull to keep the precondition defined with a precondition like : If(x => x.Type == "P", ... ).
        /// </summary>
        public Func<TRow, bool> PreCondition { get; set; }

        /// <summary>
        /// Gets or sets a method to check if property value is valid.
        /// </summary>
        public Func<TRow, bool> IsValid { get; set; }

        /// <summary>
        /// Gets or sets a method to define a custom error message.
        /// <summary>
        public Func<TRow, string> OverrideErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a method to define the final error message.
        /// <summary>
        public Func<TRow, string> ErrorMessage { get; set; }
    }
}