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
using ag.WPF.Chart.Annotations;
using ag.WPF.Chart.Values;
using ag.WPF.Chart.Series;
using Path = System.Windows.Shapes.Path;

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
        /// The style represented by straight lines with markers at control ponts
        /// </summary>
        LinesWithMarkers,
        /// <summary>
        /// The style represented by straight stacked lines with markers at control ponts
        /// </summary>
        StackedLinesWithMarkers,
        /// <summary>
        /// The style represented by straight 100% stacked lines with markers at control ponts
        /// </summary>
        FullStackedLinesWithMarkers,
        /// <summary>
        /// The style represented by smooth lines with markers at control ponts
        /// </summary>
        SmoothLinesWithMarkers,
        /// <summary>
        /// The style represented by smooth stacked lines with markers at control ponts
        /// </summary>
        SmoothStackedLinesWithMarkers,
        /// <summary>
        /// The style represented by smooth 100% stacked lines with markers at control ponts
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
        /// The style represented by radar with markers at control ponts
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
        /// Legend is left aligned
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
        /// Values of both axes are visible
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
        /// Both horizontal and vertical values are auto-adjusted
        /// </summary>
        Both
    }

    /// <summary>
    /// Specifies size of legend
    /// </summary>
    public enum LegendSize
    {
        /// <summary>
        /// 16x16 size
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
        /// Rectanglular shape
        /// </summary>
        Rectangle,
        /// <summary>
        /// Circular shape
        /// </summary>
        Circle,
        /// <summary>
        /// Star shape with five ras
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
        /// Chart boundary starts with offset from y-axes
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
    /// Represents custom control for creating simple 2-D charts
    /// </summary>
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
        /// <summary>
        /// The identifier of the <see cref="MarkerShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MarkerShapeProperty;
        /// <summary>
        /// The identifier of the <see cref="VerticalLinesCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty VerticalLinesCountProperty;
        /// <summary>
        /// The identifier of the <see cref="HorizontalLinesCount"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HorizontalLinesCountProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontFamilyProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontWeightProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontSizeProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesFontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontStyleProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesFontStretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesFontStretchProperty;
        /// <summary>
        /// The identifier of the <see cref="Caption"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionProperty;
        /// <summary>
        /// The identifier of the <see cref="CaptionFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionFontFamilyProperty;
        /// <summary>
        /// The identifier of the <see cref="CaptionFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionFontWeightProperty;
        /// <summary>
        /// The identifier of the <see cref="CaptionFontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionFontStyleProperty;
        /// <summary>
        /// The identifier of the <see cref="CaptionFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionFontSizeProperty;
        /// <summary>
        /// The identifier of the <see cref="CaptionFontStretch"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CaptionFontStretchProperty;
        /// <summary>
        /// The identifier of the <see cref="LegendFontFamily"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontFamilyProperty;
        /// <summary>
        /// The identifier of the <see cref="LegendFontWeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontWeightProperty;
        /// <summary>
        /// The identifier of the <see cref="LegendFontStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontStyleProperty;
        /// <summary>
        /// The identifier of the <see cref="LegendFontSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendFontSizeProperty;
        /// <summary>
        /// The identifier of the <see cref="XAxisText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XAxisTextProperty;
        /// <summary>
        /// The identifier of the <see cref="YAxisText"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YAxisTextProperty;
        /// <summary>
        /// The identifier of the <see cref="LegendAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendAlignmentProperty;
        /// <summary>
        /// The identifier of the <see cref="ShowLegend"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowLegendProperty;
        /// <summary>
        /// The identifier of the <see cref="ShowValuesOnBarsAndColumns"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowValuesOnBarsAndColumnsProperty;
        /// <summary>
        /// The identifier of the <see cref="ChartStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartStyleProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesValuesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesValuesVisibilityProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesLinesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesLinesVisibilityProperty;
        /// <summary>
        /// The identifier of the <see cref="SecondaryLinesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SecondaryLinesVisibilityProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesValuesFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesValuesFormatProperty;
        /// <summary>
        /// The identifier of the <see cref="CustomXAxisValues"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomXAxisValuesProperty;
        /// <summary>
        /// The identifier of the <see cref="CustomYAxisValues"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomYAxisValuesProperty;
        /// <summary>
        /// The identifier of the <see cref="ChartOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartOpacityProperty;
        /// <summary>
        /// The identifier of the <see cref="MaxX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxXProperty;
        /// <summary>
        /// The identifier of the <see cref="MaxY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxYProperty;
        /// <summary>
        /// The identifier of the <see cref="AutoAdjustment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoAdjustmentProperty;
        /// <summary>
        /// The identifier of the <see cref="LegendSize"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendSizeProperty;
        /// <summary>
        /// The identifier of the <see cref="LegendShape"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty LegendShapeProperty;
        /// <summary>
        /// The identifier of the <see cref="ShowTicks"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowTicksProperty;
        /// <summary>
        /// The identifier of the <see cref="ChartBoundary"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartBoundaryProperty;
        /// <summary>
        /// The identifier of the <see cref="CustomWaterfallLegendsProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CustomWaterfallLegendsProperty;
        /// <summary>
        /// The identifier of the <see cref="ItemsSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty;
        #endregion

        #region ctor
        static Chart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart), new FrameworkPropertyMetadata(typeof(Chart)));

            VerticalLinesCountProperty = DependencyProperty.Register(nameof(VerticalLinesCount), typeof(int), typeof(Chart),
                new FrameworkPropertyMetadata(10, OnVerticalLinesCountChanged, CoerceVerticalLinesCount));
            HorizontalLinesCountProperty = DependencyProperty.Register(nameof(HorizontalLinesCount), typeof(int), typeof(Chart),
                new FrameworkPropertyMetadata(10, OnHorizontalLinesCountChanged, CoerceHorizontalLinesCount));
            ChartOpacityProperty = DependencyProperty.Register(nameof(ChartOpacity), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(1.0, OnChartOpacityChanged, CoerceChartOpacity));
            CaptionProperty = DependencyProperty.Register(nameof(Caption), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("", OnCaptionChanged));
            XAxisTextProperty = DependencyProperty.Register(nameof(XAxisText), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("x-Axis", OnXAxisTextChanged));
            YAxisTextProperty = DependencyProperty.Register(nameof(YAxisText), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("y-Axis", OnYAxisTextChanged));
            AxesFontFamilyProperty = DependencyProperty.Register(nameof(AxesFontFamily), typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontFamily, OnAxesFontFamilyChanged));
            AxesFontWeightProperty = DependencyProperty.Register(nameof(AxesFontWeight), typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontWeight, OnAxesFontWeightChanged));
            AxesFontSizeProperty = DependencyProperty.Register(nameof(AxesFontSize), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontSize, OnAxesFontSizeChanged));
            AxesFontStyleProperty = DependencyProperty.Register(nameof(AxesFontStyle), typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontStyle, OnAxesFontStyleChanged));
            AxesFontStretchProperty = DependencyProperty.Register(nameof(AxesFontStretch), typeof(FontStretch), typeof(Chart),
                new FrameworkPropertyMetadata(FontStretches.Normal, OnAxesFontStretchChanged));
            CaptionFontFamilyProperty = DependencyProperty.Register(nameof(CaptionFontFamily), typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontFamily, OnCaptionFontFamilyChanged));
            LegendFontFamilyProperty = DependencyProperty.Register(nameof(LegendFontFamily), typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontFamily, OnLegendFontFamilyChanged));
            CaptionFontWeightProperty = DependencyProperty.Register(nameof(CaptionFontWeight), typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontWeight, OnCaptionFontWeightChanged));
            LegendFontWeightProperty = DependencyProperty.Register(nameof(LegendFontWeight), typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontWeight, OnLegendFontWeightChanged));
            CaptionFontStyleProperty = DependencyProperty.Register(nameof(CaptionFontStyle), typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontStyle, OnCaptionFontStyleChanged));
            LegendFontStyleProperty = DependencyProperty.Register(nameof(LegendFontStyle), typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontStyle, OnLegendFontStyleChanged));
            CaptionFontSizeProperty = DependencyProperty.Register(nameof(CaptionFontSize), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontSize, OnCaptionFontSizeChanged));
            CaptionFontStretchProperty = DependencyProperty.Register(nameof(CaptionFontStretch), typeof(FontStretch), typeof(Chart),
                new FrameworkPropertyMetadata(FontStretches.Normal, OnCaptionFontStretchChanged));
            LegendFontSizeProperty = DependencyProperty.Register(nameof(LegendFontSize), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontSize, OnLegendFontSizeChanged));
            LegendAlignmentProperty = DependencyProperty.Register(nameof(LegendAlignment), typeof(LegendAlignment), typeof(Chart),
                new FrameworkPropertyMetadata(LegendAlignment.Bottom, OnLegendAlignmentChanged));
            ShowLegendProperty = DependencyProperty.Register(nameof(ShowLegend), typeof(bool), typeof(Chart),
                new FrameworkPropertyMetadata(true, OnShowLegendChanged));
            ShowValuesOnBarsAndColumnsProperty = DependencyProperty.Register(nameof(ShowValuesOnBarsAndColumns), typeof(bool), typeof(Chart),
                new FrameworkPropertyMetadata(true, OnShowValuesOnBarsAndColumnsChanged));
            ChartStyleProperty = DependencyProperty.Register(nameof(ChartStyle), typeof(ChartStyle), typeof(Chart),
                new FrameworkPropertyMetadata(ChartStyle.Lines, OnChartStyleChanged));
            AxesValuesVisibilityProperty = DependencyProperty.Register(nameof(AxesValuesVisibility), typeof(AxesVisibility), typeof(Chart),
                new FrameworkPropertyMetadata(AxesVisibility.Both, OnAxesValuesVisibilityChanged));
            AxesLinesVisibilityProperty = DependencyProperty.Register(nameof(AxesLinesVisibility), typeof(AxesVisibility), typeof(Chart),
                new FrameworkPropertyMetadata(AxesVisibility.Both, OnAxesLinesVisibilityChanged));
            SecondaryLinesVisibilityProperty = DependencyProperty.Register(nameof(SecondaryLinesVisibility), typeof(AxesVisibility), typeof(Chart),
                new FrameworkPropertyMetadata(AxesVisibility.Both, OnSecondaryLinesVisibilityChanged));
            AxesValuesFormatProperty = DependencyProperty.Register(nameof(AxesValuesFormat), typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("0", OnAxesValuesFormatChanged));
            CustomXAxisValuesProperty = DependencyProperty.Register(nameof(CustomXAxisValues), typeof(IEnumerable<string>), typeof(Chart),
                new FrameworkPropertyMetadata(null, OnCustomXAxisValuesChanged));
            CustomYAxisValuesProperty = DependencyProperty.Register(nameof(CustomYAxisValues), typeof(IEnumerable<string>), typeof(Chart),
                new FrameworkPropertyMetadata(null, OnCustomYAxisValuesChanged));
            MaxXProperty = DependencyProperty.Register(nameof(MaxX), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(100.0, OnMaxXChanged, CoerceMaxX));
            MaxYProperty = DependencyProperty.Register(nameof(MaxY), typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(100.0, OnMaxYChanged, CoerceMaxY));
            AutoAdjustmentProperty = DependencyProperty.Register(nameof(AutoAdjustment), typeof(AutoAdjustmentMode), typeof(Chart),
                new FrameworkPropertyMetadata(AutoAdjustmentMode.Both, OnAutoAdjustmentChanged));
            LegendSizeProperty = DependencyProperty.Register(nameof(LegendSize), typeof(LegendSize), typeof(Chart),
                new FrameworkPropertyMetadata(LegendSize.ExtraSmall, OnLegendSizeChanged));
            LegendShapeProperty = DependencyProperty.Register(nameof(LegendShape), typeof(ShapeStyle), typeof(Chart),
                new FrameworkPropertyMetadata(ShapeStyle.Rectangle, OnLegendShapeChanged));
            ShowTicksProperty = DependencyProperty.Register(nameof(ShowTicks),
                typeof(bool), typeof(Chart), new FrameworkPropertyMetadata(true, OnShowTicksChanged));
            MarkerShapeProperty = DependencyProperty.Register(nameof(MarkerShape), typeof(ShapeStyle), typeof(Chart),
                new FrameworkPropertyMetadata(ShapeStyle.Rectangle, OnMarkerShapeCahnged));
            ChartBoundaryProperty = DependencyProperty.Register(nameof(ChartBoundary), typeof(ChartBoundary), typeof(Chart),
                new FrameworkPropertyMetadata(ChartBoundary.WithOffset, OnChartBoundaryChanged));
            CustomWaterfallLegendsProperty = DependencyProperty.RegisterAttached(nameof(CustomWaterfallLegends), typeof(IEnumerable<string>), typeof(Chart), new FrameworkPropertyMetadata(new[] { "Increase", "Decrease" }));
            ItemsSourceProperty = DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable<ISeries>), typeof(Chart), new PropertyMetadata(null, (u, e) =>
                 {
                     if (u is Chart chart)
                     {
                         if (e.OldValue is INotifyCollectionChanged oldValueNotifyCollectionChanged)
                         {
                             oldValueNotifyCollectionChanged.CollectionChanged -= chart.ItemsSource_CollectionChanged;
                         }
                         if (e.NewValue is INotifyCollectionChanged newValueNotifyCollectionChanged)
                         {
                             newValueNotifyCollectionChanged.CollectionChanged += chart.ItemsSource_CollectionChanged;
                         }
                         chart.OnItemsSourceChanged((IEnumerable<ISeries>)e.OldValue, (IEnumerable<ISeries>)e.NewValue);
                     }
                 }));
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

            _canvas = GetTemplateChild(ElementCanvas) as Canvas;

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
        private void ItemsSource_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    {
                        if (!(e.NewItems[0] is ISeries series)) break;

                        series.Index = e.NewStartingIndex;
                        series.PropertyChanged += Series_PropertyChanged;

                        for (var pathIndex = 0; pathIndex < series.Paths.Length; pathIndex++)
                        {
                            var path = series.Paths[pathIndex];
                            if (path == null) continue;
                            (int, ColoredPaths) parameter;
                            string brushPath;
                            switch (pathIndex)
                            {
                                case 0:
                                    if (series.MainBrush == null)
                                    {
                                        var min = Statics.PredefinedMainBrushes.Min(b => b.Counter);
                                        for (var i = 0; i < Statics.PredefinedMainBrushes.Length; i++)
                                        {
                                            if (Statics.PredefinedMainBrushes[i].Counter == min)
                                            {
                                                series.MainBrush = Statics.PredefinedMainBrushes[i].Brush;
                                                Statics.PredefinedMainBrushes[i].Counter++;
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
                                        var min = Statics.PredefinedSecondaryBrushes.Min(b => b.Counter);
                                        for (var i = 0; i < Statics.PredefinedSecondaryBrushes.Length; i++)
                                        {
                                            if (Statics.PredefinedSecondaryBrushes[i].Counter == min)
                                            {
                                                series.SecondaryBrush = Statics.PredefinedSecondaryBrushes[i].Brush;
                                                Statics.PredefinedSecondaryBrushes[i].Counter++;
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
                                    if (!series.IsStockSeries()) continue;
                                    parameter = (0, ColoredPaths.Stock);
                                    brushPath = "Foreground";
                                    break;
                                default:
                                    continue;
                            }
                            #region Path data
                            var ptsBinding = new MultiBinding { Converter = new ValuesToPathConverter() };
                            ptsBinding.Bindings.Add(new Binding("ActualWidth") { ElementName = ElementCanvas });
                            ptsBinding.Bindings.Add(new Binding("ActualHeight") { ElementName = ElementCanvas });
                            ptsBinding.Bindings.Add(new Binding("ItemsSource")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("ChartStyle")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("Index")
                            {
                                Source = series
                            });
                            ptsBinding.Bindings.Add(new Binding("AutoAdjustment")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("MaxX")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("MaxY")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("ChartBoundary")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("AxesFontFamily")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("AxesFontSize")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("AxesFontStyle")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("AxesFontWeight")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("AxesFontStretch")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("CustomXAxisValues")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("HorizontalLinesCount")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("VerticalLinesCount")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("ShowValuesOnBarsAndColumns")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("FlowDirection")
                            {
                                Source = this
                            });
                            ptsBinding.Bindings.Add(new Binding("MarkerShape")
                            {
                                Source = this
                            });
                            ptsBinding.ConverterParameter = parameter;
                            ptsBinding.NotifyOnSourceUpdated = true;
                            path.SetBinding(Path.DataProperty, ptsBinding);
                            #endregion

                            #region Path stroke
                            var strkBinding = new MultiBinding { Converter = new PathStrokeConverter() };
                            strkBinding.Bindings.Add(new Binding("ChartStyle")
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
                            fillBinding.Bindings.Add(new Binding("ChartStyle")
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

                            path.SetBinding(OpacityProperty, new Binding("ChartOpacity") { Source = this });
                            path.MouseLeftButtonDown += Path_MouseLeftButtonDown;
                            path.MouseMove += Path_MouseMove;
                            _canvas.Children.Add(path);
                        }

                        #region Main legend
                        var legend = new Legend() { Index = series.Index };

                        legend.SetBinding(Legend.LegendBackgroundProperty, new Binding("MainBrush") { Source = series });
                        var legendVisibilityBinding = new MultiBinding { Converter = new LegendVisibilityConverter() };
                        legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        legendVisibilityBinding.Bindings.Add(new Binding("ItemsSource")
                        {
                            Source = this
                        });
                        legendVisibilityBinding.NotifyOnSourceUpdated = true;
                        legend.SetBinding(VisibilityProperty, legendVisibilityBinding);

                        legend.SetBinding(Legend.TextProperty, new Binding("Name") { Source = series });

                        LegendsCollection.Add(legend);
                        #endregion

                        #region Positive waterfall legend
                        legend = new Legend() { Index = series.Index };

                        legend.SetBinding(Legend.LegendBackgroundProperty, new Binding("MainBrush") { Source = series });
                        legendVisibilityBinding = new MultiBinding { Converter = new LegendWaterfallVisibilityConverter() };
                        legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        legendVisibilityBinding.Bindings.Add(new Binding("Index")
                        {
                            Source = series
                        });
                        legendVisibilityBinding.Bindings.Add(new Binding("ItemsSource")
                        {
                            Source = this
                        });
                        legend.SetBinding(VisibilityProperty, legendVisibilityBinding);

                        legend.SetBinding(Legend.TextProperty, new Binding("CustomWaterfallLegends[0]") { Source = this });

                        LegendsCollection.Add(legend);
                        #endregion

                        #region Negative waterfall legend
                        legend = new Legend() { Index = series.Index };

                        legend.SetBinding(Legend.LegendBackgroundProperty, new Binding("SecondaryBrush") { Source = series });
                        legendVisibilityBinding = new MultiBinding { Converter = new LegendWaterfallVisibilityConverter() };
                        legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        legendVisibilityBinding.Bindings.Add(new Binding("Index")
                        {
                            Source = series
                        });
                        legendVisibilityBinding.Bindings.Add(new Binding("ItemsSource")
                        {
                            Source = this
                        });
                        legendVisibilityBinding.NotifyOnSourceUpdated = true;
                        legend.SetBinding(VisibilityProperty, legendVisibilityBinding);

                        legend.SetBinding(Legend.TextProperty, new Binding("CustomWaterfallLegends[1]") { Source = this });

                        LegendsCollection.Add(legend);
                        #endregion

                        if (series.IsStockSeries())
                        {
                            #region Stock legend
                            legend = new Legend() { Index = series.Index };

                            legend.SetBinding(Legend.LegendBackgroundProperty, new Binding("Foreground") { Source = this });
                            legendVisibilityBinding = new MultiBinding { Converter = new LegendStockVisibilityConverter() };
                            legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                            {
                                Source = this
                            });
                            legendVisibilityBinding.Bindings.Add(new Binding("Index")
                            {
                                Source = series
                            });
                            legendVisibilityBinding.Bindings.Add(new Binding("ItemsSource")
                            {
                                Source = this
                            });
                            legendVisibilityBinding.ConverterParameter = ColoredPaths.Stock;
                            legendVisibilityBinding.NotifyOnSourceUpdated = true;
                            legend.SetBinding(VisibilityProperty, legendVisibilityBinding);
                            legend.Text = "Close";
                            //legend.SetBinding(Legend.TextProperty, new Binding("CustomWaterfallLegends[1]") { Source = this });

                            LegendsCollection.Add(legend);
                            #endregion

                            #region Up stock legend
                            legend = new Legend() { Index = series.Index };

                            legend.SetBinding(Legend.LegendBackgroundProperty, new Binding("MainBrush") { Source = series });
                            legendVisibilityBinding = new MultiBinding { Converter = new LegendStockVisibilityConverter() };
                            legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                            {
                                Source = this
                            });
                            legendVisibilityBinding.Bindings.Add(new Binding("Index")
                            {
                                Source = series
                            });
                            legendVisibilityBinding.Bindings.Add(new Binding("ItemsSource")
                            {
                                Source = this
                            });
                            legendVisibilityBinding.ConverterParameter = ColoredPaths.Up;
                            legendVisibilityBinding.NotifyOnSourceUpdated = true;
                            legend.SetBinding(VisibilityProperty, legendVisibilityBinding);
                            legend.Text = series is HighLowCloseSeries ? "High" : "Increase";
                            //legend.SetBinding(Legend.TextProperty, new Binding("CustomWaterfallLegends[0]") { Source = this });

                            LegendsCollection.Add(legend);
                            #endregion

                            #region Stock down legend
                            legend = new Legend() { Index = series.Index };

                            legend.SetBinding(Legend.LegendBackgroundProperty, new Binding("SecondaryBrush") { Source = series });
                            legendVisibilityBinding = new MultiBinding { Converter = new LegendStockVisibilityConverter() };
                            legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                            {
                                Source = this
                            });
                            legendVisibilityBinding.Bindings.Add(new Binding("Index")
                            {
                                Source = series
                            });
                            legendVisibilityBinding.Bindings.Add(new Binding("ItemsSource")
                            {
                                Source = this
                            });
                            legendVisibilityBinding.ConverterParameter = ColoredPaths.Down;
                            legendVisibilityBinding.NotifyOnSourceUpdated = true;
                            legend.SetBinding(VisibilityProperty, legendVisibilityBinding);
                            legend.Text = legend.Text = series is HighLowCloseSeries ? "Low" : "Decrease";
                            //legend.SetBinding(Legend.TextProperty, new Binding("CustomWaterfallLegends[1]") { Source = this });

                            LegendsCollection.Add(legend);
                            #endregion
                        }

                        rebuildPieLegends(series.Values, series);

                        updateBindings();

                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (!(e.OldItems[0] is ISeries series)) break;

                        series.PropertyChanged -= Series_PropertyChanged;

                        foreach (var p in series.Paths)
                        {
                            if (p == null) continue;
                            p.MouseLeftButtonDown -= Path_MouseLeftButtonDown;
                            p.MouseMove -= Path_MouseMove;
                            var i = _canvas.Children.IndexOf(p);
                            if (i > -1)
                                _canvas.Children.RemoveAt(i);
                        }

                        for (var i = LegendsCollection.Count - 1; i >= 0; i--)
                        {
                            if (((Legend)LegendsCollection[i]).Index == series.Index)
                                LegendsCollection.RemoveAt(i);
                        }
                        foreach (Legend lg in LegendsCollection.Where(l => ((Legend)l).Index > series.Index).ToArray())
                        {
                            lg.Index--;
                        }
                        foreach (var sr in ItemsSource.Where(sc => sc.Index > series.Index).ToArray())
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

                        if (!(bool)((Series.Series)series).GetValue(Statics.HasCustomMainBrushProperty))
                        {
                            for (var i = 0; i < Statics.PredefinedMainBrushes.Length; i++)
                            {
                                if (series.MainBrush.Equals(Statics.PredefinedMainBrushes[i].Brush))
                                {
                                    Statics.PredefinedMainBrushes[i].Counter--;
                                }
                            }
                        }
                        if (!(bool)((Series.Series)series).GetValue(Statics.HasCustomSecondaryBrushProperty))
                        {
                            for (var i = 0; i < Statics.PredefinedSecondaryBrushes.Length; i++)
                            {
                                if (series.SecondaryBrush.Equals(Statics.PredefinedSecondaryBrushes[i].Brush))
                                {
                                    Statics.PredefinedSecondaryBrushes[i].Counter--;
                                }
                            }
                        }

                        updateBindings();

                        break;
                    }
            }
        }

        private void PieImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!ItemsSource.Any()) return;
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
            tooltip.Content = ItemsSource.First().Name;
        }

        private void Series_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Values":
                    if (sender is ISeries series)
                    {
                        foreach (var sr in ItemsSource.Where(s => s.Index != series.Index))
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
            OnPropertyChanged("ItemsSource");
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
                        rc = s.RealStockHighRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
                        if (rc != default)
                        {
                            var index = s.RealStockHighRects.IndexOf(rc);
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
                            rc = s.RealStockLowRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
                            if (rc != default)
                            {
                                var index = s.RealStockLowRects.IndexOf(rc);
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
                        }
                    }
                    break;
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
                rc = s.RealStockHighRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
            if (rc == default)
                rc = s.RealStockLowRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
            if (rc == default)
                return;
            var index = s.RealRects.IndexOf(rc);
            if (index == -1)
                index = s.RealStockHighRects.IndexOf(rc);
            if (index == -1)
                index = s.RealStockLowRects.IndexOf(rc);
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
            foreach (var series in ItemsSource)
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
                if (brushIndex == Statics.PredefinedMainBrushes.Length) brushIndex = 0;
                var legend = new Legend();

                legend.LegendBackground = Statics.PredefinedMainBrushes[brushIndex++].Brush;

                var textBinding = new MultiBinding { Converter = new PieSectionTextConverter(), ConverterParameter = v };
                textBinding.Bindings.Add(new Binding("Values") { Source = series });
                textBinding.Bindings.Add(new Binding("AxesValuesFormat") { Source = this });
                legend.SetBinding(Legend.TextProperty, textBinding);

                PieLegendsCollection.Add(legend);
            }
        }
        #endregion

        #region Dependency properties wrappers
        /// <summary>
        /// Gets or sets the collection of custom legend text when <see cref="ChartStyle"/> is set to <see cref="ChartStyle.Waterfall"/>.
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the collection of custom legend text when ChartStyle is set to ChartStyle.Waterfall")]
        public IEnumerable<string> CustomWaterfallLegends
        {
            get { return (IEnumerable<string>)GetValue(CustomWaterfallLegendsProperty); }
            set { SetValue(CustomWaterfallLegendsProperty, value); }
        }
        /// <summary>
        /// Gets/sets the collection of <see cref="Series"/> objects associated with chart control.
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the collection of Series objects associated with chart control")]
        public IEnumerable<ISeries> ItemsSource
        {
            get { return (IEnumerable<ISeries>)GetValue(ItemsSourceProperty); }
            set { SetValue(ItemsSourceProperty, value); }
        }
        /// <summary>
        /// Specifies whether chart boundary is started on y-axes or with offfset from y-axes.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> is set to any non-line style.</remarks>
        [Category("ChartAppearance"), Description("Specifies whether chart boundary is started on y-axes or with offfset from y-axes")]
        public ChartBoundary ChartBoundary
        {
            get { return (ChartBoundary)GetValue(ChartBoundaryProperty); }
            set { SetValue(ChartBoundaryProperty, value); }
        }
        /// <summary>
        /// Specifies whether ticks are drawn on axes.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAppearance"), Description("Specifies whether ticks are drawn on axes")]
        public bool ShowTicks
        {
            get { return (bool)GetValue(ShowTicksProperty); }
            set { SetValue(ShowTicksProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend font family.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets legend font family")]
        public FontFamily LegendFontFamily
        {
            get { return (FontFamily)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend font wweight.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets legend font wweight")]
        public FontWeight LegendFontWeight
        {
            get { return (FontWeight)GetValue(LegendFontWeightProperty); }
            set { SetValue(LegendFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend font style.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets legend font style")]
        public FontStyle LegendFontStyle
        {
            get { return (FontStyle)GetValue(LegendFontStyleProperty); }
            set { SetValue(LegendFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend font size.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets legend font size")]
        public double LegendFontSize
        {
            get { return (double)GetValue(LegendFontSizeProperty); }
            set { SetValue(LegendFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the shape of legend. Can be one of <see cref="LegendShape"/> enumeration members.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the shape of legend. Can be one of LegendShape enumeration members")]
        public ShapeStyle LegendShape
        {
            get { return (ShapeStyle)GetValue(LegendShapeProperty); }
            set { SetValue(LegendShapeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the size of legend. Can be one of <see cref="LegendSize"/> enumeration members.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the size of legend. Can be one of LegendSize enumeration members")]
        public LegendSize LegendSize
        {
            get { return (LegendSize)GetValue(LegendSizeProperty); }
            set { SetValue(LegendSizeProperty, value); }
        }
        /// <summary>
        /// Specifies whether control will automatically adjust its max x- and y- values or they should be set explicitly.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ChartStyle.Doughnut"/>.</remarks>
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
        /// Gets or sets max numeric value for y-axis. The default value is 100.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="AutoAdjustment"/> property is set to true.</remarks>
        [Category("ChartMeasures"), Description("Gets or sets max numeric value for y-axis. The default value is 100")]
        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }
        /// <summary>
        /// Gets or sets max numeric value for x-axis. The default value is 100.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="AutoAdjustment"/> property is set to true.</remarks>
        [Category("ChartMeasures"), Description("Gets or sets max numeric value for x-axis. The default value is 100")]
        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }
        /// <summary>
        /// Gets or set chart opacity in range of 0.0 (fully transparent) to 1.0 (fully opaque).
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or set chart opacity in range of 0.0 (fully transparent) to 1.0 (fully opaque)")]
        public double ChartOpacity
        {
            get { return (double)GetValue(ChartOpacityProperty); }
            set { SetValue(ChartOpacityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the sequence of strings to be drawn next to x-axis instead of numeric values.
        /// </summary>
        [Category("ChartAxes"), Description("Gets or sets the sequence of strings to be drawn next to x-axis instead of numeric values")]
        public IEnumerable<string> CustomXAxisValues
        {
            get { return (IEnumerable<string>)GetValue(CustomXAxisValuesProperty); }
            set { SetValue(CustomXAxisValuesProperty, value); }
        }
        /// <summary>
        /// Gets or sets the sequence of strings to be drawn next to y-axis instead of numeric values.
        /// </summary>
        [Category("ChartAxes"), Description("Gets or sets the sequence of strings to be drawn next to y-axis instead of numeric values")]
        public IEnumerable<string> CustomYAxisValues
        {
            get { return (IEnumerable<string>)GetValue(CustomYAxisValuesProperty); }
            set { SetValue(CustomYAxisValuesProperty, value); }
        }
        /// <summary>
        /// Gets or sets the visibility state of x- and y- axes numeric/custom values. Can be one of <see cref="AxesVisibility"/> enumeration members.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ChartStyle.Doughnut"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the visibility state of x- and y- axes numeric/custom values. Can be one of AxesValuesVisibility enumeration members")]
        public AxesVisibility AxesValuesVisibility
        {
            get { return (AxesVisibility)GetValue(AxesValuesVisibilityProperty); }
            set { SetValue(AxesValuesVisibilityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the visibility state of x- and y- axes lines. Can be one of <see cref="AxesVisibility"/> enumeration members.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ChartStyle.Doughnut"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the visibility state of x- and y- axes lines. Can be one of AxesValuesVisibility enumeration members")]
        public AxesVisibility AxesLinesVisibility
        {
            get { return (AxesVisibility)GetValue(AxesLinesVisibilityProperty); }
            set { SetValue(AxesLinesVisibilityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the visibility state of secondary horizontal and vertical lines. Can be one of <see cref="AxesVisibility"/> enumeration members.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ChartStyle.Doughnut"/>.</remarks>
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
        /// Specifies whether chart legend should be shown.
        /// </summary>
        [Category("ChartLegend"), Description("Specifies whether chart legend should be shown")]
        public bool ShowLegend
        {
            get { return (bool)GetValue(ShowLegendProperty); }
            set { SetValue(ShowLegendProperty, value); }
        }
        /// <summary>
        /// Gets or sets chart legend alignment. Can be one of <see cref="LegendAlignment"/> enumeration members.
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets chart legend alignment. Can be one of LegendAlignment enumeration members")]
        public LegendAlignment LegendAlignment
        {
            get { return (LegendAlignment)GetValue(LegendAlignmentProperty); }
            set { SetValue(LegendAlignmentProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font size of chart caption.
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font size of chart caption")]
        public double CaptionFontSize
        {
            get { return (double)GetValue(CaptionFontSizeProperty); }
            set { SetValue(CaptionFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font style of chart caption.
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font style of chart caption")]
        public FontStyle CaptionFontStyle
        {
            get { return (FontStyle)GetValue(CaptionFontStyleProperty); }
            set { SetValue(CaptionFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font weight of chart caption.
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font weight of chart caption")]
        public FontWeight CaptionFontWeight
        {
            get { return (FontWeight)GetValue(CaptionFontWeightProperty); }
            set { SetValue(CaptionFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font family of chart caption.
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font family of chart caption")]
        public FontFamily CaptionFontFamily
        {
            get { return (FontFamily)GetValue(CaptionFontFamilyProperty); }
            set { SetValue(CaptionFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font stretch of chart caption.
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font stretch of chart caption")]
        public FontStretch CaptionFontStretch
        {
            get { return (FontStretch)GetValue(CaptionFontStretchProperty); }
            set { SetValue(CaptionFontStretchProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font family of chart axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font family of chart axes")]
        public FontFamily AxesFontFamily
        {
            get { return (FontFamily)GetValue(AxesFontFamilyProperty); }
            set { SetValue(AxesFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font weight of chart axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font weight of chart axes")]
        public FontWeight AxesFontWeight
        {
            get { return (FontWeight)GetValue(AxesFontWeightProperty); }
            set { SetValue(AxesFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font size of chart axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font size of chart axes")]
        public double AxesFontSize
        {
            get { return (double)GetValue(AxesFontSizeProperty); }
            set { SetValue(AxesFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font style of chart axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font style of chart axes")]
        public FontStyle AxesFontStyle
        {
            get { return (FontStyle)GetValue(AxesFontStyleProperty); }
            set { SetValue(AxesFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font stretch of chart axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets of sets the font stretch of chart axes")]
        public FontStretch AxesFontStretch
        {
            get { return (FontStretch)GetValue(AxesFontStretchProperty); }
            set { SetValue(AxesFontStretchProperty, value); }
        }
        /// <summary>
        /// Gets or sets the text which appears on the top/bottom of y-axis.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the text which appears on the top/bottom of y-axis")]
        public string YAxisText
        {
            get { return (string)GetValue(YAxisTextProperty); }
            set { SetValue(YAxisTextProperty, value); }
        }
        /// <summary>
        /// Gets or sets the text which appears on the right/left of x-axis.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.</remarks>
        [Category("ChartAxes"), Description("Gets or sets the text which appears on the right/left of x-axis")]
        public string XAxisText
        {
            get { return (string)GetValue(XAxisTextProperty); }
            set { SetValue(XAxisTextProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart caption.
        /// </summary>
        [Category("ChartCaption"), Description("Gets or sets the chart caption")]
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
        /// <summary>
        /// Gets or sets amount of vertical lines.
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to one of the following: <see cref="ChartStyle.SolidPie"/>, <see cref="ChartStyle.SlicedPie"/>, <see cref="ChartStyle.Doughnut"/>, <see cref="ChartStyle.Radar"/>, <see cref="ChartStyle.RadarWithMarkers"/>, <see cref="ChartStyle.RadarArea"/>.
        /// </remarks>
        [Category("ChartMeasures"), Description("Gets or sets amount of vertical lines")]
        public int VerticalLinesCount
        {
            get { return (int)GetValue(VerticalLinesCountProperty); }
            set { SetValue(VerticalLinesCountProperty, value); }
        }
        /// <summary>
        /// Gets or sets amount of horizontal lines.
        /// </summary>
        /// <remarks>
        /// This property has no effect if <see cref="ChartStyle"/> is set to <see cref="ChartStyle.Bars"/>, <see cref="ChartStyle.StackedBars"/>, <see cref="ChartStyle.FullStackedBars"/>, 
        /// <see cref="ChartStyle.SolidPie"/> or <see cref="ChartStyle.SlicedPie"/> or <see cref="ChartStyle.Doughnut"/>
        /// </remarks>
        [Category("ChartMeasures"), Description("Gets or sets amount of horizontal lines")]
        public int HorizontalLinesCount
        {
            get { return (int)GetValue(HorizontalLinesCountProperty); }
            set { SetValue(HorizontalLinesCountProperty, value); }
        }
        /// <summary>
        /// Gets or sets format for numeric values drawn next to x- and/or y- axes.
        /// </summary>
        [Category("ChartAxes"), Description("Gets or sets format for numeric values drawn next to x- and/or y- axes")]
        public string AxesValuesFormat
        {
            get { return (string)GetValue(AxesValuesFormatProperty); }
            set { SetValue(AxesValuesFormatProperty, value); }
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
        /// <param name="imageFileName">Full path to image file</param>
        public void SaveAsImage(string imageFileName)
        {
            var dpi = VisualTreeHelper.GetDpi(this);
            var bounds = VisualTreeHelper.GetDescendantBounds(this);

            //var rnb = new RenderTargetBitmap(
            //    (int)(bounds.Width*dpi.DpiScaleX), (int)(bounds.Height*dpi.DpiScaleY), dpi.PixelsPerInchX, dpi.PixelsPerInchY, PixelFormats.Pbgra32);
            //var dv = new DrawingVisual();
            //using var ctx = dv.RenderOpen();
            //var vb = new VisualBrush(_Canvas);
            //ctx.DrawRectangle(vb, null, new Rect(new Point(), bounds.Size));
            //rnb.Render(dv);

            //var factors = getScalingFactors();
            var ext = System.IO.Path.GetExtension(imageFileName);
            if (ext == null) return;

            //var rnb = new RenderTargetBitmap((int)(ActualWidth * factors.Item1), (int)(ActualHeight * factors.Item2),
            //    96 * factors.Item1, 96 * factors.Item2, PixelFormats.Pbgra32);
            //rnb.Render(this);

            //var rect = new Rect(this.RenderSize);
            //rect.Width *= factors.Item1;
            //rect.Height *= factors.Item2;
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

        /// <summary>
        /// Invoked just before the <see cref="ItemsSourceChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnItemsSourceChanged(IEnumerable<ISeries> oldValue, IEnumerable<ISeries> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<IEnumerable<ISeries>>(oldValue, newValue)
            {
                RoutedEvent = ItemsSourceChangedEvent
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
            ch.OnShowTicksChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ShowTicksChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnShowTicksChanged(bool oldValue, bool newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
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

        private static void OnCustomXAxisValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCustomXAxisValuesChanged((IEnumerable<string>)e.OldValue, (IEnumerable<string>)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CustomXAxisValuesChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCustomXAxisValuesChanged(IEnumerable<string> oldValue, IEnumerable<string> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<IEnumerable<string>>(oldValue, newValue)
            {
                RoutedEvent = CustomXAxisValuesChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnCustomYAxisValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCustomYAxisValuesChanged((IEnumerable<string>)e.OldValue, (IEnumerable<string>)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CustomYAxisValuesChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCustomYAxisValuesChanged(IEnumerable<string> oldValue, IEnumerable<string> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<IEnumerable<string>>(oldValue, newValue)
            {
                RoutedEvent = CustomYAxisValuesChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnAxesValuesFormatChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAxesValuesFormatChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesValuesFormatChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesValuesFormatChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = AxesValuesFormatChangedEvent
            };
            RaiseEvent(e);
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

        private static void OnCaptionFontSizeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCaptionFontSizeChanged((double)e.OldValue, (double)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CaptionFontSizeChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCaptionFontSizeChanged(double oldValue, double newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<double>(oldValue, newValue)
            {
                RoutedEvent = CaptionFontSizeChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnCaptionFontStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCaptionFontStyleChanged((FontStyle)e.OldValue, (FontStyle)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CaptionFontStyleChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCaptionFontStyleChanged(FontStyle oldValue, FontStyle newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontStyle>(oldValue, newValue)
            {
                RoutedEvent = CaptionFontStyleChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnCaptionFontWeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCaptionFontWeightChanged((FontWeight)e.OldValue, (FontWeight)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CaptionFontWeightChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCaptionFontWeightChanged(FontWeight oldValue, FontWeight newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontWeight>(oldValue, newValue)
            {
                RoutedEvent = CaptionFontWeightChangedEvent
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

        private static void OnCaptionFontFamilyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCaptionFontFamilyChanged((FontFamily)e.OldValue, (FontFamily)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CaptionFontFamilyChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCaptionFontFamilyChanged(FontFamily oldValue, FontFamily newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontFamily>(oldValue, newValue)
            {
                RoutedEvent = CaptionFontFamilyChangedEvent
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

        private static void OnCaptionFontStretchChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCaptionFontStretchChanged((FontStretch)e.OldValue, (FontStretch)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CaptionFontStretchChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCaptionFontStretchChanged(FontStretch oldValue, FontStretch newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<FontStretch>(oldValue, newValue)
            {
                RoutedEvent = CaptionFontStretchChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnYAxisTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnYAxisTextChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="YAxisTextChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnYAxisTextChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = YAxisTextChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnXAxisTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnXAxisTextChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="XAxisTextChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnXAxisTextChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = XAxisTextChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnCaptionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnCaptionChanged((string)e.OldValue, (string)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="CaptionChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnCaptionChanged(string oldValue, string newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<string>(oldValue, newValue)
            {
                RoutedEvent = CaptionChangedEvent
            };
            RaiseEvent(e);
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

        private static void OnHorizontalLinesCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnHorizontalLinesCountChanged((int)e.OldValue, (int)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="HorizontalLinesCountChanged"/> event is raised on Chart
        /// </summary>
        /// <param name="oldValue">Old y-axis stops count</param>
        /// <param name="newValue">New y-axis stops count</param>
        protected virtual void OnHorizontalLinesCountChanged(int oldValue, int newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue)
            {
                RoutedEvent = HorizontalLinesCountChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceHorizontalLinesCount(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : (int)value <= 0 ? 10 : value;
        }

        private static void OnVerticalLinesCountChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnVerticalLinesCountChanged((int)e.OldValue, (int)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="VerticalLinesCountChanged"/> event is raised on Chart
        /// </summary>
        /// <param name="oldValue">Old x-axis stops count</param>
        /// <param name="newValue">New x-axis stops count</param>
        protected virtual void OnVerticalLinesCountChanged(int oldValue, int newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<int>(oldValue, newValue)
            {
                RoutedEvent = VerticalLinesCountChangedEvent
            };
            RaiseEvent(e);
        }

        private static object CoerceVerticalLinesCount(DependencyObject d, object value)
        {
            return !(d is Chart) ? value : (int)value <= 0 ? 10 : value;
        }
        #endregion

        #region Routed events
        /// <summary>
        /// Occurs when the <see cref="ItemsSource"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<IEnumerable<ISeries>> ItemsSourceChanged
        {
            add { AddHandler(ItemsSourceChangedEvent, value); }
            remove { RemoveHandler(ItemsSourceChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ItemsSourceChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ItemsSourceChangedEvent = EventManager.RegisterRoutedEvent("ItemsSourceChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<ObservableCollection<ISeries>>), typeof(Chart));

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
        public event RoutedPropertyChangedEventHandler<bool> ShowTicksChanged
        {
            add { AddHandler(ShowTicksChangedEvent, value); }
            remove { RemoveHandler(ShowTicksChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ShowTicksChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ShowTicksChangedEvent = EventManager.RegisterRoutedEvent("ShowTicksChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(Chart));

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
        /// Occurs when the <see cref="CaptionFontStretch"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontStretch> CaptionFontStretchChanged
        {
            add { AddHandler(CaptionFontStretchChangedEvent, value); }
            remove { RemoveHandler(CaptionFontStretchChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionFontStretchChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CaptionFontStretchChangedEvent = EventManager.RegisterRoutedEvent("CaptionFontStretchChanged",
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
        /// Occurs when the <see cref="CustomYAxisValues"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<IEnumerable<string>> CustomYAxisValuesChanged
        {
            add { AddHandler(CustomYAxisValuesChangedEvent, value); }
            remove { RemoveHandler(CustomYAxisValuesChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CustomYAxisValuesChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CustomYAxisValuesChangedEvent = EventManager.RegisterRoutedEvent("CustomYAxisValuesChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<IEnumerable<string>>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="CustomXAxisValues"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<IEnumerable<string>> CustomXAxisValuesChanged
        {
            add { AddHandler(CustomXAxisValuesChangedEvent, value); }
            remove { RemoveHandler(CustomXAxisValuesChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CustomXAxisValuesChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CustomXAxisValuesChangedEvent = EventManager.RegisterRoutedEvent("CustomXAxisValuesChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<IEnumerable<string>>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="AxesValuesFormat"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> AxesValuesFormatChanged
        {
            add { AddHandler(AxesValuesFormatChangedEvent, value); }
            remove { RemoveHandler(AxesValuesFormatChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesValuesFormatChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesValuesFormatChangedEvent = EventManager.RegisterRoutedEvent("AxesValuesFormatChanged",
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
        /// Occurs when the <see cref="CaptionFontSize"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<double> CaptionFontSizeChanged
        {
            add { AddHandler(CaptionFontSizeChangedEvent, value); }
            remove { RemoveHandler(CaptionFontSizeChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionFontSizeChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CaptionFontSizeChangedEvent = EventManager.RegisterRoutedEvent("CaptionFontSizeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<double>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="CaptionFontStyle"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontStyle> CaptionFontStyleChanged
        {
            add { AddHandler(CaptionFontStyleChangedEvent, value); }
            remove { RemoveHandler(CaptionFontStyleChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionFontStyleChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CaptionFontStyleChangedEvent = EventManager.RegisterRoutedEvent("CaptionFontStyleChanged",
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
        /// Occurs when the <see cref="CaptionFontWeight"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontWeight> CaptionFontWeightChanged
        {
            add { AddHandler(CaptionFontWeightChangedEvent, value); }
            remove { RemoveHandler(CaptionFontWeightChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionFontWeightChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CaptionFontWeightChangedEvent = EventManager.RegisterRoutedEvent("CaptionFontWeightChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<FontWeight>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="CaptionFontFamily"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<FontFamily> CaptionFontFamilyChanged
        {
            add { AddHandler(CaptionFontFamilyChangedEvent, value); }
            remove { RemoveHandler(CaptionFontFamilyChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionFontFamilyChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CaptionFontFamilyChangedEvent = EventManager.RegisterRoutedEvent("CaptionFontFamilyChanged",
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
        /// Occurs when the <see cref="YAxisText"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> YAxisTextChanged
        {
            add { AddHandler(YAxisTextChangedEvent, value); }
            remove { RemoveHandler(YAxisTextChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="YAxisTextChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent YAxisTextChangedEvent = EventManager.RegisterRoutedEvent("YAxisTextChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="XAxisText"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> XAxisTextChanged
        {
            add { AddHandler(XAxisTextChangedEvent, value); }
            remove { RemoveHandler(XAxisTextChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="XAxisTextChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent XAxisTextChangedEvent = EventManager.RegisterRoutedEvent("XAxisTextChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<string>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="Caption"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<string> CaptionChanged
        {
            add { AddHandler(CaptionChangedEvent, value); }
            remove { RemoveHandler(CaptionChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="CaptionChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent CaptionChangedEvent = EventManager.RegisterRoutedEvent("CaptionChanged",
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
        /// Occurs when the <see cref="HorizontalLinesCount"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> HorizontalLinesCountChanged
        {
            add { AddHandler(HorizontalLinesCountChangedEvent, value); }
            remove { RemoveHandler(HorizontalLinesCountChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="HorizontalLinesCountChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent HorizontalLinesCountChangedEvent = EventManager.RegisterRoutedEvent("HorizontalLinesCountChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<int>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="VerticalLinesCount"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<int> VerticalLinesCountChanged
        {
            add { AddHandler(VerticalLinesCountChangedEvent, value); }
            remove { RemoveHandler(VerticalLinesCountChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="VerticalLinesCountChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent VerticalLinesCountChangedEvent = EventManager.RegisterRoutedEvent("VerticalLinesCountChanged",
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
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    /// <summary>
    /// Represents event args for <see cref="Chart.ChartPointLeftButtonDoubleClick"/> routed event
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
        /// Initialzes a new instance of ChartPointLeftButtonDoubleClickEventArgs
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
