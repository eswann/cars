using System;
using System.Collections.Generic;

namespace Cars.EventSource
{
    /// <summary>
    /// Store the event metadatas.
    /// Some avaliable keys: <see cref="MetadataKeys"/>.
    /// </summary>
    public interface IMetadata : IReadOnlyDictionary<string, object>
    {
        /// <summary>
        /// Get the stored value.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        object GetValue(string key);

        /// <summary>
        /// Get the value, convert it using the <paramref name="converter"/> function and return the type specified.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="converter"></param>
        /// <returns></returns>
        T GetValue<T>(string key, Func<object, T> converter);
    }
}