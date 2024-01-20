using Microsoft.AppCenter.Analytics;
using Nightingale.Core.Mock.Models;
using Nightingale.Core.Mock.Services;
using Nightingale.Core.Models;
using Nightingale.Utilities;
using System;
using System.Collections.Generic;

namespace Nightingale.Mock
{
    /// <summary>
    /// View model for mock data
    /// </summary>
    public class MockDataViewModel : ObservableBase
    {
        private readonly IDeployService _deployService;

        public MockDataViewModel(
            MockData data,
            IDeployService deployService)
        {
            MockData = data
                ?? throw new ArgumentNullException(nameof(data));
            _deployService = deployService
                ?? throw new ArgumentNullException(nameof(deployService));
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
            Analytics.TrackEvent(Telemetry.MockServerDeployed, Telemetry.MockTelemetryProps(deploymentSuccessful, "mockDataView"));
        }

    }
}
