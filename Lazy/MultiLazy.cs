using System;

namespace Lazy
{
    /// <summary>
    /// MultiThread Lazy
    /// </summary>
    /// <typeparam name="T">value type</typeparam>
    public class MultiLazy<T> : ILazy<T>
    {
        private volatile bool used;

        private Func<T> supplier;

        private object flag = new object();

        private T result;

        public MultiLazy(Func<T> function)
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
                lock (flag)
                {
                    if (!used)
                    {
                        result = supplier();
                        used = true;
                        supplier = null;
                    }
                }
            }
            return result;
        }
    }
}
