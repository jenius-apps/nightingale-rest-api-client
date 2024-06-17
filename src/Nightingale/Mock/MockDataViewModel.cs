using JeniusApps.Common.Telemetry;
using Nightingale.Core.Mock.Models;
using Nightingale.Core.Mock.Services;
using Nightingale.Core.Models;
using Nightingale.Utilities;

namespace Nightingale.Mock;

/// <summary>
/// View model for mock data
/// </summary>
public class MockDataViewModel : ObservableBase
{
    private readonly IDeployService _deployService;
    private readonly ITelemetry _telemetry;

    public MockDataViewModel(
        MockData data,
        IDeployService deployService,
        ITelemetry telemetry)
    {
        MockData = data;
        _deployService = deployService;
        _telemetry = telemetry;
    }

    /// <summary>
    /// This viewmodel's instance of mock data.
    /// </summary>
    public MockData MockData { get; }

    /// <summary>
    /// The viewmodel's wrapper around the
    /// mock status code. Wrapper performs string
    /// to int parsing.
    /// </summary>
    public string StatusCode
    {
        get => MockData?.StatusCode?.ToString() ?? "";
        set
        {
            if (MockData == null || value == StatusCode)
            {
                return;
            }

            if (int.TryParse(value, out int result))
            {
                MockData.StatusCode = result;
            }
            else if (value == "")
            {
                MockData.StatusCode = null;
            }

            RaisePropertyChanged();
        }
    }

    public async void DeployServer()
    {
        var deploymentSuccessful = await _deployService.DeployAsync();
        _telemetry.TrackEvent(Telemetry.MockServerDeployed, Telemetry.MockTelemetryProps(deploymentSuccessful, "mockDataView"));
    }
}
