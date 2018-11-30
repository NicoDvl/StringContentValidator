using System;
using System.Collections.Generic;
using System.Text;

namespace StringContentValidator
{
    /// <summary>
    /// Error settings from a validation rule.
    /// </summary>
    /// <typeparam name="TRow">Type to validate.</typeparam>
    internal interface IValidationRuleError<TRow>
    {
        /// <summary>
        /// Gets or sets a method to define a custom error message.
        /// <summary>
        Func<TRow, string> OverrideErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a method to define the final error message.
        /// <summary>
        Func<TRow, string> ErrorMessage { get; set; }
    }
}
