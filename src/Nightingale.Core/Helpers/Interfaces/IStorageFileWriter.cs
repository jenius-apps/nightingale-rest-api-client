using System.Threading.Tasks;

namespace Nightingale.Core.Helpers.Interfaces
{
    public interface IStorageFileWriter
    {
        /// <summary>
        /// Opens a File Picker to select a location, 
        /// then writes the text to that file using the given file 
        /// parameters.
        /// </summary>
        /// <param name="content">The string content of the file.</param>
        /// <param name="filename">Name of the file.</param>
        /// <param name="fileDescriptor">Description to give for the file type.</param>
        /// <param name="fileType">The file extension to use. E.g. '.ncf'.</param>
        /// <returns>Full path to created file, or empty string if the write was cancelled or it failed.</returns>
        Task<string> WriteTextAsync(string content, string filename, string fileDescriptor, string fileType);
    }
}
