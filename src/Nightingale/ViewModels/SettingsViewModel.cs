using Microsoft.AppCenter.Analytics;
using Microsoft.Toolkit.Uwp.Helpers;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Settings;
using Nightingale.Handlers;
using Nightingale.Utilities;
using System;
using System.Collections.Generic;

namespace Nightingale.ViewModels
{
    public class SettingsViewModel : ViewModelBase
    {
        private readonly IRecentUrlCache _recentUrlCache;

        private bool _loading;
        private int _pivotIndex;
        private bool _deletingPasswords;
        private bool _clearingRecentUrls;
        private bool _clearRecentUrlSuccessful;
        private bool _canClearUrls;

        public event EventHandler SyncStatusUpdated;

        public bool ConfirmDeleteOn
        {
            get => UserSettings.Get<bool>(SettingsConstants.ConfirmDeletion);
            set
            {
                if (value == ConfirmDeleteOn)
                {
                    return;
                }

                UserSettings.Set<bool>(SettingsConstants.ConfirmDeletion, value);
                Analytics.TrackEvent("Settings changed: confirm deletion", new Dictionary<string, string>
                {
                    { "Value", value ? "true" : "false" }
                });
            }
        }

        public bool EnvQuickEditOn
        {
            get => UserSettings.Get<bool>(SettingsConstants.EnableEnvQuickEdit);
            set
            {
                if (value == EnvQuickEditOn)
                {
                    return;
                }

                UserSettings.Set<bool>(SettingsConstants.EnableEnvQuickEdit, value);
                Analytics.TrackEvent("Settings changed: EnableEnvQuickEdit", new Dictionary<string, string>
                {
                    { "Value", value ? "true" : "false" },
                    { "Location", "settings" }
                });
            }
        }

        public bool MvpBadgeEnabled
        {
            get => UserSettings.Get<bool>(SettingsConstants.ShowMvpBadge);
            set
            {
                if (value == MvpBadgeEnabled)
                {
                    return;
                }

                UserSettings.Set<bool>(SettingsConstants.ShowMvpBadge, value);
                Analytics.TrackEvent("Settings changed: ShowMvpBadge", new Dictionary<string, string>
                {
                    { "Value", value ? "true" : "false" },
                });
            }
        }

        public bool AutoSaveIntervalEnabled
        {
            get => UserSettings.Get<bool>(SettingsConstants.AutoSaveInterval);
            set
            {
                if (value == AutoSaveIntervalEnabled)
                {
                    return;
                }

                UserSettings.Set<bool>(SettingsConstants.AutoSaveInterval, value);
                Analytics.TrackEvent("Settings changed: AutoSaveInterval", new Dictionary<string, string>
                {
                    { "Value", value ? "true" : "false" },
                });
            }
        }

        public bool HistoryEnabled
        {
            get => UserSettings.Get<bool>(SettingsConstants.HistoryEnabled);
            set
            {
                if (value == HistoryEnabled)
                {
                    return;
                }

                UserSettings.Set<bool>(SettingsConstants.HistoryEnabled, value);
                Analytics.TrackEvent("Settings changed: history enabled", new Dictionary<string, string>
                {
                    { "Value", value ? "true" : "false" }
                });
            }
        }

        public bool TelemetryOn
        {
            get => UserSettings.Get<bool>(SettingsConstants.TelemetryEnabledKey);
            set
            {
                if (value == TelemetryOn)
                    return;

                Analytics.TrackEvent("Settings changed: telemetry", new Dictionary<string, string>
                {
                    { "Value", value.ToString() }
                });
                UserSettings.Set<bool>(SettingsConstants.TelemetryEnabledKey, value);

                ChangeTelemetry(value);
            }
        }

        public bool AutoSaveOn
        {
            get => UserSettings.Get<bool>(SettingsConstants.AutoSaveEnabled);
            set
            {
                if (value == AutoSaveOn)
                {
                    return;
                }

                UserSettings.Set<bool>(SettingsConstants.AutoSaveEnabled, value);
                RaisePropertyChanged(nameof(AutoSaveOn));

                Analytics.TrackEvent("Settings changed: Auto save", new Dictionary<string, string>
                {
                    { "Value", value.ToString() }
                });
            }
        }

        public bool AlwaysWrapUrlOn
        {
            get => UserSettings.Get<bool>(SettingsConstants.AlwaysWrapURL);
            set
            {
                if (value == AlwaysWrapUrlOn)
                {
                    return;
                }

                UserSettings.Set<bool>(SettingsConstants.AlwaysWrapURL, value);
                RaisePropertyChanged(nameof(AlwaysWrapUrlOn));

                Analytics.TrackEvent("Settings changed: always wrap", new Dictionary<string, string>
                {
                    { "Value", value.ToString() }
                });
            }
        }

        public bool WordWrapEnabled
        {
            get => UserSettings.Get<bool>(SettingsConstants.WordWrapEditor);
            set
            {
                if (value == WordWrapEnabled)
                {
                    return;
                }

                UserSettings.Set<bool>(SettingsConstants.WordWrapEditor, value);
                RaisePropertyChanged(nameof(WordWrapEnabled));

                Analytics.TrackEvent("Settings changed: word wrap editor", new Dictionary<string, string>
                {
                    { "Value", value.ToString() }
                });
            }
        }

        public bool SslValidationOn
        {
            get => UserSettings.Get<bool>(SettingsConstants.SslValidationKey);
            set
            {
                if (value == SslValidationOn)
                    return;

                UserSettings.Set<bool>(SettingsConstants.SslValidationKey, value);
                Analytics.TrackEvent("Settings changed: SSL validation", new Dictionary<string, string>
                {
                    { "Value", value.ToString() }
                });
                RaisePropertyChanged("SslValidation");
            }
        }

        public bool DeletingPasswords
        {
            get => _deletingPasswords;
            set
            {
                _deletingPasswords = value;
                RaisePropertyChanged("DeletingPasswords");
                RaisePropertyChanged("IsDeletePasswordEnabled");
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
                Analytics.TrackEvent("Theme changed", new Dictionary<string, string>
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
                RaisePropertyChanged("PivotIndex");
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
                RaisePropertyChanged("Loading");
                SyncStatusUpdated?.Invoke(this, new EventArgs());
            }
        }
        public string AppVersion
        {
            get
            {
                var version = SystemInformation.ApplicationVersion;
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
                    RaisePropertyChanged("ClearingRecentUrls");
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
                    RaisePropertyChanged("ClearRecentUrlSuccessful");
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
                    RaisePropertyChanged("CanClearUrls");
                }
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

        private async void ChangeTelemetry(bool value)
        {
            await Analytics.SetEnabledAsync(value);
        }
    }
}
