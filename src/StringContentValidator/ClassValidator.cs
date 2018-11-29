using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq.Expressions;
using System.Text;

namespace StringContentValidator
{
    /// <summary>
    /// Validation at class level.
    /// </summary>
    /// <typeparam name="TRow">The type of the model.</typeparam>
    public class ClassValidator<TRow>
    {
        private readonly IClassValidatorOptions options;

        private readonly List<PropertyValidator<TRow>> listValidator = new List<PropertyValidator<TRow>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassValidator{TRow}"/> class.
        /// Private constructor.
        /// </summary>
        private ClassValidator()
        {
        }

        private ClassValidator(IClassValidatorOptions options)
        {
            this.options = options;
        }

        /// <summary>
        /// Gets validation errors.
        /// </summary>
        public List<ValidationError> ValidationErrors { get; private set; } = new List<ValidationError>();

        /// <summary>
        /// Gets a value indicating whether the validation is successful or not.
        /// </summary>
        public bool IsValid
        {
            get { return this.ValidationErrors.Count == 0; }
        }

        /// <summary>
        /// Create an instance with no option.
        /// </summary>
        /// <returns>The created instance.</returns>
        public static ClassValidator<TRow> Init()
        {
            return new ClassValidator<TRow>();
        }

        /// <summary>
        /// Create an instance with option.
        /// </summary>
        /// <param name="options">options.</param>
        /// <returns>The created instance.</returns>
        public static ClassValidator<TRow> Init(IClassValidatorOptions options)
        {
            return new ClassValidator<TRow>(options);
        }

        /// <summary>
        /// Add a property validator. <see cref="PropertyValidator{TRow}"/>.
        /// </summary>
        /// <param name="propValidator">Targeted property validator.</param>
        /// <returns>current instance.</returns>
        public ClassValidator<TRow> AddProperty(PropertyValidator<TRow> propValidator)
        {
            this.listValidator.Add(propValidator);
            return this;
        }

        /// <summary>
        /// Validation for one row.
        /// </summary>
        /// <param name="row">Row to validate.</param>
        /// <returns>Current instance.</returns>
        public ClassValidator<TRow> Validate(TRow row)
        {
            foreach (var validator in this.listValidator)
            {
                validator.Validate(row);

                if (!validator.IsValid)
                {
                    this.ValidationErrors.AddRange(validator.ValidationErrors);
                }
            }

            return this;
        }

        /// <summary>
        /// Validation for rows collection.
        /// </summary>
        /// <param name="rows">rows collection to validate.</param>
        /// <returns>Current instance.</returns>
        public ClassValidator<TRow> ValidateList(IEnumerable<TRow> rows)
        {
            int index = 0;

            foreach (var row in rows)
            {
                foreach (var validator in this.listValidator)
                {
                    if (this.options != null && this.options.ShowRowIndex)
                    {
                        validator.SetRowIndex(index);
                    }

                    validator.Validate(row);

                    if (!validator.IsValid)
                    {
                        this.ValidationErrors.AddRange(validator.ValidationErrors);
                    }
                }

                index++;
            }

            return this;
        }
    }
}
