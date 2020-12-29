using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using StringContentValidator.Languages;

namespace StringContentValidator.Rules
{
    /// <summary>
    /// Encapsulate method to validate string.
    /// </summary>
    /// <typeparam name="TRow">type to validate.</typeparam>
    public class StringRule<TRow> : ValidationRule<TRow>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringRule{TRow}"/> class.
        /// </summary>
        /// <param name="value">delegate to get the value to validate.</param>
        public StringRule(Func<TRow, string> value)
            : base(value)
        {
        }

        /// <summary>
        /// Check if property value is not null.
        /// </summary>
        public void IsNotNull()
        {
            this.IsValid = (current) => this.Value(current) != null;
            this.ErrorMessage = (current) => Translation.IsNotNullError;
        }

        /// <summary>
        /// Check if property value is not null or empty.
        /// </summary>
        public void IsNotNullOrEmpty()
        {
            this.IsValid = (current) => !string.IsNullOrEmpty(this.Value(current));
            this.ErrorMessage = (current) => Translation.IsNotNullOrEmptyError;
        }

        /// <summary>
        /// Check if property value has required length.
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

            this.IsValid = (current) =>
            {
                int length = this.Value(current).Length;
                return length >= min || length <= max;
            };
            this.ErrorMessage = (current) => string.Format(Translation.HasLengthError, min, max);
        }

        /// <summary>
        /// Check if property value is in list of values.
        /// </summary>
        /// <param name="values">List of values.</param>
        public void IsStringValues(string[] values, IEqualityComparer<string> comparer = null)
        {
            this.IsValid = (current) => values.Contains(this.Value(current), comparer);
            this.ErrorMessage = (current) => string.Format(Translation.IsStringValuesError, this.Value(current));
        }

        /// <summary>
        /// Check if property value match regex.
        /// </summary>
        /// <param name="pattern">Regex pattern.</param>
        /// <param name="options">Regex options.</param>
        public void TryRegex(string pattern, RegexOptions options = RegexOptions.None)
        {
            this.IsValid = (current) =>
            {
                Regex regex = new Regex(pattern, options);
                return regex.IsMatch(this.Value(current));
            };
            this.ErrorMessage = (current) => string.Format(Translation.IsStringValuesError, this.Value(current));
        }
    }
}