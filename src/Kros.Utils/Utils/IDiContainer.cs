using System;

namespace Kros.Utils
{
    /// <summary>
    /// Interface for DI (Dependency Injection) container.
    /// </summary>
    public interface IDiContainer : IDisposable
    {
        /// <summary>
        /// Registers a class type.
        /// </summary>
        /// <typeparam name="T">Class type.</typeparam>
        /// <returns>Container instance for fluent registration.</returns>
        IDiContainer Register<T>();

        /// <summary>
        /// Registers a class type with name <paramref name="name"/>.
        /// </summary>
        /// <typeparam name="T">Class type.</typeparam>
        /// <param name="name">Name of the registered class type.</param>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer Register<T>(string name);

        /// <summary>
        /// Register interface or class type using lambda function.
        /// </summary>
        /// <typeparam name="T">Interface or class type.</typeparam>
        /// <param name="lambda">
        /// Function which takes current DI container as parameter and creates a new instance of <typeparamref name="T"/>.
        /// </param>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer Register<T>(Func<IDiContainer, T> lambda);

#pragma warning disable CS1573
        /// Parameter has no matching param tag in the XML comment (but other parameters do)
        /// <summary>
        /// Register named interface or class type using lambda function.
        /// </summary>
        /// <typeparam name="T">Interface or class type.</typeparam>
        /// <param name="name">Name of the registered interface or class type.</param>
        /// <inheritdoc cref="Register{T}(Func{IDiContainer, T})" select="param"/>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer Register<T>(string name, Func<IDiContainer, T> lambda);
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

        /// <summary>
        /// Registers a interface-class pair.
        /// </summary>
        /// <typeparam name="TFrom">Registered interface.</typeparam>
        /// <typeparam name="TTo">Registered class type that implements <typeparamref name="TFrom"/>.</typeparam>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer Register<TFrom, TTo>() where TTo : TFrom;

        /// <summary>
        /// Registers a named interface-class pair.
        /// </summary>
        /// <inheritdoc cref="Register{TFrom, TTo}()" select="typeparam"/>
        /// <param name="name">Name of the registered interface.</param>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer Register<TFrom, TTo>(string name) where TTo : TFrom;

        /// <summary>
        /// Registers class type as singleton.
        /// </summary>
        /// <typeparam name="T">Registered class type.</typeparam>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer RegisterInstance<T>();

        /// <summary>
        /// Registers class type with name <paramref name="name"/> as singleton.
        /// </summary>
        /// <typeparam name="T">Registered class type.</typeparam>
        /// <param name="name">Name of the registered class type.</param>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer RegisterInstance<T>(string name);

        /// <summary>
        /// Registers class instance.
        /// </summary>
        /// <typeparam name="T">Registered class type.</typeparam>
        /// <param name="instance">Registered instance.</param>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer RegisterInstance<T>(T instance);

        /// <summary>
        /// Registers class instance.
        /// </summary>
        /// <typeparam name="T">Registered class type.</typeparam>
        /// <param name="name">Name of the registered class type.</param>
        /// <param name="instance">Registered instance.</param>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer RegisterInstance<T>(string name, T instance);

        /// <summary>
        /// Registers a interface-class pair as singleton.
        /// </summary>
        /// <inheritdoc cref="Register{TFrom, TTo}()" select="typeparam"/>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer RegisterInstance<TFrom, TTo>() where TTo : TFrom;

        /// <summary>
        /// Registers a named interface to class type pair. class type <typeparamref name="TTo"/> as a singleton.
        /// </summary>
        /// <inheritdoc cref="Register{TFrom, TTo}()" select="typeparam"/>
        /// <param name="name">Name of the registered class type.</param>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer RegisterInstance<TFrom, TTo>(string name) where TTo : TFrom;

        /// <summary>
        /// Registers class instance as sigleton using function.
        /// </summary>
        /// <typeparam name="T">Registered class type.</typeparam>
        /// <inheritdoc cref="Register{T}(Func{IDiContainer, T})" select="param"/>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer RegisterInstance<T>(Func<IDiContainer, T> lambda);

#pragma warning disable CS1573
        /// Parameter has no matching param tag in the XML comment (but other parameters do)
        /// <summary>
        /// Registers named class instance as sigleton using function.
        /// </summary>
        /// <typeparam name="T">Registered class type.</typeparam>
        /// <param name="name">Name, under which is type registered.</param>
        /// <inheritdoc cref="Register{T}(Func{IDiContainer, T})" select="param"/>
        /// <inheritdoc cref="Register{T}()" select="returns"/>
        IDiContainer RegisterInstance<T>(string name, Func<IDiContainer, T> lambda);
#pragma warning restore CS1573 // Parameter has no matching param tag in the XML comment (but other parameters do)

        /// <summary>
        /// Resolves the instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to resolve from container.</typeparam>
        /// <returns>Object of type <typeparamref name="T"/>.</returns>
        T GetInstance<T>();

        /// <summary>
        /// Resolves the named instance of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type to resolve from container.</typeparam>
        /// <param name="name">Name of the desired instance.</param>
        /// <returns>Object of type <typeparamref name="T"/>.</returns>
        T GetInstance<T>(string name);

        /// <summary>
        /// Resolves the instance of type <paramref name="instanceType"/>.
        /// </summary>
        /// <param name="instanceType">Type to resolve from container.</param>
        /// <returns>Object of type <paramref name="instanceType"/>.</returns>
        object GetInstance(Type instanceType);

        /// <summary>
        /// Resolves the named instance of type <paramref name="instanceType"/>.
        /// </summary>
        /// <param name="instanceType">Type to resolve from container.</param>
        /// <param name="name">Name of the desired instance.</param>
        /// <returns>Object of type <paramref name="instanceType"/>.</returns>
        object GetInstance(Type instanceType, string name);

        /// <summary>
        /// Create a child container.
        /// </summary>
        /// <returns>The new child container.</returns>
        /// <remarks>
        /// A child container shares the parent's configuration, but can be configured with different settings or lifetime.
        /// </remarks>
        IDiContainer CreateChildContainer();
    }
}
