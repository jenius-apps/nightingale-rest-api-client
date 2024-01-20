namespace Nightingale.Core.Workspaces.Models
{
    /// <summary>
    /// Shallow reference for an item. This is useful
    /// for scenarios when you want to create a small
    /// copy of an item, but you still want a way to
    /// reference back to the original item.
    /// </summary>
    public class ItemShallowReference : Item
    {
        public ItemShallowReference(Item item)
        {
            this.Reference = item;
            this.Name = item.Name;
            this.IsExpanded = item.IsExpanded;
            this.Type = item.Type;
            this.Method = item.Method;

            foreach (var child in item.Children)
            {
                this.Children.Add(child.ShallowReference());
            }
        }

        public Item Reference { get; set; }
    }
}
