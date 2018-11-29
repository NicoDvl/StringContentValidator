using System;
using System.Collections.Generic;
using System.Text;

namespace StringContentValidator
{
    interface IMethodMessageError<TRow>
    {
        /// <summary>
        /// Gets or sets a method to define a custom error message.
        /// <summary>
        Func<TRow, string> OverrideErrorMessage { get; set; }

        /// <summary>
        /// Gets or sets a method to define the final error message.
        /// <summary>
        Func<TRow, string> ErrorMessage { get; set; }
    }
}
