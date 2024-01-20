using Nightingale.Core.Interfaces;
using Nightingale.Core.Models.Interfaces;
using System;

namespace Nightingale.Core.Models
{
    public class HistoryRequest : WorkspaceRequest, IHistoryItem
    {
        public HistoryRequest()
        {
        }

        public HistoryRequest(WorkspaceRequest request, DateTime lastUsed)
        {
            this.LastUsedDate = lastUsed;
            this.Name = request.Name;
            this.Method = request.Method;
            this.Url = new Url
            {
                Base = request.Url.Base
            };
            this.Body = request.Body.DeepClone() as RequestBody;
            this.Authentication = request.Authentication.DeepClone() as Authentication;
            this.Status = ModifiedStatus.New;

            foreach (Parameter p in request.Url.Queries)
            {
                this.Url.Queries.Add(p.DeepClone() as Parameter);
            }

            foreach (Parameter h in request.Headers)
            {
                this.Headers.Add(h.DeepClone() as Parameter);
            }

            foreach (ApiTest t in request.ApiTests)
            {
                this.ApiTests.Add(t.DeepClone() as ApiTest);
            }

            foreach (Parameter chainingRule in request.ChainingRules)
            {
                this.ChainingRules.Add(chainingRule.DeepClone() as Parameter);
            }
        }

        public DateTime LastUsedDate { get; set; }

        public override object DeepClone()
        {
            var baseClone = base.DeepClone() as WorkspaceRequest;
            var historyRequest = new HistoryRequest(baseClone, this.LastUsedDate);
            return historyRequest;
        }
    }
}
