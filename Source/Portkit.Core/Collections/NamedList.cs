using System.Collections.Generic;

namespace Portkit.Core.Collections
{
    /// <summary>
    /// Represents a list that has a name.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NamedList<T> : List<T>
    {
        /// <summary>
        /// Gets or sets the list's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Creates a new instance of the <see cref="NamedList{T}"/> class.
        /// </summary>
        /// <param name="name">List name.</param>
        /// <param name="items">List items.</param>
        public NamedList(string name, IEnumerable<T> items)
            : base(items)
        {
            Name = name;
        }
    }
}
