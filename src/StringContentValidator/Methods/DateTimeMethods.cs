using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StringContentValidator.Languages;

namespace StringContentValidator.Methods
{
    /// <summary>
    /// Encapsulate method to validate conversion to DateTime type.
    /// </summary>
    /// <typeparam name="TRow">type to validate.</typeparam>
    public class DateTimeMethods<TRow> : MethodValidator<TRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DateTimeMethods{TRow}"/> class.
        /// </summary>
        /// <param name="value">delegate to get the value to validate.</param>
        public DateTimeMethods(Func<TRow, string> value)
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

            this.ToCheck = (current) =>
            {
                DateTime d;
                bool ok = DateTime.TryParseExact(this.Value(current), format, CultureInfo.CurrentCulture, DateTimeStyles.None, out d);
                return ok;
            };
            this.ErrorMessage = (current) => string.Format(Translation.IsDateTimeError, this.Value(current), format);
        }
    }
}
