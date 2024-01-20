using Newtonsoft.Json;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Mock.Models;
using Nightingale.Core.Models;
using Nightingale.Core.Workspaces.EventHandlers;
using Nightingale.Core.Workspaces.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nightingale.Core.Workspaces.Models
{
    /// <summary>
    /// Enum for the type of item
    /// this should be displayed in the tree.
    /// </summary>
    public enum ItemType
    {
        None,
        Request,
        Collection
    }

    /// <summary>
    /// This workspace item represents an object that
    /// can be displayed in the tree of the workspace.
    /// </summary>
    public class Item : ObservableBase, IEquatable<Item>, IComparable<Item>, IDeepCloneable
    {
        public Item()
        {
            // This updates the Parent properties
            // of all children added to this Item.
            Children.CollectionChanged += (sender, e) => ItemEventHandlers.CollectionChanged(sender, e, this);
        }

        public Item(bool childenObservable)
        {
            if (childenObservable)
            {
                // This updates the Parent properties
                // of all children added to this Item.
                Children.CollectionChanged += (sender, e) => ItemEventHandlers.CollectionChanged(sender, e, this);
            }
        }

        [JsonIgnore]
        public Item Parent { get; set; }

        /// <summary>
        /// GUID for this item.
        /// </summary>
        public string Id
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_id))
                {
                    _id = Guid.NewGuid().ToString();
                }

                return _id;
            }
            set => _id = value;
        }
        private string _id;

        /// <summary>
        /// If true, the item is just a tab
        /// and it hasn't been saved to the workspace item tree.
        /// </summary>
        public bool IsTemporary
        {
            get => _isTemporary;
            set
            {
                if (_isTemporary != value)
                {
                    _isTemporary = value;
                    RaisePropertyChanged();
                }
            }
        }
        private bool _isTemporary;

        /// <summary>
        /// A dictionary of properties usually
        /// used by a GUI app.
        /// </summary>
        public Dictionary<string, object> Properties
        {
            get
            {
                if (_properties == null) _properties = new Dictionary<string, object>();
                return _properties;
            }
        }
        private Dictionary<string, object> _properties;

        public Url Url
        {
            get
            {
                if (_url == null) _url = new Url();
                return _url;
            }
            set => _url = value;
        }
        private Url _url;

        public Authentication Auth
        {
            get
            {
                if (_auth == null) _auth = new Authentication();
                return _auth;
            }
            set => _auth = value;
        }
        private Authentication _auth;

        public RequestBody Body
        {
            get
            {
                if (_body == null) _body = new RequestBody();
                return _body;
            }
            set => _body = value;
        }
        private RequestBody _body; 

        public MockData MockData
        {
            get
            {
                if (_mockData == null) _mockData = new MockData();
                return _mockData;
            }
            set => _mockData = value;
        }
        private MockData _mockData;

        public ObservableCollection<Item> Children
        {
            get
            {
                if (_children == null) _children = new ObservableCollection<Item>();
                return _children;
            }
        }
        private ObservableCollection<Item> _children;

        public ObservableCollection<Parameter> Headers
        {
            get
            {
                if (_headers == null) _headers = new ObservableCollection<Parameter>();
                return _headers;
            }
        }
        private ObservableCollection<Parameter> _headers;

        public ObservableCollection<Parameter> ChainingRules
        {
            get
            {
                if (_chainingRules == null) _chainingRules = new ObservableCollection<Parameter>();
                return _chainingRules;
            }
        }
        private ObservableCollection<Parameter> _chainingRules;

        private ItemType _type;
        public ItemType Type
        {
            get => _type;
            set
            {
                if (value != _type)
                {
                    _type = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                RaisePropertyChanged();
            }
        }


        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (value != _isExpanded)
                {
                    _isExpanded = value;
                    RaisePropertyChanged();
                }
            }
        }

        private string _method;
        public string Method
        {
            get => _method;
            set
            {
                _method = value;
                RaisePropertyChanged();
            }
        }

        private WorkspaceResponse _response;
        public WorkspaceResponse Response
        {
            get => _response;
            set
            {
                _response = value;
                RaisePropertyChanged();
            }
        }

        public override int GetHashCode()
        {
            return Method.GetHashCode() * 0x00010000 
                * Name.GetHashCode() * 0x00010000 
                * Type.GetHashCode();
        }

        public override string ToString()
        {
            return Name;
        }

        public int CompareTo(Item other)
        {
            if (other is null) return 1;

            return Name.CompareTo(other.Name);
        }

        public override bool Equals(object obj)
        {
            return this.Equals(obj as Item);
        }

        public bool Equals(Item other)
        {
            if (other is null)
            {
                return false;
            }

            return this.Id == other.Id;
        }

        /// <inheritdoc/>
        public virtual object DeepClone()
        {
            var other = new Item
            {
                Parent = this.Parent,
                Url = this.Url?.DeepClone() as Url,
                Auth = this.Auth?.DeepClone() as Authentication,
                Body = this.Body?.DeepClone() as RequestBody,
                Response = this.Response?.DeepClone() as WorkspaceResponse,
                Type = this.Type,
                Name = this.Name,
                IsExpanded = this.IsExpanded,
                Method = this.Method
            };
            other.Headers.DeepClone(this.Headers);
            other.ChainingRules.DeepClone(this.ChainingRules);
            other.Children.DeepClone(this.Children);
            return other;
        }

        /// <summary>
        /// Creates an <see cref="ItemShallowReference"/>.
        /// </summary>
        public ItemShallowReference ShallowReference()
        {
            return new ItemShallowReference(this);
        }

        public static bool operator ==(Item left, Item right)
        {
            // Check for null on left side.
            if (left is null)
            {
                if (right is null)
                {
                    // null == null = true.
                    return true;
                }

                // Only the left side is null.
                return false;
            }

            return left.Equals(right);
        }

        public static bool operator !=(Item left, Item right)
        {
            return !(left == right);
        }
    }
}
