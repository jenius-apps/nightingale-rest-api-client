using Nightingale.Core.Models;
using System.Collections.Generic;

namespace Nightingale.Core.Http
{
    /// <summary>
    /// Class that holds a list of methods
    /// and allows for other classes to make updates to
    /// the list.
    /// </summary>
    public class MethodsContainer : IMethodsContainer
    {
        private IList<string> _methods;

        /// <inheritdoc/>
        public string GetFirst()
        {
            if (_methods == null || _methods.Count == 0)
            {
                return null;
            }

            return _methods[0];
        }

        /// <inheritdoc/>
        public IList<string> GetMethods()
        {
            return _methods;
        }

        /// <inheritdoc/>
        public void SetMethods(Workspace workspace)
        {
            if (workspace == null)
            {
                return;
            }

            if (workspace.Methods == null || workspace.Methods.Count == 0)
            {
                workspace.Methods = new List<string>(Method.Defaults);
            }

            _methods = workspace.Methods;
        }

        /// <inheritdoc/>
        public void UpdateMethods(IList<string> methods)
        {
            if (methods == null)
            {
                return;
            }

            _methods.Clear();
            foreach (var m in methods)
            {
                if (!string.IsNullOrWhiteSpace(m))
                {
                    _methods.Add(m);
                }
            }
        }
    }
}
