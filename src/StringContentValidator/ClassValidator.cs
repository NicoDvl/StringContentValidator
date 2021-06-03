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
        /// Protected constructor. Default options.
        /// </summary>
        protected ClassValidator()
        {
            this.options = new ClassValidatorOption();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassValidator{TRow}"/> class.
        /// Protected constructor.
        /// </summary>
        /// <param name="options">Validation options.</param>
        protected ClassValidator(IClassValidatorOptions options)
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
        /// Create an instance with default options.
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
        /// Add a property validator for the given property with validation rules.
        /// </summary>
        /// <param name="getterExpression">Expression for the targeted property.</param>
        /// <param name="validationRules">Delegate to add validation rules. Action may N validation rules.</param>
        /// <param name="overrideFieldName">To override field Name. By default uses the name of property.</param>
        /// <returns>current instance.</returns>
        public ClassValidator<TRow> For(Expression<Func<TRow, string>> getterExpression, Action<IPropertyValidatorAction<TRow>> validationRules, string overrideFieldName = null)
        {
            var prop = PropertyValidator<TRow>.For(getterExpression, overrideFieldName);
            validationRules(prop);
            this.listValidator.Add(prop);
            return this;
        }

        /// <summary>
        /// Add a property validator for the given property with validation rules.
        /// </summary>
        /// <param name="propertyExpression">Expression for the targeted property</param>
        /// <param name="getterExpression">Expression for the getter (if differnet from property).</param>
        /// <param name="validationRules">Delegate to add validation rules. Action may N validation rules.</param>
        /// <param name="overrideFieldName">To override field Name. By default uses the name of property.</param>
        /// <returns>current instance.</returns>
        public ClassValidator<TRow> For(Expression<Func<TRow, string>> propertyExpression, Expression<Func<TRow, string>> getterExpression, Action<IPropertyValidatorAction<TRow>> validationRules, string overrideFieldName = null)
        {
            var prop = PropertyValidator<TRow>.For(propertyExpression, getterExpression, overrideFieldName);
            validationRules(prop);
            this.listValidator.Add(prop);
            return this;
        }

        /// <summary>
        /// Add a property validator for the given property with validation rules only if predicate is true.
        /// </summary>
        /// <param name="getterExpression">Expression for the targeted property.</param>
        /// <param name="ifFunc">Condition as a predicate.</param>
        /// <param name="validationRules">Delegate to add validation rules. Action may N validation rules.</param>
        /// <param name="overrideFieldName">To override field Name. By default uses the name of property.</param>
        /// <returns>current instance.</returns>
        public ClassValidator<TRow> ForIf(Expression<Func<TRow, string>> getterExpression, Func<TRow, bool> ifFunc, Action<IPropertyValidatorAction<TRow>> validationRules, string overrideFieldName = null)
        {
            var prop = PropertyValidator<TRow>.For(getterExpression, overrideFieldName);
            prop.If(ifFunc, validationRules);
            this.listValidator.Add(prop);
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
            int index = this.options.RowIndexStartsAt;

            foreach (var row in rows)
            {
                foreach (var validator in this.listValidator)
                {
                    if (this.options.ShowRowIndex)
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

        /// <summary>
        /// Allow inherited classes to add property Validator.
        /// </summary>
        /// <param name="property">a property validator.</param>
        protected void AddPropertyValidator(PropertyValidator<TRow> property)
        {
            this.listValidator.Add(property);
        }
    }
}
