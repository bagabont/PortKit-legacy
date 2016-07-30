using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using Windows.Foundation;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Represents a class for retrieving and storing settings.
    /// </summary>
    public class PortableStorage
    {
        #region Events

        /// <summary>
        /// Raised whenever a value is being changed.
        /// </summary>
        public event TypedEventHandler<object, string> ValueChanged;

        /// <summary>
        /// Fires the <see cref="ValueChanged"/> event.
        /// </summary>
        /// <param name="key">Key of the changed value.</param>
        protected virtual void OnValueChanged(string key)
        {
            ValueChanged?.Invoke(this, key);
        }

        #endregion

        #region Fields

        private readonly IDictionary<string, object> _storage;

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of elements contained in the PortableStorage.
        /// </summary>
        public int Count => _storage.Count;

        /// <summary>
        /// Gets or sets the element with the specified key.
        /// </summary>
        public object this[string key]
        {
            get
            {
                return _storage[key];
            }
            set
            {
                _storage[key] = value;
            }
        }

        /// <summary>
        /// Gets or sets whether the storage serializes the values to XML.
        /// </summary>
        public bool SerializeValues { get; set; }

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new instance of the <see cref="PortableStorage"/> class. 
        /// </summary>
        /// <param name="storage">The physical store space or a session dictionary where the settings would be stored.</param>
        public PortableStorage(IDictionary<string, object> storage)
        {
            if (storage == null)
            {
                throw new ArgumentNullException(nameof(storage));
            }
            _storage = storage;
            SerializeValues = true;
        }

        #endregion

        #region Methods

        /// <summary>
        /// When overridden saves key-value pairs from settings.
        /// </summary>
        public virtual void Save()
        {

        }

        /// <summary>
        /// When overridden loads key-value pairs into settings.
        /// </summary>
        public virtual void Load()
        {

        }

        /// <summary>
        /// Gets a value
        /// </summary>
        /// <typeparam name="T">Expected value type.</typeparam>
        /// <param name="key">Key under which the value is stored.</param>
        /// <returns>The property value.</returns>
        public T GetValue<T>([CallerMemberName] string key = null)
        {
            return InternalGetValue<T>(key);
        }

        /// <summary>
        /// Sets a value in the settings.
        /// </summary>
        /// <param name="value">Value to which the property is set to.</param>
        /// <param name="key">Key under which the value is stored.</param>
        public void SetValue(object value, [CallerMemberName] string key = null)
        {
            InternalSetValue(key, value);
        }

        /// <summary>
        /// Gets a value
        /// </summary>
        /// <typeparam name="T">Expected value type.</typeparam>
        /// <param name="keyExpression">Key name expression.</param>
        /// <returns>The property value.</returns>
        public T GetValue<T>(Expression<Func<T>> keyExpression)
        {
            if (keyExpression == null)
            {
                throw new ArgumentNullException(nameof(keyExpression));
            }
            return InternalGetValue<T>(PropertySupport.ExtractPropertyName(keyExpression));
        }

        /// <summary>
        /// Sets a value in the settings.
        /// </summary>
        /// <param name="value">Value to which the property is set to.</param>
        /// <param name="keyExpression">Key expression under which name the value is stored.</param>
        public void SetValue<T>(object value, Expression<Func<T>> keyExpression)
        {
            InternalSetValue(PropertySupport.ExtractPropertyName(keyExpression), value);
        }

        private void InternalSetValue(string key, object value)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key");
            }
            if (SerializeValues)
            {
                _storage[key] = value != null ? SerializeToString(value) : null;
            }
            else
            {
                _storage[key] = value;
            }
            OnValueChanged(key);
        }

        private T InternalGetValue<T>(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("key");
            }

            object data;
            if (!_storage.TryGetValue(key, out data))
            {
                return default(T);
            }

            if (SerializeValues)
            {
                if (data != null)
                {
                    return DeserializeFromString<T>((String)data);
                }
            }
            else
            {
                return (T)data;
            }

            return default(T);
        }

        private static string SerializeToString(object obj)
        {
            XmlSerializer serializer = new XmlSerializer(obj.GetType());
            using (StringWriter writer = new StringWriter())
            {
                serializer.Serialize(writer, obj);
                return writer.ToString();
            }
        }

        private static T DeserializeFromString<T>(string xml)
        {
            var deserializer = new XmlSerializer(typeof(T));
            using (var reader = new StringReader(xml))
            {
                return (T)deserializer.Deserialize(reader);
            }
        }

        #endregion
    }
}
