using System;
using System.Collections.Generic;
using System.Text;

namespace StringContentValidator
{
    /// <summary>
    /// Class validator options.
    /// </summary>
    public class ClassValidatorOption : IClassValidatorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether Row index must be included in message.
        /// </summary>
        public bool ShowRowIndex { get; set; }
    }
}
