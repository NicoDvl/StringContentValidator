using System;
using System.Collections.Generic;
using System.Text;

namespace StringContentValidator
{
    /// <summary>
    /// Interface for class validator otptions.
    /// </summary>
    public interface IClassValidatorOptions
    {
        /// <summary>
        /// Gets or sets a value indicating whether display row index.
        /// </summary>
        bool ShowRowIndex { get; set; }

        /// <summary>
        /// Gets or sets a value indicating row index start.
        /// </summary>
        int RowIndexStartsAt { get; set; }
    }
}