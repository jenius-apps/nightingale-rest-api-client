using System.Threading.Tasks;

namespace Nightingale.Core.Interfaces
{
    public interface IResponseFileWriter
    {
        Task WriteFileAsync(IWorkspaceResponse response);

        /// <summary>
        /// Writes the image to file.
        /// </summary>
        /// <param name="image">The byte array of the image to save to file.</param>
        /// <returns>If successful, returns the path to the file. Otherwise, returns null.</returns>
        Task<string> WriteImageAsync(byte[] image);
    }
}
