using CommunityToolkit.Mvvm.ComponentModel;
using JeniusApps.Common.Telemetry;
using Microsoft.Toolkit.Uwp.Helpers;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Settings;
using Nightingale.Handlers;
using Nightingale.Utilities;
using System;
using System.Collections.Generic;

namespace Nightingale.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IRecentUrlCache _recentUrlCache;
    private readonly IUserSettings _userSettings;
    private readonly ITelemetry _telemetry;

    public SettingsViewModel(
        IUserSettings userSettings,
        ITelemetry telemetry)
    {
        _userSettings = userSettings;
        _telemetry = telemetry;
        _timeoutText = _userSettings.Get<double>(SettingsConstants.TimeoutSecondsKey).ToString();
    }

    private bool _loading;
    private int _pivotIndex;
    private bool _deletingPasswords;
    private bool _clearingRecentUrls;
    private bool _clearRecentUrlSuccessful;
    private bool _canClearUrls;

    public event EventHandler SyncStatusUpdated;

    public bool ConfirmDeleteOn
    {
        get => _userSettings.Get<bool>(SettingsConstants.ConfirmDeletion);
        set
        {
            if (value == ConfirmDeleteOn)
            {
                return;
            }

            _userSettings.Set<bool>(SettingsConstants.ConfirmDeletion, value);
            _telemetry.TrackEvent("Settings changed: confirm deletion", new Dictionary<string, string>
            {
                { "Value", value ? "true" : "false" }
            });
        }
    }

    public bool EnvQuickEditOn
    {
        get => _userSettings.Get<bool>(SettingsConstants.EnableEnvQuickEdit);
        set
        {
            if (value == EnvQuickEditOn)
            {
                return;
            }

            _userSettings.Set<bool>(SettingsConstants.EnableEnvQuickEdit, value);
            _telemetry.TrackEvent("Settings changed: EnableEnvQuickEdit", new Dictionary<string, string>
            {
                { "Value", value ? "true" : "false" },
                { "Location", "settings" }
            });
        }
    }

    public bool MvpBadgeEnabled
    {
        get => _userSettings.Get<bool>(SettingsConstants.ShowMvpBadge);
        set
        {
            if (value == MvpBadgeEnabled)
            {
                return;
            }

            _userSettings.Set<bool>(SettingsConstants.ShowMvpBadge, value);
            _telemetry.TrackEvent("Settings changed: ShowMvpBadge", new Dictionary<string, string>
            {
                { "Value", value ? "true" : "false" },
            });
        }
    }

    public bool AutoSaveIntervalEnabled
    {
        get => _userSettings.Get<bool>(SettingsConstants.AutoSaveInterval);
        set
        {
            if (value == AutoSaveIntervalEnabled)
            {
                return;
            }

            _userSettings.Set<bool>(SettingsConstants.AutoSaveInterval, value);
            _telemetry.TrackEvent("Settings changed: AutoSaveInterval", new Dictionary<string, string>
            {
                { "Value", value ? "true" : "false" },
            });
        }
    }

    public bool HistoryEnabled
    {
        get => _userSettings.Get<bool>(SettingsConstants.HistoryEnabled);
        set
        {
            if (value == HistoryEnabled)
            {
                return;
            }

            _userSettings.Set<bool>(SettingsConstants.HistoryEnabled, value);
            _telemetry.TrackEvent("Settings changed: history enabled", new Dictionary<string, string>
            {
                { "Value", value ? "true" : "false" }
            });
        }
    }

    public bool TelemetryOn
    {
        get => _userSettings.Get<bool>(SettingsConstants.TelemetryEnabledKey);
        set
        {
            if (value == TelemetryOn)
                return;

            _telemetry.TrackEvent("Settings changed: telemetry", new Dictionary<string, string>
            {
                { "Value", value.ToString() }
            });
            _userSettings.Set<bool>(SettingsConstants.TelemetryEnabledKey, value);

            ChangeTelemetry(value);
        }
    }

    public bool AutoSaveOn
    {
        get => _userSettings.Get<bool>(SettingsConstants.AutoSaveEnabled);
        set
        {
            if (value == AutoSaveOn)
            {
                return;
            }

            _userSettings.Set<bool>(SettingsConstants.AutoSaveEnabled, value);
            OnPropertyChanged(nameof(AutoSaveOn));

            _telemetry.TrackEvent("Settings changed: Auto save", new Dictionary<string, string>
            {
                { "Value", value.ToString() }
            });
        }
    }

    public bool AlwaysWrapUrlOn
    {
        get => _userSettings.Get<bool>(SettingsConstants.AlwaysWrapURL);
        set
        {
            if (value == AlwaysWrapUrlOn)
            {
                return;
            }

            _userSettings.Set<bool>(SettingsConstants.AlwaysWrapURL, value);
            OnPropertyChanged(nameof(AlwaysWrapUrlOn));

            _telemetry.TrackEvent("Settings changed: always wrap", new Dictionary<string, string>
            {
                { "Value", value.ToString() }
            });
        }
    }

    public bool WordWrapEnabled
    {
        get => _userSettings.Get<bool>(SettingsConstants.WordWrapEditor);
        set
        {
            if (value == WordWrapEnabled)
            {
                return;
            }

            _userSettings.Set<bool>(SettingsConstants.WordWrapEditor, value);
            OnPropertyChanged(nameof(WordWrapEnabled));

            _telemetry.TrackEvent("Settings changed: word wrap editor", new Dictionary<string, string>
            {
                { "Value", value.ToString() }
            });
        }
    }

    public bool SslValidationOn
    {
        get => _userSettings.Get<bool>(SettingsConstants.SslValidationKey);
        set
        {
            if (value == SslValidationOn)
                return;

            _userSettings.Set<bool>(SettingsConstants.SslValidationKey, value);
            _telemetry.TrackEvent("Settings changed: SSL validation", new Dictionary<string, string>
            {
                { "Value", value.ToString() }
            });
            OnPropertyChanged("SslValidation");
        }
    }

    public bool DeletingPasswords
    {
        get => _deletingPasswords;
        set
        {
            _deletingPasswords = value;
            OnPropertyChanged("DeletingPasswords");
            OnPropertyChanged("IsDeletePasswordEnabled");
        }
    }

    public bool IsDeletePasswordEnabled
    {
        get => !DeletingPasswords;
    }

    public int SelectedThemeIndex
    {
        get => (int)UserSettings.GetTheme();
        set
        {
            ThemeController.ChangeTheme((SelectedTheme)value);
            _telemetry.TrackEvent("Theme changed", new Dictionary<string, string>
            {
                { "Theme", ((SelectedTheme)value).ToString() }
            });
        }
    }

    public int PivotIndex
    {
        get => _pivotIndex;
        set
        {
            _pivotIndex = value;
            OnPropertyChanged("PivotIndex");
        }
    }

    //public bool SyncToggle
    //{
    //    get => _syncToggle;
    //    set => SetToggle(value);
    //}

    public bool Loading
    {
        get => _loading;
        set
        {
            _loading = value;
            OnPropertyChanged("Loading");
            SyncStatusUpdated?.Invoke(this, new EventArgs());
        }
    }
    public string AppVersion
    {
        get
        {
            var version = SystemInformation.Instance.ApplicationVersion;
            return $"{version.Major}.{version.Minor}.{version.Build}.{version.Revision}";
        }
    }

    public SettingsViewModel(
        IRecentUrlCache recentUrlCache,
        BackgroundSettingsViewModel backgroundSettingsViewModel)
    {
        BackgroundSettingsViewModel = backgroundSettingsViewModel;
        _recentUrlCache = recentUrlCache ?? throw new ArgumentNullException(nameof(recentUrlCache));
        DeletingPasswords = false;
        CanClearUrls = true;
    }

    public BackgroundSettingsViewModel BackgroundSettingsViewModel { get; set; }

    public bool ClearingRecentUrls
    {
        get => _clearingRecentUrls;
        set
        {
            if (_clearingRecentUrls != value)
            {
                _clearingRecentUrls = value;
                OnPropertyChanged("ClearingRecentUrls");
            }
        }
    }

    public bool ClearRecentUrlSuccessful
    {
        get => _clearRecentUrlSuccessful;
        set
        {
            if (_clearRecentUrlSuccessful != value)
            {
                _clearRecentUrlSuccessful = value;
                OnPropertyChanged("ClearRecentUrlSuccessful");
            }
        }
    }

    public bool CanClearUrls
    {
        get => _canClearUrls;
        set
        {
            if (_canClearUrls != value)
            {
                _canClearUrls = value;
                OnPropertyChanged("CanClearUrls");
            }
        }
    }

    public bool InfiniteTimeoutEnabled
    {
        get => _userSettings.Get<bool>(SettingsConstants.InfiniteTimeoutKey);
        set
        {
            _userSettings.Set<bool>(SettingsConstants.InfiniteTimeoutKey, value);
            OnPropertyChanged(nameof(TimeoutTextEnabled));
        }
    }

    public bool TimeoutTextEnabled => !InfiniteTimeoutEnabled;

    [ObservableProperty]
    private string _timeoutText;

    partial void OnTimeoutTextChanged(string value)
    {
        var currentValue = _userSettings.Get<double>(SettingsConstants.TimeoutSecondsKey);

        if (double.TryParse(value.Trim(), out double result) &&
            result > 0 &&
            result != currentValue)
        {
            _userSettings.Set<double>(SettingsConstants.TimeoutSecondsKey, result);
        }
        else
        {
            TimeoutText = currentValue.ToString();
        }
    }

    public async void ClearRecentUrlsAsync()
    {
        CanClearUrls = false;
        ClearingRecentUrls = true;

        await _recentUrlCache.InitializeAsync();
        await _recentUrlCache.ClearAllUrlsAsync();

        ClearingRecentUrls = false;
        ClearRecentUrlSuccessful = true;
    }

    public async void RateAndReviewAsync()
    {
        await StoreHandler.ShowRatingReviewDialog();
    }

    private void ChangeTelemetry(bool value)
    {
        _telemetry.SetEnabled(value);
    }
}
