using Windows.UI.Xaml;

namespace Nightingale.VisualState
{
    /// <remarks>
    /// Ref: https://jefdaels.wordpress.com/2015/11/08/using-state-triggers-to-implement-data-trigger-behavior/
    /// </remarks>
    public class BooleanStateTrigger : StateTriggerBase
    {
        /// <summary>
        /// Gets and sets the value of the trigger data.
        /// </summary>
        public bool DataValue
        {
            get => (bool)GetValue(DataValueProperty);
            set => SetValue(DataValueProperty, value);
        }
        
        /// <summary>
        /// Gets or sets the value which will activate the state.
        /// </summary>
        public bool TriggerValue
        {
            get => (bool)GetValue(TriggerValueProperty);
            set => SetValue(TriggerValueProperty, value);
        }

        public static readonly DependencyProperty DataValueProperty =
            DependencyProperty.Register("DataValue", typeof(bool), typeof(BooleanStateTrigger),
                new PropertyMetadata(false, DataValueChanged));

        public static readonly DependencyProperty TriggerValueProperty =
            DependencyProperty.Register("TriggerValue", typeof(bool), typeof(BooleanStateTrigger),
                new PropertyMetadata(false, TriggerValueChanged));


        private static void DataValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            bool triggerValue = (bool)target.GetValue(TriggerValueProperty);
            TriggerStateCheck(target, (bool)e.NewValue, triggerValue);
        }

        private static void TriggerValueChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
        {
            bool dataValue = (bool)target.GetValue(DataValueProperty);
            TriggerStateCheck(target, dataValue, (bool)e.NewValue);
        }

        private static void TriggerStateCheck(DependencyObject target, bool dataValue, bool triggerValue)
        {
            if (target is BooleanStateTrigger trigger)
            {
                trigger.SetActive(triggerValue == dataValue);
            }
        }

    }
}
