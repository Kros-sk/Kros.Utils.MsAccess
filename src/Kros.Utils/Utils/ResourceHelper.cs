using System;
using System.IO;
using System.Reflection;

namespace Kros.Utils
{
    /// <summary>
    /// Helper for getting content of file resources.
    /// </summary>
    internal static class ResourceHelper
    {
        /// <summary>
        /// Gets the resource content of the resource with name <paramref name="resourceName"/>.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>
        /// Resource content if resource exist; otherwise <see langword="null"/>.
        /// </returns>
        /// <remarks>
        /// .NET Core CLI currently does not know to return content of file resources using standard way (direct access to
        /// <c>Resources</c> class).
        /// </remarks>
        /// <exception cref="System.ArgumentNullException">
        /// Value of <paramref name="resourceName"/> is <see langword="null"/>.
        /// </exception>
        /// <exception cref="System.ArgumentException">Value of <paramref name="resourceName"/> is empty string,
        /// or string containing whitespace characters only.</exception>
        public static string GetResourceContent(string resourceName)
        {
            Check.NotNullOrWhiteSpace(resourceName, nameof(resourceName));

            var assembly = Assembly.GetExecutingAssembly();
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    return null;
                }
                using (var reader = new StreamReader(stream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
