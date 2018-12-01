using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StringContentValidator.Languages;

namespace StringContentValidator.Rules
{
    /// <summary>
    /// Encapsulate method to validate conversion to DateTime type.
    /// </summary>
    /// <typeparam name="TRow">type to validate.</typeparam>
    public class DateTimeRule<TRow> : ValidationRule<TRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeRule{TRow}"/> class.
        /// </summary>
        /// <param name="value">delegate to get the value to validate.</param>
        public DateTimeRule(Func<TRow, string> value)
            : base(value)
        {
        }

        /// <summary>
        /// Check if property is convertible to decimal.
        /// </summary>
        /// <param name="format">specified format.</param>
        public void TryParseDateTime(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException(nameof(format), "format can't be null or empty.");
            }

            this.IsValid = (current) =>
            {
                DateTime d;
                bool ok = DateTime.TryParseExact(this.Value(current), format, CultureInfo.CurrentCulture, DateTimeStyles.None, out d);
                return ok;
            };
            this.ErrorMessage = (current) => string.Format(Translation.IsDateTimeError, this.Value(current), format);
        }
    }
}
