namespace Portkit.Core.Net
{
    /// <summary>
    /// Base class to store key-value pair of a query parameter.
    /// </summary>
    public sealed class HttpValue
    {
        /// <summary>
        /// Creates a new instance of the <see cref="HttpValue"/> class.
        /// </summary>
        public HttpValue()
        {
        }

        /// <summary>
        /// Creates a new instance of the <see cref="HttpValue"/> class.
        /// </summary>
        /// <param name="key">Key of the instance.</param>
        /// <param name="value">Value of the instance.</param>
        public HttpValue(string key, string value)
        {
            Key = key;
            Value = value;
        }

        /// <summary>
        /// Gets or sets the key of the item.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the value of the item.
        /// </summary>
        public string Value { get; set; }
    }
}