using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StringContentValidator.Languages;

namespace StringContentValidator.Methods
{
    /// <summary>
    /// Encapsulate method to validate string.
    /// </summary>
    /// <typeparam name="TRow">type to validate.</typeparam>
    public class StringMethods<TRow> : MethodValidator<TRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringMethods{TRow}"/> class.
        /// </summary>
        /// <param name="value">delegate to get the value to validate.</param>
        public StringMethods(Func<TRow, string> value)
            : base(value)
        {
        }

        /// <summary>
        /// Check if property is not null.
        /// </summary>
        public void IsNotNull()
        {
            this.ToCheck = (current) => this.Value(current) != null;
            this.ErrorMessage = (current) => Translation.IsNotNullError;
        }

        /// <summary>
        /// Check if property is not null or empty.
        /// </summary>
        public void IsNotNullOrEmpty()
        {
            this.ToCheck = (current) => !string.IsNullOrEmpty(this.Value(current));
            this.ErrorMessage = (current) => Translation.IsNotNullOrEmptyError;
        }

        /// <summary>
        /// Check if property has required length.
        /// </summary>
        /// <param name="min">min length.</param>
        /// <param name="max">maw length.</param>
        public void HasLength(int min, int max)
        {
            if (min < 0 || max < 0)
            {
                throw new ArgumentOutOfRangeException("Min Max", "Min and Max can't be negative.");
            }

            if (max < min)
            {
                throw new ArgumentOutOfRangeException("Min Max", "Max can't be smaller than min.");
            }

            this.ToCheck = (current) =>
            {
                int length = this.Value(current).Length;
                return length >= min || length <= max;
            };
            this.ErrorMessage = (current) => string.Format(Translation.HasLengthError, min, max);
        }

        /// <summary>
        /// Check if property is in list values.
        /// </summary>
        /// <param name="values">List of values.</param>
        public void IsStringValues(string[] values)
        {
            this.ToCheck = (current) => values.Contains(this.Value(current));
            this.ErrorMessage = (current) => string.Format(Translation.IsStringValuesError, this.Value(current));
        }
    }
}