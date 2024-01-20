using Newtonsoft.Json;
using Nightingale.Core.Extensions;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nightingale.Core.CodeGenerators
{
    public class CurlGenerator : ICurlGenerator
    {
        public Task<string> GenerateCodeAsync(Item request)
        {
            var t = Task.Run(() =>
            {
                return GenerateCode(request);
            });

            return t;
        }

        private string GenerateCode(Item request)
        {
            var commandsList = new List<string>();

            // Initialize
            commandsList.Add($"curl -X {request.Method}");

            // URL
            commandsList.Add($"\t\'{request.Url}\'");

            // headers
            foreach (Parameter p in request.Headers.GetActive())
            {
                commandsList.Add($"\t-H \'{p.Key}\': \'{p.Value}\'");
            }

            // body
            if (request.Body.BodyType == RequestBodyType.Json)
            {
                commandsList.Add($"\t-d \'{JsonConvert.SerializeObject(request.Body.JsonBody)}\'");
            }
            else if (request.Body.BodyType == RequestBodyType.Xml)
            {
                string xmlBodyString = string.Empty;

                try
                {
                    XDocument doc = XDocument.Parse(request.Body.XmlBody.StripBom());
                    xmlBodyString = $"@\"{doc.ToString().Replace('"', '\'')}\"";
                }
                catch { }

                commandsList.Add($"\t-d \'{xmlBodyString}\'");
            }
            else if (request.Body.BodyType == RequestBodyType.FormEncoded)
            {
                var pairs = new List<string>();
                foreach (Parameter p in request.Body.FormEncodedData.GetActive())
                {
                    pairs.Add($"{p.Key}={Uri.EscapeUriString(p.Value.ToString())}");
                }

                commandsList.Add($"\t-d \'{string.Join("&", pairs)}\'");
            }
            else if (request.Body.BodyType == RequestBodyType.Binary)
            {
                commandsList.Add($"\t-d \'@{request.Body.BinaryFilePath}\'");
            }

            return string.Join(" \\" + System.Environment.NewLine, commandsList);
        }
    }
}
