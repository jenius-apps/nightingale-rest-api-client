using System;
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;
using Nightingale.Core.Enums;

namespace Nightingale.Core.Helpers
{
    public static class TextBeautifier
    {
        private static readonly string _byteOrderMarkUtf8 = Encoding.UTF8.GetString(Encoding.UTF8.GetPreamble());

        public static string Beautify(string value, ContentType contentType)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return string.Empty;
            }

            string result = value;

            if (contentType == ContentType.Xml)
            {
                try
                {
                    result = StripBom(result);

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(result);

                    using (StringWriter sw = new StringWriter())
                    {
                        doc.Save(sw);
                        return sw.GetStringBuilder().ToString();
                    }
                }
                catch
                {
                    // Text isn't a proper xml
                }
            }
            else
            {
                try
                {
                    result = JsonConvert.DeserializeObject(value ?? "").ToString();
                }
                catch
                {
                    // Text isn't a proper json
                }
            }

            return result;
        }

        private static string StripBom(string xmlInput)
        {
            if (xmlInput.StartsWith(_byteOrderMarkUtf8, StringComparison.Ordinal))
                xmlInput = xmlInput.Remove(0, _byteOrderMarkUtf8.Length);

            return xmlInput.Trim();
        }
    }
}
