using System;
using System.Text;

namespace Nightingale.Core.Extensions
{
    public static class StringBomRemoverEx
    {
        private static readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        public static string StripBom(this string xmlInput)
        {
            if (xmlInput.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
                xmlInput = xmlInput.Remove(0, _byteOrderMarkUtf8.Length);

            return xmlInput.Trim();
        }
    }
}
