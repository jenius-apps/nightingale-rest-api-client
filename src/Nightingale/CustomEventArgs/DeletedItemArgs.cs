using System;

namespace Nightingale.CustomEventArgs
{
    public class DeletedItemArgs<T> : EventArgs
    {
        public T DeletedItem { get; }

        public DeletedItemArgs(T deletedItem)
        {
            DeletedItem = deletedItem != null ? deletedItem : throw new ArgumentNullException(nameof(deletedItem));
        }
    }
}
