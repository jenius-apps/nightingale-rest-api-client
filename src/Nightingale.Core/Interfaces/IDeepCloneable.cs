namespace Nightingale.Core.Interfaces
{
    /// <summary>
    /// Interface for classes that implement
    /// deep copy abilities.
    /// </summary>
    public interface IDeepCloneable
    {
        /// <summary>
        /// Deep copies this object.
        /// </summary>
        /// <returns>Returns a deep copy of this object.</returns>
        object DeepClone();
    }
}
