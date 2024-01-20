using Microsoft.AppCenter.Analytics;
using System.Collections.Generic;

namespace Nightingale.Utilities
{
    /// <summary>
    /// Contains the constants for telemetry events
    /// being tracked.
    /// </summary>
    public class Telemetry
    {
        // welcome screen
        private const string Welcome = "Welcome: ";
        public const string WelcomeNewTab = Welcome + "NewTab";
        public const string WelcomeNewRequest = Welcome + "NewRequest";
        public const string WelcomeNewCollection = Welcome + "NewCollection";
        public const string WelcomeImportCollection = Welcome + "ImportCollection";

        // mock
        private const string Mock = "Mock: ";
        public const string MockServerDeployed = Mock + "server deployed";
        public static Dictionary<string, string> MockTelemetryProps(
            bool successful,
            string uiLocation)
        {
            return new Dictionary<string, string>
            {
                { nameof(successful), successful.ToString() },
                { nameof(uiLocation), uiLocation }
            };
        }

        // support
        public const string SupportRate = "rate";
        public const string SupportGitHub = "github";
        public const string SupportShare = "share";
        public const string SupportMvp = "mvp";
        public const string SupportPaypal = "paypal";
        public static void Support(string supportOption)
        {
            Analytics.TrackEvent("support opened", new Dictionary<string, string>
            {
                { "option", supportOption }
            });
        }
        

        // tabs
        private const string Tabs = "Tabs: ";
        public const string TempTabAdded = Tabs + "Temp tab added";
        public const string OpenTreeItemTabFocused = Tabs + "Open tree item tab focused";
        public const string TreeItemTabOpened = Tabs + "Tree item tab opened";
        public const string TempTabSaved = Tabs + "Temp tab saved to workspace";

        // tab context menu
        public const string TabContext = "Tab context menu";
        public const string TabContextSave = "save";
        public const string TabContextRename = "rename";
        public const string TabContextDuplicate = "duplicate";
        public const string TabContextCloseOthers = "closeOthers";
        public const string TabContextClose = "close";
        public static Dictionary<string, string> Props(string propValue)
        {
            return new Dictionary<string, string>
            {
                { "prop", propValue }
            };
        }

        // item context menu
        private const string ItemContext = "Item context menu: ";
        public const string NewColInParent = ItemContext + "new collection in parent";
        public const string ItemEdited = ItemContext + "item edited";
        public const string NewReqInParent = ItemContext + "new request in parent";
        public const string NewReqInRoot = ItemContext + "new request in root";
        public const string NewColInRoot = ItemContext + "new collection in root";
        public const string DeleteAll = ItemContext + "delete all";
        public const string ItemDeleted = ItemContext + "item deleted";
        public const string GenCode = ItemContext + "generate code clicked";
        public const string ItemCloned = ItemContext + "item cloned";

        // keyboard shortcuts
        private const string KeyboardShortcut = "Keyboard shortcut: ";
        public const string CtrlS = KeyboardShortcut + "S";
        public const string CtrlT = KeyboardShortcut + "T";
        public const string CtrlW = KeyboardShortcut + "W";
        public const string CtrlTab = KeyboardShortcut + "Tab";
        public const string CtrlShiftTab = KeyboardShortcut + "ShiftTab";

        // ncf file
        public const string NcfFileLaunched = "NCF file launched";
        public const string NcfFileInvalid = "NCF file invalid";

        // custom methods
        public const string CustomMethodRemoved = "Custom method removed";
        public const string CustomMethodReset = "Custom method reset";
        public const string CustomMethodAdded = "Custom method added";

        // delete
        public const string DeleteWithoutAskingChecked = "Delete without asking checked";

        // response body
        public const string SaveBodyClicked = "Save body clicked";
        public const string OutputCopied = "Output copied";
        public const string RawBytesCopied = "Raw bytes copied";
        public const string SaveBodyError = "Save body error";
        public const string HtmlPreviewToggled = "HTML preview toggled";

