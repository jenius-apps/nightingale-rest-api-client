namespace Nightingale.Core.Interfaces
{
    public interface IStorageItem 
    {
        /// <summary>
        /// The Id of this <see cref="IStorageItem"/>.
        /// </summary>
        string Id { get; set; }

        /// <summary>
        /// The parent Id of this item.
        /// </summary>
        string ParentId { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ModifiedStatus"/> of this item.
        /// </summary>
        ModifiedStatus Status { get; set; }
    }

    public enum ModifiedStatus
    {
        Unchanged,
        New,
        Modified
    }
}
