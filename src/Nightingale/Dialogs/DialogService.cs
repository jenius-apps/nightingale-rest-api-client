using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Dialogs;
using Nightingale.Core.Export;
using Nightingale.Core.Models;
using Nightingale.Core.Settings;
using Nightingale.Core.Workspaces.Factories;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Utilities;
using Nightingale.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Dialogs
{
    /// <summary>
    /// Class that provides all the dialogs
    /// used by Nightingale.
    /// </summary>
    public class DialogService : BaseDialogService, IDialogService
    {
        public static string NewRequestDialogTitle = "📝 " + ResourceLoader.GetForCurrentView().GetString("NewRequest/Text");
        public static string NewCollectionDialogTitle = "📂 " + ResourceLoader.GetForCurrentView().GetString("NewCollection/Text");
        public static string EditItemDialogTitle = "✏ " + ResourceLoader.GetForCurrentView().GetString("EditName");
        private readonly IServiceProvider _scope;
        private readonly IUserSettings _userSettings;
        private readonly IItemFactory _itemFactory;
        private readonly ITelemetry _telemetry;

        public DialogService(
            IServiceProvider scope,
            IUserSettings userSettings,
            IItemFactory itemFactory,
            ITelemetry telemetry)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
            _userSettings = userSettings ?? throw new ArgumentNullException(nameof(userSettings));
            _itemFactory = itemFactory ?? throw new ArgumentNullException(nameof(itemFactory));
            _telemetry = telemetry;
        }

        /// <inheritdoc/>
        public async Task OpenSettingsAsync()
        {
            if (IsDialogActive)
            {
                return;
            }

            IsDialogActive = true;
            var dialog = new SettingsDialog();
            await dialog.ShowAsync();
            IsDialogActive = false;
        }

        /// <inheritdoc/>
        public async Task OpenMvpAsync()
        {
            if (IsDialogActive)
            {
                return;
            }

            IsDialogActive = true;
            var dialog = new MvpDialog
            {
                MvpViewModel = _scope.GetRequiredService<MvpViewModel>(),
                RequestedTheme = ThemeController.GetTheme()
            };
            await dialog.ShowAsync();
            IsDialogActive = false;
        }


        /// <inheritdoc/>
        public async Task TutorialAsync()
        {
            if (IsDialogActive)
            {
                return;
            }

            IsDialogActive = true;
            var dialog = new TutorialDialog
            {
                RequestedTheme = ThemeController.GetTheme()
            };
            await dialog.ShowAsync();
            IsDialogActive = false;
        }

        /// <inheritdoc/>
        public async Task<bool> MockServerDialogAsync(
            string workspaceName,
            string itemName,
            string envName)
        {
            if (IsDialogActive)
            {
                return false;
            }

            IsDialogActive = true;
            var dialog = new MockServerDialog()
            {
                WorkspaceName = workspaceName,
                ItemName = itemName,
                EnvName = envName,
                RequestedTheme = ThemeController.GetTheme()
            };

            var result = await dialog.ShowAsync();
            IsDialogActive = false;
            return result == ContentDialogResult.Primary;
        }

        private async Task<(bool, string, Item)> ItemDialogAsync(
            string dialogTitle, 
            IList<Item> rootWorkspaceItems, 
            string prepopulatedName = "", 
            Item prepopulatedParent = null)
        {
            if (IsDialogActive)
            {
                return (false, "", null);
            }

            IsDialogActive = true;
            var dialog = new NewRequestDialog();
            dialog.DialogTitle = dialogTitle;
            dialog.NewName = prepopulatedName;

            if (rootWorkspaceItems != null)
            {
                dialog.IsLocationSelectorVisible = true;

                // Prepare the tree of locations
                Item rootItem = _itemFactory.Create(ItemType.Collection, "Root", childrenObservable: false);
                rootItem.IsExpanded = true;

                // Add the real list of workspace items into the fake root
                foreach (var child in rootWorkspaceItems)
                {
                    rootItem.Children.Add(child);
                }

                // Encapsulate the items with a shallow reference
                // instead of using a deep clone. Because if deep clone is used,
                // we lose the reference to the original workspace tree item, and thus 
                // we will return an item that doesn't actually exist in the workspace.
                dialog.Locations = new List<ItemShallowReference> { rootItem.ShallowReference() };

                // TODO if a populated parent is provided, then select the item in the tree.
            }

            await dialog.ShowAsync();

            Item parent;
            if (rootWorkspaceItems == null && prepopulatedParent != null)
            {
                parent = prepopulatedParent;
            }
            // If the selected parent is the root, then the parent is set to null
            else if (dialog.Locations?.FirstOrDefault() == null || dialog.SelectedLocation == dialog.Locations[0])
            {
                parent = null;
            }
            else
            {
                parent = dialog.SelectedLocation?.Reference;
            }

            IsDialogActive = false;
            return (dialog.Create, dialog.NewName, parent);
        }

        /// <inheritdoc/>
        public async Task<bool> ConfirmParameterOverwriteAsync(Parameter p)
        {
            if (IsDialogActive || p == null)
            {
                return false;
            }

            IsDialogActive = true;
            var dialog = new ContentDialog()
            {
                Title = "Key Collision",
                Content = $"There is already a variable with the key \"{p.Key}\". Its current value is \"{p.Value}\". " +
                    $"Do you want to overwrite the value?",
                PrimaryButtonText = "Yes",
                CloseButtonText = "No",
                CornerRadius = new Windows.UI.Xaml.CornerRadius(8)
            };

            var result = await dialog.ShowAsync();

            IsDialogActive = false;
            return result == ContentDialogResult.Primary;
        }

        /// <inheritdoc/>
        public async Task<(bool, string, Item)> NewItemDialogAsync(
            ItemType type,
            IList<Item> rootItems = null,
            Item currentParent = null,
            string prepopulatedName = null)
        {
            if (type == ItemType.None)
            {
                return (false, "", null);
            }

            var dialogTitle = type == ItemType.Request
                ? NewRequestDialogTitle
                : NewCollectionDialogTitle;

            (var confirmed, var name, var parent) = await ItemDialogAsync(
                dialogTitle,
                rootItems,
                prepopulatedName: prepopulatedName ?? "Untitled",
                prepopulatedParent: currentParent);

            return (confirmed, name, parent);
        }

        /// <inheritdoc/>
        public async Task<(bool, string)> EditItemDialogAsync(string itemName)
        {
            (var confirmed, var newName, _) = await ItemDialogAsync(
                EditItemDialogTitle,
                null,
                itemName);

            return (confirmed, newName);
        }

        /// <inheritdoc/>
        public async Task<bool> ConfirmDeleteAllAsync()
        {
            if (IsDialogActive)
            {
                return false;
            }

            IsDialogActive = true;
            var dialog = new ContentDialog()
            {
                RequestedTheme = ThemeController.GetTheme(),
                Title = "⚠ Delete all",
                Content = "Are you sure you want to delete all?",
                CloseButtonText = "Cancel",
                PrimaryButtonText = "Delete All",
                CornerRadius = new Windows.UI.Xaml.CornerRadius(8)
            };

            var result = await dialog.ShowAsync();
            IsDialogActive = false;
            return result == ContentDialogResult.Primary;
        }

        /// <inheritdoc/>
        public async Task<bool> ConfirmDeleteAsync(bool forceConfirm = false)
        {
            if (IsDialogActive)
            {
                return false;
            }

            if (!forceConfirm)
            {
                bool confirmDeletion = await _userSettings.GetAsync<bool>(SettingsConstants.ConfirmDeletion);
                if (!confirmDeletion)
                {
                    return true;
                }
            }

            IsDialogActive = true;
            var dialog = new ConfirmDeleteDialog()
            {
                RequestedTheme = ThemeController.GetTheme(),
                ShowCheckBox = forceConfirm ? false : true
            };
            var result = await dialog.ShowAsync();

            if (dialog.DeleteWithoutAsking)
            {
                _telemetry.TrackEvent(Telemetry.DeleteWithoutAskingChecked);
                await _userSettings.SetAsync(SettingsConstants.ConfirmDeletion, false);
            }

            IsDialogActive = false;
            return result == ContentDialogResult.Primary;
        }

        /// <inheritdoc/>
        public async Task<(bool, string)> WorkspaceDialogAsync(string prepopulatedName = null)
        {
            (var confirm, var name, _) = await ItemDialogAsync(
                "🗃️ Workspace",
                null,
                prepopulatedName);

            return (confirm, name);
        }

        /// <inheritdoc/>
        public async Task<(IList<Item>, IList<Workspace>)> ImportDialogAsync()
        {
            if (IsDialogActive)
            {
                return (null, null);
            }

            IsDialogActive = true;
            var dialog = new ImportDialog();
            dialog.ViewModel = _scope.GetRequiredService<ImportPostmanViewModel>();
            await dialog.ShowAsync();
            IsDialogActive = false;

            return (dialog.ImportedCollections, dialog.ImportedWorkspaces);
        }

        /// <inheritdoc/>
        public async Task<ExportConfiguration> ExportDialogAsync(IList<Workspace> workspaces)
        {
            if (workspaces == null 
                || IsDialogActive
                || workspaces.Count == 0)
            {
                return null;
            }

            IsDialogActive = true;
            var dialog = new ExportDialog
            {
                RequestedTheme = ThemeController.GetTheme()
            };
            dialog.Load(workspaces);
            var dialogResult = await dialog.ShowAsync();

            IsDialogActive = false;
            return dialogResult == ContentDialogResult.Primary
                ? dialog.ExportConfiguration
                : null;
        }
    }
}
