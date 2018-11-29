using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StringContentValidator.Languages;

namespace StringContentValidator.Methods
{
    /// <summary>
    /// Encapsulate method to validate conversion to decimal type.
    /// </summary>
    /// <typeparam name="TRow">type to validate.</typeparam>
    public class DecimalMethods<TRow> : MethodValidator<TRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DecimalMethods{TRow}"/> class.
        /// </summary>
        /// <param name="value">delegate to get the value to validate.</param>
        public DecimalMethods(Func<TRow, string> value)
            : base(value)
        {
        }

        /// <summary>
        /// Check if property is convertible to decimal.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific parsing information. Default CurrentCulture.</param>
        public void TryParseDecimal(IFormatProvider provider = null)
        {
            if (provider == null)
            {
                provider = CultureInfo.CurrentCulture;
            }

            this.ToCheck = (current) =>
            {
                decimal d;
                bool ok = decimal.TryParse(this.Value(current), NumberStyles.AllowDecimalPoint, provider, out d);
                return ok;
            };
            this.ErrorMessage = (current) => string.Format(Translation.IsDecimalError, this.Value(current));
        }
    }
}
