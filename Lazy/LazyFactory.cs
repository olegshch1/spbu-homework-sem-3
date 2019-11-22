using System;

namespace Lazy
{
    /// <summary>
    /// Making Lazy
    /// </summary>
    /// <typeparam name="T">value type</typeparam>
    public class LazyFactory<T>
    {
        /// <summary>
        /// Simple Lazy
        /// </summary>
        /// <param name="function">function</param>
        /// <returns>result</returns>
        public static ILazy<T> CreateLazy(Func<T> function) => new Lazy<T>(function);
        
        /// <summary>
        /// Multithread Lazy
        /// </summary>
        /// <param name="function">function</param>
        /// <returns>result</returns>
        public static ILazy<T> CreateMultiLazy(Func<T> function) => new MultiLazy<T>(function);
    }
}
