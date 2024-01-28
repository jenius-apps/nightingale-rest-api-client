using Microsoft.Extensions.DependencyInjection;
using Nightingale.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Nightingale.Dialogs
{
    public class EnvironmentDialogService : BaseDialogService, IEnvironmentDialogService
    {
        private readonly IServiceProvider _scope;

        public EnvironmentDialogService(IServiceProvider scope)
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
                ViewModel = _scope.GetRequiredService<EnvironmentsViewModel>()
            };
            dialog.ViewModel.Initialize(list);
            await dialog.ShowAsync();

            IsDialogActive = false;
            return dialog;
        }
    }
}
