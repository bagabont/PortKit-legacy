namespace Portkit.ComponentModel
{
    /// <summary>
    /// Represents a property validation interface.
    /// </summary>
    public interface IPropertyValidation
    {
        /// <summary>
        /// Gets the property name.
        /// </summary>
        string PropertyName { get; }

        /// <summary>
        /// Gets the property validation error.
        /// </summary>
        string ErrorMessage { get; }

        /// <summary>
        /// Checks if the property is invalid.
        /// </summary>
        /// <returns>True if the validation failed, else false.</returns>
        bool IsInvalid();
    }
}
