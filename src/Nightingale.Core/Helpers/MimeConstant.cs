using System.Linq;

namespace Nightingale.Core.Helpers
{
    public class MimeConstant
    {
        public const string Json = "application/json";

        public const string Xml = "application/xml";

        public const string FormEncoded = "application/x-www-form-urlencoded";

        public const string OctetStream = "application/octet-stream";

        public const string FormData = "multipart/form-data";

        public const string TextPlain = "text/plain";

        /// <summary>
        /// Method for checking if given string
        /// matches exactly with a mime type. Returns false 
        /// if string has customizations.
        /// </summary>
        public static bool IsStandardMime(string mimeType)
        {
            string[] mimes = new[]
            {
                Json,
                Xml,
                FormEncoded,
                OctetStream,
                FormData,
                TextPlain
            };

            return mimes.Contains(mimeType);
        }
    }
}
