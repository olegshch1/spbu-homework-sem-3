using System;

namespace Lazy
{
    /// <summary>
    /// Simple Lazy
    /// </summary>
    /// <typeparam name="T">value type</typeparam>
    public class Lazy<T> : ILazy<T>
    {
        private bool used = false;

        private Func<T> supplier;

        private T result;

        public Lazy(Func<T> function)
        {
            supplier = function;
        }

        /// <summary>
        /// Gets result
        /// </summary>
        /// <returns>result</returns>
        public T Get()
        {
            if (!used)
            {
                result = supplier();
                used = true;
            }
            return result;
        }
    }
}
