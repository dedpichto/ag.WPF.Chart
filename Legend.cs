// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program; if not, write to the Free Software
// Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ag.WPF.Chart
{
    
    /// <summary>
    /// Represents <see cref="ISeries"/> legend
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
    public class Legend : Control
    {
        /// <summary>
        /// The identifier of the <see cref="Text"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string),
            typeof(Legend), new FrameworkPropertyMetadata(""));
        /// <summary>
        /// The identifier of the <see cref="LegendBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendBackgroundProperty =
            DependencyProperty.Register("LegendBackground", typeof(Brush), typeof(Legend),
                new FrameworkPropertyMetadata(null));

        static Legend()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Legend), new FrameworkPropertyMetadata(typeof(Legend)));
        }

        /// <summary>
        /// Gets or sets legend background
        /// </summary>
        public Brush LegendBackground
        {
            get { return (Brush)GetValue(LegendBackgroundProperty); }
            set { SetValue(LegendBackgroundProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend text
        /// </summary>
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        internal int Index { get; set; }
    }
}
