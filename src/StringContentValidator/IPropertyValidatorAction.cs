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
        /// Check if property is convertible to decimal.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific parsing information. Default CurrentCulture.</param>
        /// <returns>Current instance.</returns>
        PropertyValidator<TRow> TryParseDecimal(IFormatProvider provider = null);

        /// <summary>
        /// Check if property is convertible to DateTime.
        /// </summary>
        /// <param name="format">specified format.</param>
        /// <returns>Current instance.</returns>
        PropertyValidator<TRow> TryParseDateTime(string format);

        /// <summary>
        /// Check if property value is in list values.
        /// </summary>
        /// <param name="values">List of values.</param>
        /// <returns>Current instance.</returns>
        PropertyValidator<TRow> IsStringValues(string[] values, IEqualityComparer<string> comparer = null);
    }
}