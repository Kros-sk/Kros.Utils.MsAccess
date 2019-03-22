using Kros.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kros.Utils
{
    /// <summary>
    /// Helper class for validating method parameters. Every validation throws some kind of <see cref="ArgumentException"/>
    /// if it fails (<see cref="ArgumentException"/>, <see cref="ArgumentNullException"/>,
    /// <see cref="ArgumentOutOfRangeException"/>).
    /// </summary>
    /// <remarks>
    /// Default way of validating method parameters is:
    /// <code language="cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\CheckExamples.cs" region="CheckArgumentsOld"/>
    /// With the help of <c>Check</c> class, this is very easy. If it is possible, the validation methods return input value,
    /// so the parameter's value can be validated and assigned on one line:
    /// <code language = "cs" source="..\..\..\..\Documentation\Examples\Kros.Utils\CheckExamples.cs" region="CheckArgumentsNew"/>
    /// </remarks>
    public static class Check
    {
        #region Object

        /// <summary>
        /// The value of <paramref name="param"/> can not be <see langword="null"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of <paramref name="param"/> is <see langword="null"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static T NotNull<T>(T param, string paramName)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName, Resources.Check_NotNull);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> can not be <see langword="null"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentNullException">
        /// The value of <paramref name="param"/> is <see langword="null"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static T NotNull<T>(T param, string paramName, string message)
        {
            if (param == null)
            {
                throw new ArgumentNullException(paramName, message);
            }
            return param;
        }

        #endregion

        #region Type

        /// <summary>
        /// The value of <paramref name="param"/> must be of given type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Expected type of <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is not of required type.</exception>
        [DebuggerStepThrough]
        public static void IsOfType<T>(object param, string paramName)
        {
            IsOfType(param, typeof(T), paramName);
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be of given type <typeparamref name="T"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Expected type of <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is not of required type.</exception>
        [DebuggerStepThrough]
        public static void IsOfType<T>(object param, string paramName, string message)
        {
            IsOfType(param, typeof(T), paramName, message);
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be of given type <paramref name="expectedType"/>.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="expectedType">Required type of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is not of required type.</exception>
        [DebuggerStepThrough]
        public static void IsOfType(object param, Type expectedType, string paramName)
        {
            if (param.GetType() != expectedType)
            {
                throw new ArgumentException(
                    string.Format(Resources.Check_IsOfType, expectedType.FullName, param.GetType().FullName), paramName);
            }
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be of given type <paramref name="expectedType"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="expectedType">Required type of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is not of required type.</exception>
        [DebuggerStepThrough]
        public static void IsOfType(object param, Type expectedType, string paramName, string message)
        {
            if (param.GetType() != expectedType)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        /// <summary>
        /// The value of <paramref name="param"/> must not be of given type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The value of <paramref name="param"/> must not be of this type.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is of forbidden type.</exception>
        [DebuggerStepThrough]
        public static void IsNotOfType<T>(object param, string paramName)
        {
            IsNotOfType(param, typeof(T), paramName);
        }

        /// <summary>
        /// The value of <paramref name="param"/> must not be of given type <typeparamref name="T"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">The value of <paramref name="param"/> must not be of this type.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is of forbidden type.</exception>
        [DebuggerStepThrough]
        public static void IsNotOfType<T>(object param, string paramName, string message)
        {
            IsNotOfType(param, typeof(T), paramName, message);
        }

        /// <summary>
        /// The value of <paramref name="param"/> must not be of type <paramref name="notExpectedType"/>.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="notExpectedType">The value of <paramref name="param"/> must not be of this type.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is of forbidden type.</exception>
        [DebuggerStepThrough]
        public static void IsNotOfType(object param, Type notExpectedType, string paramName)
        {
            if (param.GetType() == notExpectedType)
            {
                throw new ArgumentException(
                    string.Format(Resources.Check_IsNotOfType, notExpectedType.FullName, param.GetType().FullName), paramName);
            }
        }

        /// <summary>
        /// The value of <paramref name="param"/> must not be of type <paramref name="notExpectedType"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="notExpectedType">The value of <paramref name="param"/> must not be of this type.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is of forbidden type.</exception>
        [DebuggerStepThrough]
        public static void IsNotOfType(object param, Type notExpectedType, string paramName, string message)
        {
            if (param.GetType() == notExpectedType)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        #endregion

        #region String

        /// <summary>
        /// The value of <paramref name="param"/> can not be <see langword="null"/>, nor empty string.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of <paramref name="param"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is empty string.</exception>
        [DebuggerStepThrough]
        public static string NotNullOrEmpty(string param, string paramName)
        {
            NotNull(param, paramName);
            if (string.IsNullOrEmpty(param))
            {
                throw new ArgumentException(Resources.Check_StringNotNullOrEmpty, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> can not be <see langword="null"/>, nor empty string.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of <paramref name="param"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is empty string.</exception>
        [DebuggerStepThrough]
        public static string NotNullOrEmpty(string param, string paramName, string message)
        {
            NotNull(param, paramName, message);
            if (string.IsNullOrEmpty(param))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> can not be <see langword="null"/>, empty string, nor string containing
        /// only whitespace characters.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of <paramref name="param"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is empty string or string containing
        /// only whitespace characters.</exception>
        [DebuggerStepThrough]
        public static string NotNullOrWhiteSpace(string param, string paramName)
        {
            NotNull(param, paramName);
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException(Resources.Check_StringNotNullOrWhiteSpace, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> can not be <see langword="null"/>, empty string, nor string containing
        /// only whitespace characters.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentNullException">The value of <paramref name="param"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is empty string or string containing
        /// only whitespace characters.</exception>
        [DebuggerStepThrough]
        public static string NotNullOrWhiteSpace(string param, string paramName, string message)
        {
            NotNull(param, paramName, message);
            if (string.IsNullOrWhiteSpace(param))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        #endregion

        #region Values

        /// <summary>
        /// The value of <paramref name="param"/> must be <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Required value of the <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is not <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T Equal<T>(T param, T value, string paramName)
        {
            if (!param.Equals(value))
            {
                throw new ArgumentException(string.Format(Resources.Check_Equal, value, param), paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be <paramref name="value"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Required value of the <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is not <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T Equal<T>(T param, T value, string paramName, string message)
        {
            if (!param.Equals(value))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must not be <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T NotEqual<T>(T param, T value, string paramName)
        {
            if (param.Equals(value))
            {
                throw new ArgumentException(string.Format(Resources.Check_NotEqual, value), paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must not be <paramref name="value"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T NotEqual<T>(T param, T value, string paramName, string message)
        {
            if (param.Equals(value))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be less than <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is greater or equal than
        /// <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T LessThan<T>(T param, T value, string paramName) where T : IComparable<T>
        {
            if (!((param as IComparable<T>).CompareTo(value) < 0))
            {
                throw new ArgumentException(string.Format(Resources.Check_LessThan, value, param), paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be less than <paramref name="value"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is greater or equal than
        /// <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T LessThan<T>(T param, T value, string paramName, string message) where T : IComparable<T>
        {
            if (!((param as IComparable<T>).CompareTo(value) < 0))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be less or equal than <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is greater than
        /// <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T LessOrEqualThan<T>(T param, T value, string paramName) where T : IComparable<T>
        {
            if (!((param as IComparable<T>).CompareTo(value) <= 0))
            {
                throw new ArgumentException(string.Format(Resources.Check_LessOrEqualThan, value, param), paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be less or equal than <paramref name="value"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is greater than
        /// <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T LessOrEqualThan<T>(T param, T value, string paramName, string message) where T : IComparable<T>
        {
            if (!((param as IComparable<T>).CompareTo(value) <= 0))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be greater than <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is less or equal than
        /// <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T GreaterThan<T>(T param, T value, string paramName) where T : IComparable<T>
        {
            if (!((param as IComparable<T>).CompareTo(value) > 0))
            {
                throw new ArgumentException(string.Format(Resources.Check_GreaterThan, value, param), paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be greater than <paramref name="value"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is less or equal than
        /// <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T GreaterThan<T>(T param, T value, string paramName, string message) where T : IComparable<T>
        {
            if (!((param as IComparable<T>).CompareTo(value) > 0))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be greater or equal than <paramref name="value"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is less than
        /// <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T GreaterOrEqualThan<T>(T param, T value, string paramName) where T : IComparable<T>
        {
            if (!((param as IComparable<T>).CompareTo(value) >= 0))
            {
                throw new ArgumentException(string.Format(Resources.Check_GreaterOrEqualThan, value, param), paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be greater or equal than <paramref name="value"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">Validated value.</param>
        /// <param name="value">Checked value of <paramref name="param"/>.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">The value of <paramref name="param"/> is less than
        /// <paramref name="value"/>.</exception>
        [DebuggerStepThrough]
        public static T GreaterOrEqualThan<T>(T param, T value, string paramName, string message)
            where T : IComparable<T>
        {
            if (!((param as IComparable<T>).CompareTo(value) >= 0))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be in list <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">The value, which must be in the list <paramref name="list"/>.</param>
        /// <param name="list">List of checked values.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">
        /// The value of <paramref name="param"/> is not in the list <paramref name="list"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static T IsInList<T>(T param, IEnumerable<T> list, string paramName)
        {
            if (!list.Contains(param))
            {
                throw new ArgumentException(GetIsInListDefaultMessage(param, list), paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must be in list <paramref name="list"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">The value, which must be in the list <paramref name="list"/>.</param>
        /// <param name="list">List of checked values.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">
        /// The value of <paramref name="param"/> is not in the list <paramref name="list"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static T IsInList<T>(T param, IEnumerable<T> list, string paramName, string message)
        {
            if (!list.Contains(param))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must not be in list <paramref name="list"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">The value, which must not be in the list <paramref name="list"/>.</param>
        /// <param name="list">List of checked values.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">
        /// The value of <paramref name="param"/> is in the list <paramref name="list"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static T IsNotInList<T>(T param, IEnumerable<T> list, string paramName)
        {
            if (list.Contains(param))
            {
                throw new ArgumentException(GetIsNotInListDefaultMessage(param, list), paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> must not be in list <paramref name="list"/>.
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <typeparam name="T">Type of the <paramref name="param"/>.</typeparam>
        /// <param name="param">The value, which must not be in the list <paramref name="list"/>.</param>
        /// <param name="list">List of checked values.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">
        /// The value of <paramref name="param"/> is in the list <paramref name="list"/>.
        /// </exception>
        [DebuggerStepThrough]
        public static T IsNotInList<T>(T param, IEnumerable<T> list, string paramName, string message)
        {
            if (list.Contains(param))
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        [DebuggerStepThrough]
        private static string GetIsInListDefaultMessage<T>(T param, IEnumerable<T> list)
        {
            return string.Format(Resources.Check_IsInListDefaultMessage, param, GetValuesInListAsString(list));
        }

        [DebuggerStepThrough]
        private static string GetIsNotInListDefaultMessage<T>(T param, IEnumerable<T> list)
        {
            return string.Format(Resources.Check_IsNotInListDefaultMessage, param, GetValuesInListAsString(list));
        }

        [DebuggerStepThrough]
        private static string GetValuesInListAsString<T>(IEnumerable<T> list)
        {
            const int maxItemsInString = 10;
            bool hasMoreValues = false;
            StringBuilder sb = new StringBuilder();

            int i = 0;
            foreach (T item in list)
            {
                i++;
                if (i > maxItemsInString)
                {
                    hasMoreValues = true;
                    break;
                }
                if (sb.Length > 0)
                {
                    sb.Append(", ");
                }
                sb.Append(item);
            }
            if (hasMoreValues)
            {
                sb.Append("...");
            }

            return sb.ToString();
        }

        #endregion

        #region Guid

        /// <summary>
        /// The value of <paramref name="param"/> can not be empty GUID (<see cref="Guid.Empty"/>).
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">Paramere je prázdny GUID (<see cref="Guid.Empty"/>).</exception>
        [DebuggerStepThrough]
        public static Guid NotEmptyGuid(Guid param, string paramName)
        {
            if (param == Guid.Empty)
            {
                throw new ArgumentException(Resources.Check_NotEmptyGuid, paramName);
            }
            return param;
        }

        /// <summary>
        /// The value of <paramref name="param"/> can not be empty GUID (<see cref="Guid.Empty"/>).
        /// Thrown exception has custom message <paramref name="message"/>.
        /// </summary>
        /// <param name="param">Validated value.</param>
        /// <param name="paramName">Name of the method parameter.</param>
        /// <param name="message">Custom exception message.</param>
        /// <returns>Input value of <paramref name="param"/>.</returns>
        /// <exception cref="ArgumentException">
        /// Value of <paramref name="param"/> is empty GUID (<see cref="Guid.Empty"/>).
        /// </exception>
        [DebuggerStepThrough]
        public static Guid NotEmptyGuid(Guid param, string paramName, string message)
        {
            if (param == Guid.Empty)
            {
                throw new ArgumentException(message, paramName);
            }
            return param;
        }

        #endregion
    }
}
