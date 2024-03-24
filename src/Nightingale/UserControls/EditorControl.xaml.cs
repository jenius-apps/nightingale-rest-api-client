using ActiproSoftware.Text;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Implementation;
using Nightingale.Editor;
using Nightingale.ViewModels;
using System;
using System.Threading.Tasks;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Nightingale.Core.Helpers;
using Microsoft.AppCenter.Analytics;
using Nightingale.Utilities;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Highlighting;
using ActiproSoftware.UI.Xaml.Controls.SyntaxEditor.Highlighting.Implementation;
using System.Collections.Generic;
using System.Linq;
using Nightingale.Handlers;
using Nightingale.Core.Settings;
using Microsoft.Extensions.DependencyInjection;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class EditorControl : UserControl
    {
        private static readonly List<(VirtualKey, VirtualKeyModifiers)> shortcutsToRemove = new List<(VirtualKey, VirtualKeyModifiers)>()
        {
            (VirtualKey.F, VirtualKeyModifiers.Control),
            (VirtualKey.Enter, VirtualKeyModifiers.Control),
            (VirtualKey.L, VirtualKeyModifiers.Control),
        };

        private string _lastSearchString = "";

        public EditorControl()
        {
            this.InitializeComponent();

            // Ensure all classification types and related styles have been registered
            // since classification types are used for the highlight display
            new DisplayItemClassificationTypeProvider().RegisterAll();

            Editor.Document.TabSize = 2;
            UpdateSyntaxHighlighting();
            ThemeController.ThemeChanged += ThemeController_ThemeChanged;

            foreach (var shortcut in shortcutsToRemove)
            {
                var binding = Editor.InputBindings
                    .Where(x => x.Key == shortcut.Item1 && x.Modifiers == shortcut.Item2)
                    .FirstOrDefault();

                Editor.InputBindings.Remove(binding);
            }

            Editor.IsWordWrapEnabled = App.Services.GetRequiredService<IUserSettings>().Get<bool>(SettingsConstants.WordWrapEditor);
        }

        private void ThemeController_ThemeChanged(object sender, EventArgs e)
        {
            UpdateSyntaxHighlighting();
        }

        private void Editor_ActualThemeChanged(FrameworkElement sender, object args)
        {
            UpdateSyntaxHighlighting();
        }

        private void UpdateSyntaxHighlighting()
        {
            if (this.ActualTheme == ElementTheme.Dark)
            {
                // xml/html
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlEntity, new HighlightingStyle(GetWindowsColour("#FF8fc7f1")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlDelimiter, new HighlightingStyle(GetWindowsColour("#FFc5c5c5")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlName, new HighlightingStyle(GetWindowsColour("#FF3786D4")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlAttribute, new HighlightingStyle(GetWindowsColour("#FF8fc7f1")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlAttributeValue, new HighlightingStyle(GetWindowsColour("#FFc5c5c5")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlComment, new HighlightingStyle(GetWindowsColour("#FF008000")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlProcessingInstruction, new HighlightingStyle(GetWindowsColour("#FFFF00FF")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlDeclaration, new HighlightingStyle(GetWindowsColour("#FFFF00FF")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.ServerSideScript, new HighlightingStyle(GetWindowsColour("#FFFFEE62")), true);

                // json
                AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.Comment, new HighlightingStyle(GetWindowsColour("#FF6a9955")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.Keyword, new HighlightingStyle(GetWindowsColour("#FF569cd6")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.Number, new HighlightingStyle(GetWindowsColour("#FFb5cea8")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.String, new HighlightingStyle(GetWindowsColour("#FFCE9178")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(new DisplayItemClassificationTypeProvider().FindMatchHighlight, new HighlightingStyle(null, GetWindowsColour("#FF623916")), true);
            }
            else
            {
                // xml/html
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlEntity, new HighlightingStyle(GetWindowsColour("#FFFF0000")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlDelimiter, new HighlightingStyle(GetWindowsColour("#FF0000FF")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlName, new HighlightingStyle(GetWindowsColour("#FF800000")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlAttribute, new HighlightingStyle(GetWindowsColour("#FFFF0000")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlAttributeValue, new HighlightingStyle(GetWindowsColour("#FF0000FF")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlComment, new HighlightingStyle(GetWindowsColour("#FF008000")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlProcessingInstruction, new HighlightingStyle(GetWindowsColour("#FFFF00FF")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.XmlDeclaration, new HighlightingStyle(GetWindowsColour("#FFFF00FF")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(XmlClassificationTypes.ServerSideScript, new HighlightingStyle(GetWindowsColour("#FFFFEE62")), true);

                // json
                AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.Comment, new HighlightingStyle(GetWindowsColour("#FF008000")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.Keyword, new HighlightingStyle(GetWindowsColour("#FF0000ff")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.Number, new HighlightingStyle(GetWindowsColour("#FF09885a")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(ClassificationTypes.String, new HighlightingStyle(GetWindowsColour("#FFa31515")), true);
                AmbientHighlightingStyleRegistry.Instance.Register(new DisplayItemClassificationTypeProvider().FindMatchHighlight, new HighlightingStyle(null, GetWindowsColour("#FFf8c9ab")), true);
            }
        }

        public Windows.UI.Color GetWindowsColour(string hex)
        {
            hex = hex.Replace("#", "");
            byte a = (byte)(Convert.ToUInt32(hex.Substring(0, 2), 16));
            byte r = (byte)(Convert.ToUInt32(hex.Substring(2, 2), 16));
            byte g = (byte)(Convert.ToUInt32(hex.Substring(4, 2), 16));
            byte b = (byte)(Convert.ToUInt32(hex.Substring(6, 2), 16));
            return Windows.UI.Color.FromArgb(a, r, g, b);
        }

        public event EventHandler EditorTextChanged;

        public EditorViewModel ViewModel { get; } = new EditorViewModel();

        public string Text
        {
            get => ViewModel.Text;
            set => ApplyText(value);
        }

        public SyntaxType Syntax
        {
            get => (SyntaxType)GetValue(SyntaxProperty);
            set => SetSyntaxDp(SyntaxProperty, value);
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //public static DependencyProperty TextProperty =
        //    DependencyProperty.Register("Text", typeof(string), typeof(EditorControl), new PropertyMetadata(string.Empty));

        public static DependencyProperty SyntaxProperty =
            DependencyProperty.Register("Syntax", typeof(SyntaxType), typeof(EditorControl), new PropertyMetadata(SyntaxType.Plain));

        //private void SetValueDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] string p = null)
        //{
        //    // Ref: http://blog.jerrynixon.com/2013/07/solved-two-way-binding-inside-user.html
        //    SetValue(property, value);
        //    //PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
            
        //    if (value is string s)
        //    {
        //        ApplyText(s);
        //    }
        //}

        private async void SetSyntaxDp(DependencyProperty property, object value, [System.Runtime.CompilerServices.CallerMemberName] string p = null)
        {
            SetValue(property, value);
            
            if (value is SyntaxType syntaxType)
            {
                await ApplySyntaxType(syntaxType);
            }
        }

        private void ApplyText(string s)
        {
            ViewModel.Text = s;
            Editor.Document.SetText(s);
        }

        private async Task ApplySyntaxType(SyntaxType syntax)
        {
            if (syntax == SyntaxType.Plain)
            {
                Editor.Document.Language = ActiproSoftware.Text.Implementation.SyntaxLanguage.PlainText;
                return;
            }

            if (syntax == SyntaxType.Json)
            {
                var x = new JavascriptSyntaxLanguage();
                await x.Initialize();

                Editor.Document.Language = x;
            }
            else if (syntax == SyntaxType.Xml)
            {
                var x = new XmlSyntaxLanguage();
                await x.Initialize();

                Editor.Document.Language = x;
            }
            else if (syntax == SyntaxType.Html)
            {
                var x = new HtmlSyntaxLanguage();
                await x.Initialize();
                Editor.Document.Language = x;
            }
        }

        private async void ActiproEditor_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (KeyboardState.IsCtrlKeyPressed() && e.Key == VirtualKey.F)
            {
                SearchPanel.Visibility = Visibility.Visible;
                await Task.Delay(10); // Delay required to fix focus bug
                SearchBox.Focus(FocusState.Programmatic);
                SearchBox.SelectAll();
                Analytics.TrackEvent(Telemetry.SearchOpened);
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                CloseFindPanel();
                e.Handled = true;
            }
        }

        private void RefreshHighlights(bool reverse, string text = "")
        {
            if (Editor == null)
            {
                return;
            }

            Analytics.TrackEvent(Telemetry.SearchPerformed, new Dictionary<string, string>
            {
                { "Reverse", reverse.ToString() }
            });

            EditorSearchOptions options = new EditorSearchOptions
            {
                FindText = text,
                SearchUp = reverse
            };

            // highlight all words found
            Editor.ActiveView.HighlightedResultSearchOptions = options;

            if (!string.IsNullOrWhiteSpace(options.FindText))
            {
                // Determine where to start search,
                // either beginning or where we left off last time.
                int startOffset = options.FindText.Equals(_lastSearchString, StringComparison.OrdinalIgnoreCase)
                    ? GetStartOffset(reverse)
                    : 0;

                // perform search
                var results = Editor.Document.CurrentSnapshot.FindNext(options, startOffset, Editor.WordWrapMode == WordWrapMode.Word);

                // Search again from the top of the document if the prevoius search found nothing and it was at the end of the document
                if (results?.Results != null && results.Results.Count == 0 && startOffset > 0)
                {
                    results = Editor.Document.CurrentSnapshot.FindNext(options, 0, Editor.WordWrapMode == WordWrapMode.Word);
                }

                if (results?.Results != null && results.Results.Count > 0)
                {
                    // Select the first string found
                    Editor.ActiveView.Selection.SelectRange(
                        results.Results[0].FindSnapshotRange.TranslateTo(
                            Editor.ActiveView.CurrentSnapshot,
                            TextRangeTrackingModes.Default)
                        .TextRange);

                    // Center the selected string in middle of editor
                    Editor.ActiveView.Scroller.ScrollLineToVisibleMiddle();
                }
            }

            _lastSearchString = text;
        }

        private int GetStartOffset(bool reverseSearch)
        {
            int startOffset = Editor.Document.CurrentSnapshot.PositionToOffset(Editor.ActiveView.Selection.CaretPosition) - (reverseSearch ? _lastSearchString.Length : 0);
            return startOffset < 0 ? 0 : startOffset;
        }

        private void ActiproEditor_DocumentTextChanged(object sender, EditorSnapshotChangedEventArgs e)
        {
            if (e.TextChange.Type == TextChangeTypes.TextReplacement && e.NewSnapshot.Text == string.Empty)
            {
                return;
            }

            ViewModel.Text = e.NewSnapshot.Text;
            EditorTextChanged?.Invoke(this, new EventArgs());
        }

        private void SearchButtonClicked(object sender, RoutedEventArgs e)
        {
            this.RefreshHighlights(false, SearchBox.Text);
        }

        private void SearchBox_KeyDown(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                this.RefreshHighlights(KeyboardState.IsShiftKeyPressed(), SearchBox.Text);
                e.Handled = true;
            }
            else if (e.Key == VirtualKey.Escape)
            {
                CloseFindPanel();
                e.Handled = true;
            }
            else if (KeyboardState.IsCtrlKeyPressed() && e.Key == VirtualKey.F)
            {
                SearchBox.SelectAll();
                e.Handled = true;
            }
        }

        private void CloseFindPanel()
        {
            Analytics.TrackEvent(Telemetry.SearchClosed);
            SearchPanel.Visibility = Visibility.Collapsed;
            this.RefreshHighlights(false);
            Editor.Focus(FocusState.Programmatic);
        }

        private void ToggleWrapClicked()
        {
            Editor.IsWordWrapEnabled = !Editor.IsWordWrapEnabled;
            Analytics.TrackEvent(Telemetry.WordWrapClicked);
        }

        private void BeautifyClicked()
        {
            if (this.Syntax == SyntaxType.Plain)
            {
                return;
            }

            string beautifiedText = TextBeautifier.Beautify(
                this.Text,
                this.Syntax == SyntaxType.Xml
                    ? Core.Enums.ContentType.Xml
                    : Core.Enums.ContentType.Json);

            this.Text = beautifiedText;

            Analytics.TrackEvent(Telemetry.BeautifyClicked);
        }
    }
}
