using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Xps.Packaging;
using ag.WPF.Chart.Values;
using ag.WPF.Chart.Series;
using Path = System.Windows.Shapes.Path;
using System.Collections;

namespace ag.WPF.Chart
{
    /// <summary>
    /// Specifies chart style
    /// </summary>
    public enum ChartStyle
    {
        /// <summary>
        /// The style represented by straight lines
        /// </summary>
        Lines,
        /// <summary>
        /// The style represented by straight stacked lines
        /// </summary>
        StackedLines,
        /// <summary>
        /// The style represented by straight 100% stacked lines
        /// </summary>
        FullStackedLines,
        /// <summary>
        /// The style represented by smooth lines
        /// </summary>
        SmoothLines,
        /// <summary>
        /// The style represented by smooth stacked lines
        /// </summary>
        SmoothStackedLines,
        /// <summary>
        /// The style represented by smooth 100% stacked lines
        /// </summary>
        SmoothFullStackedLines,
        /// <summary>
        /// The style represented by straight lines with markers at control points
        /// </summary>
        LinesWithMarkers,
        /// <summary>
        /// The style represented by straight stacked lines with markers at control points
        /// </summary>
        StackedLinesWithMarkers,
        /// <summary>
        /// The style represented by straight 100% stacked lines with markers at control points
        /// </summary>
        FullStackedLinesWithMarkers,
        /// <summary>
        /// The style represented by smooth lines with markers at control points
        /// </summary>
        SmoothLinesWithMarkers,
        /// <summary>
        /// The style represented by smooth stacked lines with markers at control points
        /// </summary>
        SmoothStackedLinesWithMarkers,
        /// <summary>
        /// The style represented by smooth 100% stacked lines with markers at control points
        /// </summary>
        SmoothFullStackedLinesWithMarkers,
        /// <summary>
        /// The style represented by columns
        /// </summary>
        Columns,
        /// <summary>
        /// The style represented by stacked columns
        /// </summary>
        StackedColumns,
        /// <summary>
        /// The style represented by 100% stacked columns
        /// </summary>
        FullStackedColumns,
        /// <summary>
        /// The style represented by bars
        /// </summary>
        Bars,
        /// <summary>
        /// The style represented by stacked bars
        /// </summary>
        StackedBars,
        /// <summary>
        /// The style represented by 100% stacked bars
        /// </summary>
        FullStackedBars,
        /// <summary>
        /// The style represented by areas
        /// </summary>
        Area,
        /// <summary>
        /// The style represented by stacked areas
        /// </summary>
        StackedArea,
        /// <summary>
        /// The style represented by 100% stacked areas
        /// </summary>
        FullStackedArea,
        /// <summary>
        /// The style represented by smooth areas
        /// </summary>
        SmoothArea,
        /// <summary>
        /// The style represented by smooth stacked areas
        /// </summary>
        SmoothStackedArea,
        /// <summary>
        /// The style represented by 100% smooth stacked areas
        /// </summary>
        SmoothFullStackedArea,
        /// <summary>
        /// The style represented by bubbles
        /// </summary>
        Bubbles,
        /// <summary>
        /// The style represented by solid sectors
        /// </summary>
        SolidPie,
        /// <summary>
        /// The style represented by sectors divided with thin lines
        /// </summary>
        SlicedPie,
        /// <summary>
        /// The style represented by solid arcs
        /// </summary>
        Doughnut,
        /// <summary>
        /// The style represented by cumulated effect of positive and negative values
        /// </summary>
        Waterfall,
        /// <summary>
        /// The style represented by radar
        /// </summary>
        Radar,
        /// <summary>
        /// The style represented by radar with markers at control points
        /// </summary>
        RadarWithMarkers,
        /// <summary>
        /// The style represented by radar areas
        /// </summary>
        RadarArea,
        /// <summary>
        /// The style represented by funnel
        /// </summary>
        Funnel,
        /// <summary>
        /// The stock style represented by high, low, and close prices
        /// </summary>
        HighLowClose,
        /// <summary>
        /// The stock style represented by open, high, low, and close prices
        /// </summary>
        OpenHighLowClose
    }

    /// <summary>
    /// Specifies chart legend alignment
    /// </summary>
    public enum LegendAlignment
    {
        /// <summary>
        /// Legend is left aligned (default)
        /// </summary>
        Left,
        /// <summary>
        /// Legend is top aligned
        /// </summary>
        Top,
        /// <summary>
        /// Legend is right aligned
        /// </summary>
        Right,
        /// <summary>
        /// Legend is bottom aligned
        /// </summary>
        Bottom
    }

    /// <summary>
    /// Specifies visiblity of numeric/custom values drawn next to axes
    /// </summary>
    public enum AxesVisibility
    {
        /// <summary>
        /// No values are visible
        /// </summary>
        None,
        /// <summary>
        /// Horizontal (X) axis values are visible
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical (Y) axis values are visible
        /// </summary>
        Vertical,
        /// <summary>
        /// Values of both axes are visible (default)
        /// </summary>
        Both
    }

    /// <summary>
    /// Specifies auto-adjustment mode of chart control
    /// </summary>
    public enum AutoAdjustmentMode
    {
        /// <summary>
        /// No auto-adjustment
        /// </summary>
        None,
        /// <summary>
        /// Horizontal values are auto-adjusted
        /// </summary>
        Horizontal,
        /// <summary>
        /// Vertical values are auto-adjusted
        /// </summary>
        Vertical,
        /// <summary>
        /// Both horizontal and vertical values are auto-adjusted (default)
        /// </summary>
        Both
    }

    /// <summary>
    /// Specifies size of legend
    /// </summary>
    public enum LegendSize
    {
        /// <summary>
        /// 16x16 size (default)
        /// </summary>
        ExtraSmall = 16,
        /// <summary>
        /// 24x24 size
        /// </summary>
        Small = 24,
        /// <summary>
        /// 32x32 size
        /// </summary>
        Menium = 32,
        /// <summary>
        /// 48x48 size
        /// </summary>
        Large = 48,
        /// <summary>
        /// 64x64 size
        /// </summary>
        ExtraLarge = 64
    }

    /// <summary>
    /// Specifies shape of legend
    /// </summary>
    public enum ShapeStyle
    {
        /// <summary>
        /// Rectanglular shape (default)
        /// </summary>
        Rectangle,
        /// <summary>
        /// Circular shape
        /// </summary>
        Circle,
        /// <summary>
        /// Star shape with five rays
        /// </summary>
        Star5,
        /// <summary>
        /// Star shape with six rays
        /// </summary>
        Star6,
        /// <summary>
        /// Star shape with eight rays
        /// </summary>
        Star8
    }

    /// <summary>
    /// Specifies chart boundary
    /// </summary>
    public enum ChartBoundary
    {
        /// <summary>
        /// Chart boundary starts on y-axes
        /// </summary>
        OnAxes,
        /// <summary>
        /// Chart boundary starts with offset from y-axes (default)
        /// </summary>
        WithOffset
    }

    internal enum ColoredPaths
    {
        Stock,
        Up,
        Down
    }

    /// <summary>
    /// Represents a custom control containing series, axes, legends and other hosted content.
    /// </summary>
    ///<remarks>
    /// A <see cref="Chart"/> contains a collection of <see cref="Series.Series"/> objects, which are in <see cref="SeriesSource"/> property.
    /// <para>
    /// A <see cref="Series.Series"/> may be of type <see cref="PlainSeries"/>, which consits of simple double values, associated with each series point, or any financial series, such as <see cref="HighLowCloseSeries"/>, <see cref="OpenHighLowCloseSeries"/> and so on.
    /// </para>
    /// <para>
    /// Each <see cref="Series.Series"/> contains a collection of <see cref="ChartValue"/> objects, which can be of type <see cref="PlainChartValue"/>, <see cref="HighLowCloseChartValue"/>, <see cref="OpenHighLowCloseChartValue"/>.
    /// </para>
    ///</remarks>
    #region Named parts
    [TemplatePart(Name = ElementCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = ElementPieImage, Type = typeof(Image))]
    [TemplatePart(Name = ElementVPlaceholder, Type = typeof(Border))]
    [TemplatePart(Name = ElementPathYAxisValues, Type = typeof(Path))]
    [TemplatePart(Name = ElementHPlaceholder, Type = typeof(Border))]
    [TemplatePart(Name = ElementPathXAxisValues, Type = typeof(Path))]
    [TemplatePart(Name = ElementPathRadarValues, Type = typeof(Path))]
    [TemplatePart(Name = ElementPathRadarLines, Type = typeof(Path))]
    [TemplatePart(Name = ElementPathXAxesLines, Type = typeof(Path))]
    [TemplatePart(Name = ElementPathYAxesLines, Type = typeof(Path))]
    [TemplatePart(Name = ElementPathTicks, Type = typeof(Path))]
    [TemplatePart(Name = ElementPathHorzLines, Type = typeof(Path))]
    [TemplatePart(Name = ElementPathVertLines, Type = typeof(Path))]

    #endregion

