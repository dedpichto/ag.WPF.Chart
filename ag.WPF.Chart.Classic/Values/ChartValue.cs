using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ag.WPF.Chart.Values
{
    /// <summary>
    /// Represents the basic abstract class of chart value
    /// </summary>
    public abstract class ChartValue : DependencyObject, IChartValue
    {
        #region Dependency properties
        ///// <summary>
        ///// The identifier of the <see cref="Values"/> dependency property.
        ///// </summary>
        //public static readonly DependencyProperty ValuesProperty = DependencyProperty.Register(nameof(Values), typeof(ValuesStruct), typeof(ChartValue), new FrameworkPropertyMetadata(new ValuesStruct()));
        //#endregion

        //#region Public properties
        ///// <inheritdoc />
        //public ValuesStruct Values 
        //{
        //    get { return (ValuesStruct)GetValue(ValuesProperty); }
        //    set { SetValue(ValuesProperty, value); }
        //}
        /// <inheritdoc />
        public ValuesStruct CompositeValue { get; set; }
        /// <inheritdoc />
        public string CustomValue { get; set; }
        /// <inheritdoc />
        public abstract IChartValue Clone();
        #endregion
    }
}
