using System;
using System.Collections.Generic;
using System.Text;

namespace Nightingale.Core.Exceptions
{
    public class UnsupportedFileException : Exception
    {
        public UnsupportedFileException(string message, string fileContentType, string fileName): base(message)
        {
            ContentTypeOrVersion = fileContentType;
            FileName = fileName;
        }

        public string ContentTypeOrVersion { get; }

        public string FileName { get; }
    }
}
