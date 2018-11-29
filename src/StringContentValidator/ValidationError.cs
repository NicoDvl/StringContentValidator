using System;
using System.Collections.Generic;
using System.Text;

namespace StringContentValidator
{
    /// <summary>
    /// Description of a validation error.
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationError"/> class.
        /// </summary>
        /// <param name="fieldName">Field name.</param>
        /// <param name="errorMessage">Error message.</param>
        private ValidationError(string fieldName, string errorMessage)
        {
            this.FieldName = fieldName;
            this.ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets fieldname for this error.
        /// </summary>
        public string FieldName { get; set; }

        /// <summary>
        /// Gets or sets error message for this error.
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Gets a value indicating whether success or not.
        /// </summary>
        public bool IsValid
        {
            get => string.IsNullOrEmpty(this.ErrorMessage);
        }

        /// <summary>
        /// Create a <see cref="ValidationError"/> with a failure status.
        /// </summary>
        /// <param name="fieldName">Field name for this error.</param>
        /// <param name="errorMessage">Error message for this field.</param>
        /// <returns>The created error.</returns>
        public static ValidationError Failure(string fieldName, string errorMessage)
        {
            return new ValidationError(fieldName, errorMessage);
        }
    }
}
