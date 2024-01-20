using Nightingale.Core.Models;
using Nightingale.Core.Models.Interfaces;

namespace Nightingale.Core.Extensions
{
    public static class WorkspaceItemEx
    {
        public static bool Contains(this WorkspaceItem parentItem, WorkspaceItem child)
        {
            if (child == null)
            {
                return false;
            }

            return parentItem is IWorkspaceCollection c ? c.Children.Contains(child) : false;
        }
    }
}
