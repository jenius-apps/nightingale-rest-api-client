using Nightingale.Core.Helpers;
using Nightingale.Core.Mock.Models;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Services;
using Nightingale.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Nightingale.Core.Mock.Services
{
    /// <summary>
    /// Class for matching a request URL and method
    /// with the mock return value.
    /// </summary>
    public class RequestProcessor : IRequestProcessor
    {
        public MockNode _root;

        /// <inheritdoc/>
        public MockData GetReturnValue(string path, string method)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException("Path is null, whitespace, or empty");
            }

            var segments = path.TrimStart('/').Split('/');
            var queue = new Queue<string>(segments);
            var firstSegmentToLookFor = queue.Dequeue();
            var result = FindResult(_root, queue, method, firstSegmentToLookFor);
            return result;
        }

        private MockData FindResult(MockNode current, Queue<string> segments, string method, string segmentToLookFor)
        {
            if (current?.Children == null || segments == null)
            {
                return null;
            }

            if (segments.Count == 0 && current.Method == method && segmentToLookFor == current.PathSegment)
            {
                return current.ReturnValue;
            }

            var next = current.Children.FirstOrDefault(x => x.PathSegment == segmentToLookFor);
            string nextSegmentToLookFor = segmentToLookFor;
            if (next != null && segments.Count > 0)
            {
                nextSegmentToLookFor = segments.Dequeue();
            }

            return FindResult(next, segments, method, nextSegmentToLookFor);
        }

        /// <inheritdoc/>
        public void Initialize(ServerConfiguration config, DocumentFile ncf)
        {
            _root = new MockNode();

            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (ncf == null)
            {
                throw new ArgumentNullException(nameof(ncf));
            }
            if (string.IsNullOrWhiteSpace(config.WorkspaceId))
            {
                throw new InvalidOperationException("Configuration is missing a workspace Id. Cancelling initialization.");
            }

            if (!string.IsNullOrWhiteSpace(config.ItemId))
            {
                InitializeItem(config.WorkspaceId, config.ItemId, config.EnvironmentId, ncf);
            }
            else
            {
                InitializeWorkspace(config.WorkspaceId, config.EnvironmentId, ncf);
            }
        }

        private void InitializeWorkspace(string workspaceId, string envId, DocumentFile ncf)
        {
            var workspace = ncf.Workspaces.FirstOrDefault(x => x.Id == workspaceId);
            if (workspace == null)
            {
                throw new InvalidOperationException("No workspace found with this ID: " + workspaceId);
            }
            var env = workspace.Environments.FirstOrDefault(x => x.Id == envId);

            var queue = new Queue<Item>(workspace.Items);
            TraverseItems(queue, env);
        }

        private void InitializeItem(string workspaceId, string itemId, string envId, DocumentFile ncf)
        {
            var workspace = ncf.Workspaces.FirstOrDefault(x => x.Id == workspaceId);
            if (workspace == null)
            {
                throw new InvalidOperationException("No workspace found with this ID: " + workspaceId);
            }
            var env = workspace.Environments.FirstOrDefault(x => x.Id == envId);

            Item rootItem = ItemFinder.GetItemsFromTree(new[] { itemId }, workspace.Items)?.FirstOrDefault();
            var queue = new Queue<Item>(new[] { rootItem });
            TraverseItems(queue, env);
        }

        private void TraverseItems(Queue<Item> queue, Core.Models.Environment env)
        {
            if (queue == null || queue.Count == 0)
            {
                return;
            }

            var currentItem = queue.Dequeue();
            if (currentItem != null)
            {
                AddToTree(currentItem, env);
                foreach (var child in currentItem.Children) queue.Enqueue(child);
            }

            TraverseItems(queue, env);
        }

        private void AddToTree(Item item, Core.Models.Environment env)
        {
            if (item == null || item.Type == ItemType.Collection)
            {
                // TODO add support for collections
                return;
            }

            string url = item.Url.ToString();
            url = VariableResolver.ResolveVariable(url, env?.Variables?.GetActive().ToList());
            string path = Uri.IsWellFormedUriString(url, UriKind.Absolute)
                ? new Uri(url).AbsolutePath.TrimStart('/')
                : throw new InvalidOperationException("URL is not a real path: " + url);
            var segments = path.Split('/');

            var insertQueue = new Queue<string>(segments);
            InsertNode(_root, insertQueue, item.Method, item.MockData);
        }

        private void InsertNode(MockNode parent, Queue<string> insertQueue, string method, MockData returnValue)
        {
            if (parent == null || insertQueue == null || insertQueue.Count == 0)
            {
                return;
            }

            var current = insertQueue.Dequeue();
            var existingNode = parent.Children.FirstOrDefault(x => x.PathSegment == current);

            // Skip an internal node if it exists
            if (existingNode != null && insertQueue.Count > 0)
            {
                InsertNode(existingNode, insertQueue, method, returnValue);
            }
            else
            {
                bool isChild = insertQueue.Count == 0;
                var node = new MockNode
                {
                    Method = isChild ? method : null,
                    PathSegment = current,
                    ReturnValue = isChild ? returnValue : null
                };

                parent.Children.Add(node);
                InsertNode(node, insertQueue, method, returnValue);
            }
        }
    }
}
