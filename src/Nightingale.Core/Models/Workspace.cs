using Newtonsoft.Json;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Workspaces.EventHandlers;
using Nightingale.Core.Workspaces.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nightingale.Core.Models
{
    public class Workspace : ObservableBase, IStorageItem
    {
        public Workspace()
        {
            Items.CollectionChanged += (sender, e) => ItemEventHandlers.CollectionChanged(sender, e, null);
        }

        public string Id { get; set; }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                }
            }
        }

        public string ParentId { get; set; }

        /// <summary>
        /// The list of HTTP methods configured
        /// for this workspace.
        /// </summary>
        public List<string> Methods { get; set; }

        /// <summary>
        /// List of items that were open as a tab at the
        /// time of closing the workspace.
        /// </summary>
        /// <remarks>
        /// The intention of this property is
        /// to provide a way to restore the open tabs
        /// when the customer re-opens Nightingale.
        /// </remarks>
        public List<string> OpenItemIds
        {
            get
            {
                if (_openItemIds == null)
                {
                    _openItemIds = new List<string>();
                }
                return _openItemIds;
            }
            set => _openItemIds = value;
        }
        private List<string> _openItemIds;

        /// <summary>
        /// List of items that are shown as tabs
        /// but are not stored in the Items tree.
        /// </summary>
        public List<Item> TempItems
        {
            get
            {
                if (_tempItems == null)
                {
                    _tempItems = new List<Item>();
                }
                return _tempItems;
            }
            set => _tempItems = value;
        }
        private List<Item> _tempItems;

        /// <summary>
        /// Tree of <see cref="Item"/> in this workspace.
        /// </summary>
        public ObservableCollection<Item> Items { get; } = new ObservableCollection<Item>();

        public ObservableCollection<HistoryItem> HistoryItems { get; } = new ObservableCollection<HistoryItem>();

        public ObservableCollection<Environment> Environments { get; } = new ObservableCollection<Environment>();

        public ObservableCollection<Cookies.Models.Cookie> WorkspaceCookies { get; } = new ObservableCollection<Cookies.Models.Cookie>();

        [JsonIgnore]
        public Item CurrentItem
        {
            get => _currentItem;
            set
            {
                if (value != _currentItem)
                {
                    _currentItem = value;
                    RaisePropertyChanged();
                }
            }
        }
        private Item _currentItem;

        public override string ToString()
        {
            return this.Name;
        }
    }
}
