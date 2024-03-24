using JeniusApps.Common.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using Nightingale.Core.Helpers;
using Nightingale.Utilities;
using Windows.UI.Xaml;

namespace Nightingale.UserControls;

public sealed partial class StatusBar : ObservableUserControl
{
    public StatusBar()
    {
        this.InitializeComponent();
    }

    public void OpenFlyout() => App.Services.GetRequiredService<ITelemetry>().TrackEvent("Show status flyout clicked");

    public string StatusString
    {
        get { return (string)GetValue(StatusStringProperty); }
        set { SetValueDp(StatusStringProperty, value); }
    }

    public string StatusCode
    {
        get => (string)GetValue(StatusCodeProperty);
        set => StatusUpdated(value);
    }

    private void StatusUpdated(string value)
    {
        SetValueDp(StatusCodeProperty, value);
        StatusCodeInfo = StatusInfo.Codes.ContainsKey(value) 
            ? StatusInfo.Codes[value] 
            : "";
    }

    public string StatusCodeInfo
    {
        get => _statusInfo;
        set
        {
            _statusInfo = value;
            RaisePropertyChanged();
        }
    }
    private string _statusInfo;

    public bool? ResponseSuccessful
    {
        get { return (bool?)GetValue(ResponseSuccessfulProperty); }
        set { SetValueDp(ResponseSuccessfulProperty, value); }
    }

    public bool? TestsAllPass
    {
        get => (bool?)GetValue(TestsAllPassProperty);
        set => SetValueDp(TestsAllPassProperty, value);
    }

    public string TimeElapsedString
    {
        get => (string)GetValue(TimeElapsedStringProperty);
        set => SetValueDp(TimeElapsedStringProperty, value);
    }

    public string SizeString
    {
        get => (string)GetValue(SizeStringProperty);
        set => SetValueDp(SizeStringProperty, value);
    }

    public static readonly DependencyProperty SizeStringProperty = DependencyProperty.Register(
        "SizeString",
        typeof(string),
        typeof(StatusBar),
        null);

    public static readonly DependencyProperty TimeElapsedStringProperty = DependencyProperty.Register(
        "TimeElapsedString",
        typeof(string),
        typeof(StatusBar),
        null);

    public static readonly DependencyProperty TestsAllPassProperty = DependencyProperty.Register(
        "TestsAllPass",
        typeof(bool?),
        typeof(StatusBar),
        null);

    public static readonly DependencyProperty StatusCodeProperty = DependencyProperty.Register(
        "StatusCode",
        typeof(string),
        typeof(StatusBar),
        null);

    public static readonly DependencyProperty StatusStringProperty = DependencyProperty.Register(
        "StatusString",
        typeof(string),
        typeof(StatusBar),
        null);

    public static readonly DependencyProperty ResponseSuccessfulProperty = DependencyProperty.Register(
        "ResponseSuccessful",
        typeof(bool?),
        typeof(StatusBar),
        null);
}
