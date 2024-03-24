using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Settings;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Handlers;
using Nightingale.Navigation;
using Nightingale.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Navigation;

#nullable enable

namespace Nightingale.Views;

public sealed partial class WorkspacePage : Page
{
    private bool historyPivotFirstVisit = true;

    public WorkspacePage()
    {
        this.InitializeComponent();
    }

    public WorkspaceViewModel? ViewModel { get; set; }

    public History.HistoryViewModel? HistoryViewModel { get; set; }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        if (e.Parameter is WorkspaceNavigationArgs args)
        {
            ViewModel = args.WorkspaceViewModel;
            ViewModel.SetWorkspaceItemFrame(this.ItemFrame);
            HistoryViewModel = args.HistoryViewModel;
        }
    }

    private void Grid_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (e.PreviousSize.Width == 0 || e.PreviousSize.Height == 0 || e.NewSize.Width == 0)
        {
            return;
        }

        if (e.NewSize.Width != e.PreviousSize.Width)
        {
            var inputWidth = SideBarColumn.Width.Value;
            if (!double.IsNaN(inputWidth))
            {
                App.Services.GetRequiredService<IUserSettings>().Set<double>(SettingsConstants.SidebarWidth, inputWidth);
            }
        }
    }

    private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems[0] is PivotItem p && p.Tag.ToString() == "HistoryPivot")
        {
            // Only refresh history list after the first visit
            if (!historyPivotFirstVisit)
            {
                HistoryViewModel?.RefreshAsync();
            }
            else
            {
                historyPivotFirstVisit = false;
            }
        }
    }

    private void ListView_RightTapped(object sender, Windows.UI.Xaml.Input.RightTappedRoutedEventArgs e)
    {
        if (e.PointerDeviceType != Windows.Devices.Input.PointerDeviceType.Mouse)
        {
            return;
        }

        if (sender is UIElement ui)
        {
            HistoryListFlyout.ShowAt(ui, e.GetPosition(ui));
        }

        if (HistoryViewModel is not null && e.OriginalSource is FrameworkElement item && item.DataContext is HistoryItem i)
        {
            HistoryViewModel.ContextSelectedItem = i;
        }
    }

    private void ListView_Holding(object sender, Windows.UI.Xaml.Input.HoldingRoutedEventArgs e)
    {
        if (e.HoldingState != Windows.UI.Input.HoldingState.Started)
        {
            return;
        }

        if (sender is UIElement ui)
        {
            HistoryListFlyout.ShowAt(ui, e.GetPosition(ui));
        }

        if (HistoryViewModel is not null && e.OriginalSource is FrameworkElement item && item.DataContext is HistoryItem i)
        {
            HistoryViewModel.ContextSelectedItem = i;
        }
    }

    private void ListView_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        if (e.OriginalSource is FrameworkElement fe
            && fe.DataContext is Item hr)
        {
            ViewModel.InvokeItem(sender, new CustomEventArgs.AddedItemArgs<object>(hr));
        }
    }

    private void MainGridSizeChanged(object sender, SizeChangedEventArgs e)
    {
        if (ViewModel is null)
        {
            return;
        }

        if (e.PreviousSize.Width < 600 && e.NewSize.Width >= 600 && !ViewModel.IsSideBarVisible)
        {
            ViewModel.IsSideBarVisible = true;
            ViewModel.SetTempSinglePane(false);
        }
        else if (e.NewSize.Width <= 600 && e.PreviousSize.Width > 600 && ViewModel.IsSideBarVisible)
        {
            ViewModel.IsSideBarVisible = false;
            ViewModel.SetTempSinglePane(true); // auto switch to single pane when window is narrow
        }
    }

    private void EnvQuickEditClicked(object sender, RoutedEventArgs e)
    {
        if (App.Services.GetRequiredService<IUserSettings>().Get<bool>(SettingsConstants.EnableEnvQuickEdit))
        {
            if (sender is FrameworkElement fe)
            {
                FlyoutBase.ShowAttachedFlyout(fe);
            }
        }
        else
        {
            ViewModel?.OpenEnvironmentManagerDialog();
        }
    }

    private void OpenEnvManagerClicked(object sender, RoutedEventArgs e)
    {
        EnvFlyout.Hide();
        ViewModel?.OpenEnvironmentManagerDialog();
    }

    private async void SaveTabClicked(object sender, RoutedEventArgs e)
    {
        if (ViewModel is not null && sender is FrameworkElement fe && fe.DataContext is RequestViewModel r)
        {
            await ViewModel.SaveTempItemAsync(r.ViewModel?.Request);
        }
    }
}
