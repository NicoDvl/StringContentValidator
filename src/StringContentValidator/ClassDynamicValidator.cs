using System;
using System.Collections.Generic;
using System.Text;

namespace StringContentValidator
{
    /// <summary>
    /// Specific implementation for dynamic object.
    /// </summary>
    public class ClassDynamicValidator : ClassValidator<dynamic>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDynamicValidator"/> class.
        /// </summary>
        protected ClassDynamicValidator()
            : base()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ClassDynamicValidator"/> class.
        /// </summary>
        /// <param name="options">Validation options.</param>
        protected ClassDynamicValidator(IClassValidatorOptions options)
            : base(options)
        {
        }

        /// <summary>
        /// Create an instance with option.
        /// </summary>
        /// <returns>The created instance.</returns>
        public static new ClassDynamicValidator Init()
        {
            return new ClassDynamicValidator();
        }

        /// <summary>
        /// Create an instance with option.
        /// </summary>
        /// <param name="options">options.</param>
        /// <returns>The created instance.</returns>
        public static new ClassDynamicValidator Init(IClassValidatorOptions options)
        {
            return new ClassDynamicValidator();
        }

        /// <summary>
        /// Add a property validator for the given property with validation rules.
        /// </summary>
        /// <param name="getter">delegate for the targeted property.</param>
        /// <param name="fieldName">To override field Name. By default uses the name of property.</param>
        /// <param name="validationRules">Delegate to add validation rules. Action may N validation rules.</param>
        /// <returns>current instance.</returns>
        public ClassDynamicValidator For(Func<dynamic, string> getter, string fieldName, Action<IPropertyValidatorAction<dynamic>> validationRules)
        {
            var prop = PropertyValidator<dynamic>.ForDynamic(getter, fieldName);
            validationRules(prop);
            this.AddPropertyValidator(prop);
            return this;
        }
    }
}
