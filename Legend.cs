using ag.WPF.Chart.Series;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ag.WPF.Chart
{
    
    /// <summary>
    /// Represents series legend
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
