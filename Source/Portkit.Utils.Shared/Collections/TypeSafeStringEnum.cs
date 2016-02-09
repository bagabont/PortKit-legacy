using System;

namespace Portkit.Utils.Collections
{
    /// <summary>
    /// Represents an abstract implementation of a type-safe enumerable, that stores string values.
    /// </summary>
    public abstract class TypeSafeStringEnum : IEquatable<TypeSafeStringEnum>, IComparable<TypeSafeStringEnum>
    {
        private readonly string _value;

        /// <summary>
        /// Creates a new instance of the <see cref="TypeSafeStringEnum"/> class.
        /// </summary>
        /// <param name="value">Enum string value.</param>
        protected TypeSafeStringEnum(string value)
        {
            _value = value;
        }

        /// <summary>
        /// Returns the string value representing the enumerable.
        /// </summary>
        /// <returns>String value of the enumerable.</returns>
        public override string ToString()
        {
            return _value;
        }

        #region Operators

        /// <summary>
        /// Compares two instances of <see cref="TypeSafeStringEnum"/>
        /// </summary>
        /// <param name="left">First instance</param>
        /// <param name="right">Second instance</param>
        /// <returns>True, if the underlying string value of the two instances is equal, otherwise false.</returns>
        public static bool operator ==(TypeSafeStringEnum left, TypeSafeStringEnum right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (((object)left == null) || ((object)right == null))
            {
                return false;
            }
            return left._value == right._value;
        }

        /// <summary>
        /// Compares two instances of <see cref="TypeSafeStringEnum"/>
        /// </summary>
        /// <param name="left">First instance</param>
        /// <param name="right">Second instance</param>
        /// <returns>True, if the underlying string value of the two instances is not equal, otherwise false.</returns>
        public static bool operator !=(TypeSafeStringEnum left, TypeSafeStringEnum right)
        {
            return !(left == right);
        }

        #endregion

        #region IEquatable

        /// <inheritdoc />     
        public override bool Equals(object other)
        {
            var otherObject = other as TypeSafeStringEnum;
            if ((object) otherObject == null)
            {
                return false;
            }
            return (_value == otherObject._value);
        }

        /// <inheritdoc />
        public bool Equals(TypeSafeStringEnum other)
        {
            if ((object)other == null)
            {
                return false;
            }
            return (_value == other._value);
        }

        /// <summary>
        /// Returns the hash code of the enum's string value.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        #endregion

        #region IComparable

        /// <summary>
        /// Compares two specified <see cref="TypeSafeStringEnum"/> objects.
        /// </summary>
        public int CompareTo(TypeSafeStringEnum other)
        {
            return string.Compare(_value, (other)._value, StringComparison.Ordinal);
        }

        #endregion
    }
}