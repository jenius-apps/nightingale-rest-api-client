using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace Nightingale.Utilities
{
    public static class RadioButtonExtension
    {
        /// <summary>
        /// Returns zero-based index of radio button within its grouping. 
        /// Returns -1 if not in a group or if not found.
        /// </summary>
        /// <param name="radioButton">The <see cref="RadioButton"/> to inspect.</param>
        /// <returns>
        /// The zero-based index of the first occurrence of item within the group
        /// if found; otherwise, –1.
        /// </returns>
        public static int GetIndexWithinGroup(this RadioButton radioButton)
        {
            int index = -1;

            if (radioButton.Parent is StackPanel sp)
            {
                var radioButtonArray = sp.Children.OfType<RadioButton>().ToList();
                index = radioButtonArray.IndexOf(radioButton);
            }

            return index;
        }
    }
}
