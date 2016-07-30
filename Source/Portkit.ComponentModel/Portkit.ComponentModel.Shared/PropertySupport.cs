using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Expression extensions class.
    /// </summary>
    public static class PropertySupport
    {
        /// <summary>
        /// Gets property name from a property expression.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="propertyExpression">Source property expression</param>
        /// <returns>Property name.</returns>
        public static string ExtractPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentNullException(nameof(propertyExpression));
            }
            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
            {
                throw new ArgumentException("Invalid argument", nameof(propertyExpression));
            }
            var property = body.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("Argument is not a property", nameof(propertyExpression));
            }
            return property.Name;
        }

    }
}
