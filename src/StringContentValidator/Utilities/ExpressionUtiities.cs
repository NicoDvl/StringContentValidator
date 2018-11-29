using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace StringContentValidator.Utilities
{
    /// <summary>
    /// Utilities for lambda expression.
    /// </summary>
    public class ExpressionUtiities
    {
        /// <summary>
        /// Get property name from lambda expression.
        /// </summary>
        /// <typeparam name="T">Generix type.</typeparam>
        /// <param name="expression">A strongly typed lambda expression as a data structure in the form.</param>
        /// <returns>The property name.</returns>
        public static string PropertyName<T>(Expression<Func<T, string>> expression)
        {
            var body = expression.Body as MemberExpression;

            if (body == null)
            {
                body = ((UnaryExpression)expression.Body).Operand as MemberExpression;
            }

            return body.Member.Name;
        }
    }
}