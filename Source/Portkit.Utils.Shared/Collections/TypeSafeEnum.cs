using System;

namespace Portkit.Utils.Collections
{
    public abstract class TypeSafeEnum<T> : IEquatable<TypeSafeEnum<T>>, IComparable<TypeSafeEnum<T>>
        where T : IComparable<T>, IEquatable<T>
    {
        public T Value { get; }

        protected TypeSafeEnum(T value)
        {
            Value = value;
        }

        #region Operators

        public override bool Equals(object value)
        {
            var other = value as TypeSafeEnum<T>;
            return other != null && Equals(Value, other.Value);
        }

        public static bool operator ==(TypeSafeEnum<T> left, TypeSafeEnum<T> right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            return Equals(left, right);
        }

        public static bool operator !=(TypeSafeEnum<T> left, TypeSafeEnum<T> right)
        {
            return !(left == right);
        }

        #endregion

        #region IEquatable

        public bool Equals(TypeSafeEnum<T> other)
        {
            return other != null && Equals(Value, other.Value);
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        #endregion

        #region IComparable

        public int CompareTo(TypeSafeEnum<T> other)
        {
            return Value.CompareTo(other.Value);
        }

        #endregion
    }
}