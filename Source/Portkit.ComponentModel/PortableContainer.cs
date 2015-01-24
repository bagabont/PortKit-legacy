﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Portkit.Core.Collections;
using Portkit.Core.Extensions;

namespace Portkit.ComponentModel
{
    /// <summary>
    /// Represents a fast and easy to use IoC service container, that implements <see cref="IServiceProvider"/>.
    /// </summary>
    public class PortableContainer : StateDisposable, IServiceProvider
    {
        private readonly GroupedEnumerable<Type, Type> _register = new GroupedEnumerable<Type, Type>();
        private readonly Dictionary<Type, object> _instances = new Dictionary<Type, object>();
        private readonly HashSet<KeyValuePair<Type, Type>> _transients = new HashSet<KeyValuePair<Type, Type>>();

        /// <summary>
        /// Registers a component to the container.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component.</typeparam>
        public void Register<TComponent>() where TComponent : class
        {
            _register.Add(typeof(TComponent), typeof(TComponent));
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
            _register.Add(typeof(TComponent), typeof(TImplementation));
        }
        /// <summary>
        /// Registers a component to the container and specifies it's implementation.
        /// </summary>
        /// <typeparam name="TComponent">Type of the component.</typeparam>
        /// <typeparam name="TImplementation">Type of the component implementation.</typeparam>
        /// <param name="instance">Instance.</param>
        public void Register<TComponent, TImplementation>(object instance)
            where TComponent : class
            where TImplementation : TComponent
        {
            Register<TComponent, TImplementation>();
            _instances.Add(typeof(TImplementation), instance);
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
            _transients.Add(new KeyValuePair<Type, Type>(typeof(TComponent), typeof(TImplementation)));
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
            if (_instances.ContainsKey(implementation))
            {
                var disposable = _instances[implementation] as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }

            _instances.Remove(implementation);

            _register[typeof(TComponent)].Remove(typeof(TImplementation));
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

            foreach (Type implementation in _register[component])
            {
                if (_instances.ContainsKey(implementation))
                {
                    var disposable = _instances[implementation] as IDisposable;
                    if (disposable != null)
                    {
                        disposable.Dispose();
                    }
                }
                _instances.Remove(implementation);
            }
            _register.RemoveAll(component);
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

        // Summary:
        //     
        //
        // Parameters:
        //   serviceType:
        //     
        //
        // Returns:
        //     A service object of type serviceType.-or- null if there is no service object
        //     of type serviceType.

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
            Type implementation = _register[component].FirstOrDefault();

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

            return _register[component]
                .Select(implementation => ResolveInstance(component, implementation));
        }

        private object ResolveInstance(Type component, Type implementation)
        {
            lock (_instances)
                return _instances.ContainsKey(implementation)
                    ? _instances[implementation]
                    : CreateNewInstance(component, implementation);
        }

        private object CreateNewInstance(Type component, Type implementation)
        {
            ConstructorInfo constructor = SelectConstructor(component, implementation);
            object[] arguments = constructor.GetParameters().Select(parameter => GetService(parameter.ParameterType)).ToArray();

            try
            {
                object instance = constructor.Invoke(arguments);

                if (!_transients.Contains(new KeyValuePair<Type, Type>(component, implementation)))
                {
                    _instances.Add(implementation, instance);
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
            ConstructorInfo constructor =
                implementation.GetTypeInfo()
                    .DeclaredConstructors.Where(c => c.IsPublic && !c.IsStatic)
                    .OrderByDescending(c => c.GetParameters().Length)
                    .FirstOrDefault();
            if (constructor == null)
            {
                throw new InvalidOperationException(
                    String.Format("No constructors available for {0} with implementation {1}", component, implementation));
            }

            return constructor;
        }

        #region IDisposable Members

        /// <summary>
        /// Releases all container resources.
        /// </summary>
        /// <param name="disposing"></param>
        public override void Dispose(bool disposing)
        {
            if (disposing && !IsDisposed)
            {
                foreach (IDisposable instance in _instances.Values)
                {
                    instance.Dispose();
                }
                _register.Clear();
                _instances.Clear();
            }
            base.Dispose(disposing);
        }

        #endregion

    }
}