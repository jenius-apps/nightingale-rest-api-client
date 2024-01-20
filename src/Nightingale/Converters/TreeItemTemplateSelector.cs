using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.Models;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Converters
{
    public class TreeItemTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CollectionTemplate { get; set; }
        public DataTemplate RequestTemplate { get; set; }

        protected override DataTemplate SelectTemplateCore(object item)
        {
            if (item is Item i)
            {
                return i.Type == ItemType.Collection ? CollectionTemplate : RequestTemplate;
            }
            else
            {
                throw new NotImplementedException();
            }
        }
    }
}
