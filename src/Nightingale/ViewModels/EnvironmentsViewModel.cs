using Microsoft.AppCenter.Analytics;
using Nightingale.Core.Extensions;
using Nightingale.Core.Helpers.Interfaces;
using Nightingale.Core.Interfaces;
using Nightingale.Core.Settings;
using Nightingale.Core.Storage.Interfaces;
using Nightingale.CustomEventArgs;
using Nightingale.Handlers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Nightingale.ViewModels
{
    public class EnvironmentsViewModel : ViewModelBase
    {
        private readonly IEnvironmentListModifier _envListModifier;
        private readonly IParameterStorageAccessor _parameterStorageAccessor;
        private readonly IUserSettings _userSettings;
        private string _newEnvName;
        private bool _premiumMessageVisible;
        private bool _addButtonEnabled = true;
        private ObservableCollection<Core.Models.Environment> _envList;

        public EnvironmentsViewModel(
            IEnvironmentListModifier listModifier,
            IParameterStorageAccessor parameterStorageAccessor,
            IUserSettings userSettings)
        {
            _envListModifier = listModifier;
            _parameterStorageAccessor = parameterStorageAccessor;
            _userSettings = userSettings;
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
                Analytics.TrackEvent("Settings changed: EnableEnvQuickEdit", new Dictionary<string, string>
                {
                    { "Value", value ? "true" : "false" },
                    { "Location", "environment manager" }
                });
            }
        }


        public bool AddButtonEnabled
        {
            get => _addButtonEnabled;
            set
            {
                if (_addButtonEnabled != value)
                {
                    _addButtonEnabled = value;
                    RaisePropertyChanged("AddButtonEnabled");
                }
            }
        }

        public bool PremiumMessageVisible
        {
            get => _premiumMessageVisible;
            set
            {
                if (_premiumMessageVisible == value)
                {
                    return;
                }

                _premiumMessageVisible = value;
                RaisePropertyChanged("PremiumMessageVisible");
                RaisePropertyChanged("HelpMessageVisible");
            }
        }

        public string NewEnvName
        {
            get => _newEnvName;
            set
            {
                _newEnvName = value;
                RaisePropertyChanged("NewEnvName");
            }
        }

        public string SelectedIcon { get; set; }

        /// <summary>
        /// The selected environment wrapped
        /// in a view model.
        /// </summary>
        public EnvItemViewModel SelectedVm { get; set; }

        /// <summary>
        /// The exposed list of environments
        /// wrapped in view models.
        /// </summary>
        public ObservableCollection<EnvItemViewModel> EnvItemViewModels { get; } = new ObservableCollection<EnvItemViewModel>();

        public void Initialize(
            ObservableCollection<Core.Models.Environment> environmentList)
        {
            _envList = environmentList;

            // Wrap each env in a view model
            foreach (var env in environmentList)
            {
                if (env == null) continue;

                var vm = new EnvItemViewModel(env);
                EnvItemViewModels.Add(vm);
                if (env.IsActive) SelectedVm = vm;
            }
        }

        public void CloneEnv(object sender, AddedItemArgs<object> e)
        {
            if (_envList == null)
            {
                return;
            }

            if (e.AddedItem is EnvItemViewModel vm)
            {
                var clone = _envListModifier.CloneEnvironment(_envList, vm.Env);
                if (clone == null) return;

                EnvItemViewModels.Add(new EnvItemViewModel(clone));
                Analytics.TrackEvent("Environment cloned");
            }
        }

        /// <summary>
        /// Adds new env to EnvManager's list.
        /// </summary>
        public void AddEnv()
        {
            if (string.IsNullOrWhiteSpace(NewEnvName) || _envList == null)
            {
                NewEnvName = "";
                return;
            }

            var newEnv = _envListModifier.AddNewEnvironment(_envList, NewEnvName, SelectedIcon);
            if (newEnv != null) EnvItemViewModels.Add(new EnvItemViewModel(newEnv));

            NewEnvName = "";
            Analytics.TrackEvent("Environment added", new Dictionary<string, string>()
            {
                { "Icon", SelectedIcon ?? "🌐" }
            });
        }

        public async void ParameterDeleted(object sender, DeletedItemArgs<Core.Models.Parameter> args)
        {
            if (args?.DeletedItem == null)
            {
                return;
            }

            await _parameterStorageAccessor.DeleteParameterAsync(args.DeletedItem);
        }

        public void EmojiChanged(object sender, AddedItemArgs<(string, string)> e)
        {
            string content = e.AddedItem.Item1;
            SelectedIcon = content ?? "🌐";
        }

        public async void EnvironmentDeleted(object sender, DeletedItemArgs<object> e)
        {
            if (e.DeletedItem is EnvItemViewModel vm)
            {
                await DeleteEnv(vm);
            }
        }

        public void EnvActivated(object sender, AddedItemArgs<object> e)
        {
            if (e.AddedItem is EnvItemViewModel vm)
            {
                _envList.SetActive(vm.Env);
            }
        }

        /// <summary>
        /// Deletes given env from EnvManager's list.
        /// </summary>
        private async Task DeleteEnv(EnvItemViewModel forDeletion)
        {
            if (forDeletion == null) return;
            if (SelectedVm == forDeletion) SelectedVm = null;
            EnvItemViewModels.Remove(forDeletion);
            await _envListModifier.DeleteEnvironmentAsync(_envList, forDeletion.Env);
            Analytics.TrackEvent("Environment deleted");
        }
    }
}
