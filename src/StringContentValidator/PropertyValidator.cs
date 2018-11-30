using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using StringContentValidator.Languages;
using StringContentValidator.Methods;
using StringContentValidator.Utilities;

namespace StringContentValidator
{
    /// <summary>
    /// Define a validator for a property in <see cref="PropertyValidator{TRow}"/> type.
    /// </summary>
    /// <typeparam name="TRow">Type for this validator.</typeparam>
    public class PropertyValidator<TRow> : IPropertyValidatorAction<TRow>
    {
        private readonly Collection<ValidationRule<TRow>> validationRules = new Collection<ValidationRule<TRow>>();
        private int? rowIndex;
        private string fieldName;
        private Func<TRow, string> getter;
        private Expression getterExpression;
        private dynamic extraObject;
        private bool preserveErrorHeader = true;

        private PropertyValidator()
        {
        }

        /// <summary>
        /// Gets the list of validation errors.
        /// </summary>
        public List<ValidationError> ValidationErrors { get; private set; } = new List<ValidationError>();

        /// <summary>
        /// Gets or sets a value indicating whether the validation of the property must stop on first error.
        /// </summary>
        public bool StopOnFirstFailure { get; set; } = true;

        /// <summary>
        /// Gets a value indicating whether the validation is successfull.
        /// </summary>
        public bool IsValid
        {
            get { return this.ValidationErrors.Count == 0; }
        }

        /// <summary>
        /// Creates a property validator for the given property.
        /// </summary>
        /// <param name="getterExpression">Expression for the targeted property.</param>
        /// <param name="overrideFieldName">To override field Name. By default uses the name of property.</param>
        /// <returns>A property validator.</returns>
        public static PropertyValidator<TRow> For(Expression<Func<TRow, string>> getterExpression, string overrideFieldName = null)
        {
            PropertyValidator<TRow> prop = new PropertyValidator<TRow>();
            prop.getter = getterExpression.Compile();
            prop.getterExpression = getterExpression;
            prop.fieldName = overrideFieldName != null ? overrideFieldName : ExpressionUtiities.PropertyName(getterExpression);
            return prop;
        }

        /// <summary>
        /// Creates a property validator for a dynamic property.
        /// </summary>
        /// <param name="getter">The targeted dynamic property.</param>
        /// <param name="fieldName">Fieldname for the dynamic property.</param>
        /// <returns>A property validator.</returns>
        public static PropertyValidator<dynamic> ForDynamic(Func<dynamic, string> getter, string fieldName)
        {
            PropertyValidator<dynamic> prop = new PropertyValidator<dynamic>();
            prop.getter = getter;
            prop.fieldName = fieldName;
            return prop;
        }

        /// <summary>
        /// Allow to set a row index. Usefull to display a line number in error message.
        /// </summary>
        /// <param name="rowIndex">Row index.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> SetRowIndex(int rowIndex)
        {
            this.rowIndex = rowIndex;
            return this;
        }

        /// <summary>
        /// Allow to set an object to pass an extra object to the validator.
        /// </summary>
        /// <param name="extraObject">Extra dynamic object.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> SetExtraObject(dynamic extraObject)
        {
            this.extraObject = extraObject;
            return this;
        }

        /// <summary>
        /// Allow overriding default error message.
        /// </summary>
        /// <param name="msgErrorFunc">Custom error message as a Func.</param>
        /// <param name="preserveErrorHeader">Preserve line, field name header in error message.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> OverrideErrorMessage(Func<TRow, string> msgErrorFunc, bool preserveErrorHeader = false)
        {
            this.preserveErrorHeader = preserveErrorHeader;
            this.validationRules.Last().OverrideErrorMessage = (current) => msgErrorFunc(current);
            return this;
        }