        // premium
        public const string UnlockDurableClicked = "Unlock durable clicked";

        // editor
        public const string SearchOpened = "Search opened";
        public const string SearchPerformed = "Search performed";
        public const string SearchClosed = "Search closed";
        public const string BeautifyClicked = "Beautify clicked";
        public const string WordWrapClicked = "Word wrap clicked";

        // form data
        public const string FormDataTypeChanged = "Form data type changed";
        public const string FormDataDeleted = "Form data deleted";
        public const string FormDataFilesCleared = "Form data files cleared";

        // url
        public const string StatusFlyoutOpened = "Status flyout opened";

        // layout
        public const string ToggleLayoutClicked = "Toggle layout clicked";
        public const string ToggleSideBarClicked = "Toggle side bar clicked";

        // collection
        public const string CollectionRunCancelled = "Collection run cancelled";
        public const string CollectionRunClicked = "Collection run clicked";

        // env
        public const string EnvCloned = "Environment cloned";
        public const string EnvDeleted = "Environment deleted";
        public const string EnvAdded = "Environment added";
        public const string PrivateClicked = "Environment private button clicked";

        // import
        public const string ImportPerformed = "Import performed";
        public const string ImportUnsupported = "Import file unsupported";
        public const string CurlPaste = "Curl paste";

        // export
        public const string ExportLocationOpened = "Export location opened";
        public const string ContextMenuExportClicked = "Export via context menu successful";

        // menu buttons
        private const string MenuBar = "Menu bar: ";
        public const string MenuNewRequestAdded = MenuBar + "New request added";
        public const string MenuNewCollectionAdded = MenuBar + "New collection added";
        public const string MenuExportClicked = MenuBar + "export clicked";
        public const string MenuSaveClicked = MenuBar + "save clicked";
        public const string MenuCookiesClicked = MenuBar + "cookies clicked";
        public const string MenuWorkspaceFlyoutOpened = MenuBar + "workspace flyout opened";
        public const string MenuNewWorkspaceClicked = MenuBar + "new workspace clicked";
        public const string MenuWorkspaceDeleted = MenuBar + "workspace deleted";
        public const string MenuWorkspaceEdited = MenuBar + "workspace edited";
        public const string MenuRateClicked = MenuBar + "rate button clicked";
        public const string MenuImportClicked = MenuBar + "import clicked";
        public const string MenuSettingsClicked = MenuBar + "settings clicked";
        public const string MenuWorkspaceChanged = MenuBar + "workspace changed";
        public const string MenuGitHub = MenuBar + "github";
        public const string GitHubIssue = MenuBar + "github issue";
        public const string LocalhostHelp = MenuBar + "troubleshoot localhost";
        public const string KnownBugs = MenuBar + "known bugs";
        public const string EmailDev = MenuBar + "email dev";
        public const string WhatsNew = MenuBar + "what's new";
        public const string TwoPane = MenuBar + "two pane";
        public const string SinglePane = MenuBar + "single pane";
        public const string MenuSideBar = MenuBar + "side bar toggled";
        public const string MenuHeaderClicked = MenuBar + "header clicked";
        public const string MenuEnvManager = MenuBar + "env manager clicked";
        public const string MenuSaveTab = MenuBar + "tab saved";
        public const string MenuTutorial = "tutorial";
        public static void MenuButton(string buttonName)
        {
            Analytics.TrackEvent("menu button", new Dictionary<string, string>
            {
                { "name", buttonName }
            });
        }


        // history
        public const string HistoryCleared = "History cleared";
        public const string HistoryItemDeleted = "History item deleted";

        // docs
        public const string Docs = "docs clicked";
        public const string MockDocs = "mock docs";
        public const string ConverterSource = "converter source";
        public static Dictionary<string, string> DocsTelemetryProps(
            string docsLocation)
        {
            return new Dictionary<string, string>
            {
                { nameof(docsLocation), docsLocation },
            };
        }

        // theme
        public const string ThemeBackgroundImageChanged = "Background image changed";
    }
}
