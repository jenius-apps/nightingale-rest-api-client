using ActiproSoftware.Text;
using ActiproSoftware.Text.Implementation;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Nightingale.Editor
{
    public class LangDefLoader
    {
        /// <summary>
        /// Initializes an existing <see cref="ISyntaxLanguage"/> from a language definition (.langdef file) from a resource stream.
        /// </summary>
        /// <param name="filename">The filename.</param>
        public static async Task InitializeLanguageFromResourceStream(ISyntaxLanguage language, string filename)
        {
            var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Editor/" + filename));

            using (Stream stream = await file.OpenStreamForReadAsync())
            {
                if (stream != null)
                {
                    SyntaxLanguageDefinitionSerializer serializer = new SyntaxLanguageDefinitionSerializer();
                    serializer.InitializeFromStream(language, stream);
                }
            }
        }
    }
}
