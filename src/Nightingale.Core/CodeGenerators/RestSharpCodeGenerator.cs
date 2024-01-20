using Newtonsoft.Json;
using Nightingale.Core.Extensions;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Nightingale.Core.CodeGenerators
{
    public class RestSharpCodeGenerator : IRestSharpCodeGenerator
    {
        public Task<string> GenerateCodeAsync(Item request)
        {
            Task<string> t = Task.Run(() =>
            {
                return GenerateCode(request);
            });

            return t;
        }

        private string GenerateCode(Item request)
        {
            var commandsList = new List<string>();

            // Initialize client
            commandsList.Add($"var client = new RestClient();");

            // Assign base url
            commandsList.Add($"client.BaseUrl = new Uri(\"{request.Url.Base}\");");

            // Assign user agent
            var userAgentHeader = request.Headers.FirstOrDefault(x => x.Key.Equals("user-agent", StringComparison.OrdinalIgnoreCase));
            if (userAgentHeader != null && userAgentHeader.Enabled)
            {
                commandsList.Add($"client.UserAgent = {userAgentHeader.Value}");
            }

            // Initialize request
            commandsList.Add($"var request = new RestRequest(RestSharp.Method.{request.Method});");

            // Add headers
            foreach (var h in request.Headers.GetActive())
            {
                commandsList.Add($"request.AddHeader(\"{h.Key}\", \"{h.Value}\");");
            }

            // Add queries
            foreach (var q in request.Url.Queries.GetActive())
            {
                commandsList.Add($"request.AddQueryParameter(\"{q.Key}\", \"{q.Value}\");");
            }

            if (request.Body.BodyType == RequestBodyType.FormEncoded)
            {
                string formEncodedString = request.Body.FormEncodedData.ToQueryString().Trim('?');
                commandsList.Add($"request.AddParameter(\"application/x-www-form-urlencoded\", \"{formEncodedString}\", ParameterType.RequestBody);");
            }
            else if (request.Body.BodyType == RequestBodyType.Json)
            {
                string json = JsonConvert.SerializeObject(request.Body.JsonBody).ToString();
                commandsList.Add($"request.AddParameter(\"application/json\", \"{json}\", ParameterType.RequestBody);");
            }
            else if (request.Body.BodyType == RequestBodyType.Xml)
            {
                string xml = string.Empty;

                try
                {
                    XDocument doc = XDocument.Parse(request.Body.XmlBody.StripBom());
                    xml = $"@\"{doc.ToString().Replace('"', '\'')}\"";
                }
                catch { }

                commandsList.Add($"request.AddParameter(\"application/xml\", \"{xml}\", ParameterType.RequestBody);");
            }
            else if (request.Body.BodyType == RequestBodyType.Binary)
            {
                commandsList.Add($"request.AddFile(\"attachment\", @\"{request.Body.BinaryFilePath}\", \"application/octet-stream\");");
            }

            commandsList.Add("var response = client.Execute(request);");

            return string.Join(System.Environment.NewLine, commandsList);
        }
    }
}
