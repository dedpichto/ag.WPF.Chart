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
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(Legend),
                new FrameworkPropertyMetadata(""));
        /// <summary>
        /// The identifier of the <see cref="LegendBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendBackgroundProperty =
            DependencyProperty.Register(nameof(LegendBackground), typeof(Brush), typeof(Legend),
                new FrameworkPropertyMetadata(null));
        /// <summary>
        /// The identifier of the <see cref="IsChecked"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckedProperty =
            DependencyProperty.Register(nameof(IsChecked), typeof(bool), typeof(Legend),
                new FrameworkPropertyMetadata(true));
        static Legend()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Legend), new FrameworkPropertyMetadata(typeof(Legend)));
        }

        /// <summary>
        /// Gets or sets legend background
        /// </summary>
        public Brush LegendBackground
        {
            get => (Brush)GetValue(LegendBackgroundProperty);
            set => SetValue(LegendBackgroundProperty, value);
        }
        /// <summary>
        /// Gets or sets legend text
        /// </summary>
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }
        /// <summary>
        /// Gets or sets legend's checkbox state
        /// </summary>
        public bool IsChecked
        {
            get => (bool)GetValue(IsCheckedProperty);
            set => SetValue(IsCheckedProperty, value);
        }
        internal int SeriesIndex { get; set; }
    }
}
