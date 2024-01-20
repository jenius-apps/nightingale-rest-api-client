using Nightingale.Core.Models;
using System.Collections.Generic;

namespace Nightingale.Core.Extensions
{
    public static class WorkspaceItemListEx
    {
        public static void RemoveWorkspaceItem(this IList<WorkspaceItem> list, WorkspaceItem forRemoval)
        {
            if (forRemoval == null)
            {
                return;
            }

            if (list.Contains(forRemoval))
            {
                list.Remove(forRemoval);
            }
            else
            {
                WorkspaceCollection parent = list.GetItemParent(forRemoval);
                parent?.Children?.Remove(forRemoval);
            }
        }

        public static WorkspaceCollection GetItemParent(this IList<WorkspaceItem> list, WorkspaceItem item)
        {
            if (list == null || item == null || list.Contains(item))
            {
                return null;
            }

            foreach (WorkspaceItem i in list)
            {
                if (i is WorkspaceCollection c)
                {
                    WorkspaceCollection parent = GetItemParent(c, item);

                    if (parent != null)
                    {
                        return parent;
                    }
                }
            }

            return null;
        }

        private static WorkspaceCollection GetItemParent(this WorkspaceCollection root, WorkspaceItem item)
        {
            if (root == null || item == null || root == item)
            {
                return null;
            }

            if (root.Children.Contains(item))
            {
                return root;
            }

            foreach (WorkspaceItem childItem in root.Children)
            {
                if (childItem is WorkspaceCollection c)
                {
                    var parent = GetItemParent(c, item);

                    if (parent != null)
                    {
                        return parent;
                    }
                }
            }

            return null;
        }
    }
}
