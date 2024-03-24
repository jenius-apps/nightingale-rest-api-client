using System;
using System.Collections.Generic;

namespace Nightingale.Core.Settings
{
    /// <summary>
    /// Class holding constants and defaults
    /// for Nightingale settings.
    /// </summary>
    public sealed class SettingsConstants
    {
        // Settings Keys
        public const string ShowMvpBadge = "ShowMvpBadge";
        public const string SyncEnabledKey = "SyncEnabled";
        public const string SelectedThemeKey = "SelectedTheme";
        public const string SslValidationKey = "SslValidation";
        public const string LoopbackDialogNeverShowKey = "LoopbackDialogNeverShow";
        public const string TelemetryEnabledKey = "TelemetryEnabled";
        public const string InputPaneWidthKey = "InputPaneWidth";
        public const string OutputPaneWidthKey = "OutputPaneWidth";
        public const string InputPaneHeightKey = "InputPaneHeight";
        public const string OutputPaneHeightKey = "OutputPaneHeight";
        public const string SidebarWidth = "SidebarWidth";
        public const string AppVersionKey = "AppVersion";
        public const string IsPremiumUnlocked = "IsPremiumUnlocked";
        public const string IsAppRated = "IsAppRated";
        public const string AutoSaveEnabled = "AutoSaveEnabled";
        public const string HistoryEnabled = "HistoryEnabled";
        public const string PaneLayoutSideBySide = "PaneLayoutSideBySide";
        public const string SideBarVisible = "SideBarVisible";
        public const string PremiumDateUnlocked = "PremiumDateUnlocked";
        public const string PremiumIapId = "PremiumIapId";
        public const string LastImportTypeUsed = "LastImportTypeUsed";
        public const string ConfirmDeletion = "ConfirmDeletion";
        public const string EnableEnvQuickEdit = "EnableEnvQuickEdit";
        public const string AutoSaveInterval = "AutoSaveInterval";
        public const string BackgroundImage = "BackgroundImage";
        public const string AlwaysWrapURL = "AlwaysWrapURL";
        public const string WordWrapEditor = "WordWrapEditor";
        public const string TimeoutSecondsKey = "TimeOutSeconds";
        public const string InfiniteTimeoutKey = "InfiniteTimeout";

        /// <summary>
        ///  Settings defaults.
        /// </summary>
        public static readonly Dictionary<string, object> Defaults = new Dictionary<string, object>()
        {
            { SyncEnabledKey, false },
            { SelectedThemeKey, SelectedTheme.Auto },
            { SslValidationKey, false },
            { LoopbackDialogNeverShowKey, false },
            { TelemetryEnabledKey, true },
            { InputPaneWidthKey, double.NaN },
            { OutputPaneWidthKey, double.NaN },
            { InputPaneHeightKey, double.NaN },
            { OutputPaneHeightKey, double.NaN },
            { SidebarWidth, 120d },
            { AppVersionKey, string.Empty },
            { IsPremiumUnlocked, false },
            { IsAppRated, false },
            { AutoSaveEnabled, true },
            { HistoryEnabled, true },
            { PaneLayoutSideBySide, false },
            { SideBarVisible, true },
            { PremiumDateUnlocked, DateTime.MinValue.Ticks },
            { PremiumIapId, "" },
            { LastImportTypeUsed, 0 },
            { ConfirmDeletion, true },
            { EnableEnvQuickEdit, true },
            { ShowMvpBadge, true },
            { AlwaysWrapURL, false },
            { WordWrapEditor, false },
            { BackgroundImage, "" },
            { AutoSaveInterval, true },
            { TimeoutSecondsKey, 100d },
            { InfiniteTimeoutKey, false },
        };
    }
}
