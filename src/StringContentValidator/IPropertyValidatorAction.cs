using System;
using System.Collections.Generic;
using System.Text;

namespace StringContentValidator
{
    /// <summary>
    /// Validations actions for a property validator.
    /// </summary>
    /// <typeparam name="TRow">Row to validate.</typeparam>
    public interface IPropertyValidatorAction<TRow>
    {
        /// <summary>
        /// Check if property value is not null.
        /// </summary>
        /// <returns>Current instance.</returns>
        PropertyValidator<TRow> IsNotNull();

        /// <summary>
        /// Check if property value is not null or empty.
        /// </summary>
        /// <returns>Current instance.</returns>
        PropertyValidator<TRow> IsNotNullOrEmpty();

        /// <summary>
        /// Check if property value is convertible to decimal.
        /// </summary>
        /// <returns>Current instance.</returns>
        PropertyValidator<TRow> TryParseDecimal();

        /// <summary>
        /// Check if property value is in list values.
        /// </summary>
        /// <param name="values">List of values.</param>
        /// <returns>Current instance.</returns>
        PropertyValidator<TRow> IsStringValues(string[] values);
    }
}