    public class Chart : Control, INotifyPropertyChanged
    {
        private (Brush Brush, int Counter)[] PredefinedMainBrushes { get; } =
        {
            (new SolidColorBrush(Color.FromArgb(255, 91, 155, 213)),0),
            (new SolidColorBrush(Color.FromArgb(255, 237, 125, 49)),0),
            (new SolidColorBrush(Color.FromArgb(255, 165, 165, 165)),0),
            (new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)),0),
            (new SolidColorBrush(Color.FromArgb(255, 68, 114, 196)),0),
            (new SolidColorBrush(Color.FromArgb(255, 112, 173, 71)),0),
            (new SolidColorBrush(Color.FromArgb(255, 37, 94, 145)),0),
            (new SolidColorBrush(Color.FromArgb(255, 158, 72, 14)),0),
            (new SolidColorBrush(Color.FromArgb(255, 99, 99, 99)),0),
            (new SolidColorBrush(Color.FromArgb(255, 153, 115, 0)),0)
        };

        private (Brush Brush, int Counter)[] PredefinedSecondaryBrushes { get; } =
        {
            (new SolidColorBrush(Color.FromArgb(255, 237, 125, 49)),0),
            (new SolidColorBrush(Color.FromArgb(255, 165, 165, 165)),0),
            (new SolidColorBrush(Color.FromArgb(255, 255, 192, 0)),0),
            (new SolidColorBrush(Color.FromArgb(255, 68, 114, 196)),0),
            (new SolidColorBrush(Color.FromArgb(255, 112, 173, 71)),0),
            (new SolidColorBrush(Color.FromArgb(255, 37, 94, 145)),0),
            (new SolidColorBrush(Color.FromArgb(255, 158, 72, 14)),0),
            (new SolidColorBrush(Color.FromArgb(255, 99, 99, 99)),0),
            (new SolidColorBrush(Color.FromArgb(255, 153, 115, 0)),0),
            (new SolidColorBrush(Color.FromArgb(255, 91, 155, 213)),0)
        };

        private ChartItemsCollection<ISeries> _series;

        #region Constants
        private const string ElementCanvas = "PART_Canvas";
        private const string ElementPieImage = "PART_PieImage";
        private const string ElementVPlaceholder = "PART_VPlaceHolder";
        private const string ElementPathYAxisValues = "PART_PathYAxisValues";
        private const string ElementHPlaceholder = "PART_HPlaceHolder";
        private const string ElementPathXAxisValues = "PART_PathXAxisValues";
        private const string ElementPathRadarValues = "PART_PathRadarValues";
        private const string ElementPathRadarLines = "PART_PathRadarLines";
        private const string ElementPathXAxesLines = "PART_PathXAxesLines";
        private const string ElementPathYAxesLines = "PART_PathYAxesLines";
        private const string ElementPathTicks = "PART_PathTicks";
        private const string ElementPathHorzLines = "PART_PathHorzLines";
        private const string ElementPathVertLines = "PART_PathVertLines";

        #endregion

        #region Elements
        private Canvas _canvas;
        private Image _pieImage;
        private Border _borderVPlaceholder;
        private Path _pathYAxisValues;
        private Border _borderHPlaceholder;
        private Path _pathXAxisValues;
        private Path _pathRadarValues;
        private Path _pathRadarLines;
        private Path _pathXAxesLines;
        private Path _pathYAxesLines;
        private Path _pathTicks;
        private Path _pathHorzLines;
        private Path _pathVertLines;

        #endregion

        #region Dependency properties

        #region Title properties
        /// <summary>
        /// The identifier of the <see cref="Title"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(nameof(Title), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("", OnTitleChanged));
        /// <summary>
        /// The identifier of the <see cref="TitleFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontFamilyProperty = DependencyProperty.Register(nameof(TitleFontFamily), typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontFamily, OnTitleFontFamilyChanged));
        /// <summary>
        /// The identifier of the <see cref="TitleFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontWeightProperty = DependencyProperty.Register(nameof(TitleFontWeight), typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontWeight, OnTitleFontWeightChanged));
        /// <summary>
        /// The identifier of the <see cref="TitleFontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontStyleProperty = DependencyProperty.Register(nameof(TitleFontStyle), typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontStyle, OnTitleFontStyleChanged));
        /// <summary>
        /// The identifier of the <see cref="TitleFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontSizeProperty = DependencyProperty.Register(nameof(TitleFontSize), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontSize, OnTitleFontSizeChanged));
        /// <summary>
        /// The identifier of the <see cref="TitleFontStretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleFontStretchProperty = DependencyProperty.Register(nameof(TitleFontStretch), typeof(FontStretch), typeof(Chart),
                new FrameworkPropertyMetadata(FontStretches.Normal, OnTitleFontStretchChanged));

        #endregion

        #region Legend properties
        /// <summary>
        /// The identifier of the <see cref="LegendFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontFamilyProperty = DependencyProperty.Register(nameof(LegendFontFamily), typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontFamily, OnLegendFontFamilyChanged));
        /// <summary>
        /// The identifier of the <see cref="LegendFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontWeightProperty = DependencyProperty.Register(nameof(LegendFontWeight), typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontWeight, OnLegendFontWeightChanged));
        /// <summary>
        /// The identifier of the <see cref="LegendFontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontStyleProperty = DependencyProperty.Register(nameof(LegendFontStyle), typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontStyle, OnLegendFontStyleChanged));
        /// <summary>
        /// The identifier of the <see cref="LegendFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontSizeProperty = DependencyProperty.Register(nameof(LegendFontSize), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontSize, OnLegendFontSizeChanged));
        /// <summary>
        /// The identifier of the <see cref="LegendAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendAlignmentProperty = DependencyProperty.Register(nameof(LegendAlignment), typeof(LegendAlignment), typeof(Chart),
                new FrameworkPropertyMetadata(LegendAlignment.Bottom, OnLegendAlignmentChanged));
        /// <summary>
        /// The identifier of the <see cref="LegendSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendSizeProperty = DependencyProperty.Register(nameof(LegendSize), typeof(LegendSize), typeof(Chart),
                new FrameworkPropertyMetadata(LegendSize.ExtraSmall, OnLegendSizeChanged));
        /// <summary>
        /// The identifier of the <see cref="LegendShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendShapeProperty = DependencyProperty.Register(nameof(LegendShape), typeof(ShapeStyle), typeof(Chart),
                new FrameworkPropertyMetadata(ShapeStyle.Rectangle, OnLegendShapeChanged));
        /// <summary>
        /// The identifier of the <see cref="ShowLegend"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowLegendProperty = DependencyProperty.Register(nameof(ShowLegend), typeof(bool), typeof(Chart),
                new FrameworkPropertyMetadata(true, OnShowLegendChanged));

        #endregion

        #region Axes properties
        /// <summary>
        /// The identifier of the <see cref="AxesFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontFamilyProperty = DependencyProperty.Register(nameof(AxesFontFamily), typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontFamily, OnAxesFontFamilyChanged));
        /// <summary>
        /// The identifier of the <see cref="AxesFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontWeightProperty = DependencyProperty.Register(nameof(AxesFontWeight), typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontWeight, OnAxesFontWeightChanged));
        /// <summary>
        /// The identifier of the <see cref="AxesFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontSizeProperty = DependencyProperty.Register(nameof(AxesFontSize), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontSize, OnAxesFontSizeChanged));
        /// <summary>
        /// The identifier of the <see cref="AxesFontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontStyleProperty = DependencyProperty.Register(nameof(AxesFontStyle), typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontStyle, OnAxesFontStyleChanged));
        /// <summary>
        /// The identifier of the <see cref="AxesFontStretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontStretchProperty = DependencyProperty.Register(nameof(AxesFontStretch), typeof(FontStretch), typeof(Chart),
                new FrameworkPropertyMetadata(FontStretches.Normal, OnAxesFontStretchChanged));
        /// <summary>
        /// The identifier of the <see cref="AxisTitleX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisTitleXProperty = DependencyProperty.Register(nameof(AxisTitleX), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("x-Axis", OnAxisTitleXChanged));
        /// <summary>
        /// The identifier of the <see cref="AxisTitleY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisTitleYProperty = DependencyProperty.Register(nameof(AxisTitleY), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("y-Axis", OnAxisTitleYChanged));
        /// <summary>
        /// The identifier of the <see cref="AxesValuesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesValuesVisibilityProperty = DependencyProperty.Register(nameof(AxesValuesVisibility), typeof(AxesVisibility), typeof(Chart),
                new FrameworkPropertyMetadata(AxesVisibility.Both, OnAxesValuesVisibilityChanged));
        /// <summary>
        /// The identifier of the <see cref="AxesLinesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesLinesVisibilityProperty = DependencyProperty.Register(nameof(AxesLinesVisibility), typeof(AxesVisibility), typeof(Chart),
                new FrameworkPropertyMetadata(AxesVisibility.Both, OnAxesLinesVisibilityChanged));
        /// <summary>
        /// The identifier of the <see cref="SecondaryLinesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryLinesVisibilityProperty = DependencyProperty.Register(nameof(SecondaryLinesVisibility), typeof(AxesVisibility), typeof(Chart),
                new FrameworkPropertyMetadata(AxesVisibility.Both, OnSecondaryLinesVisibilityChanged));
        /// <summary>
        /// The identifier of the <see cref="ValuesFormatY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValuesFormatYProperty = DependencyProperty.Register(nameof(ValuesFormatY), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("0", OnValuesFormatYChanged, CoerceFormats));
        /// <summary>
        /// The identifier of the <see cref="HorizontalAxisValuesFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ValuesFormatX = DependencyProperty.Register(nameof(HorizontalAxisValuesFormat), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("0", OnHorizontalAxisValuesFormatChanged, CoerceFormats));
        /// <summary>
        /// The identifier of the <see cref="PiePercentsFormatProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PiePercentsFormatProperty = DependencyProperty.Register(nameof(PiePercentsFormat), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("0", OnPiePercentsFormatChanged, CoerceFormats));
        /// <summary>
        /// The identifier of the <see cref="CustomValuesX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomValuesXProperty = DependencyProperty.Register(nameof(CustomValuesX), typeof(IEnumerable<string>), typeof(Chart),
                new FrameworkPropertyMetadata(null, OnCustomValuesXChanged));
        /// <summary>
        /// The identifier of the <see cref="CustomValuesY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomValuesYProperty = DependencyProperty.Register(nameof(CustomValuesY), typeof(IEnumerable<string>), typeof(Chart),
                new FrameworkPropertyMetadata(null, OnCustomValuesYChanged));

        #endregion

        #region Misc properties
        /// <summary>
        /// The identifier of the <see cref="MarkerShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerShapeProperty = DependencyProperty.Register(nameof(MarkerShape), typeof(ShapeStyle), typeof(Chart),
                new FrameworkPropertyMetadata(ShapeStyle.Rectangle, OnMarkerShapeCahnged));
        /// <summary>
        /// The identifier of the <see cref="SectionsX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SectionsXProperty = DependencyProperty.Register(nameof(SectionsX), typeof(int), typeof(Chart),
                new FrameworkPropertyMetadata(10, OnSectionsXChanged, CoerceSectionsX));
        /// <summary>
        /// The identifier of the <see cref="SectionsY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SectionsYProperty = DependencyProperty.Register(nameof(SectionsY), typeof(int), typeof(Chart),
                new FrameworkPropertyMetadata(10, OnSectionsYChanged, CoerceSectionsY));
        /// <summary>
        /// The identifier of the <see cref="ShowValuesOnBarsAndColumns"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowValuesOnBarsAndColumnsProperty = DependencyProperty.Register(nameof(ShowValuesOnBarsAndColumns), typeof(bool), typeof(Chart),
                new FrameworkPropertyMetadata(true, OnShowValuesOnBarsAndColumnsChanged));
        /// <summary>
        /// The identifier of the <see cref="ChartStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartStyleProperty = DependencyProperty.Register(nameof(ChartStyle), typeof(ChartStyle), typeof(Chart),
                new FrameworkPropertyMetadata(ChartStyle.Lines, OnChartStyleChanged));
        /// <summary>
        /// The identifier of the <see cref="ChartOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartOpacityProperty = DependencyProperty.Register(nameof(ChartOpacity), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(1.0, OnChartOpacityChanged, CoerceChartOpacity));
        /// <summary>
        /// The identifier of the <see cref="MaxX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxXProperty = DependencyProperty.Register(nameof(MaxX), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(100.0, OnMaxXChanged, CoerceMaxX));
        /// <summary>
        /// The identifier of the <see cref="MaxY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxYProperty = DependencyProperty.Register(nameof(MaxY), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(100.0, OnMaxYChanged, CoerceMaxY));
        /// <summary>
        /// The identifier of the <see cref="LineThickness"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LineThicknessProperty = DependencyProperty.Register(nameof(LineThickness), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(2.0, OnLineThicknessChanged, CoerceLineThickness));
        /// <summary>
        /// The identifier of the <see cref="AutoAdjustment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoAdjustmentProperty = DependencyProperty.Register(nameof(AutoAdjustment), typeof(AutoAdjustmentMode), typeof(Chart),
                new FrameworkPropertyMetadata(AutoAdjustmentMode.Both, OnAutoAdjustmentChanged));
        /// <summary>
        /// The identifier of the <see cref="ShowTicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowTicksProperty = DependencyProperty.Register(nameof(ShowTicks),
                typeof(AxesVisibility), typeof(Chart), new FrameworkPropertyMetadata(AxesVisibility.Both, OnShowTicksChanged));
        /// <summary>
        /// The identifier of the <see cref="ChartBoundary"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartBoundaryProperty = DependencyProperty.Register(nameof(ChartBoundary), typeof(ChartBoundary), typeof(Chart),
                new FrameworkPropertyMetadata(ChartBoundary.WithOffset, OnChartBoundaryChanged));
        /// <summary>
        /// The identifier of the <see cref="LegendsWaterfallProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendsWaterfallProperty = DependencyProperty.RegisterAttached(nameof(LegendsWaterfall), typeof(IEnumerable<string>), typeof(Chart),
            new FrameworkPropertyMetadata(new[] { "Increase", "Decrease" }, null, CoerceLegendsWaterfall));
        /// <summary>
        /// The identifier of the <see cref="LegendsHighLowCloseProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendsHighLowCloseProperty = DependencyProperty.RegisterAttached(nameof(LegendsHighLowClose), typeof(IEnumerable<string>), typeof(Chart),
            new FrameworkPropertyMetadata(new[] { "High", "Low" }, null, CoerceLegendsHighLowClose));
        /// <summary>
        /// The identifier of the <see cref="LegendsOpenHighLowCloseProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendsOpenHighLowCloseProperty = DependencyProperty.RegisterAttached(nameof(LegendsOpenHighLowClose), typeof(IEnumerable<string>), typeof(Chart),
            new FrameworkPropertyMetadata(new[] { "Increase", "Decrease" }, null, CoerceLegendsOpenHighLowClose));
        /// <summary>
        /// The identifier of the <see cref="SeriesSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesSourceProperty = DependencyProperty.Register(nameof(SeriesSource), typeof(IEnumerable<ISeries>), typeof(Chart), new FrameworkPropertyMetadata(null, OnSeriesSourceChanged));

        #endregion

        #endregion

        #region ctor
        static Chart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart), new FrameworkPropertyMetadata(typeof(Chart)));
        }
        #endregion

        #region Private fields
        private readonly ObservableCollection<FrameworkElement> _legendsCollection = new ObservableCollection<FrameworkElement>();
        private readonly ObservableCollection<FrameworkElement> _pieLegendsCollection = new ObservableCollection<FrameworkElement>();
        #endregion

        #region Overrides

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            _borderVPlaceholder = GetTemplateChild(ElementVPlaceholder) as Border;
            _pathYAxisValues = GetTemplateChild(ElementPathYAxisValues) as Path;
            _borderHPlaceholder = GetTemplateChild(ElementHPlaceholder) as Border;
            _pathXAxisValues = GetTemplateChild(ElementPathXAxisValues) as Path;
            _pathRadarValues = GetTemplateChild(ElementPathRadarValues) as Path;
            _pathRadarLines = GetTemplateChild(ElementPathRadarLines) as Path;
            _pathXAxesLines = GetTemplateChild(ElementPathXAxesLines) as Path;
            _pathYAxesLines = GetTemplateChild(ElementPathYAxesLines) as Path;
            _pathTicks = GetTemplateChild(ElementPathTicks) as Path;
            _pathHorzLines = GetTemplateChild(ElementPathHorzLines) as Path;
            _pathVertLines = GetTemplateChild(ElementPathVertLines) as Path;

            if (_canvas != null)
            {
                _canvas.Loaded -= Canvas_Loaded;
            }
            _canvas = GetTemplateChild(ElementCanvas) as Canvas;
            if (_canvas != null)
            {
                _canvas.Loaded += Canvas_Loaded;
            }

            if (_pieImage != null)
            {
                _pieImage.MouseMove -= PieImage_MouseMove;
            }
            _pieImage = GetTemplateChild(ElementPieImage) as Image;
            if (_pieImage != null)
            {
                _pieImage.MouseMove += PieImage_MouseMove;
            }
        }

        #endregion

        #region Private event handlers

        private void Canvas_Loaded(object sender, RoutedEventArgs e)
        {
            var actualSeries = getActualSeries();
            if (actualSeries == null) return;
            // the event is raised every time the control becomes visible
            // if all paths have been added before - just exit
            if (!actualSeries.SelectMany(s => s.Paths).Any(p => p != null && !(bool)p.GetValue(Statics.AddedToCanvasProperty)))
                return;
            foreach (var series in actualSeries)
            {
                for (var pathIndex = 0; pathIndex < series.Paths.Count(); pathIndex++)
                {
                    if (series.Paths[pathIndex] == null || (bool)series.Paths[pathIndex].GetValue(Statics.AddedToCanvasProperty)) continue;
                    setPathProperties(series, pathIndex);
                }
            }
        }

        private void SeriesSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (!(e.NewItems[0] is ISeries series)) break;

                        addSeries(series, e.NewStartingIndex);

                        break;
                    }
                case NotifyCollectionChangedAction.Replace:
                    {
                        if (!(e.OldItems[0] is ISeries oldSeries)) break;

                        removeSeries(oldSeries, false);

                        if (!(e.NewItems[0] is ISeries newSeries)) break;

                        addSeries(newSeries, e.OldStartingIndex, false);
                        break; ;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (!(e.OldItems[0] is ISeries series)) break;

                        removeSeries(series);

                        break;
                    }
            }
        }

        private void PieImage_MouseMove(object sender, MouseEventArgs e)
        {
            var actualSeries = getActualSeries();
            if (!actualSeries.Any()) return;
            if (!(_pieImage.ToolTip is ToolTip tooltip)) return;
            var position = e.GetPosition(_pieImage);
            if (_pieImage.Source is DrawingImage dw)
            {
                if (dw.Drawing is DrawingGroup dwg)
                {
                    foreach (var gd in dwg.Children.OfType<GeometryDrawing>())
                    {
                        if (!(gd.Geometry is Geometry geometry)) continue;
                        if (!geometry.FillContains(position)) continue;
                        var data = (string)geometry.GetValue(Statics.SectorDataProperty);
                        if (!Equals(tooltip.Content, data))
                        {
                            tooltip.Content = data;
                            tooltip.Placement = PlacementMode.Relative;
                        }
                        tooltip.HorizontalOffset = position.X + 10;
                        tooltip.VerticalOffset = position.Y + 10;
                        return;
                    }
                }
            }
            tooltip.Placement = PlacementMode.Mouse;
            tooltip.HorizontalOffset = 0;
            tooltip.VerticalOffset = 0;
            tooltip.Content = actualSeries.First().Name;
        }

        private void Series_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Values":
                    if (sender is ISeries series)
                    {
                        var actualSeries = getActualSeries();
                        foreach (var sr in actualSeries.Where(s => s.Index != series.Index))
                        {
                            foreach (var p in sr.Paths)
                            {
                                if (p == null) continue;
                                var b = BindingOperations.GetMultiBindingExpression(p, Path.DataProperty);
                                if (b != null)
                                    b.UpdateTarget();
                            }
                        }
                        rebuildPieLegends(series.Values, series);
                    }

                    updateBindings();
                    break;
            }
            OnPropertyChanged("SeriesSource");
        }

        private void Path_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is Path path)) return;
            if (!(path.Tag is ISeries s)) return;
            if (!(path.ToolTip is ToolTip tooltip)) return;
            Rect rc;
            switch (ChartStyle)
            {
                case ChartStyle.LinesWithMarkers:
                case ChartStyle.StackedLinesWithMarkers:
                case ChartStyle.FullStackedLinesWithMarkers:
                case ChartStyle.SmoothLinesWithMarkers:
                case ChartStyle.SmoothStackedLinesWithMarkers:
                case ChartStyle.SmoothFullStackedLinesWithMarkers:
                case ChartStyle.Bubbles:
                case ChartStyle.RadarWithMarkers:
                case ChartStyle.Funnel:
                case ChartStyle.Bars:
                case ChartStyle.StackedBars:
                case ChartStyle.FullStackedBars:
                case ChartStyle.Columns:
                case ChartStyle.StackedColumns:
                case ChartStyle.FullStackedColumns:
                case ChartStyle.Waterfall:
                    rc = s.RealRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
                    if (rc != default)
                    {
                        var index = s.RealRects.IndexOf(rc);
                        if (s.Values.Count <= index)
                        {
                            tooltip.Content = s.Name;
                            break;
                        }
                        var content = $"{s.Name}\n{s.Values[index].Value.PlainValue.ToString(CultureInfo.InvariantCulture)}";
                        if (!string.IsNullOrEmpty(s.Values[index].CustomValue))
                            content += $"\n{s.Values[index].CustomValue}";
                        tooltip.Content = content;
                    }
                    else
                    {
                        tooltip.Content = s.Name;
                    }
                    break;
                case ChartStyle.HighLowClose:
                    rc = s.RealRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
                    if (rc != default)
                    {
                        var index = s.RealRects.IndexOf(rc);
                        if (s.Values.Count <= index)
                        {
                            tooltip.Content = s.Name;
                            break;
                        }
                        var content = $"High\t{s.Values[index].Value.HighValue.ToString(CultureInfo.InvariantCulture)}\n" +
                            $"Low\t{s.Values[index].Value.LowValue.ToString(CultureInfo.InvariantCulture)}\n" +
                            $"Close\t{s.Values[index].Value.CloseValue.ToString(CultureInfo.InvariantCulture)}";
                        if (!string.IsNullOrEmpty(s.Values[index].CustomValue))
                            content += $"\n{s.Values[index].CustomValue}";
                        tooltip.Content = content;
                    }
                    else
                    {
                        tooltip.Content = s.Name;
                    }
                    break;
                case ChartStyle.OpenHighLowClose:
                    rc = s.RealRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
                    if (rc != default)
                    {
                        var index = s.RealRects.IndexOf(rc);
                        if (s.Values.Count <= index)
                        {
                            tooltip.Content = s.Name;
                            break;
                        }
                        var content = $"Open\t{s.Values[index].Value.OpenValue.ToString(CultureInfo.InvariantCulture)}\n" +
                            $"High\t{s.Values[index].Value.HighValue.ToString(CultureInfo.InvariantCulture)}\n" +
                            $"Low\t{s.Values[index].Value.LowValue.ToString(CultureInfo.InvariantCulture)}\n" +
                            $"Close\t{s.Values[index].Value.CloseValue.ToString(CultureInfo.InvariantCulture)}";
                        if (!string.IsNullOrEmpty(s.Values[index].CustomValue))
                            content += $"\n{s.Values[index].CustomValue}";
                        tooltip.Content = content;
                    }
                    else
                    {
                        tooltip.Content = s.Name;
                    }
                    return;
                default:
                    tooltip.Content = s.Name;
                    break;
            }
        }

        private void Path_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Path path)) return;
            if (!(path.Tag is ISeries s)) return;
            if (!ChartStyle.In(ChartStyle.LinesWithMarkers, ChartStyle.StackedLinesWithMarkers,
                ChartStyle.FullStackedLinesWithMarkers, ChartStyle.SmoothLinesWithMarkers,
                ChartStyle.SmoothStackedLinesWithMarkers, ChartStyle.SmoothFullStackedLinesWithMarkers,
                ChartStyle.Bubbles, ChartStyle.Columns, ChartStyle.StackedColumns, ChartStyle.FullStackedColumns,
                ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars,
                ChartStyle.RadarWithMarkers, ChartStyle.Waterfall, ChartStyle.HighLowClose, ChartStyle.OpenHighLowClose) || e.ClickCount != 2) return;
            var rc = s.RealRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
            if (rc == default)
                return;
            var index = s.RealRects.IndexOf(rc);
            if (index == -1)
                return;
            if (s.Values.Count <= index)
            {
                return;
            }
            RaiseMarkerLeftButtonDoubleClickEvent(s.Values[index], s);
        }
        #endregion

        #region Private functions
        private IEnumerable<ISeries> getActualSeries()
        {
            var b = BindingOperations.GetBinding(this, SeriesSourceProperty);
            if (b == null)
            {
                return Series;
            }
            else
            {
                return SeriesSource;
            }
        }

        private void addSeries(ISeries series, int index, bool shiftIndexes = true)
        {
            series.Index = index;
            series.PropertyChanged += Series_PropertyChanged;

            if (shiftIndexes)
            {
                var actualSeries = getActualSeries();
                foreach (var sr in actualSeries.Skip(index + 1).ToArray())
                {
                    sr.Index++;
                    foreach (var p in sr.Paths)
                    {
                        if (p == null) continue;
                        var binding = BindingOperations.GetMultiBindingExpression(p, Path.DataProperty);
                        if (binding != null)
                            binding.UpdateTarget();
                    }
                    rebuildPieLegends(sr.Values, sr);
                }
            }

            // The canvas may be null, because OnApplyTemplate is raised only when control is currently visible
            // So if the control is on the tab that isn't initially visible, for example, the canvas will be null
            if (_canvas != null)
            {
                for (var pathIndex = 0; pathIndex < series.Paths.Length; pathIndex++)
                {
                    setPathProperties(series, pathIndex);
                }
            }

            #region Main legend
            var legend = new Legend() { Index = series.Index };

            legend.SetBinding(Legend.LegendBackgroundProperty, new Binding(nameof(ISeries.MainBrush)) { Source = series });
            var legendVisibilityBinding = new MultiBinding { Converter = new LegendVisibilityConverter() };
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(ChartStyle))
            {
                Source = this
            });
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(SeriesSource))
            {
                Source = this
            });
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(Series))
            {
                Source = this
            });
            legendVisibilityBinding.NotifyOnSourceUpdated = true;
            legend.SetBinding(VisibilityProperty, legendVisibilityBinding);

            legend.SetBinding(Legend.TextProperty, new Binding(nameof(ISeries.Name)) { Source = series });

            LegendsCollection.Add(legend);
            #endregion

            #region Positive waterfall legend
            legend = new Legend() { Index = series.Index };

            legend.SetBinding(Legend.LegendBackgroundProperty, new Binding(nameof(ISeries.MainBrush)) { Source = series });
            legendVisibilityBinding = new MultiBinding { Converter = new LegendWaterfallVisibilityConverter() };
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(ChartStyle))
            {
                Source = this
            });
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(ISeries.Index))
            {
                Source = series
            });
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(SeriesSource))
            {
                Source = this
            });
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(Series))
            {
                Source = this
            });
            legend.SetBinding(VisibilityProperty, legendVisibilityBinding);

            legend.SetBinding(Legend.TextProperty, new Binding("LegendsWaterfall[0]") { Source = this });

            LegendsCollection.Add(legend);
            #endregion

            #region Negative waterfall legend
            legend = new Legend() { Index = series.Index };

            legend.SetBinding(Legend.LegendBackgroundProperty, new Binding(nameof(ISeries.SecondaryBrush)) { Source = series });
            legendVisibilityBinding = new MultiBinding { Converter = new LegendWaterfallVisibilityConverter() };
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(ChartStyle))
            {
                Source = this
            });
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(ISeries.Index))
            {
                Source = series
            });
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(SeriesSource))
            {
                Source = this
            });
            legendVisibilityBinding.Bindings.Add(new Binding(nameof(Series))
            {
                Source = this
            });
            legendVisibilityBinding.NotifyOnSourceUpdated = true;
            legend.SetBinding(VisibilityProperty, legendVisibilityBinding);

            legend.SetBinding(Legend.TextProperty, new Binding("LegendsWaterfall[1]") { Source = this });

            LegendsCollection.Add(legend);
            #endregion

            if (series.IsStockSeries())
            {
                //#region Stock legend
                //legend = new Legend() { Index = series.Index };

                //legend.SetBinding(Legend.LegendBackgroundProperty, new Binding("Foreground") { Source = this });
                //legendVisibilityBinding = new MultiBinding { Converter = new LegendStockVisibilityConverter() };
                //legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                //{
                //    Source = this
                //});
                //legendVisibilityBinding.Bindings.Add(new Binding("Index")
                //{
                //    Source = series
                //});
                //legendVisibilityBinding.Bindings.Add(new Binding("SeriesSource")
                //{
                //    Source = this
                //});
                //legendVisibilityBinding.ConverterParameter = ColoredPaths.Stock;
                //legendVisibilityBinding.NotifyOnSourceUpdated = true;
                //legend.SetBinding(VisibilityProperty, legendVisibilityBinding);
                //legend.Text = "Close";
                ////legend.SetBinding(Legend.TextProperty, new Binding("LegendsWaterfall[1]") { Source = this });

                //LegendsCollection.Add(legend);
                //#endregion

                #region Up stock legend
                legend = new Legend() { Index = series.Index };

                legend.SetBinding(Legend.LegendBackgroundProperty, new Binding(nameof(ISeries.MainBrush)) { Source = series });
                legendVisibilityBinding = new MultiBinding { Converter = new LegendStockVisibilityConverter() };
                legendVisibilityBinding.Bindings.Add(new Binding(nameof(ChartStyle))
                {
                    Source = this
                });
                legendVisibilityBinding.Bindings.Add(new Binding(nameof(ISeries.Index))
                {
                    Source = series
                });
                legendVisibilityBinding.Bindings.Add(new Binding(nameof(SeriesSource))
                {
                    Source = this
                });
                legendVisibilityBinding.Bindings.Add(new Binding(nameof(Series))
                {
                    Source = this
                });
                legendVisibilityBinding.ConverterParameter = ColoredPaths.Up;
                legendVisibilityBinding.NotifyOnSourceUpdated = true;
                legend.SetBinding(VisibilityProperty, legendVisibilityBinding);
                if (series is HighLowCloseSeries)
                    legend.SetBinding(Legend.TextProperty, new Binding("LegendsHighLowClose[0]") { Source = this });
                else if (series is OpenHighLowCloseSeries)
                    legend.SetBinding(Legend.TextProperty, new Binding("LegendsOpenHighLowClose[0]") { Source = this });
                LegendsCollection.Add(legend);
                #endregion

                #region Stock down legend
                legend = new Legend() { Index = series.Index };

                legend.SetBinding(Legend.LegendBackgroundProperty, new Binding(nameof(ISeries.SecondaryBrush)) { Source = series });
                legendVisibilityBinding = new MultiBinding { Converter = new LegendStockVisibilityConverter() };
                legendVisibilityBinding.Bindings.Add(new Binding(nameof(ChartStyle))
                {
                    Source = this
                });
                legendVisibilityBinding.Bindings.Add(new Binding(nameof(ISeries.Index))
                {
                    Source = series
                });
                legendVisibilityBinding.Bindings.Add(new Binding(nameof(SeriesSource))
                {
                    Source = this
                });
                legendVisibilityBinding.Bindings.Add(new Binding(nameof(Series))
                {
                    Source = this
                });
                legendVisibilityBinding.ConverterParameter = ColoredPaths.Down;
                legendVisibilityBinding.NotifyOnSourceUpdated = true;
                legend.SetBinding(VisibilityProperty, legendVisibilityBinding);
                if (series is HighLowCloseSeries)
                    legend.SetBinding(Legend.TextProperty, new Binding("LegendsHighLowClose[1]") { Source = this });
                else if (series is OpenHighLowCloseSeries)
                    legend.SetBinding(Legend.TextProperty, new Binding("LegendsOpenHighLowClose[1]") { Source = this });
                LegendsCollection.Add(legend);
                #endregion
            }

            rebuildPieLegends(series.Values, series);

            // if canvas is null, so are all other elements
            if (_canvas == null)
                return;

            updateBindings();
        }

        private void removeSeries(ISeries series, bool shiftIndexes = true)
        {
            series.PropertyChanged -= Series_PropertyChanged;

            foreach (var p in series.Paths)
            {
                if (p == null) continue;
                p.MouseLeftButtonDown -= Path_MouseLeftButtonDown;
                p.MouseMove -= Path_MouseMove;
                var i = _canvas.Children.IndexOf(p);
                if (i > -1)
                {
                    p.SetValue(Statics.AddedToCanvasProperty, false);
                    _canvas.Children.RemoveAt(i);
                }
            }

            for (var i = LegendsCollection.Count - 1; i >= 0; i--)
            {
                if (((Legend)LegendsCollection[i]).Index == series.Index)
                    LegendsCollection.RemoveAt(i);
            }

            if (shiftIndexes)
            {
                foreach (Legend lg in LegendsCollection.Where(l => ((Legend)l).Index > series.Index).ToArray())
                {
                    lg.Index--;
                }
                var actualSeries = getActualSeries();
                foreach (var sr in actualSeries.Where(sc => sc.Index > series.Index).ToArray())
                {
                    sr.Index--;
                    foreach (var p in sr.Paths)
                    {
                        if (p == null) continue;
                        var binding = BindingOperations.GetMultiBindingExpression(p, Path.DataProperty);
                        if (binding != null)
                            binding.UpdateTarget();
                    }
                    rebuildPieLegends(sr.Values, sr);
                }
            }

            if (!(bool)((Series.Series)series).GetValue(Statics.HasCustomMainBrushProperty))
            {
                for (var i = 0; i < PredefinedMainBrushes.Length; i++)
                {
                    if (series.MainBrush.Equals(PredefinedMainBrushes[i].Brush))
                    {
                        PredefinedMainBrushes[i].Counter--;
                    }
                }
            }
            if (!(bool)((Series.Series)series).GetValue(Statics.HasCustomSecondaryBrushProperty))
            {
                for (var i = 0; i < PredefinedSecondaryBrushes.Length; i++)
                {
                    if (series.SecondaryBrush.Equals(PredefinedSecondaryBrushes[i].Brush))
                    {
                        PredefinedSecondaryBrushes[i].Counter--;
                    }
                }
            }

            updateBindings();
        }

        private void setPathProperties(ISeries series, int pathIndex)
        {
            var path = series.Paths[pathIndex];
            if (path == null) return;
            (int, ColoredPaths) parameter;
            string brushPath;
            switch (pathIndex)
            {
                case 0:
                    if (series.MainBrush == null)
                    {
                        var min = PredefinedMainBrushes.Min(b => b.Counter);
                        for (var i = 0; i < PredefinedMainBrushes.Length; i++)
                        {
                            if (PredefinedMainBrushes[i].Counter == min)
                            {
                                series.MainBrush = PredefinedMainBrushes[i].Brush;
                                PredefinedMainBrushes[i].Counter++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        ((Series.Series)series).SetValue(Statics.HasCustomMainBrushProperty, true);
                    }
                    parameter = (0, ColoredPaths.Up);
                    brushPath = "MainBrush";
                    break;
                case 1:
                    if (series.SecondaryBrush == null)
                    {
                        var min = PredefinedSecondaryBrushes.Min(b => b.Counter);
                        for (var i = 0; i < PredefinedSecondaryBrushes.Length; i++)
                        {
                            if (PredefinedSecondaryBrushes[i].Counter == min)
                            {
                                series.SecondaryBrush = PredefinedSecondaryBrushes[i].Brush;
                                PredefinedSecondaryBrushes[i].Counter++;
                                break;
                            }
                        }
                    }
                    else
                    {
                        ((Series.Series)series).SetValue(Statics.HasCustomSecondaryBrushProperty, true);
                    }
                    parameter = (1, ColoredPaths.Down);
                    brushPath = "SecondaryBrush";
                    break;
                case 2:
                    if (!series.IsStockSeries()) return;
                    parameter = (0, ColoredPaths.Stock);
                    brushPath = "Foreground";
                    break;
                default:
                    return;
            }
            #region Path data
            var ptsBinding = new MultiBinding { Converter = new ValuesToPathConverter() };
            ptsBinding.Bindings.Add(new Binding("ActualWidth") { ElementName = ElementCanvas });
            ptsBinding.Bindings.Add(new Binding("ActualHeight") { ElementName = ElementCanvas });
            ptsBinding.Bindings.Add(new Binding(nameof(SeriesSource))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(ChartStyle))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(ISeries.Index))
            {
                Source = series
            });
            ptsBinding.Bindings.Add(new Binding(nameof(AutoAdjustment))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(MaxX))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(MaxY))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(ChartBoundary))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(AxesFontFamily))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(AxesFontSize))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(AxesFontStyle))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(AxesFontWeight))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(AxesFontStretch))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(CustomValuesX))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(SectionsY))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(SectionsX))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(ShowValuesOnBarsAndColumns))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(FlowDirection))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(MarkerShape))
            {
                Source = this
            });
            ptsBinding.Bindings.Add(new Binding(nameof(Series))
            {
                Source = this
            });
            ptsBinding.ConverterParameter = parameter;
            ptsBinding.NotifyOnSourceUpdated = true;
            path.SetBinding(Path.DataProperty, ptsBinding);
            #endregion

            #region Path stroke
            path.SetBinding(Shape.StrokeThicknessProperty, new Binding(nameof(LineThickness)) { Source = this });

            var strkBinding = new MultiBinding { Converter = new PathStrokeConverter() };
            strkBinding.Bindings.Add(new Binding(nameof(ChartStyle))
            {
                Source = this
            });
            if (pathIndex.In(0, 1))
            {
                strkBinding.Bindings.Add(new Binding(brushPath)
                {
                    Source = series
                }); ;
            }
            else
            {
                strkBinding.Bindings.Add(new Binding(brushPath)
                {
                    Source = this
                }); ;
            }
            strkBinding.NotifyOnSourceUpdated = true;
            path.SetBinding(Shape.StrokeProperty, strkBinding);
            #endregion

            #region Path fill
            var fillBinding = new MultiBinding { Converter = new PathFillConverter() };
            fillBinding.Bindings.Add(new Binding(nameof(ChartStyle))
            {
                Source = this
            });
            if (pathIndex.In(0, 1))
            {
                fillBinding.Bindings.Add(new Binding(brushPath)
                {
                    Source = series
                });
            }
            else
            {
                fillBinding.Bindings.Add(new Binding(brushPath)
                {
                    Source = this
                });
            }
            fillBinding.NotifyOnSourceUpdated = true;
            path.SetBinding(Shape.FillProperty, fillBinding);
            #endregion

            path.SetBinding(OpacityProperty, new Binding(nameof(ChartOpacity)) { Source = this });
            path.MouseLeftButtonDown += Path_MouseLeftButtonDown;
            path.MouseMove += Path_MouseMove;

            path.SetValue(Statics.AddedToCanvasProperty, true);
            _canvas.Children.Add(path);
        }

        private void updateBindings()
        {
            var binding = BindingOperations.GetMultiBindingExpression(_borderVPlaceholder, Grid.ColumnProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_borderVPlaceholder, WidthProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathYAxisValues, Grid.ColumnProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathYAxisValues, HorizontalAlignmentProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathYAxisValues, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_borderHPlaceholder, Grid.RowProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_borderHPlaceholder, HeightProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathXAxisValues, Grid.RowProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathXAxisValues, VerticalAlignmentProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathXAxisValues, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathRadarValues, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathRadarLines, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathXAxesLines, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathYAxesLines, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathTicks, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathHorzLines, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            binding = BindingOperations.GetMultiBindingExpression(_pathVertLines, Path.DataProperty);
            if (binding != null)
                binding.UpdateTarget();
            var actualSeries = getActualSeries();
            foreach (var series in actualSeries)
            {
                foreach (var p in series.Paths)
                {
                    if (p == null) continue;
                    binding = BindingOperations.GetMultiBindingExpression(p, Path.DataProperty);
                    if (binding != null)
                        binding.UpdateTarget();
                }
            }
            foreach (var legend in LegendsCollection)
            {
                binding = BindingOperations.GetMultiBindingExpression(legend, VisibilityProperty);
                if (binding != null)
                    binding.UpdateTarget();
            }
        }

        private void rebuildPieLegends(ObservableCollection<IChartValue> values, ISeries series)
        {
            if (series.Index > 0) return;
            PieLegendsCollection.Clear();
            for (int i = 0, brushIndex = 0; i < values.Count; i++)
            {
                var v = values[i];
                if (brushIndex == PredefinedMainBrushes.Length) brushIndex = 0;
                var legend = new Legend
                {
                    LegendBackground = PredefinedMainBrushes[brushIndex++].Brush
                };

                var textBinding = new MultiBinding { Converter = new PieSectionTextConverter(), ConverterParameter = v };
                textBinding.Bindings.Add(new Binding(nameof(ISeries.Values)) { Source = series });
                textBinding.Bindings.Add(new Binding(nameof(PiePercentsFormat)) { Source = this });
                legend.SetBinding(Legend.TextProperty, textBinding);

                PieLegendsCollection.Add(legend);
            }
        }
        #endregion

        #region Dependency properties wrappers
        /// <summary>
        /// Gets or sets the collection of custom legend text when <see cref="ChartStyle"/> is set to <see cref="ChartStyle.OpenHighLowClose"/>.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the collection of custom legend text when ChartStyle is set to ChartStyle.OpenHighLowClose")]
        public IEnumerable<string> LegendsOpenHighLowClose
        {
            get { return (IEnumerable<string>)GetValue(LegendsOpenHighLowCloseProperty); }
            set { SetValue(LegendsOpenHighLowCloseProperty, value); }
        }
        /// <summary>
        /// Gets or sets the collection of custom legend text when <see cref="ChartStyle"/> is set to <see cref="ChartStyle.HighLowClose"/>.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the collection of custom legend text when ChartStyle is set to ChartStyle.HighLowClose")]
        public IEnumerable<string> LegendsHighLowClose
        {
            get { return (IEnumerable<string>)GetValue(LegendsHighLowCloseProperty); }
            set { SetValue(LegendsHighLowCloseProperty, value); }
        }
        /// <summary>
        /// Gets or sets the collection of custom legend text when <see cref="ChartStyle"/> is set to <see cref="ChartStyle.Waterfall"/>.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the collection of custom legend text when ChartStyle is set to ChartStyle.Waterfall")]
        public IEnumerable<string> LegendsWaterfall
        {
            get { return (IEnumerable<string>)GetValue(LegendsWaterfallProperty); }
            set { SetValue(LegendsWaterfallProperty, value); }
        }
        /// <summary>
        /// Gets or sets the collection of <see cref="WPF.Chart.Series"/> objects associated with chart control.
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the collection of Series objects associated with chart control")]
        public IEnumerable<ISeries> SeriesSource
        {
            get
            {
                var b = BindingOperations.GetBinding(this, SeriesSourceProperty);
                if (b != null)
                    return (IEnumerable<ISeries>)GetValue(SeriesSourceProperty);
                else
                    return Series;
            }
            set { SetValue(SeriesSourceProperty, value); }
        }
        /// <summary>
        /// Gets the collection of <see cref="ISeries"/> objects used to generate the content of the <see cref="Chart"/> control.
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]
        public ChartItemsCollection<ISeries> Series
        {
            get
            {
                var b = BindingOperations.GetBinding(this, SeriesSourceProperty);
                if (b == null)
                {
                    if (_series == null)
                    {
                        _series = new ChartItemsCollection<ISeries>();
                        _series.CollectionChanged += SeriesSource_CollectionChanged;
                    }
                }
                else
                {
                    if (SeriesSource != null)
                        _series = new ChartItemsCollection<ISeries>(SeriesSource);
                    else
                    {
                        if (_series == null)
                        {
                            _series = new ChartItemsCollection<ISeries>();
                            _series.CollectionChanged += SeriesSource_CollectionChanged;
                        }
                    }
                }
                return _series;
            }
        }
        /// <summary>
        /// Specifies whether chart boundary is started on y-axes or with offfset from y-axes./>.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> is set to any non-line style.</remarks>
        [Category("ChartAppearance"), Description("Specifies whether chart boundary is started on y-axes or with offfset from y-axes")]
        public ChartBoundary ChartBoundary
        {
            get { return (ChartBoundary)GetValue(ChartBoundaryProperty); }
            set { SetValue(ChartBoundaryProperty, value); }
        }
        /// <summary>
        /// Specifies whether ticks are drawn on axes. Can be one of <see cref="AxesVisibility"/> enumeration members.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAppearance"), Description("Specifies whether ticks are drawn on axes. Can be one of AxesValuesVisibility enumeration members")]
        public AxesVisibility ShowTicks
        {
            get { return (AxesVisibility)GetValue(ShowTicksProperty); }
            set { SetValue(ShowTicksProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart legend's font family.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the chart legend's font family")]
        public FontFamily LegendFontFamily
        {
            get { return (FontFamily)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart legend's font wweight.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the chart legend's font wweight")]
        public FontWeight LegendFontWeight
        {
            get { return (FontWeight)GetValue(LegendFontWeightProperty); }
            set { SetValue(LegendFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart legend's font style.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the chart legend's font style")]
        public FontStyle LegendFontStyle
        {
            get { return (FontStyle)GetValue(LegendFontStyleProperty); }
            set { SetValue(LegendFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart legend's font size.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the chart legend's font size")]
        public double LegendFontSize
        {
            get { return (double)GetValue(LegendFontSizeProperty); }
            set { SetValue(LegendFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the shape of the chart legend. Can be one of <see cref="LegendShape"/> enumeration members.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the shape of the chart legend. Can be one of LegendShape enumeration members")]
        public ShapeStyle LegendShape
        {
            get { return (ShapeStyle)GetValue(LegendShapeProperty); }
            set { SetValue(LegendShapeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the size of the chart legend. Can be one of <see cref="LegendSize"/> enumeration members.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the size of the chart legend. Can be one of LegendSize enumeration members")]
        public LegendSize LegendSize
        {
            get { return (LegendSize)GetValue(LegendSizeProperty); }
            set { SetValue(LegendSizeProperty, value); }
        }
        /// <summary>
        /// Specifies whether control will automatically adjust its max x- and y- values or they should be set explicitly.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAppearance"), Description("Specifies whether control will automatically adjust its max x- and y- values or they should be set explicitly")]
        public AutoAdjustmentMode AutoAdjustment
        {
            get { return (AutoAdjustmentMode)GetValue(AutoAdjustmentProperty); }
            set { SetValue(AutoAdjustmentProperty, value); }
        }
        /// <summary>
        /// Gets or sets the shape of chart series markers.
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the shape of chart series markers")]
        public ShapeStyle MarkerShape
        {
            get { return (ShapeStyle)GetValue(MarkerShapeProperty); }
            set { SetValue(MarkerShapeProperty, value); }
        }
        /// <summary>
        /// Gets or sets max numeric value for vertical axis. The default value is 100.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="AutoAdjustment"/> property is set to <see cref="AutoAdjustmentMode.Both"/>,or <see cref="AutoAdjustmentMode.Vertical"/>,or <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartMeasures"), Description("Gets or sets max numeric value for vertical axis. The default value is 100")]
        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }
        /// <summary>
        /// Gets or sets max numeric value for horizontal axis. The default value is 100.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="AutoAdjustment"/> property is set to <see cref="AutoAdjustmentMode.Both"/>, or <see cref="AutoAdjustmentMode.Horizontal"/>,or <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartMeasures"), Description("Gets or sets max numeric value for horizontal axis. The default value is 100")]
        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }
        /// <summary>
        /// Gets or sets the thickness of the chart lines. The default value is 2.0 px.
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the thickness of the chart lines. The default value is 2.0 px")]
        public double LineThickness
        {
            get { return (double)GetValue(LineThicknessProperty); }
            set { SetValue(LineThicknessProperty, value); }
        }
        /// <summary>
        /// Gets or set the chart opacity in range of 0.0 (fully transparent) to 1.0 (fully opaque).
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or set the chart opacity in range of 0.0 (fully transparent) to 1.0 (fully opaque)")]
        public double ChartOpacity
        {
            get { return (double)GetValue(ChartOpacityProperty); }
            set { SetValue(ChartOpacityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the custom sequence of strings to be drawn next to x-axis instead of numeric values.
        /// </summary>
        [Category("ChartAxes"), Description("Gets or sets the custom sequence of strings to be drawn next to x-axis instead of numeric values")]
        public IEnumerable<string> CustomValuesX
        {
            get { return (IEnumerable<string>)GetValue(CustomValuesXProperty); }
            set { SetValue(CustomValuesXProperty, value); }
        }
        /// <summary>
        /// Gets or sets the custom sequence of strings to be drawn next to y-axis instead of numeric values.
        /// </summary>
        [Category("ChartAxes"), Description("Gets or sets the custom sequence of strings to be drawn next to y-axis instead of numeric values")]
        public IEnumerable<string> CustomValuesY
        {
            get { return (IEnumerable<string>)GetValue(CustomValuesYProperty); }
            set { SetValue(CustomValuesYProperty, value); }
        }
        /// <summary>
        /// Gets or sets the visibility state of x- and y- axes numeric/custom values. Can be one of <see cref="AxesVisibility"/> enumeration members.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the visibility state of x- and y- axes numeric/custom values. Can be one of AxesValuesVisibility enumeration members")]
        public AxesVisibility AxesValuesVisibility
        {
            get { return (AxesVisibility)GetValue(AxesValuesVisibilityProperty); }
            set { SetValue(AxesValuesVisibilityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the visibility state of x- and y- axes lines. Can be one of <see cref="AxesVisibility"/> enumeration members.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the visibility state of x- and y- axes lines. Can be one of AxesValuesVisibility enumeration members")]
        public AxesVisibility AxesLinesVisibility
        {
            get { return (AxesVisibility)GetValue(AxesLinesVisibilityProperty); }
            set { SetValue(AxesLinesVisibilityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the visibility state of secondary horizontal and vertical lines. Can be one of <see cref="AxesVisibility"/> enumeration members.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAppearance"), Description("Gets or sets the visibility state of secondary horizontal and vertical lines. Can be one of AxesValuesVisibility enumeration members")]
        public AxesVisibility SecondaryLinesVisibility
        {
            get { return (AxesVisibility)GetValue(SecondaryLinesVisibilityProperty); }
            set { SetValue(SecondaryLinesVisibilityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart style. Can be one of <see cref="ChartStyle"/> enumeration members.
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the chart style. Can be one of ChartStyle enumeration members")]
        public ChartStyle ChartStyle
        {
            get { return (ChartStyle)GetValue(ChartStyleProperty); }
            set { SetValue(ChartStyleProperty, value); }
        }
        /// <summary>
        /// Specifies whether values should be drawn on bars and columns.
        /// </summary>
        [Category("ChartAppearance"), Description("Specifies whether values should be drawn on bars and columns")]
        public bool ShowValuesOnBarsAndColumns
        {
            get { return (bool)GetValue(ShowValuesOnBarsAndColumnsProperty); }
            set { SetValue(ShowValuesOnBarsAndColumnsProperty, value); }
        }
        /// <summary>
        /// Specifies whether chart legends should be shown.
        /// </summary>
        [Category("ChartLegend"), Description("Specifies whether chart legends should be shown")]
        public bool ShowLegend
        {
            get { return (bool)GetValue(ShowLegendProperty); }
            set { SetValue(ShowLegendProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart legend alignment. Can be one of <see cref="LegendAlignment"/> enumeration members.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the chart legend alignment. Can be one of LegendAlignment enumeration members")]
        public LegendAlignment LegendAlignment
        {
            get { return (LegendAlignment)GetValue(LegendAlignmentProperty); }
            set { SetValue(LegendAlignmentProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font size of the chart's title.
        /// </summary>
        [Category("ChartTitle"), Description("Gets of sets the font size of the chart's title")]
        public double TitleFontSize
        {
            get { return (double)GetValue(TitleFontSizeProperty); }
            set { SetValue(TitleFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font style of the chart's title.
        /// </summary>
        [Category("ChartTitle"), Description("Gets of sets the font style of the chart's title")]
        public FontStyle TitleFontStyle
        {
            get { return (FontStyle)GetValue(TitleFontStyleProperty); }
            set { SetValue(TitleFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font weight of the chart's title.
        /// </summary>
        [Category("ChartTitle"), Description("Gets of sets the font weight of the chart's title")]
        public FontWeight TitleFontWeight
        {
            get { return (FontWeight)GetValue(TitleFontWeightProperty); }
            set { SetValue(TitleFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font family of the chart's title.
        /// </summary>
        [Category("ChartTitle"), Description("Gets of sets the font family of the chart's title")]
        public FontFamily TitleFontFamily
        {
            get { return (FontFamily)GetValue(TitleFontFamilyProperty); }
            set { SetValue(TitleFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font stretch of the chart's title.
        /// </summary>
        [Category("ChartTitle"), Description("Gets of sets the font stretch of the chart's title")]
        public FontStretch TitleFontStretch
        {
            get { return (FontStretch)GetValue(TitleFontStretchProperty); }
            set { SetValue(TitleFontStretchProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font family of the chart's axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font family of the chart's axes")]
        public FontFamily AxesFontFamily
        {
            get { return (FontFamily)GetValue(AxesFontFamilyProperty); }
            set { SetValue(AxesFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font weight of the chart's axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font weight of the chart's axes")]
        public FontWeight AxesFontWeight
        {
            get { return (FontWeight)GetValue(AxesFontWeightProperty); }
            set { SetValue(AxesFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font size of the chart's axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font size of the chart's axes")]
        public double AxesFontSize
        {
            get { return (double)GetValue(AxesFontSizeProperty); }
            set { SetValue(AxesFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font style of the chart's axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font style of the chart's axes")]
        public FontStyle AxesFontStyle
        {
            get { return (FontStyle)GetValue(AxesFontStyleProperty); }
            set { SetValue(AxesFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font stretch of the chart's axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font stretch of the chart's axes")]
        public FontStretch AxesFontStretch
        {
            get { return (FontStretch)GetValue(AxesFontStretchProperty); }
            set { SetValue(AxesFontStretchProperty, value); }
        }
        /// <summary>
        /// Gets or sets the text which appears on the top/bottom of the vertical axis.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the text which appears on the top/bottom of the vertical axis")]
        public string AxisTitleY
        {
            get { return (string)GetValue(AxisTitleYProperty); }
            set { SetValue(AxisTitleYProperty, value); }
        }
        /// <summary>
        /// Gets or sets the text which appears on right/left of the horizontal axis.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the text which appears on right/left of the horizontal axis")]
        public string AxisTitleX
        {
            get { return (string)GetValue(AxisTitleXProperty); }
            set { SetValue(AxisTitleXProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart Title.
        /// </summary>
        [Category("ChartTitle"), Description("Gets or sets the chart's title")]
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        /// <summary>
        /// Gets or sets the amount of sections on x-axis.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartMeasures"), Description("Gets or sets the amount of vertical lines")]
        public int SectionsX
        {
            get { return (int)GetValue(SectionsXProperty); }
            set { SetValue(SectionsXProperty, value); }
        }
        /// <summary>
        /// Gets or sets the amount of sections on y-axis.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartMeasures"), Description("Gets or sets the amount of horizontal lines")]
        public int SectionsY
        {
            get { return (int)GetValue(SectionsYProperty); }
            set { SetValue(SectionsYProperty, value); }
        }
        /// <summary>
        /// Gets or sets the format for numeric values drawn next to the vertical axis.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the format for numeric values drawn next to the vertical axis")]
        public string ValuesFormatY
        {
            get { return (string)GetValue(ValuesFormatYProperty); }
            set { SetValue(ValuesFormatYProperty, value); }
        }
        /// <summary>
        /// Gets or sets the format for numeric values drawn next to the horizontal axis.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the format for numeric values drawn next to the horizontal axis")]
        public string HorizontalAxisValuesFormat
        {
            get { return (string)GetValue(ValuesFormatX); }
            set { SetValue(ValuesFormatX, value); }
        }
        /// <summary>
        /// Gets or sets the numeric format for pie percents.
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the numeric format for pie percents")]
        public string PiePercentsFormat
        {
            get { return (string)GetValue(PiePercentsFormatProperty); }
            set { SetValue(PiePercentsFormatProperty, value); }
        }
        /// <summary>
        /// Gets legends collection.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public ObservableCollection<FrameworkElement> LegendsCollection => _legendsCollection;
        /// <summary>
        /// Gets pie legends collection
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public ObservableCollection<FrameworkElement> PieLegendsCollection => _pieLegendsCollection;
        #endregion

        #region Public methods
        /// <summary>
        /// Saves the chart as image
        /// </summary>
        /// <param name="imageFileName">The full path to image file</param>
        public void SaveAsImage(string imageFileName)
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            var bounds = VisualTreeHelper.GetDescendantBounds(this);

            var ext = System.IO.Path.GetExtension(imageFileName);
            if (ext == null) return;

            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                dc.DrawRectangle(new VisualBrush(this), null, new Rect(new Point(), bounds.Size));
            }

            var rnb = new RenderTargetBitmap(
                (int)(bounds.Width * dpi.DpiScaleX), (int)(bounds.Height * dpi.DpiScaleY), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
            rnb.Render(visual);

            BitmapEncoder enc;
            switch (ext.ToUpper())
            {
                case ".PNG":
                    enc = new PngBitmapEncoder();
                    break;
                case ".JPG":
                case ".JPEG":
                    enc = new JpegBitmapEncoder();
                    break;
                case ".GIF":
                    enc = new GifBitmapEncoder();
                    break;
                case ".BMP":
                    enc = new BmpBitmapEncoder();
                    break;
                case ".TIF":
                case ".TIFF":
                    enc = new TiffBitmapEncoder();
                    break;
                case ".XPS":
                    var transform = LayoutTransform;
                    LayoutTransform = null;
                    if (File.Exists(imageFileName))
                        File.Delete(imageFileName);
                    var package = Package.Open(imageFileName, FileMode.CreateNew, FileAccess.ReadWrite);
                    var xps = new XpsDocument(package);
                    var writer = XpsDocument.CreateXpsDocumentWriter(xps);
                    writer.Write(this);
                    xps.Close();
                    package.Close();
                    LayoutTransform = transform;
                    //this.Measure(bounds.Size);
                    Arrange(new Rect(new Point(), bounds.Size));
                    //var fixedDoc = new FixedDocument();
                    //var pageContent = new PageContent();
                    //var fixedPage = new FixedPage();
                    //fixedPage.Children.Add(this);
                    //((System.Windows.Markup.IAddChild)pageContent).AddChild(fixedPage);
                    //fixedDoc.Pages.Add(pageContent);
                    //using (var xpsDocument = new XpsDocument(imageFileName, FileAccess.ReadWrite))
                    //{
                    //    var xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDocument);
                    //    xpsWriter.Write(fixedDoc);
                    //}
                    return;
                default:
                    return;
            }
            enc.Frames.Add(BitmapFrame.Create(rnb));
            if (File.Exists(imageFileName))
                File.Delete(imageFileName);
            using var stream = new FileStream(imageFileName, FileMode.CreateNew, FileAccess.ReadWrite);
            enc.Save(stream);
        }

        #endregion

        #region Callbacks
        private static object CoerceLegendsWaterfall(DependencyObject d, object value)
        {
            if (!(value is IEnumerable<string> legends))
                throw new ArgumentException("LegendsWaterfall must be of type IEnumerable<string>", nameof(value));
            if (legends.Count() < 2)
                throw new ArgumentException("LegendsWaterfall must have at least two items", nameof(value));
            return value;
        }
        private static object CoerceLegendsHighLowClose(DependencyObject d, object value)
        {
            if (!(value is IEnumerable<string> legends))
                throw new ArgumentException("LegendsHighLowClose must be of type IEnumerable<string>", nameof(value));
            if (legends.Count() < 2)
                throw new ArgumentException("LegendsHighLowClose must have at least two items", nameof(value));
            return value;
        }
        private static object CoerceLegendsOpenHighLowClose(DependencyObject d, object value)
        {
            if (!(value is IEnumerable<string> legends))
                throw new ArgumentException("LegendsOpenHighLowClose must be of type IEnumerable<string>", nameof(value));
            if (legends.Count() < 2)
                throw new ArgumentException("LegendsOpenHighLowClose must have at least two items", nameof(value));
            return value;
        }
        private static void OnSeriesSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart chart)) return;
            if (e.OldValue is INotifyCollectionChanged oldValueNotifyCollectionChanged)
            {
                oldValueNotifyCollectionChanged.CollectionChanged -= chart.SeriesSource_CollectionChanged;
            }
            if (e.NewValue is INotifyCollectionChanged newValueNotifyCollectionChanged)
            {
                newValueNotifyCollectionChanged.CollectionChanged += chart.SeriesSource_CollectionChanged;
            }
            chart.OnSeriesSourceChanged((IEnumerable<ISeries>)e.OldValue, (IEnumerable<ISeries>)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="SeriesSourceChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnSeriesSourceChanged(IEnumerable<ISeries> oldValue, IEnumerable<ISeries> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<IEnumerable<ISeries>>(oldValue, newValue)
            {
                RoutedEvent = SeriesSourceChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnChartBoundaryChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnChartBoundaryChanged((ChartBoundary)e.OldValue, (ChartBoundary)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ChartBoundaryChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnChartBoundaryChanged(ChartBoundary oldValue, ChartBoundary newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<ChartBoundary>(oldValue, newValue)
            {
                RoutedEvent = ChartBoundaryChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnShowTicksChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnShowTicksChanged((AxesVisibility)e.OldValue, (AxesVisibility)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ShowTicksChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnShowTicksChanged(AxesVisibility oldValue, AxesVisibility newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<AxesVisibility>(oldValue, newValue)
            {
                RoutedEvent = ShowTicksChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnMarkerShapeCahnged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnMarkerShapeCahnged((ShapeStyle)e.OldValue, (ShapeStyle)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="MarkerShapeChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnMarkerShapeCahnged(ShapeStyle oldValue, ShapeStyle newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<ShapeStyle>(oldValue, newValue)
            {
                RoutedEvent = MarkerShapeChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnLegendFontSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnLegendFontSizeChanged((double)e.OldValue, (double)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="LegendFontSizeChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLegendFontSizeChanged(double oldValue, double newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = LegendFontSizeChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnLegendFontStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnLegendFontStyleChanged((FontStyle)e.OldValue, (FontStyle)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="LegendFontStyleChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLegendFontStyleChanged(FontStyle oldValue, FontStyle newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontStyle>(oldValue, newValue)
            {
                RoutedEvent = LegendFontStyleChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnLegendFontWeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnLegendFontWeightChanged((FontWeight)e.OldValue, (FontWeight)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="LegendFontWeightChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLegendFontWeightChanged(FontWeight oldValue, FontWeight newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontWeight>(oldValue, newValue)
            {
                RoutedEvent = LegendFontWeightChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnLegendFontFamilyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnLegendFontFamilyChanged((FontFamily)e.OldValue, (FontFamily)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="LegendFontFamilyChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLegendFontFamilyChanged(FontFamily oldValue, FontFamily newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontFamily>(oldValue, newValue)
            {
                RoutedEvent = LegendFontFamilyChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnLegendShapeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnLegendShapeChanged((ShapeStyle)e.OldValue, (ShapeStyle)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="LegendShapeChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLegendShapeChanged(ShapeStyle oldValue, ShapeStyle newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<ShapeStyle>(oldValue, newValue)
            {
                RoutedEvent = LegendShapeChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnLegendSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnLegendSizeChanged((LegendSize)e.OldValue, (LegendSize)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="LegendSizeChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLegendSizeChanged(LegendSize oldValue, LegendSize newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<LegendSize>(oldValue, newValue)
            {
                RoutedEvent = LegendSizeChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAutoAdjustmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAutoAdjustmentChanged((AutoAdjustmentMode)e.OldValue, (AutoAdjustmentMode)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AutoAdjustmentChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAutoAdjustmentChanged(AutoAdjustmentMode oldValue, AutoAdjustmentMode newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<AutoAdjustmentMode>(oldValue, newValue)
            {
                RoutedEvent = AutoAdjustmentChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnCustomValuesXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCustomValuesXChanged((IEnumerable<string>)e.OldValue, (IEnumerable<string>)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CustomValuesXChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCustomValuesXChanged(IEnumerable<string> oldValue, IEnumerable<string> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<IEnumerable<string>>(oldValue, newValue)
            {
                RoutedEvent = CustomValuesXChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnCustomValuesYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCustomValuesYChanged((IEnumerable<string>)e.OldValue, (IEnumerable<string>)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CustomValuesYChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCustomValuesYChanged(IEnumerable<string> oldValue, IEnumerable<string> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<IEnumerable<string>>(oldValue, newValue)
            {
                RoutedEvent = CustomValuesYChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnValuesFormatYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnValuesFormatYChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ValuesFormatYChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnValuesFormatYChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = ValuesFormatYChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnHorizontalAxisValuesFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnHorizontalAxisValuesFormatChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="HorizontalAxisValuesFormatChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnHorizontalAxisValuesFormatChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = HorizontalAxisValuesFormatChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnPiePercentsFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnPiePercentsFormatChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="PiePercentsFormatChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnPiePercentsFormatChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = PiePercentsFormatChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceFormats(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : string.IsNullOrEmpty((string)value) ? "0" : value;
        }

        private static void OnAxesValuesVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxesValuesVisibilityChanged((AxesVisibility)e.OldValue, (AxesVisibility)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesValuesVisibilityChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesValuesVisibilityChanged(AxesVisibility oldValue, AxesVisibility newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<AxesVisibility>(oldValue, newValue)
            {
                RoutedEvent = AxesValuesVisibilityChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxesLinesVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxesLinesVisibilityChanged((AxesVisibility)e.OldValue, (AxesVisibility)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesLinesVisibilityChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesLinesVisibilityChanged(AxesVisibility oldValue, AxesVisibility newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<AxesVisibility>(oldValue, newValue)
            {
                RoutedEvent = AxesLinesVisibilityChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnSecondaryLinesVisibilityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnSecondaryLinesVisibilityChanged((AxesVisibility)e.OldValue, (AxesVisibility)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="SecondaryLinesVisibilityChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnSecondaryLinesVisibilityChanged(AxesVisibility oldValue, AxesVisibility newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<AxesVisibility>(oldValue, newValue)
            {
                RoutedEvent = SecondaryLinesVisibilityChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnChartStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnChartStyleChanged((ChartStyle)e.OldValue, (ChartStyle)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ChartStyleChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnChartStyleChanged(ChartStyle oldValue, ChartStyle newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<ChartStyle>(oldValue, newValue)
            {
                RoutedEvent = ChartStyleChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnShowLegendChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnShowLegendChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ShowLegendChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnShowLegendChanged(bool oldValue, bool newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = ShowLegendChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnShowValuesOnBarsAndColumnsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnShowValuesOnBarsAndColumnsChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ShowValuesOnBarsAndColumnsChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnShowValuesOnBarsAndColumnsChanged(bool oldValue, bool newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = ShowValuesOnBarsAndColumnsChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnLegendAlignmentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnLegendAlignmentChanged((LegendAlignment)e.OldValue, (LegendAlignment)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="LegendAlignmentChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLegendAlignmentChanged(LegendAlignment oldValue, LegendAlignment newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<LegendAlignment>(oldValue, newValue)
            {
                RoutedEvent = LegendAlignmentChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnTitleFontSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnTitleFontSizeChanged((double)e.OldValue, (double)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="TitleFontSizeChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnTitleFontSizeChanged(double oldValue, double newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = TitleFontSizeChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnTitleFontStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnTitleFontStyleChanged((FontStyle)e.OldValue, (FontStyle)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="TitleFontStyleChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnTitleFontStyleChanged(FontStyle oldValue, FontStyle newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontStyle>(oldValue, newValue)
            {
                RoutedEvent = TitleFontStyleChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnTitleFontWeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnTitleFontWeightChanged((FontWeight)e.OldValue, (FontWeight)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="TitleFontWeightChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnTitleFontWeightChanged(FontWeight oldValue, FontWeight newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontWeight>(oldValue, newValue)
            {
                RoutedEvent = TitleFontWeightChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxesFontWeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxesFontWeightChanged((FontWeight)e.OldValue, (FontWeight)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesFontWeightChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesFontWeightChanged(FontWeight oldValue, FontWeight newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontWeight>(oldValue, newValue)
            {
                RoutedEvent = AxesFontWeightChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnTitleFontFamilyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnTitleFontFamilyChanged((FontFamily)e.OldValue, (FontFamily)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="TitleFontFamilyChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnTitleFontFamilyChanged(FontFamily oldValue, FontFamily newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontFamily>(oldValue, newValue)
            {
                RoutedEvent = TitleFontFamilyChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxesFontFamilyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxesFontFamilyChanged((FontFamily)e.OldValue, (FontFamily)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesFontFamilyChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesFontFamilyChanged(FontFamily oldValue, FontFamily newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontFamily>(oldValue, newValue)
            {
                RoutedEvent = AxesFontFamilyChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxesFontSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxesFontSizeChanged((double)e.OldValue, (double)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesFontSizeChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesFontSizeChanged(double oldValue, double newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = AxesFontSizeChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxesFontStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxesFontStyleChanged((FontStyle)e.OldValue, (FontStyle)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesFontStyleChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesFontStyleChanged(FontStyle oldValue, FontStyle newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontStyle>(oldValue, newValue)
            {
                RoutedEvent = AxesFontStyleChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxesFontStretchChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxesFontStretchChanged((FontStretch)e.OldValue, (FontStretch)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesFontStretchChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesFontStretchChanged(FontStretch oldValue, FontStretch newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontStretch>(oldValue, newValue)
            {
                RoutedEvent = AxesFontStretchChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnTitleFontStretchChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnTitleFontStretchChanged((FontStretch)e.OldValue, (FontStretch)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="TitleFontStretchChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnTitleFontStretchChanged(FontStretch oldValue, FontStretch newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontStretch>(oldValue, newValue)
            {
                RoutedEvent = TitleFontStretchChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxisTitleYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxisTitleYChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxisTitleYChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxisTitleYChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = AxisTitleYChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxisTitleXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxisTitleXChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxisTitleXChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxisTitleXChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = AxisTitleXChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnTitleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnTitleChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="TitleChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnTitleChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = TitleChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnLineThicknessChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnLineThicknessChanged((double)e.OldValue, (double)e.NewValue);
        }

        /// <summary>
        /// Invoked just before the <see cref="LineThicknessChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLineThicknessChanged(double oldValue, double newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = LineThicknessChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceLineThickness(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : (double)value <= 0 ? 2.0 : value;
        }

        private static void OnMaxYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnMaxYChanged((double)e.OldValue, (double)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="MaxYChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnMaxYChanged(double oldValue, double newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = MaxYChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceMaxY(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : (double)value < 0 ? 100.0 : value;
        }

        private static void OnMaxXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnMaxXChanged((double)e.OldValue, (double)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="MaxXChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnMaxXChanged(double oldValue, double newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = MaxXChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceMaxX(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : (double)value < 0 ? 100.0 : value;
        }

        private static void OnChartOpacityChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnChartOpacityChanged((double)e.OldValue, (double)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ChartOpacityChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected virtual void OnChartOpacityChanged(double oldValue, double newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = ChartOpacityChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceChartOpacity(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : (double)value < 0 || (double)value > 1 ? 1 : value;
        }

        private static void OnSectionsYChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnSectionsYChanged((int)e.OldValue, (int)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="SectionsYChanged"/> event is raised on Chart
        /// </summary>
        /// <param name="oldValue">Old y-axis stops count</param>
        /// <param name="newValue">New y-axis stops count</param>
        protected virtual void OnSectionsYChanged(int oldValue, int newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue)
            {
                RoutedEvent = SectionsYChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceSectionsY(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : (int)value <= 0 ? 10 : value;
        }

        private static void OnSectionsXChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnSectionsXChanged((int)e.OldValue, (int)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="SectionsXChanged"/> event is raised on Chart
        /// </summary>
        /// <param name="oldValue">Old x-axis stops count</param>
        /// <param name="newValue">New x-axis stops count</param>
        protected virtual void OnSectionsXChanged(int oldValue, int newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue)
            {
                RoutedEvent = SectionsXChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceSectionsX(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : (int)value <= 0 ? 10 : value;
        }
        #endregion

        #region Routed events
        /// <summary>
        /// Occurs when the <see cref="SeriesSource"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<IEnumerable<ISeries>> SeriesSourceChanged
        {
            add { AddHandler(SeriesSourceChangedEvent, value); }
            remove { RemoveHandler(SeriesSourceChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="SeriesSourceChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent SeriesSourceChangedEvent = EventManager.RegisterRoutedEvent("SeriesSourceChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<IEnumerable<ISeries>>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="ChartBoundary"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ChartBoundary> ChartBoundaryChanged
        {
            add { AddHandler(ChartBoundaryChangedEvent, value); }
            remove { RemoveHandler(ChartBoundaryChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ChartBoundaryChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ChartBoundaryChangedEvent = EventManager.RegisterRoutedEvent("ChartBoundaryChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<ChartBoundary>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="ShowTicks"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<AxesVisibility> ShowTicksChanged
        {
            add { AddHandler(ShowTicksChangedEvent, value); }
            remove { RemoveHandler(ShowTicksChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ShowTicksChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ShowTicksChangedEvent = EventManager.RegisterRoutedEvent("ShowTicksChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<AxesVisibility>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="MarkerShape"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ShapeStyle> MarkerShapeChanged
        {
            add { AddHandler(MarkerShapeChangedEvent, value); }
            remove { RemoveHandler(MarkerShapeChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="MarkerShapeChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent MarkerShapeChangedEvent = EventManager.RegisterRoutedEvent("MarkerShapeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<ShapeStyle>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxesFontSize"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> AxesFontSizeChanged
        {
            add { AddHandler(AxesFontSizeChangedEvent, value); }
            remove { RemoveHandler(AxesFontSizeChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesFontSizeChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesFontSizeChangedEvent = EventManager.RegisterRoutedEvent("AxesFontSizeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="LegendFontSize"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> LegendFontSizeChanged
        {
            add { AddHandler(LegendFontSizeChangedEvent, value); }
            remove { RemoveHandler(LegendFontSizeChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LegendFontSizeChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LegendFontSizeChangedEvent = EventManager.RegisterRoutedEvent("LegendFontSizeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Chart));


        /// <summary>
        /// Occurs when the <see cref="LegendFontStyle"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontStyle> LegendFontStyleChanged
        {
            add { AddHandler(LegendFontStyleChangedEvent, value); }
            remove { RemoveHandler(LegendFontStyleChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LegendFontStyleChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LegendFontStyleChangedEvent = EventManager.RegisterRoutedEvent("LegendFontStyleChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontStyle>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxesFontStyle"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontStyle> AxesFontStyleChanged
        {
            add { AddHandler(AxesFontStyleChangedEvent, value); }
            remove { RemoveHandler(AxesFontStyleChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesFontStyleChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesFontStyleChangedEvent = EventManager.RegisterRoutedEvent("AxesFontStyleChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontStyle>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxesFontStretch"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontStretch> AxesFontStretchChanged
        {
            add { AddHandler(AxesFontStretchChangedEvent, value); }
            remove { RemoveHandler(AxesFontStretchChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesFontStretchChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesFontStretchChangedEvent = EventManager.RegisterRoutedEvent("AxesFontStretchChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontStretch>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="TitleFontStretch"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontStretch> TitleFontStretchChanged
        {
            add { AddHandler(TitleFontStretchChangedEvent, value); }
            remove { RemoveHandler(TitleFontStretchChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="TitleFontStretchChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent TitleFontStretchChangedEvent = EventManager.RegisterRoutedEvent("TitleFontStretchChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontStretch>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="LegendFontWeight"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontWeight> LegendFontWeightChanged
        {
            add { AddHandler(LegendFontWeightChangedEvent, value); }
            remove { RemoveHandler(LegendFontWeightChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LegendFontWeightChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LegendFontWeightChangedEvent = EventManager.RegisterRoutedEvent("LegendFontWeightChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontWeight>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="LegendFontFamily"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontFamily> LegendFontFamilyChanged
        {
            add { AddHandler(LegendFontFamilyChangedEvent, value); }
            remove { RemoveHandler(LegendFontFamilyChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LegendFontFamilyChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LegendFontFamilyChangedEvent = EventManager.RegisterRoutedEvent("LegendFontFamilyChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontFamily>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="LegendShape"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ShapeStyle> LegendShapeChanged
        {
            add { AddHandler(LegendShapeChangedEvent, value); }
            remove { RemoveHandler(LegendShapeChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LegendShapeChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LegendShapeChangedEvent = EventManager.RegisterRoutedEvent("LegendShapeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<ShapeStyle>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="LegendSize"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<LegendSize> LegendSizeChanged
        {
            add { AddHandler(LegendSizeChangedEvent, value); }
            remove { RemoveHandler(LegendSizeChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LegendSizeChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LegendSizeChangedEvent = EventManager.RegisterRoutedEvent("LegendSizeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<LegendSize>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="CustomValuesY"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<IEnumerable<string>> CustomValuesYChanged
        {
            add { AddHandler(CustomValuesYChangedEvent, value); }
            remove { RemoveHandler(CustomValuesYChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CustomValuesYChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CustomValuesYChangedEvent = EventManager.RegisterRoutedEvent("CustomValuesYChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<IEnumerable<string>>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="CustomValuesX"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<IEnumerable<string>> CustomValuesXChanged
        {
            add { AddHandler(CustomValuesXChangedEvent, value); }
            remove { RemoveHandler(CustomValuesXChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CustomValuesXChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CustomValuesXChangedEvent = EventManager.RegisterRoutedEvent("CustomValuesXChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<IEnumerable<string>>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="ValuesFormatY"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> ValuesFormatYChanged
        {
            add { AddHandler(ValuesFormatYChangedEvent, value); }
            remove { RemoveHandler(ValuesFormatYChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ValuesFormatYChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ValuesFormatYChangedEvent = EventManager.RegisterRoutedEvent("ValuesFormatYChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="HorizontalAxisValuesFormat"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> HorizontalAxisValuesFormatChanged
        {
            add { AddHandler(HorizontalAxisValuesFormatChangedEvent, value); }
            remove { RemoveHandler(HorizontalAxisValuesFormatChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="HorizontalAxisValuesFormatChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent HorizontalAxisValuesFormatChangedEvent = EventManager.RegisterRoutedEvent("HorizontalAxisValuesFormatChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="PiePercentsFormat"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> PiePercentsFormatChanged
        {
            add { AddHandler(PiePercentsFormatChangedEvent, value); }
            remove { RemoveHandler(PiePercentsFormatChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="PiePercentsFormatChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent PiePercentsFormatChangedEvent = EventManager.RegisterRoutedEvent("PiePercentsFormatChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxesValuesVisibility"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<AxesVisibility> AxesValuesVisibilityChanged
        {
            add { AddHandler(AxesValuesVisibilityChangedEvent, value); }
            remove { RemoveHandler(AxesValuesVisibilityChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesValuesVisibilityChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesValuesVisibilityChangedEvent = EventManager.RegisterRoutedEvent("AxesValuesVisibilityChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<AxesVisibility>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxesLinesVisibility"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<AxesVisibility> AxesLinesVisibilityChanged
        {
            add { AddHandler(AxesLinesVisibilityChangedEvent, value); }
            remove { RemoveHandler(AxesLinesVisibilityChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesLinesVisibilityChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesLinesVisibilityChangedEvent = EventManager.RegisterRoutedEvent("AxesLinesVisibilityChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<AxesVisibility>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="SecondaryLinesVisibility"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<AxesVisibility> SecondaryLinesVisibilityChanged
        {
            add { AddHandler(SecondaryLinesVisibilityChangedEvent, value); }
            remove { RemoveHandler(SecondaryLinesVisibilityChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="SecondaryLinesVisibilityChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent SecondaryLinesVisibilityChangedEvent = EventManager.RegisterRoutedEvent("SecondaryLinesVisibilityChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<AxesVisibility>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="ChartStyle"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ChartStyle> ChartStyleChanged
        {
            add { AddHandler(ChartStyleChangedEvent, value); }
            remove { RemoveHandler(ChartStyleChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ChartStyleChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ChartStyleChangedEvent = EventManager.RegisterRoutedEvent("ChartStyleChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<ChartStyle>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AutoAdjustment"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<AutoAdjustmentMode> AutoAdjustmentChanged
        {
            add { AddHandler(AutoAdjustmentChangedEvent, value); }
            remove { RemoveHandler(AutoAdjustmentChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AutoAdjustmentChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AutoAdjustmentChangedEvent = EventManager.RegisterRoutedEvent("AutoAdjustmentChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<AutoAdjustmentMode>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="ShowLegend"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> ShowLegendChanged
        {
            add { AddHandler(ShowLegendChangedEvent, value); }
            remove { RemoveHandler(ShowLegendChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ShowLegendChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ShowLegendChangedEvent = EventManager.RegisterRoutedEvent("ShowLegendChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="ShowValuesOnBarsAndColumns"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> ShowValuesOnBarsAndColumnsChanged
        {
            add { AddHandler(ShowValuesOnBarsAndColumnsChangedEvent, value); }
            remove { RemoveHandler(ShowValuesOnBarsAndColumnsChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ShowValuesOnBarsAndColumnsChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ShowValuesOnBarsAndColumnsChangedEvent = EventManager.RegisterRoutedEvent("ShowValuesOnBarsAndColumnsChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="LegendAlignment"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<LegendAlignment> LegendAlignmentChanged
        {
            add { AddHandler(LegendAlignmentChangedEvent, value); }
            remove { RemoveHandler(LegendAlignmentChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LegendAlignmentChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LegendAlignmentChangedEvent = EventManager.RegisterRoutedEvent("LegendAlignmentChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<LegendAlignment>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="TitleFontSize"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> TitleFontSizeChanged
        {
            add { AddHandler(TitleFontSizeChangedEvent, value); }
            remove { RemoveHandler(TitleFontSizeChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="TitleFontSizeChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent TitleFontSizeChangedEvent = EventManager.RegisterRoutedEvent("TitleFontSizeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="TitleFontStyle"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontStyle> TitleFontStyleChanged
        {
            add { AddHandler(TitleFontStyleChangedEvent, value); }
            remove { RemoveHandler(TitleFontStyleChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="TitleFontStyleChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent TitleFontStyleChangedEvent = EventManager.RegisterRoutedEvent("TitleFontStyleChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontStyle>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxesFontWeight"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontWeight> AxesFontWeightChanged
        {
            add { AddHandler(AxesFontWeightChangedEvent, value); }
            remove { RemoveHandler(AxesFontWeightChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesFontWeightChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesFontWeightChangedEvent = EventManager.RegisterRoutedEvent("AxesFontWeightChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontWeight>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="TitleFontWeight"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontWeight> TitleFontWeightChanged
        {
            add { AddHandler(TitleFontWeightChangedEvent, value); }
            remove { RemoveHandler(TitleFontWeightChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="TitleFontWeightChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent TitleFontWeightChangedEvent = EventManager.RegisterRoutedEvent("TitleFontWeightChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontWeight>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="TitleFontFamily"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontFamily> TitleFontFamilyChanged
        {
            add { AddHandler(TitleFontFamilyChangedEvent, value); }
            remove { RemoveHandler(TitleFontFamilyChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="TitleFontFamilyChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent TitleFontFamilyChangedEvent = EventManager.RegisterRoutedEvent("TitleFontFamilyChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontFamily>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxesFontFamily"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontFamily> AxesFontFamilyChanged
        {
            add { AddHandler(AxesFontFamilyChangedEvent, value); }
            remove { RemoveHandler(AxesFontFamilyChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesFontFamilyChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesFontFamilyChangedEvent = EventManager.RegisterRoutedEvent("AxesFontFamilyChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontFamily>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxisTitleY"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> AxisTitleYChanged
        {
            add { AddHandler(AxisTitleYChangedEvent, value); }
            remove { RemoveHandler(AxisTitleYChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxisTitleYChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxisTitleYChangedEvent = EventManager.RegisterRoutedEvent("AxisTitleYChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxisTitleX"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> AxisTitleXChanged
        {
            add { AddHandler(AxisTitleXChangedEvent, value); }
            remove { RemoveHandler(AxisTitleXChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxisTitleXChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxisTitleXChangedEvent = EventManager.RegisterRoutedEvent("AxisTitleXChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="Title"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> TitleChanged
        {
            add { AddHandler(TitleChangedEvent, value); }
            remove { RemoveHandler(TitleChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="TitleChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent TitleChangedEvent = EventManager.RegisterRoutedEvent("TitleChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="MaxY"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> MaxYChanged
        {
            add { AddHandler(MaxYChangedEvent, value); }
            remove { RemoveHandler(MaxYChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="MaxYChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent MaxYChangedEvent = EventManager.RegisterRoutedEvent("MaxYChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="MaxX"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> MaxXChanged
        {
            add { AddHandler(MaxXChangedEvent, value); }
            remove { RemoveHandler(MaxXChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="MaxXChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent MaxXChangedEvent = EventManager.RegisterRoutedEvent("MaxXChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="LineThickness"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> LineThicknessChanged
        {
            add { AddHandler(LineThicknessChangedEvent, value); }
            remove { RemoveHandler(LineThicknessChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LineThicknessChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LineThicknessChangedEvent = EventManager.RegisterRoutedEvent("LineThicknessChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="ChartOpacity"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> ChartOpacityChanged
        {
            add { AddHandler(ChartOpacityChangedEvent, value); }
            remove { RemoveHandler(ChartOpacityChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ChartOpacityChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ChartOpacityChangedEvent = EventManager.RegisterRoutedEvent("ChartOpacityChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="SectionsY"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> SectionsYChanged
        {
            add { AddHandler(SectionsYChangedEvent, value); }
            remove { RemoveHandler(SectionsYChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="SectionsYChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent SectionsYChangedEvent = EventManager.RegisterRoutedEvent("SectionsYChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="SectionsX"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> SectionsXChanged
        {
            add { AddHandler(SectionsXChangedEvent, value); }
            remove { RemoveHandler(SectionsXChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="SectionsXChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent SectionsXChangedEvent = EventManager.RegisterRoutedEvent("SectionsXChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(Chart));

        /// <summary>
        /// Occurs when chart point/column/bar is double-clicked by left mouse button
        /// </summary>
        public event RoutedEventHandler ChartPointLeftButtonDoubleClick
        {
            add { AddHandler(ChartPointLeftButtonDoubleClickEvent, value); }
            remove { RemoveHandler(ChartPointLeftButtonDoubleClickEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ChartPointLeftButtonDoubleClick"/> routed event
        /// </summary>
        public static readonly RoutedEvent ChartPointLeftButtonDoubleClickEvent =
            EventManager.RegisterRoutedEvent("ChartPointLeftButtonDoubleClick", RoutingStrategy.Bubble,
                typeof(RoutedEventHandler), typeof(Chart));

        private void RaiseMarkerLeftButtonDoubleClickEvent(IChartValue point, ISeries series)
        {
            var e = new ChartPointLeftButtonDoubleClickEventArgs(ChartPointLeftButtonDoubleClickEvent, point, series);
            RaiseEvent(e);
        }
        #endregion

        #region INotifyPropertyChanged members
        /// <summary>Occurs when a property value changes.</summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises the PropertyChanged event when the property value has changed
        /// </summary>
        /// <param name="propertyName">Property name</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// Represents event args for <see cref="Chart.ChartPointLeftButtonDoubleClick"/> routed event.
    /// </summary>
    public class ChartPointLeftButtonDoubleClickEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets current <see cref="ChartValue"/>
        /// </summary>
        public IChartValue Value { get; private set; }
        /// <summary>
        /// Gets current <see cref="Series"/>
        /// </summary>
        public ISeries Series { get; private set; }
        /// <summary>
        /// Initialzes a new instance of <see cref="ChartPointLeftButtonDoubleClickEventArgs"/>s.
        /// </summary>
        /// <param name="baseEvent">Base routed event</param>
        /// <param name="value">Current <see cref="ChartValue"/></param>
        /// <param name="series">Current <see cref="Series"/></param>
        public ChartPointLeftButtonDoubleClickEventArgs(RoutedEvent baseEvent, IChartValue value, ISeries series)
            : base(baseEvent)
        {
            Value = value;
            Series = series;
        }
    }
}
