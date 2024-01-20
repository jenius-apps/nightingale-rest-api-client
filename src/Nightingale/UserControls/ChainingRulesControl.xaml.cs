using Nightingale.Core.Models;
using Nightingale.CustomEventArgs;
using Nightingale.Utilities;
using System;
using System.Collections.ObjectModel;
using Windows.UI.Xaml;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Nightingale.UserControls
{
    public sealed partial class ChainingRulesControl : ObservableUserControl
    {
        public event EventHandler<DeletedItemArgs<Parameter>> ChainingRuleDeleted;
        public event EventHandler ChainValuesUpdated;

        public ChainingRulesControl()
        {
            this.InitializeComponent();
        }

        public ObservableCollection<Parameter> ChainingRules
        {
            get => (ObservableCollection<Parameter>)GetValue(ChainingRulesProperty);
            set => SetValueDp(ChainingRulesProperty, value);
        }

        public static readonly DependencyProperty ChainingRulesProperty = DependencyProperty.Register(
            "ChainingRules",
            typeof(ObservableCollection<Parameter>),
            typeof(ChainingRulesControl),
            null);

        private void DeleteChainRule(object s, DeletedItemArgs<Parameter> e) => ChainingRuleDeleted?.Invoke(s, e);

        public void UpdateChainValues() => ChainValuesUpdated?.Invoke(this, new EventArgs());
    }
}
