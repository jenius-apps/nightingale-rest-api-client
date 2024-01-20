using Microsoft.AppCenter.Analytics;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Http;
using Nightingale.CustomEventArgs;
using Nightingale.Dialogs;
using Nightingale.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Nightingale.ViewModels
{
    public class UrlBarViewModel : ViewModelBase
    {
        private readonly IRecentUrlCache _recentUrlCache;
        private readonly IMethodsContainer _methodsContainer;

        public UrlBarViewModel(
            IRecentUrlCache recentUrlCache,
            IMethodsContainer methodsContainer)
        {
            _recentUrlCache = recentUrlCache ?? throw new ArgumentNullException(nameof(recentUrlCache));
            _methodsContainer = methodsContainer ?? throw new ArgumentNullException(nameof(methodsContainer));
        }

        public ObservableCollection<string> Methods { get; } = new ObservableCollection<string>();

        public async Task Initialize()
        {
            await _recentUrlCache.InitializeAsync();

            // retrieve methods configured for the 
            // active workspace.
            var methods = _methodsContainer.GetMethods();
            PopulateMethods(methods);
        }

        /// <summary>
        /// Opens dialog if the newly selected
        /// method is CUSTOM.
        /// </summary>
        public async void MethodChanged(object sender, AddedItemArgs<string> args)
        {
            if (args?.AddedItem == "CUSTOM")
            {
                var dialog = new CustomMethodsDialog()
                {
                    RequestedTheme = ThemeController.GetTheme(),
                    Methods = new ObservableCollection<string>(_methodsContainer.GetMethods() ?? new List<string>())
                };
                var result = await dialog.ShowAsync();
                if (result == Windows.UI.Xaml.Controls.ContentDialogResult.Primary)
                {
                    PopulateMethods(dialog.Methods);
                    _methodsContainer.UpdateMethods(dialog.Methods);
                    Analytics.TrackEvent("CUSTOM clicked - changes made", new Dictionary<string, string>
                    {
                        { "Method list", string.Join(',', dialog.Methods) }
                    });
                }
                else
                {
                    Analytics.TrackEvent("CUSTOM clicked - cancelled");
                }
            }
        }

        private void PopulateMethods(IList<string> methods)
        {
            if (Methods == null)
            {
                return;
            }

            Methods.Clear();
            if (methods != null)
            {
                foreach (var m in methods)
                {
                    if (!string.IsNullOrWhiteSpace(m))
                    {
                        Methods.Add(m);
                    }
                }
            }

            // Always make sure CUSTOM is last.
            // Without it, the user cannot open
            // the customize menu.
            if (Methods.LastOrDefault() != "CUSTOM")
            {
                Methods.Add("CUSTOM");
            }
        }

        public async void AddRecentUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return;
            }

            await _recentUrlCache.AddRecentUrlAsync(url);
        }

        public IList<string> GetSimilarUrls(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return null;
            }

            return _recentUrlCache.GetSimilarUrls(url);
        }
    }
}
