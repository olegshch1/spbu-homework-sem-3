namespace Lazy
{
    /// <summary>
    /// Interface for Lazy
    /// </summary>
    /// <typeparam name="T">operand value</typeparam>
    public interface ILazy<T>
    {
        /// <summary>
        /// Gets result
        /// </summary>
        /// <returns>result</returns>
        T Get();
    }
}
