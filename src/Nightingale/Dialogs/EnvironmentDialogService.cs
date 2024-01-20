using Autofac;
using Nightingale.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Dialogs
{
    public class EnvironmentDialogService : BaseDialogService, IEnvironmentDialogService
    {
        private readonly ILifetimeScope _scope;

        public EnvironmentDialogService(ILifetimeScope scope)
        {
            _scope = scope ?? throw new ArgumentNullException(nameof(scope));
        }

        /// <inheritdoc/>
        public async Task<ManageEnvironmentsDialog> OpenEnvironmentDialog(
            ObservableCollection<Core.Models.Environment> list)
        {
            if (IsDialogActive)
            {
                return null;
            }

            IsDialogActive = true;

            var dialog = new ManageEnvironmentsDialog
            {
                ViewModel = _scope.Resolve<EnvironmentsViewModel>()
            };
            dialog.ViewModel.Initialize(list);
            await dialog.ShowAsync();

            IsDialogActive = false;
            return dialog;
        }
    }
}
