using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using StringContentValidator.Languages;
using StringContentValidator.Utilities;

namespace StringContentValidator
{
    /// <summary>
    /// Define a validator for a property in <see cref="PropertyValidator{TRow}"/> type.
    /// </summary>
    /// <typeparam name="TRow">Type for this validator.</typeparam>
    public class PropertyValidator<TRow> : IPropertyValidatorAction<TRow>
    {
        private readonly Collection<MethodValidator<TRow>> methods = new Collection<MethodValidator<TRow>>();
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
        /// Gets a value indicating whether the validation is successfull.
        /// </summary>
        public bool IsValid
        {
            get { return this.ValidationErrors.Count == 0; }
        }

        /// <summary>
        /// Creates a property validator for the given property.
        /// </summary>
        /// <param name="getterExpression">The targeted property.</param>
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
            this.methods.Last().OverrideErrorMessage = (current, extra) => msgErrorFunc(current);
            return this;
        }

        /// <summary>
        /// Allow overriding default error message.
        /// </summary>
        /// <param name="msgErrorFunc">
        /// Custom error message as a Func.
        /// TRow as the current row
        /// dyanmic as the extra object instance.
        /// </param>
        /// <param name="preserveErrorHeader">Preserve line, field name header in error message.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> OverrideErrorMessage(Func<TRow, dynamic, string> msgErrorFunc, bool preserveErrorHeader = false)
        {
            this.preserveErrorHeader = preserveErrorHeader;
            this.methods.Last().OverrideErrorMessage = (current, extra) => msgErrorFunc(current, extra);
            return this;
        }

