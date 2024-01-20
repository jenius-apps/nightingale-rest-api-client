using Nightingale.Core.Http.Interfaces;

namespace Nightingale.Core.Http
{
    public class RestSharpMethodConverter : IHttpMethodConverter
    {
        public string IntToFullMethodName(int methodIndex)
        {
            switch (methodIndex)
            {
                case 0:
                    return "GET";
                case 1:
                    return "POST";
                case 2:
                    return "PUT";
                case 3:
                    return "DELETE";
                case 4:
                    return "HEAD";
                case 5:
                    return "OPTIONS";
                case 6:
                    return "PATCH";
                case 7:
                    return "MERGE";
                case 8:
                    return "COPY";
                default:
                    return "--";
            }
        }

        public string IntToShortMethodName(int methodIndex)
        {
            switch (methodIndex)
            {
                case 0:
                    return "GET";
                case 1:
                    return "P0ST";
                case 2:
                    return "PUT";
                case 3:
                    return "DEL";
                case 4:
                    return "HEAD";
                case 5:
                    return "OPT";
                case 6:
                    return "PATC";
                case 7:
                    return "MRG";
                case 8:
                    return "COPY";
                default:
                    return "--";
            }
        }

        public int StringToInt(string method)
        {
            if (string.IsNullOrWhiteSpace(method))
            {
                return 0;
            }

            switch (method.ToUpper())
            {
                case "GET":
                    return 0;
                case "POST":
                    return 1;
                case "PUT":
                    return 2;
                case "DELETE":
                    return 3;
                case "HEAD":
                    return 4;
                case "OPTIONS":
                    return 5;
                case "PATCH":
                    return 6;
                case "MERGE":
                    return 7;
                case "COPY":
                    return 8;
                default:
                    return 0;
            }
        }
    }
}
