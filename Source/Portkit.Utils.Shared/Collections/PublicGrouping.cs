using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Portkit.Utils.Collections
{
    /// <summary>
    /// A class used to expose the Key property on a dynamically-created Linq grouping.
    /// The grouping will be generated as an internal class, so the Key property will not
    /// otherwise be available to data bind.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TElement">The type of the items.</typeparam>
    public class PublicGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IGrouping<TKey, TElement> _internalGrouping;

        /// <summary>
        /// Creates a new instance of the class.
        /// </summary>
        /// <param name="internalGrouping"></param>
        public PublicGrouping(IGrouping<TKey, TElement> internalGrouping)
        {
            _internalGrouping = internalGrouping;
        }

        #region IGrouping<TKey,TElement> Members

        /// <summary>
        /// Gets the grouping key.
        /// </summary>
        public TKey Key
        {
            get { return _internalGrouping.Key; }
        }

        /// <summary>
        ///  Returns an enumerator that iterates through the collection.
        /// </summary>
        public IEnumerator<TElement> GetEnumerator()
        {
            return _internalGrouping.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalGrouping.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Determents whether the specified object is equal to the current object.
        /// </summary>
        public override bool Equals(object obj)
        {
            var that = obj as PublicGrouping<TKey, TElement>;

            return (that != null) && (Key.Equals(that.Key));
        }

        /// <summary>
        /// Gets the instance hash code.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}