        /// <summary>
        /// Check if property is not null.
        /// </summary>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> IsNotNull()
        {
            MethodValidator<TRow> method = new MethodValidator<TRow>();
            method.ToCheck = (current) => this.getter(current) != null;
            method.ErrorMessage = (current) => this.MessageErrorFactory(current, Translation.IsNotNullError, method.OverrideErrorMessage);
            this.methods.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property is not null or empty.
        /// </summary>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> IsNotNullOrEmpty()
        {
            MethodValidator<TRow> method = new MethodValidator<TRow>();
            method.ToCheck = (current) => !string.IsNullOrEmpty(this.getter(current));
            method.ErrorMessage = (current) => this.MessageErrorFactory(current, Translation.IsNotNullOrEmptyError, method.OverrideErrorMessage);
            this.methods.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property is convertible to decimal.
        /// </summary>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> TryParseDecimal()
        {
            MethodValidator<TRow> method = new MethodValidator<TRow>();
            method.ToCheck = (current) =>
            {
                decimal d;
                bool ok = decimal.TryParse(this.getter(current), NumberStyles.AllowDecimalPoint, CultureInfo.CurrentCulture, out d);
                return ok;
            };
            method.ErrorMessage = (current) => this.MessageErrorFactory(current, string.Format(Translation.IsDecimalError, this.getter(current)), method.OverrideErrorMessage);
            this.methods.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property is convertible to DateTime.
        /// </summary>
        /// <param name="format">specified format.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> TryParseDateTime(string format)
        {
            if (string.IsNullOrEmpty(format))
            {
                throw new ArgumentNullException(nameof(format), "format can't be null or empty.");
            }

            MethodValidator<TRow> method = new MethodValidator<TRow>();
            method.ToCheck = (current) =>
            {
                DateTime d;
                bool ok = DateTime.TryParseExact(this.getter(current), format, CultureInfo.CurrentCulture, DateTimeStyles.None, out d);
                return ok;
            };
            method.ErrorMessage = (current) => this.MessageErrorFactory(current, string.Format(Translation.IsDateTimeError, this.getter(current), format), method.OverrideErrorMessage);
            this.methods.Add(method);
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
            if (min < 0 || max < 0)
            {
                throw new ArgumentOutOfRangeException("Min Max", "Min and Max can't be negative.");
            }

            if (max < min)
            {
                throw new ArgumentOutOfRangeException("Min Max", "Max can't be smaller than min.");
            }

            MethodValidator<TRow> method = new MethodValidator<TRow>();
            method.ToCheck = (current) =>
            {
                int length = this.getter(current).Length;
                return length >= min || length <= max;
            };
            method.ErrorMessage = (current) => this.MessageErrorFactory(current, string.Format(Translation.HasLengthError, min, max), method.OverrideErrorMessage);
            this.methods.Add(method);
            return this;
        }

        /// <summary>
        /// Check if property is in list values.
        /// </summary>
        /// <param name="values">List of values.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> IsStringValues(string[] values)
        {
            MethodValidator<TRow> method = new MethodValidator<TRow>();
            method.ToCheck = (current) => values.Contains(this.getter(current));
            method.ErrorMessage = (current) => this.MessageErrorFactory(current, string.Format(Translation.IsStringValuesError, this.getter(current)), method.OverrideErrorMessage);
            this.methods.Add(method);
            return this;
        }

        /// <summary>
        /// Add a condition on a validation action of <see cref="IPropertyValidatorAction"/>.
        /// </summary>
        /// <param name="ifFunc">Condition as a predicate.</param>
        /// <param name="action">
        /// Validation action from <see cref="IPropertyValidatorAction{TRow}"/> interface.
        /// Action may add 1 to N methods.
        /// </param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> If(Func<TRow, bool> ifFunc, Action<IPropertyValidatorAction<TRow>> action)
        {
            // track method count before and after to link condition on all new methods.
            var before = this.methods.Count;
            action(this);
            var after = this.methods.Count;
            var nbAdded = after - before;

            // take last N methods
            var newMethods = this.methods.Skip(Math.Max(0, after - nbAdded));

            // add condition on new methods
            foreach (var item in newMethods)
            {
                item.Condition = (current) => ifFunc(current);
            }

            return this;
        }

        /// <summary>
        /// Add a condition on a custom validation action.
        /// </summary>
        /// <param name="predicate">Condition as a predicate.</param>
        /// <param name="tocheck">tocheck if condition is ok. TRow as the current row.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> If(Func<TRow, bool> predicate, Func<TRow, bool> tocheck)
        {
            MethodValidator<TRow> method = new MethodValidator<TRow>();
            method.Condition = (current) => predicate(current);
            method.ToCheck = (current) => tocheck(current);
            method.ErrorMessage = (current) => this.MessageErrorFactory(current, Translation.IfError, method.OverrideErrorMessage);
            this.methods.Add(method);
            return this;
        }

        /// <summary>
        /// Add a condition on a custom validation action.
        /// </summary>
        /// <param name="predicate">Condition as a predicate.</param>
        /// <param name="tocheck">todo if condition is ok.</param>
        /// <returns>Current instance.</returns>
        public PropertyValidator<TRow> If(Func<TRow, bool> predicate, Func<TRow, dynamic, bool> tocheck)
        {
            MethodValidator<TRow> method = new MethodValidator<TRow>();
            method.Condition = (current) => predicate(current);
            method.ToCheck = (current) => tocheck(current, this.extraObject);
            method.ErrorMessage = (current) => this.MessageErrorFactory(current, Translation.IfError, method.OverrideErrorMessage);
            this.methods.Add(method);
            return this;
        }

        /// <summary>
        /// Validate the property with the current row.
        /// </summary>
        /// <param name="row">Current row.</param>
        public void Validate(TRow row)
        {
            this.ValidationErrors = new List<ValidationError>();

            foreach (var method in this.methods)
            {
                bool condition = true;
                if (method.Condition != null)
                {
                    condition = method.Condition(row);
                }

                if (condition)
                {
                    bool ok = method.ToCheck(row);
                    if (!ok)
                    {
                        this.ValidationErrors.Add(ValidationError.Failure(this.fieldName, method.ErrorMessage(row)));

                        break; // by default breaks if error
                    }
                }
            }
        }

        /// <summary>
        /// Create an error message.
        /// </summary>
        /// <param name="current">Current element.</param>
        /// <param name="defaultMsg">Default error message.</param>
        /// <param name="msgOverrideFunc">Func to customize error message.</param>
        /// <returns>Error message.</returns>
        private string MessageErrorFactory(TRow current, string defaultMsg, Func<TRow, dynamic, string> msgOverrideFunc)
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
            if (msgOverrideFunc == null)
            {
                errorMsg.Append(defaultMsg);
            }
            else
            {
                errorMsg.Append(msgOverrideFunc(current, this.extraObject));
            }

            return errorMsg.ToString();
        }
    }
}