        /// <summary>
        /// Check if property is not null.
        /// </summary>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> IsNotNull()
        {
            StringMethods<TRow> method = new StringMethods<TRow>((x) => this.getter(x));
            method.IsNotNull();
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property is not null or empty.
        /// </summary>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> IsNotNullOrEmpty()
        {
            StringMethods<TRow> method = new StringMethods<TRow>((x) => this.getter(x));
            method.IsNotNullOrEmpty();
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property is convertible to decimal.
        /// </summary>
        /// <param name="provider">An object that supplies culture-specific parsing information. Default CurrentCulture.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> TryParseDecimal(IFormatProvider provider = null)
        {
            DecimalMethods<TRow> method = new DecimalMethods<TRow>((x) => this.getter(x));
            method.TryParseDecimal(provider);
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property is convertible to DateTime.
        /// </summary>
        /// <param name="format">specified format.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> TryParseDateTime(string format)
        {
            DateTimeMethods<TRow> method = new DateTimeMethods<TRow>((x) => this.getter(x));
            method.TryParseDateTime(format);
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property has required length.
        /// </summary>
        /// <param name="min">min length.</param>
        /// <param name="max">maw length.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> HasLength(int min, int max)
        {
            StringMethods<TRow> method = new StringMethods<TRow>((x) => this.getter(x));
            method.HasLength(min, max);
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property is in list values.
        /// </summary>
        /// <param name="values">List of values.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> IsStringValues(string[] values)
        {
            StringMethods<TRow> method = new StringMethods<TRow>((x) => this.getter(x));
            method.IsStringValues(values);
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property value match regex.
        /// </summary>
        /// <param name="pattern">Regex pattern.</param>
        /// <param name="options">Regex options.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> TryRegex(string pattern, RegexOptions options = RegexOptions.None)
        {
            StringMethods<TRow> method = new StringMethods<TRow>((x) => this.getter(x));
            method.TryRegex(pattern, options);
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Add a condition on a validation action of <see cref="IPropertyValidatorAction"/>.
        /// </summary>
        /// <param name="ifFunc">Condition as a predicate.</param>
        /// <param name="action">Delegate to add validation rules. Action may N validation rules.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> If(Func<TRow, bool> ifFunc, Action<IPropertyValidatorAction<TRow>> action)
        {
            // track method count before and after to link condition on all new methods.
            var before = this.validationRules.Count;
            action(this);
            var after = this.validationRules.Count;
            var nbAdded = after - before;

            // take last N methods
            var newValidationRules = this.validationRules.Skip(Math.Max(0, after - nbAdded));

            // add condition on new methods
            foreach (var rule in newValidationRules)
            {
                rule.PreCondition = (current) => ifFunc(current);
            }

            return this;
        }

        /// <summary>
        /// Add a condition on a custom validation action.
        /// </summary>
        /// <param name="predicate">Pre condition as a predicate.</param>
        /// <param name="isValid">Delegate to validate the current row.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> If(Func<TRow, bool> predicate, Func<TRow, bool> isValid)
        {
            ValidationRule<TRow> method = new ValidationRule<TRow>((x) => this.getter(x));
            method.PreCondition = (current) => predicate(current);
            method.IsValid = (current) => isValid(current);
            method.ErrorMessage = (current) => Translation.IfError;
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Add a condition on a custom validation action.
        /// </summary>
        /// <param name="predicate">Condition as a predicate.</param>
        /// <param name="isValid">Delegate to validate the current row.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> If(Func<TRow, bool> predicate, Func<TRow, dynamic, bool> isValid)
        {
            ValidationRule<TRow> method = new ValidationRule<TRow>((x) => this.getter(x));
            method.PreCondition = (current) => predicate(current);
            method.IsValid = (current) => isValid(current, this.extraObject);
            method.ErrorMessage = (current) => Translation.IfError;
            this.validationRules.Add(method);
            return this;
        }

        /// <summary>
        /// Validate the property with the current row.
        /// </summary>
        /// <param name="row">Current row.</param>
        public void Validate(TRow row)
        {
            this.ValidationErrors = new List<ValidationError>();

            foreach (var validationRule in this.validationRules)
            {
                bool result = true;
                if (validationRule.PreCondition != null)
                {
                    // used by condition like .If(x => x.Type == "P", action:
                    result = validationRule.PreCondition(row);
                }

                if (result)
                {
                    bool ok = validationRule.IsValid(row);
                    if (!ok)
                    {
                        this.ValidationErrors.Add(ValidationError.Failure(this.fieldName, this.MessageErrorFactory(row, validationRule)));

                        // TODO at this stage always break on first error
                        // if (this.StopOnFirstFailure)
                        //// {
                        break;
                        //// }
                    }
                }
            }
        }

        /// <summary>
        /// Create an error message.
        /// </summary>
        /// <param name="current">Current element.</param>
        /// <param name="messageError">Error informations from method. <see cref="IValidationRuleError{TRow}"/>.</param>
        /// <returns>Error message.</returns>
        private string MessageErrorFactory(TRow current, IValidationRuleError<TRow> messageError)
        {
            StringBuilder errorMsg = new StringBuilder();

            // when overriden error message suppress header by default
            if (this.preserveErrorHeader)
            {
                if (this.rowIndex.HasValue)
                {
                    errorMsg.Append($"{Translation.Row} {this.rowIndex} ");
                }

                errorMsg.Append($"{Translation.Field} {this.fieldName} : ");
            }

            // default or override msg
            if (messageError.OverrideErrorMessage == null)
            {
                errorMsg.Append(messageError.ErrorMessage(current));
            }
            else
            {
                errorMsg.Append(messageError.OverrideErrorMessage(current));
            }

            return errorMsg.ToString();
        }
    }
}
