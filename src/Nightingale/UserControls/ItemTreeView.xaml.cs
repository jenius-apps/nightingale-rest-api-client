using Nightingale.Core.Export;
using Nightingale.Core.Workspaces.Models;
using Nightingale.CustomEventArgs;
using Nightingale.Utilities;
using System;
using Windows.Devices.Input;
using Windows.Foundation.Metadata;
using Windows.UI.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using MUXC = Microsoft.UI.Xaml.Controls;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class ItemTreeView : ObservableUserControl
    {
        public event EventHandler<AddedItemArgs<object>> CloneItemClicked;
        public event EventHandler<AddedItemArgs<object>> DeployServerClicked;
        public event EventHandler<AddedItemArgs<object>> EditItemClicked;
        public event EventHandler<AddedItemArgs<object>> DeleteItemClicked;
        public event EventHandler<AddedItemArgs<object>> AddRequestToParentClicked;
        public event EventHandler<AddedItemArgs<object>> AddCollectionToParentClicked;
        public event EventHandler<AddedItemArgs<object>> AddRequestToRootClicked;
        public event EventHandler<AddedItemArgs<object>> AddCollectionToRootClicked;
        public event EventHandler<AddedItemArgs<object>> GenerateCodeClicked;
        public event EventHandler<AddedItemArgs<object>> ItemInvoked;
        public event EventHandler<ExportConfiguration> ExportClicked;
        public event EventHandler DeleteAllClicked;

        private object _contextSelectedItem;

        public ItemTreeView()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// List of hierarchical objects 
        /// used for the treeview.
        /// </summary>
        public object ItemsSource
        {
            get => GetValue(ItemsSourceProperty);
            set => SetValueDp(ItemsSourceProperty, value);
        }

        /// <summary>
        /// The currently selected item. Supports
        /// two way binding.
        /// </summary>
        public object SelectedItem
        {
            get => GetValue(SelectedItemProperty);
            set
            {
                // TODO https://github.com/jenius-apps/nightingale-rest-api-client/issues/32
                //UpdateSelectedNode(value);
                SetValueDp(SelectedItemProperty, value);
            }
        }

        public bool ContextMenuEnabled
        {
            get => (bool)GetValue(ContextMenuProperty);
            set => SetValueDp(ContextMenuProperty, value);
        }

        public bool ReorderEnabled
        {
            get => (bool)GetValue(ReorderEnabledProperty);
            set => SetValueDp(ReorderEnabledProperty, value);
        }

        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
            "ItemsSource",
            typeof(object),
            typeof(ItemTreeView),
            null);

        public static readonly DependencyProperty ContextMenuProperty = DependencyProperty.Register(
            "ContextMenuEnabled",
            typeof(bool),
            typeof(ItemTreeView),
            null);

        public static readonly DependencyProperty ReorderEnabledProperty = DependencyProperty.Register(
            "ReorderEnabled",
            typeof(bool),
            typeof(ItemTreeView),
            null);

        public static readonly DependencyProperty SelectedItemProperty = DependencyProperty.Register(
            "SelectedItem",
            typeof(object),
            typeof(ItemTreeView),
            null);

        /// <summary>
        /// Pop the menu content when the user right clicks with mouse on the tree.
        /// </summary>
        private void ItemRightTapped(object sender, RightTappedRoutedEventArgs e)
        {
            if (e.PointerDeviceType != PointerDeviceType.Mouse || !ContextMenuEnabled)
                return;

            var uiElement = (MUXC.TreeView)sender;

            /// Determine if using request or collection flyout
            MenuFlyout flyoutToUse = DetermineTreeFlyout((FrameworkElement)e.OriginalSource);
            flyoutToUse?.ShowAt(uiElement, e.GetPosition(uiElement));
        }

        /// <summary>
        /// Pop the menu content when the user right clicks with touch on the tree.
        /// </summary>
        private void ItemHolding(object sender, HoldingRoutedEventArgs e)
        {
            if (e.HoldingState != HoldingState.Started || !ContextMenuEnabled)
                return;

            var uiElement = (MUXC.TreeView)sender;

            /// Determine if using request or collection flyout
            MenuFlyout flyoutToUse = DetermineTreeFlyout((FrameworkElement)e.OriginalSource);
            flyoutToUse?.ShowAt(uiElement, e.GetPosition(uiElement));
        }

        /// <summary>
        /// Explores original source to determine whether
        /// a request or a collection was selected in treeview.
        /// Then returns the corresponding flyout.
        /// </summary>
        private MenuFlyout DetermineTreeFlyout(FrameworkElement f)
        {
            if (f == null)
            {
                return null;
            }

            MenuFlyout result = null;

            if (f.DataContext is Item item)
            {
                _contextSelectedItem = item;

                if (item.Type == ItemType.Request)
                {
                    result = RequestsListFlyout;
                }
                else if (item.Type == ItemType.Collection)
                {
                    result = CollectionListFlyout;
                }
            }
            else if (f.DataContext == null)
            {
                result = RootFlyout;
            }

            return result;
        }

        /// <summary>
        /// Flag for whether or not this instance is running
        /// with Full Trust component available.
        /// </summary>
        private bool IsFullTrustAvailable => ApiInformation.IsApiContractPresent("Windows.ApplicationModel.FullTrustAppContract", 1, 0);

        private void InvokeItem(MUXC.TreeView sender, MUXC.TreeViewItemInvokedEventArgs args) 
        {
            ItemInvoked?.Invoke(this, new AddedItemArgs<object>(args.InvokedItem));
        }
        private void CloneItem(object sender, RoutedEventArgs e) => CloneItemClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void DeployServer(object sender, RoutedEventArgs e) => DeployServerClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void EditItem(object sender, RoutedEventArgs e) => EditItemClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void DeleteItem(object sender, RoutedEventArgs e) => DeleteItemClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void AddRequestToParent(object sender, RoutedEventArgs e) => AddRequestToParentClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void AddCollectionToParent(object sender, RoutedEventArgs e) => AddCollectionToParentClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void AddRequestToRoot(object sender, RoutedEventArgs e) => AddRequestToRootClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void AddCollectionToRoot(object sender, RoutedEventArgs e) => AddCollectionToRootClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void GenerateCode(object sender, RoutedEventArgs e) => GenerateCodeClicked?.Invoke(this, new AddedItemArgs<object>(_contextSelectedItem));
        private void DeleteAll(object sender, RoutedEventArgs e) => DeleteAllClicked?.Invoke(this, new EventArgs());

        private void ExportAs(object sender, RoutedEventArgs e)
        {
            if (_contextSelectedItem is Item i && 
                i.Type == ItemType.Collection &&
                sender is MenuFlyoutItem f && 
                Enum.TryParse(f.Text, true, out ExportFormat format))
            {
                var config = new ExportConfiguration
                {
                    Format = format,
                    Scope = ExportScope.Collection,
                    CollectionToExport = i
                };

                ExportClicked?.Invoke(this, config);
            }
        }

        private void NodeExpanding(MUXC.TreeView sender, MUXC.TreeViewExpandingEventArgs args)
        {
            if (args.Node.Content is Item item)
            {
                item.IsExpanded = true;
            }
        }

        private void NodeCollapsing(MUXC.TreeView sender, MUXC.TreeViewCollapsedEventArgs args)
        {
            if (args.Node.Content is Item item)
            {
                item.IsExpanded = false;
            }
        }

        private void ItemsTree_ContextRequested(UIElement sender, ContextRequestedEventArgs args)
        {

        }
    }
}
