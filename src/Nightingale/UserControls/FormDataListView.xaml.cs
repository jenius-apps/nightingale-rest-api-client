using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Models;
using Nightingale.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Nightingale.UserControls;

public sealed partial class FormDataListView : ObservableUserControl
{
    private FormData _selectedItem;

    public event TypedEventHandler<object, FormData> SelectFilesClicked;

    public FormDataListView()
    {
        this.InitializeComponent();
    }

    public ObservableCollection<FormData> ItemsSource
    {
        get => (ObservableCollection<FormData>)GetValue(ItemsSourceProperty);
        set
        {
            if (value != null && value.Count == 0)
            {
                value.Add(new FormData(enabled: true));
            }

            SetValueDp(ItemsSourceProperty, value);
        }
    }

    public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.Register(
        "ItemsSource",
        typeof(ObservableCollection<FormData>),
        typeof(FormDataListView),
        null);

    /// <remarks>
    /// This method updates the data type of the associated FormData.
    /// </remarks>
    private void MenuFlyoutItem_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedItem == null)
        {
            return;
        }

        if (sender is MenuFlyoutItem item 
            && Enum.TryParse(item.Tag.ToString(), out FormDataType formDataType))
        {
            _selectedItem.FormDataType = formDataType;

            App.Services.GetRequiredService<ITelemetry>().TrackEvent("Form data type changed", new Dictionary<string, string>
            {
                { "Form data type", formDataType.ToString() }
            });
        }
    }

    /// <remarks>
    /// This method caches the selected FormData for later use.
    /// </remarks>
    private void DropDownButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is FormData item)
        {
            _selectedItem = item;
        }
    }

    private void DeleteFormData(object sender, RoutedEventArgs e)
    {
        if (ItemsSource == null)
        {
            return;
        }

        if (sender is FrameworkElement fe && fe.DataContext is FormData item)
        {
            ItemsSource.Remove(item);

            if (ItemsSource.Count == 0)
            {
                ItemsSource.Add(new FormData(enabled: true));
            }

            App.Services.GetRequiredService<ITelemetry>().TrackEvent("Form data deleted");
        }
    }

    private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (ItemsSource == null)
        {
            return;
        }

        if (sender is TextBox t
            && t.DataContext is FormData f
            && t.Text != ""
            && ItemsSource.LastOrDefault() == f)
        {
            ItemsSource.Add(new FormData(enabled: true));
        }
    }

    private void SelectFilesButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is FormData item)
        {
            SelectFilesClicked?.Invoke(sender, item);
        }
    }

    private void ClearFilesClicked(object sender, RoutedEventArgs e)
    {
        if (sender is FrameworkElement fe && fe.DataContext is FormData item)
        {
            App.Services.GetRequiredService<ITelemetry>().TrackEvent("Form data files cleared", new Dictionary<string, string>
            {
                { "Number of files", item.FilePaths == null ? "0" : item.FilePaths.Count.ToString() }
            });

            item.FilePaths = new List<string>();
            item.AutoContentType = null;
        }
    }

    // Used for auto expand
    private void TextBoxFocused(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox b)
        {
            b.TextWrapping = TextWrapping.Wrap;
        }
    }

    // Used for auto expand
    private void TextBoxLostFocus(object sender, RoutedEventArgs e)
    {
        if (sender is TextBox b)
        {
            b.TextWrapping = TextWrapping.NoWrap;
        }
    }
}
