using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Nightingale.Core.Models
{
    public abstract class ObservableBase : ModifiableBase, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            ObjectModified();
        }
    }
}
