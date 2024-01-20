using Nightingale.Core.Models;
using Nightingale.Utilities;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class CollectionControl : ObservableUserControl
    {
        public event EventHandler RunAllClicked;

        public CollectionControl()
        {
            this.InitializeComponent();
        }

        public void RunAllRequests() => RunAllClicked?.Invoke(this, new EventArgs());

        public object Children
        {
            get => GetValue(ChildrenProperty);
            set => SetValueDp(ChildrenProperty, value);
        }

        public bool IsEmpty
        {
            get => (bool)GetValue(IsEmptyProperty);
            set => SetValueDp(IsEmptyProperty, value);
        }

        public long TotalElapsedMilliseconds
        {
            get => (long)GetValue(TotalElapsedMillisecondsProperty);
            set => SetValueDp(TotalElapsedMillisecondsProperty, value);
        }

        public int TotalPassed
        {
            get => (int)GetValue(TotalPassedProperty);
            set => SetValueDp(TotalPassedProperty, value);
        }

        public int TotalFailed
        {
            get => (int)GetValue(TotalFailedProperty);
            set => SetValueDp(TotalFailedProperty, value);
        }

        public int TotalRequestCount
        {
            get => (int)GetValue(TotalRequestCountProperty);
            set => SetValueDp(TotalRequestCountProperty, value);
        }

        public bool IsLoadingRingActive
        {
            get => (bool)GetValue(IsLoadingRingActiveProperty);
            set => SetValueDp(IsLoadingRingActiveProperty, value);
        }

        public bool ExecuteButtonVisible
        {
            get => (bool)GetValue(ExecuteButtonVisibleProperty);
            set => SetValueDp(ExecuteButtonVisibleProperty, value);
        }

        public bool CancelButtonVisible
        {
            get => (bool)GetValue(CancelButtonVisibleProperty);
            set => SetValueDp(CancelButtonVisibleProperty, value);
        }

        public static readonly DependencyProperty CancelButtonVisibleProperty = DependencyProperty.Register(
            "CancelButtonVisible",
            typeof(bool),
            typeof(CollectionControl),
            null);

        public static readonly DependencyProperty ExecuteButtonVisibleProperty = DependencyProperty.Register(
            "ExecuteButtonVisible",
            typeof(bool),
            typeof(CollectionControl),
            null);

        public static readonly DependencyProperty IsLoadingRingActiveProperty = DependencyProperty.Register(
            "IsLoadingRingActive",
            typeof(bool),
            typeof(CollectionControl),
            null);

        public static readonly DependencyProperty ChildrenProperty = DependencyProperty.Register(
            "Children",
            typeof(object),
            typeof(CollectionControl),
            null);

        public static readonly DependencyProperty IsEmptyProperty = DependencyProperty.Register(
            "IsEmpty",
            typeof(bool),
            typeof(CollectionControl),
            null);

        public static readonly DependencyProperty TotalRequestCountProperty = DependencyProperty.Register(
            "TotalRequestCount",
            typeof(int),
            typeof(CollectionControl),
            null);

        public static readonly DependencyProperty TotalElapsedMillisecondsProperty = DependencyProperty.Register(
            "TotalElapsedMilliseconds",
            typeof(long),
            typeof(CollectionControl),
            null);

        public static readonly DependencyProperty TotalPassedProperty = DependencyProperty.Register(
            "TotalPassed",
            typeof(int),
            typeof(CollectionControl),
            null);

        public static readonly DependencyProperty TotalFailedProperty = DependencyProperty.Register(
            "TotalFailed",
            typeof(int),
            typeof(CollectionControl),
            null);
    }
}
