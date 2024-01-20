using Microsoft.AppCenter.Analytics;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Workspaces.Models;
using Nightingale.Core.Workspaces.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Nightingale.Core.Models;
using Nightingale.CustomEventArgs;
using Nightingale.Core.Storage.Interfaces;
using Nightingale.Core.Extensions;

namespace Nightingale.ViewModels
{
    public class CollectionViewModel : ObservableBase
    {
        private readonly IRequestSender _requestSender;
        public readonly IEnvironmentContainer EnvironmentContainer;
        private readonly IParameterStorageAccessor _parameterStorage;

        private Item _selectedCollection;
        private CancellationTokenSource _cts;
        private bool _isDocked;
        private bool _loading;

        public CollectionViewModel(
            IEnvironmentContainer environmentContainer,
            IRequestSender requestSender,
            IParameterStorageAccessor parameterStorageAccessor)
        {
            EnvironmentContainer = environmentContainer
                ?? throw new ArgumentNullException(nameof(environmentContainer));
            _requestSender = requestSender
                ?? throw new ArgumentNullException(nameof(requestSender));
            _parameterStorage = parameterStorageAccessor
                ?? throw new ArgumentNullException(nameof(parameterStorageAccessor));
            _cts = new CancellationTokenSource();
        }

        public bool ReadyToExecute => !Loading;

        public bool Loading
        {
            get => _loading;
            set
            {
                _loading = value;
                RaisePropertyChanged("Loading");
                RaisePropertyChanged("ReadyToExecute");
            }
        }

        public int PivotIndex
        {
            get => (int)(SelectedCollection?.GetProperty<long>(ItemPropertyExtensions.CollectionPivotIndex) ?? 0);
            set
            {
                if (SelectedCollection?.Properties != null && PivotIndex != value)
                {
                    SelectedCollection.SetProperty(ItemPropertyExtensions.CollectionPivotIndex, (long)value);
                    RaisePropertyChanged("PivotIndex");
                }
            }
        }

        public bool IsDocked
        {
            get => _isDocked;
            set
            {
                _isDocked = value;
                RaisePropertyChanged("IsDocked");
                RaisePropertyChanged("IsUndocked");
            }
        }

        public bool IsUndocked
        {
            get => !IsDocked;
        }

        public bool IsEmpty
        {
            get => (SelectedCollection?.Children == null || SelectedCollection.Children.Count == 0);
        }

        public long TotalElapsedMilliseconds
        {
            get
            {
                if (SelectedCollection == null)
                {
                    return 0;
                }

                return SelectedCollection.Children.Sum(x => x.Response?.TimeElapsed ?? 0);
            }
        }

        public int TotalRequestCount
        {
            get
            {
                if (SelectedCollection == null || SelectedCollection.Children == null)
                    return 0;

                return SelectedCollection.Children.Count;
            }
        }

        public int TotalFailed
        {
            get
            {
                if (SelectedCollection == null)
                {
                    return 0;
                }

                return SelectedCollection.Children.Count(x => x.Response != null && !x.Response.Successful);
            }
        }

        public int TotalPassed
        {
            get
            {
                if (SelectedCollection == null)
                {
                    return 0;
                }

                return SelectedCollection.Children.Count(x => x.Response != null && x.Response.Successful);
            }
        }

        public string HeaderCount
        {
            get => SelectedCollection?.Headers == null ? "" : SelectedCollection.Headers.GetDisplayCount();
        }

        public string QueryCount
        {
            get => SelectedCollection?.Url?.Queries == null ? "" : SelectedCollection?.Url?.Queries.GetDisplayCount();
        }

        public void UpdateStatus()
        {
            RaisePropertyChanged(nameof(TotalPassed));
            RaisePropertyChanged(nameof(TotalFailed));
            RaisePropertyChanged(nameof(TotalRequestCount));
            RaisePropertyChanged(nameof(TotalElapsedMilliseconds));
        }

        public Item SelectedCollection
        {
            get => _selectedCollection;
            set
            {
                if (_selectedCollection?.Children != null)
                {
                    _selectedCollection.Children.CollectionChanged -= Children_CollectionChanged;
                }

                _selectedCollection = value;
                _selectedCollection.Children.CollectionChanged += Children_CollectionChanged;
                RaisePropertyChanged(string.Empty);
            }
        }

        private void Children_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RaisePropertyChanged(nameof(IsEmpty));
        }

        public async void AddVariableClicked(object sender, AddedItemArgs<Parameter> args)
        {
            if (string.IsNullOrWhiteSpace(args?.AddedItem?.Key))
            {
                return;
            }

            bool success = await EnvironmentContainer.AddVariablePromptAsync(args.AddedItem);

            Analytics.TrackEvent("Variable quick add", new Dictionary<string, string>
            {
                { "successful", success.ToString() },
            });
        }

        public async void ParameterDeleted(object sender, DeletedItemArgs<Parameter> args)
        {
            if (args?.DeletedItem == null)
            {
                return;
            }

            await _parameterStorage.DeleteParameterAsync(args.DeletedItem);
        }

        public void QueryValuesUpdated()
        {
            RaisePropertyChanged(nameof(QueryCount));
        }

        public void HeaderValuesUpdated()
        {
            RaisePropertyChanged(nameof(HeaderCount));
        }

        public async void RunAllRequests()
        {
            RaisePropertyChanged(nameof(TotalRequestCount));

            if (Loading)
            {
                Cancel();
                UpdateStatus();
                return;
            }
            // TODO add loading animations
            // TODO test with actual run.
            Loading = true;
            bool success;
            try
            {
                success = await _requestSender.RunCollectionAsync(SelectedCollection, _cts.Token);
            }
            catch (TaskCanceledException)
            {
                success = false;
                Analytics.TrackEvent("Collection run cancelled");
            }
            catch (Exception e)
            {
                success = false;
                Analytics.TrackEvent("Collection run failed", new Dictionary<string, string>()
                {
                    { "message", e.Message }
                });
                // TODO display error message?
            }

            if (success)
            {
                Analytics.TrackEvent("Collection run button pressed", new Dictionary<string, string>
                {
                    { "Request count", this.SelectedCollection?.Children.Count.ToString() ?? "0" }
                });
            }

            // TODO raise appropriate properties
            Loading = false;
            UpdateStatus();
        }

        public void Cancel()
        {
            if (_cts != null)
            {
                _cts.Cancel();
                _cts.Dispose();
                _cts = new CancellationTokenSource();
            }
        }
    }
}
