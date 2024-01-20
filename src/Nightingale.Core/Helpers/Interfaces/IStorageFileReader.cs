using System.Threading.Tasks;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IStorageFileReader
    {
        Task<string> ReadFileAsync();

        /// <summary>
        /// Retrieves the contents of
        /// the given file as a byte array.
        /// </summary>
        /// <param name="filepath">Path to file.</param>
        /// <returns>Contents of file as byte array.</returns>
        Task<byte[]> GetBytesAsync(string filepath);
    }
}
