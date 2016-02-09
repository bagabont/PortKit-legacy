using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Represents a class for property validation.
    /// </summary>
    public class PropertyValidation<T> : IPropertyValidation
    {
        #region Fields

        private const string DEFAULT_VALIDATION_ERROR = "Invalid data.";
        private readonly List<Func<bool>> _validationConditions = new List<Func<bool>>();

        #endregion

        #region Properties

        /// <inheritdoc />
        public string PropertyName { get; private set; }

        /// <inheritdoc />
        public string ErrorMessage { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Creates new instance of the PropertyValidation class.
        /// </summary>
        /// <param name="expression">Property validation expression.</param>
        public PropertyValidation(Expression<Func<T>> expression)
        {
            PropertyName = GetPropertyName(expression);
            ErrorMessage = DEFAULT_VALIDATION_ERROR;
        }

        /// <summary>
        /// Sets the validation condition.
        /// </summary>
        /// <param name="condition">Validation contrition.</param>
        public PropertyValidation<T> When(Func<bool> condition)
        {
            _validationConditions.Add(condition);
            return this;
        }

        /// <summary>
        /// Sets the error message that describes the validation failure.
        /// </summary>
        /// <param name="message">Property validation failure message.</param>
        public PropertyValidation<T> OnError(string message)
        {
            if (ErrorMessage != null && ErrorMessage != DEFAULT_VALIDATION_ERROR)
            {
                throw new InvalidOperationException("Error message can be set only once.");
            }
            ErrorMessage = message;
            return this;
        }

        /// <summary>
        /// Checks if the property is invalid.
        /// </summary>
        /// <returns>True if the validation failed, else false.</returns>
        public bool IsInvalid()
        {
            if (_validationConditions.Count == 0)
            {
                throw new InvalidOperationException("Cannot complete validation without any conditions provided.");
            }
            return _validationConditions.Any(f => !f());
        }

        private static string GetPropertyName(Expression<Func<T>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException("expression");
            }
            MemberExpression memberExpression;
            var body = expression.Body as UnaryExpression;
            if (body != null)
            {
                memberExpression = body.Operand as MemberExpression;
            }
            else
            {
                memberExpression = expression.Body as MemberExpression;
            }

            if (memberExpression == null)
            {
                throw new ArgumentException("The expression is not a member access expression", "expression");
            }
            var property = memberExpression.Member as PropertyInfo;
            if (property == null)
            {
                throw new ArgumentException("The member access expression does not access a property", "expression");
            }
#if UNIVERSAL
            if (property.GetGetMethod().IsStatic)
#else
            if (property.GetMethod.IsStatic)
#endif
            {
                throw new ArgumentException("The referenced property is a static property", "expression");
            }
            return memberExpression.Member.Name;
        }

        #endregion
    }
}
