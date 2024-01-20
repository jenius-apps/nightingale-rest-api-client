using Microsoft.AppCenter.Analytics;
using Nightingale.UserControls;
using Nightingale.Utilities;
using System;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.Mock
{
    public sealed partial class MockDataView : ObservableUserControl
    {
        public MockDataView()
        {
            this.InitializeComponent();
        }

        public MockDataViewModel ViewModel
        {
            get => (MockDataViewModel)GetValue(ViewModelProperty);
            set => SetValueDp(ViewModelProperty, value);
        }

        // Required for updating data when tab switching
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(MockDataViewModel),
            typeof(MockDataView),
            null);

        private void EditorControl_EditorTextChanged(object sender, EventArgs e)
        {
            if (sender is EditorControl ec && ViewModel?.MockData != null)
            {
                ViewModel.MockData.Body = ec.Text;
            }
        }

        private void HyperlinkButton_Click(object sender, RoutedEventArgs e)
        {
            Analytics.TrackEvent(Telemetry.Docs, Telemetry.DocsTelemetryProps(Telemetry.MockDocs));
        }
    }
}
