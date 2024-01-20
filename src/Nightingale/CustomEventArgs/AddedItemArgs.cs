using System;

namespace Nightingale.CustomEventArgs
{
    public class AddedItemArgs<T> : EventArgs
    {
        public T AddedItem { get; }

        public AddedItemArgs(T addedItem)
        {
            AddedItem = addedItem;
        }
    }
}
