using Portkit.Core.Collections;
using Portkit.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Represents a fast and easy to use IoC service container, that implements <see cref="IServiceProvider"/>.
    /// </summary>
    public class PortableContainer : IServiceProvider, IDisposable
    {
        #region Fields

        private static readonly object SyncLock = new object();
        private static PortableContainer _default;

        private readonly GroupedEnumerable<Type, Type> _componentsRegister = new GroupedEnumerable<Type, Type>();
        private readonly Dictionary<Type, object> _instancesRegister = new Dictionary<Type, object>();
        private readonly HashSet<KeyValuePair<Type, Type>> _transientsRegister = new HashSet<KeyValuePair<Type, Type>>();

        #endregion

        /// <summary>
        /// Gets the default instance of the IoC service container.
        /// </summary>
        public static PortableContainer Default
        {
            get
            {
                if (_default == null)
                {
                    lock (SyncLock)
                    {
                        if (_default == null)
                        {
                            _default = new PortableContainer();
                        }
                    }
                }

                return _default;
            }
        }

        /// <summary>
        /// Registers a component to the container and specifies it's implementation.
        /// </summary>
        /// <param name="componenType">Type of the component.</param>
        /// <param name="implementationType">Optional: Type of the component implementation, which will be used for instantiations.</param>
        /// <param name="instance">Optional: Component instance.</param>
        public void Register(Type componenType, Type implementationType = null, object instance = null)
        {
            if (componenType == null)
            {
                throw new ArgumentNullException("Component type cannot be null.");
            }

            // If no explicit implementation is defined, used the component type
            if (implementationType == null)
            {
                implementationType = componenType;
            }

            // add component to registry
            _componentsRegister.Add(componenType, implementationType);

            if (instance == null)
            {
                return;
            }

            bool isAssignable = componenType.GetTypeInfo().IsAssignableFrom(instance.GetType().GetTypeInfo());
            if (!isAssignable)
            {
                string errorMsg = String.Format("Instance of type '{0}' cannot be cast to component type '{1}'",
                    instance.GetType(), componenType);
                throw new InvalidCastException(errorMsg);
            }
            _instancesRegister.Add(implementationType, instance);
        }

        /// <summary>
        /// Registers a component instance to the container and specifies it's implementation.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component.</typeparam>
        /// <typeparam name="TImplementation">Type of the component implementation.</typeparam>
        /// <param name="instance">Instance.</param>
        public void Register<TComponent, TImplementation>(TComponent instance)
            where TComponent : class
            where TImplementation : TComponent
        {
            Register(typeof(TComponent), typeof(TImplementation), instance);
        }

        /// <summary>
        /// Registers a component to the container and specifies it's implementation.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component.</typeparam>
        /// <typeparam name="TImplementation">Type of the component implementation.</typeparam>
        public void Register<TComponent, TImplementation>()
            where TComponent : class
            where TImplementation : TComponent
        {
            Register(typeof(TComponent), typeof(TImplementation));
        }

        /// <summary>
        /// Registers a component to the container.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component.</typeparam>
        public void Register<TComponent>()
            where TComponent : class
        {
            Register(typeof(TComponent));
        }

        /// <summary>
        /// Registers a component to the container.
        /// </summary>
        /// <param name="instance">Component instance.</param>
        public void Register<TComponent>(object instance)
            where TComponent : class
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", "Registered component instance cannot be null.");
            }

            // Cast instance before registering
            Register(typeof(TComponent), typeof(TComponent), instance);
        }

        /// <summary>
        /// Registers a component instance to the container.
        /// </summary>
        /// <param name="instance">Component instance.</param>
        public void Register(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance", "Registered component instance cannot be null.");
            }
            Type componenType = instance.GetType();
            Register(componenType, componenType, instance);
        }

        /// <summary>
        /// Registers a transient component, the instance of which will not be cached.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component.</typeparam>
        /// <typeparam name="TImplementation">Type of the component implementation.</typeparam>
        public void RegisterTransient<TComponent, TImplementation>()
            where TComponent : class
            where TImplementation : TComponent
        {
            Register<TComponent, TImplementation>();
            _transientsRegister.Add(new KeyValuePair<Type, Type>(typeof(TComponent), typeof(TImplementation)));
        }

        /// <summary>
        /// Registers a transient component, the instance of which will not be cached.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component.</typeparam>
        public void RegisterTransient<TComponent>()
            where TComponent : class
        {
            Register<TComponent>();
            _transientsRegister.Add(new KeyValuePair<Type, Type>(typeof(TComponent), typeof(TComponent)));
        }

        /// <summary>
        /// Removes all components of the specified from the container.
        /// </summary>
        /// <typeparam name="TComponent">Type of the components to be removed.</typeparam>
        public void RemoveAll<TComponent>()
            where TComponent : class
        {
            RemoveAll(typeof(TComponent));
        }

        /// <summary>
        /// Removes a component and it's mapped implementation from the container.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component.</typeparam>
        /// <typeparam name="TImplementation">Type of the component implementation.</typeparam>
        public void Remove<TComponent, TImplementation>()
            where TComponent : class
            where TImplementation : TComponent
        {
            Type implementation = typeof(TImplementation);
            if (_instancesRegister.ContainsKey(implementation))
            {
                var disposable = _instancesRegister[implementation] as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            _instancesRegister.Remove(implementation);

            _componentsRegister[typeof(TComponent)].Remove(typeof(TImplementation));
        }

        /// <summary>
        /// Removes all implementations of the specified component type.
        /// </summary>
        /// <param name="component">Type of the component.</param>
        public void RemoveAll(Type component)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            foreach (Type implementation in _componentsRegister[component])
            {
                if (_instancesRegister.ContainsKey(implementation))
                {
                    var disposable = _instancesRegister[implementation] as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                _instancesRegister.Remove(implementation);
            }
            _componentsRegister.RemoveAll(component);
        }

        /// <summary>
        /// Resolves an instance of the specified component type.
        /// </summary>
        /// <typeparam name="TComponent"></typeparam>
        /// <returns></returns>
        public TComponent Resolve<TComponent>()
            where TComponent : class
        {
            return (TComponent)GetService(typeof(TComponent));
        }

        /// <summary>
        /// Resolves all instances of the specified component type.
        /// </summary>
        /// <typeparam name="T">Type of the component.</typeparam>
        /// <returns>Resolved instances.</returns>
        public IEnumerable<T> ResolveAll<T>()
            where T : class
        {
            return ResolveAll(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Gets the service object of the specified type.
        /// </summary>
        /// <param name="component">An object that specifies the type of service object to get.</param>
        /// <returns>  
        /// A service object of type serviceType.-or- null if there is no service object of type serviceType.
        /// </returns>
        public object GetService(Type component)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            if (component.GetTypeInfo().IsGenericType)
            {
                Type gtd = component.GetGenericTypeDefinition();
                Type argument = component.GetTypeInfo().GenericTypeArguments[0];

                TypeInfo info = gtd.GetTypeInfo();
                if (info.IsInterface && typeof(IEnumerable<>).GetTypeInfo().IsAssignableFrom(info))
                {
                    return ResolveAll(argument).CastSlow(argument);
                }
            }
            Type implementation = _componentsRegister[component].FirstOrDefault();
            if (implementation == null)
            {
                throw new InvalidOperationException(String.Format("Component {0} is not registered.", (component)));
            }

            return ResolveInstance(component, implementation);
        }

        /// <summary>
        /// Resolves all instances of the specified component type.
        /// </summary>
        /// <param name="component">Type of the component.</param>
        /// <returns>Resolved instances.</returns>
        public IEnumerable<object> ResolveAll(Type component)
        {
            if (component == null)
            {
                throw new ArgumentNullException("component");
            }

            return _componentsRegister[component]
                .Select(implementation => ResolveInstance(component, implementation));
        }

        private object ResolveInstance(Type component, Type implementation)
        {
            lock (_instancesRegister)
            {
                if (_instancesRegister.ContainsKey(implementation))
                {
                    return _instancesRegister[implementation];
                }
                return CreateNewInstance(component, implementation);
            }
        }

        private object CreateNewInstance(Type component, Type implementation)
        {
            ConstructorInfo constructor = SelectConstructor(component, implementation);
            object[] arguments = constructor.GetParameters()
                .Select(parameter => GetService(parameter.ParameterType))
                .ToArray();

            try
            {
                object instance = constructor.Invoke(arguments);

                // Cache instance if not registered as transient
                if (!_transientsRegister.Contains(new KeyValuePair<Type, Type>(component, implementation)))
                {
                    _instancesRegister.Add(implementation, instance);
                }

                return instance;
            }
            catch (TargetInvocationException ex)
            {
                Exception innerException = ex.InnerException;
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(innerException).Throw();
                throw innerException;
            }
        }

        private static ConstructorInfo SelectConstructor(Type component, Type implementation)
        {
            ConstructorInfo constructor = implementation.GetTypeInfo()
                    .DeclaredConstructors.Where(c => c.IsPublic && !c.IsStatic)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .FirstOrDefault();

            if (constructor == null)
            {
                string errorMsg = String.Format("No constructors available for {0} with implementation {1}",
                    component, implementation);
                throw new InvalidOperationException(errorMsg);
            }

            return constructor;
        }

        #region IDisposable Members

        private bool _isDisposed;

        /// <summary>
        /// Releases all container resources.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing && !_isDisposed)
            {
                lock (this)
                {
                    foreach (IDisposable instance in _instancesRegister.Values)
                    {
                        instance.Dispose();
                    }
                    _componentsRegister.Clear();
                    _instancesRegister.Clear();

                    _isDisposed = true;
                    GC.SuppressFinalize(this);
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Releases resources before the object is reclaimed by garbage collection.
        /// </summary>
        ~PortableContainer()
        {
            Dispose(false);
        }

        #endregion

    }
}