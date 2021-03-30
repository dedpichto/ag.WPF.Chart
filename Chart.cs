﻿// This program is free software; you can redistribute it and/or modify
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using Path = System.Windows.Shapes.Path;

namespace ag.WPF.Chart
{
    /// <summary>
    /// Specifies chart style
    /// </summary>
    public enum ChartStyle
    {
        /// <summary>
        /// Style represented by straight lines
        /// </summary>
        Lines,
        /// <summary>
        /// Style represented by straight stacked lines
        /// </summary>
        StackedLines,
        /// <summary>
        /// Style represented by straight 100% stacked lines
        /// </summary>
        FullStackedLines,
        /// <summary>
        /// Style represented by smooth lines
        /// </summary>
        SmoothLines,
        /// <summary>
        /// Style represented by smooth stacked lines
        /// </summary>
        SmoothStackedLines,
        /// <summary>
        /// Style represented by smooth 100% stacked lines
        /// </summary>
        SmoothFullStackedLines,
        /// <summary>
        /// Style represented by straight lines with markers at control ponts
        /// </summary>
        LinesWithMarkers,
        /// <summary>
        /// Style represented by straight stacked lines with markers at control ponts
        /// </summary>
        StackedLinesWithMarkers,
        /// <summary>
        /// Style represented by straight 100% stacked lines with markers at control ponts
        /// </summary>
        FullStackedLinesWithMarkers,
        /// <summary>
        /// Style represented by smooth lines with markers at control ponts
        /// </summary>
        SmoothLinesWithMarkers,
        /// <summary>
        /// Style represented by smooth stacked lines with markers at control ponts
        /// </summary>
        SmoothStackedLinesWithMarkers,
        /// <summary>
        /// Style represented by smooth 100% stacked lines with markers at control ponts
        /// </summary>
        SmoothFullStackedLinesWithMarkers,
        /// <summary>
        /// Style represented by columns
        /// </summary>
        Columns,
        /// <summary>
        /// Style represented by stacked columns
        /// </summary>
        StackedColumns,
        /// <summary>
        /// Style represented by 100% stacked columns
        /// </summary>
        FullStackedColumns,
        /// <summary>
        /// Style represented by bars
        /// </summary>
        Bars,
        /// <summary>
        /// Style represented by stacked bars
        /// </summary>
        StackedBars,
        /// <summary>
        /// Style represented by 100% stacked bars
        /// </summary>
        FullStackedBars,
        /// <summary>
        /// Style represented by areas
        /// </summary>
        Area,
        /// <summary>
        /// Style represented by stacked areas
        /// </summary>
        StackedArea,
        /// <summary>
        /// Style represented by 100% stacked areas
        /// </summary>
        FullStackedArea,
        /// <summary>
        /// Style represented by smooth areas
        /// </summary>
        SmoothArea,
        /// <summary>
        /// Style represented by bubbles
        /// </summary>
        Bubbles,
        /// <summary>
        /// Style represented by solid sectors
        /// </summary>
        SolidPie,
        /// <summary>
        /// Style represented by sectors divided with thin lines
        /// </summary>
        SlicedPie,
        /// <summary>
        /// Style represented by solid arcs
        /// </summary>
        Doughnut,
        /// <summary>
        /// Style represented by cumulated effect of positive and negative values
        /// </summary>
        Waterfall,
        /// <summary>
        /// Style represented by radar
        /// </summary>
        Radar,
        /// <summary>
        /// Style represented by radar with markers
        /// </summary>
        RadarWithMarkers,
        /// <summary>
        /// Style represented by radar areas
        /// </summary>
        RadarArea
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
    public enum AxesValuesVisibility
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
    public enum LegendShape
    {
        /// <summary>
        /// Rectanglular shape
        /// </summary>
        Rectangle,
        /// <summary>
        /// Circular shape
        /// </summary>
        Circle
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

    /// <summary>
    /// Represents custom control for creating simple 2-D charts
    /// </summary>
    #region Named parts
    [TemplatePart(Name = ElementCanvas, Type = typeof(Canvas))]
    [TemplatePart(Name = ElementPieImage, Type = typeof(Image))]
    #endregion

    public class Chart : Control, INotifyPropertyChanged
    {
        #region Constants
        private const string ElementCanvas = "PART_Canvas";
        private const string ElementPieImage = "PART_PieImage";
        #endregion

        #region Elements
        private Canvas _canvas;
        private Image _pieImage;
        #endregion

        #region Dependency properties
        /// <summary>
        /// The identifier of the <see cref="SectionsX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SectionsXProperty;
        /// <summary>
        /// The identifier of the <see cref="SectionsY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SectionsYProperty;
        /// <summary>
        /// The identifier of the <see cref="ShowSecondaryXLines"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowSecondaryXLinesProperty;
        /// <summary>
        /// The identifier of the <see cref="ShowSecondaryYLines"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowSecondaryYLinesProperty;
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
        /// The identifier of the <see cref="ChartStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartStyleProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesValuesVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesValuesVisibilityProperty;
        /// <summary>
        /// The identifier of the <see cref="AxesValuesFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxesValuesFormatProperty;
        /// <summary>
        /// The identifier of the <see cref="XAxisCustomValues"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty XAxisCustomValuesProperty;
        /// <summary>
        /// The identifier of the <see cref="YAxisCustomValues"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty YAxisCustomValuesProperty;
        /// <summary>
        /// The identifier of the <see cref="ChartOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ChartOpacityProperty;
        /// <summary>
        /// The identifier of the <see cref="DisabledBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledBrushProperty;
        /// <summary>
        /// The identifier of the <see cref="MaxX"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxXProperty;
        /// <summary>
        /// The identifier of the <see cref="MaxY"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MaxYProperty;
        /// <summary>
        /// The identifier of the <see cref="AutoAdjust"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoAdjustProperty;
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
        /// The identifier of the <see cref="SeriesCollection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesCollectionProperty;
        /// <summary>
        /// The identifier of the <see cref="SeriesCountProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SeriesCountProperty;
        /// <summary>
        /// The identifier of the <see cref="WaterfallLegendsProperty"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WaterfallLegendsProperty;
        #endregion

        static Chart()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(Chart), new FrameworkPropertyMetadata(typeof(Chart)));

            SectionsXProperty = DependencyProperty.Register("SectionsX", typeof(int), typeof(Chart),
                new FrameworkPropertyMetadata(10, OnSectionsXChanged, CoerceSectionsX));
            SectionsYProperty = DependencyProperty.Register("SectionsY", typeof(int), typeof(Chart),
                new FrameworkPropertyMetadata(10, OnSectionsYChanged, CoerceSectionsY));
            ChartOpacityProperty = DependencyProperty.Register("ChartOpacity", typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(1.0, OnChartOpacityChanged, CoerceChartOpacity));
            ShowSecondaryXLinesProperty = DependencyProperty.Register("ShowSecondaryXLines", typeof(bool), typeof(Chart),
                new FrameworkPropertyMetadata(false, OnShowSecondaryXLinesChanged));
            ShowSecondaryYLinesProperty = DependencyProperty.Register("ShowSecondaryYLines", typeof(bool), typeof(Chart),
                new FrameworkPropertyMetadata(false, OnShowSecondaryYLinesChanged));
            CaptionProperty = DependencyProperty.Register("Caption", typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("", OnCaptionChanged));
            XAxisTextProperty = DependencyProperty.Register("XAxisText", typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("x-Axis", OnXAxisTextChanged));
            YAxisTextProperty = DependencyProperty.Register("YAxisText", typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("y-Axis", OnYAxisTextChanged));
            AxesFontFamilyProperty = DependencyProperty.Register("AxesFontFamily", typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontFamily, OnAxesFontFamilyChanged));
            AxesFontWeightProperty = DependencyProperty.Register("AxesFontWeight", typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontWeight, OnAxesFontWeightChanged));
            AxesFontSizeProperty = DependencyProperty.Register("AxesFontSize", typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontSize, OnAxesFontSizeChanged));
            AxesFontStyleProperty = DependencyProperty.Register("AxesFontStyle", typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontStyle, OnAxesFontStyleChanged));
            AxesFontStretchProperty = DependencyProperty.Register("AxesFontStretch", typeof(FontStretch), typeof(Chart),
                new FrameworkPropertyMetadata(FontStretches.Normal, OnAxesFontStretchChanged));
            CaptionFontFamilyProperty = DependencyProperty.Register("CaptionFontFamily", typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontFamily, OnCaptionFontFamilyChanged));
            LegendFontFamilyProperty = DependencyProperty.Register("LegendFontFamily", typeof(FontFamily), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontFamily, OnLegendFontFamilyChanged));
            CaptionFontWeightProperty = DependencyProperty.Register("CaptionFontWeight", typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontWeight, OnCaptionFontWeightChanged));
            LegendFontWeightProperty = DependencyProperty.Register("LegendFontWeight", typeof(FontWeight), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontWeight, OnLegendFontWeightChanged));
            CaptionFontStyleProperty = DependencyProperty.Register("CaptionFontStyle", typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontStyle, OnCaptionFontStyleChanged));
            LegendFontStyleProperty = DependencyProperty.Register("LegendFontStyle", typeof(FontStyle), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontStyle, OnLegendFontStyleChanged));
            CaptionFontSizeProperty = DependencyProperty.Register("CaptionFontSize", typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.CaptionFontSize, OnCaptionFontSizeChanged));
            CaptionFontStretchProperty = DependencyProperty.Register("CaptionFontStretch", typeof(FontStretch), typeof(Chart),
                new FrameworkPropertyMetadata(FontStretches.Normal, OnCaptionFontStretchChanged));
            LegendFontSizeProperty = DependencyProperty.Register("LegendFontSize", typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(SystemFonts.StatusFontSize, OnLegendFontSizeChanged));
            LegendAlignmentProperty = DependencyProperty.Register("LegendAlignment", typeof(LegendAlignment), typeof(Chart),
                new FrameworkPropertyMetadata(LegendAlignment.Bottom, OnLegendAlignmentChanged));
            ShowLegendProperty = DependencyProperty.Register("ShowLegend", typeof(bool), typeof(Chart),
                new FrameworkPropertyMetadata(true, OnShowLegendChanged));
            ChartStyleProperty = DependencyProperty.Register("ChartStyle", typeof(ChartStyle), typeof(Chart),
                new FrameworkPropertyMetadata(ChartStyle.Lines, OnChartStyleChanged));
            AxesValuesVisibilityProperty = DependencyProperty.Register("AxesValuesVisibility", typeof(AxesValuesVisibility), typeof(Chart),
                new FrameworkPropertyMetadata(AxesValuesVisibility.None, OnAxesValuesVisibilityChanged));
            AxesValuesFormatProperty = DependencyProperty.Register("AxesValuesFormat", typeof(string), typeof(Chart),
                new FrameworkPropertyMetadata("0", OnAxesValuesFormatChanged));
            XAxisCustomValuesProperty = DependencyProperty.Register("XAxisCustomValues", typeof(List<string>), typeof(Chart),
                new FrameworkPropertyMetadata(new List<string>(), OnXAxisCustomValuesChanged));
            YAxisCustomValuesProperty = DependencyProperty.Register("YAxisCustomValues", typeof(List<string>), typeof(Chart),
                new FrameworkPropertyMetadata(new List<string>(), OnYAxisCustomValuesChanged));
            DisabledBrushProperty = DependencyProperty.Register("DisabledBrush", typeof(Brush), typeof(Chart),
                new FrameworkPropertyMetadata(null));
            MaxXProperty = DependencyProperty.Register("MaxX", typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(100.0, OnMaxXChanged, CoerceMaxX));
            MaxYProperty = DependencyProperty.Register("MaxY", typeof(double), typeof(Chart),
                new FrameworkPropertyMetadata(100.0, OnMaxYChanged, CoerceMaxY));
            AutoAdjustProperty = DependencyProperty.Register("AutoAdjust", typeof(bool), typeof(Chart),
                new FrameworkPropertyMetadata(true, OnAutoAdjustChanged));
            LegendSizeProperty = DependencyProperty.Register("LegendSize", typeof(LegendSize), typeof(Chart),
                new FrameworkPropertyMetadata(LegendSize.ExtraSmall, OnLegendSizeChanged));
            LegendShapeProperty = DependencyProperty.Register("LegendShape", typeof(LegendShape), typeof(Chart),
                new FrameworkPropertyMetadata(LegendShape.Rectangle, OnLegendShapeChanged));
            ShowTicksProperty = DependencyProperty.Register("ShowTicks",
                typeof(bool), typeof(Chart), new FrameworkPropertyMetadata(true, OnShowTicksChanged));
            ChartBoundaryProperty = DependencyProperty.Register("ChartBoundary", typeof(ChartBoundary), typeof(Chart),
                new FrameworkPropertyMetadata(ChartBoundary.WithOffset, OnChartBoundaryChanged));
            SeriesCollectionProperty = DependencyProperty.Register("SeriesCollection", typeof(ObservableCollection<Series>), typeof(Chart),
                new FrameworkPropertyMetadata(new ObservableCollection<Series>(), OnSeriesCollectionChanged));
            SeriesCountProperty = DependencyProperty.Register("SeriesCount", typeof(int), typeof(Chart), new FrameworkPropertyMetadata(0));
            WaterfallLegendsProperty = DependencyProperty.RegisterAttached("WaterfallLegends", typeof(string[]), typeof(Chart), new FrameworkPropertyMetadata(new[] { "Increase", "Decrease" }));
        }

        private bool _Initialized;
        private readonly ObservableCollection<FrameworkElement> _LegendCollection = new ObservableCollection<FrameworkElement>();
        private readonly ObservableCollection<FrameworkElement> _PieLegends = new ObservableCollection<FrameworkElement>();

        #region Overrides

        /// <summary>When overridden in a derived class, is invoked whenever application code or internal processes call <see cref="M:System.Windows.FrameworkElement.ApplyTemplate" />.</summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
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

            if (_Initialized) return;
            SeriesCollection.CollectionChanged += SeriesCollection_CollectionChanged;
            _Initialized = true;
        }

        #endregion

        #region Private event handlers
        private void PieImage_MouseMove(object sender, MouseEventArgs e)
        {
            if (!SeriesCollection.Any()) return;
            if (!(_pieImage.ToolTip is ToolTip tooltip)) return;
            var position = e.GetPosition(_pieImage);
            if (_pieImage.Source is DrawingImage dw)
            {
                if (dw.Drawing is DrawingGroup dwg)
                {
                    foreach (var gd in dwg.Children.OfType<GeometryDrawing>())
                    {
                        if (!(gd.Geometry is PathGeometry geometry)) continue;
                        if (!geometry.FillContains(position)) continue;
                        var data = (string)geometry.GetValue(Series.SectorDataProperty);
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
            tooltip.Content = SeriesCollection.First().Name;
        }

        private void SeriesCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                    {
                        if (!(e.NewItems[0] is Series series)) break;

                        series.Index = e.NewStartingIndex;
                        series.PropertyChanged += Series_PropertyChanged;
                        //s.Values.CollectionChanged += Values_CollectionChanged;

                        if (series.MainBrush == null)
                        {
                            series.MainBrush = series.Index < Series.PredefinedPieBrushes.Length
                                ? Series.PredefinedPieBrushes[series.Index]
                                : Series.PredefinedPieBrushes[series.Index % Series.PredefinedPieBrushes.Length];
                        }

                        if (series.SecondaryBrush == null)
                        {
                            series.SecondaryBrush = series.Index + 1 < Series.PredefinedPieBrushes.Length
                                ? Series.PredefinedPieBrushes[series.Index + 1]
                                : Series.PredefinedPieBrushes[(series.Index + 1) % Series.PredefinedPieBrushes.Length];
                        }

                        #region Main series path
                        var ptsBinding = new MultiBinding { Converter = new ValuesToPathConverter() };
                        ptsBinding.Bindings.Add(new Binding("ActualWidth") { ElementName = ElementCanvas });
                        ptsBinding.Bindings.Add(new Binding("ActualHeight") { ElementName = ElementCanvas });
                        ptsBinding.Bindings.Add(new Binding("SeriesCollection")
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
                        ptsBinding.Bindings.Add(new Binding("AutoAdjust")
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
                        ptsBinding.Bindings.Add(new Binding("XAxisCustomValues")
                        {
                            Source = this
                        });
                        ptsBinding.Bindings.Add(new Binding("SectionsY")
                        {
                            Source = this
                        });
                        ptsBinding.Bindings.Add(new Binding("SectionsX")
                        {
                            Source = this
                        });

                        ptsBinding.NotifyOnSourceUpdated = true;
                        series.Path.SetBinding(Path.DataProperty, ptsBinding);
                        #endregion

                        #region Positive waterfall path
                        var positiveWaterfallBinding = new MultiBinding { Converter = new ValuesToWaterfallConverter() };
                        positiveWaterfallBinding.Bindings.Add(new Binding("ActualWidth") { ElementName = ElementCanvas });
                        positiveWaterfallBinding.Bindings.Add(new Binding("ActualHeight") { ElementName = ElementCanvas });
                        positiveWaterfallBinding.Bindings.Add(new Binding("SeriesCollection")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("Index")
                        {
                            Source = series
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("AutoAdjust")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("MaxX")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("MaxY")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("AxesFontFamily")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("AxesFontSize")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("AxesFontStyle")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("AxesFontWeight")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("AxesFontStretch")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("XAxisCustomValues")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("SectionsY")
                        {
                            Source = this
                        });
                        positiveWaterfallBinding.Bindings.Add(new Binding("SectionsX")
                        {
                            Source = this
                        });

                        positiveWaterfallBinding.ConverterParameter = true;
                        positiveWaterfallBinding.NotifyOnSourceUpdated = true;
                        series.PositivePath.SetBinding(Path.DataProperty, positiveWaterfallBinding);
                        #endregion

                        #region Negative waterfall path
                        var negativeWaterfallBinding = new MultiBinding { Converter = new ValuesToWaterfallConverter() };
                        negativeWaterfallBinding.Bindings.Add(new Binding("ActualWidth") { ElementName = ElementCanvas });
                        negativeWaterfallBinding.Bindings.Add(new Binding("ActualHeight") { ElementName = ElementCanvas });
                        negativeWaterfallBinding.Bindings.Add(new Binding("SeriesCollection")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("Index")
                        {
                            Source = series
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("AutoAdjust")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("MaxX")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("MaxY")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("AxesFontFamily")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("AxesFontSize")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("AxesFontStyle")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("AxesFontWeight")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("AxesFontStretch")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("XAxisCustomValues")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("SectionsY")
                        {
                            Source = this
                        });
                        negativeWaterfallBinding.Bindings.Add(new Binding("SectionsX")
                        {
                            Source = this
                        });

                        negativeWaterfallBinding.ConverterParameter = false;
                        negativeWaterfallBinding.NotifyOnSourceUpdated = true;
                        series.NegativePath.SetBinding(Path.DataProperty, negativeWaterfallBinding);
                        #endregion

                        #region Main series stroke
                        var strkBinding = new MultiBinding { Converter = new PathStrokeConverter() };
                        strkBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        strkBinding.Bindings.Add(new Binding("MainBrush")
                        {
                            Source = series
                        });
                        strkBinding.Bindings.Add(new Binding("DisabledBrush")
                        {
                            Source = this
                        });
                        strkBinding.Bindings.Add(new Binding("IsEnabled")
                        {
                            Source = this
                        });
                        strkBinding.NotifyOnSourceUpdated = true;
                        series.Path.SetBinding(Shape.StrokeProperty, strkBinding);
                        #endregion

                        #region Main series fill
                        var fillBinding = new MultiBinding { Converter = new PathFillConverter() };
                        fillBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        fillBinding.Bindings.Add(new Binding("MainBrush")
                        {
                            Source = series
                        });
                        fillBinding.Bindings.Add(new Binding("DisabledBrush")
                        {
                            Source = this
                        });
                        fillBinding.Bindings.Add(new Binding("IsEnabled")
                        {
                            Source = this
                        });
                        fillBinding.NotifyOnSourceUpdated = true;
                        series.Path.SetBinding(Shape.FillProperty, fillBinding);
                        #endregion

                        #region Positive waterfall fill
                        var fillPositiveBinding = new MultiBinding { Converter = new PathFillConverter() };
                        fillPositiveBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        fillPositiveBinding.Bindings.Add(new Binding("MainBrush")
                        {
                            Source = series
                        });
                        fillPositiveBinding.Bindings.Add(new Binding("DisabledBrush")
                        {
                            Source = this
                        });
                        fillPositiveBinding.Bindings.Add(new Binding("IsEnabled")
                        {
                            Source = this
                        });
                        fillPositiveBinding.NotifyOnSourceUpdated = true;
                        series.PositivePath.SetBinding(Shape.FillProperty, fillPositiveBinding);
                        #endregion

                        #region Negative waterfall fill
                        var fillNegativeBinding = new MultiBinding { Converter = new PathFillConverter() };
                        fillNegativeBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        fillNegativeBinding.Bindings.Add(new Binding("SecondaryBrush")
                        {
                            Source = series
                        });
                        fillNegativeBinding.Bindings.Add(new Binding("DisabledBrush")
                        {
                            Source = this
                        });
                        fillNegativeBinding.Bindings.Add(new Binding("IsEnabled")
                        {
                            Source = this
                        });
                        fillNegativeBinding.NotifyOnSourceUpdated = true;
                        series.NegativePath.SetBinding(Shape.FillProperty, fillNegativeBinding);
                        #endregion

                        #region Common opacities
                        series.Path.SetBinding(OpacityProperty, new Binding("ChartOpacity") { Source = this });
                        series.PositivePath.SetBinding(OpacityProperty, new Binding("ChartOpacity") { Source = this });
                        series.NegativePath.SetBinding(OpacityProperty, new Binding("ChartOpacity") { Source = this });
                        #endregion

                        #region Common events
                        series.Path.MouseLeftButtonDown += Path_MouseLeftButtonDown;
                        series.Path.MouseMove += Path_MouseMove;
                        series.PieBrushes.BrushChanged += PieBrushes_BrushChanged;

                        series.PositivePath.MouseLeftButtonDown += Path_MouseLeftButtonDown;
                        series.PositivePath.MouseMove += Path_MouseMove;

                        series.NegativePath.MouseLeftButtonDown += Path_MouseLeftButtonDown;
                        series.NegativePath.MouseMove += Path_MouseMove;
                        #endregion

                        #region Main legend
                        var legend = new Legend() { Index = series.Index };

                        var legendBackgroundBinding = new MultiBinding { Converter = new PathBrushConverter() };
                        legendBackgroundBinding.Bindings.Add(new Binding("IsEnabled")
                        {
                            Source = this
                        });
                        legendBackgroundBinding.Bindings.Add(new Binding("MainBrush")
                        {
                            Source = series
                        });
                        legendBackgroundBinding.Bindings.Add(new Binding("DisabledBrush")
                        {
                            Source = this
                        });
                        legendBackgroundBinding.NotifyOnSourceUpdated = true;
                        legend.SetBinding(Legend.LegendBackgroundProperty, legendBackgroundBinding);

                        var legendVisibilityBinding = new MultiBinding { Converter = new LegendVisibilityConverter(), ConverterParameter = true };
                        legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        legendVisibilityBinding.Bindings.Add(new Binding("Index")
                        {
                            Source = series
                        });
                        legendVisibilityBinding.NotifyOnSourceUpdated = true;
                        legend.SetBinding(Legend.VisibilityProperty, legendVisibilityBinding);

                        legend.SetBinding(Legend.TextProperty, new Binding("Name") { Source = series });

                        _LegendCollection.Add(legend);
                        #endregion

                        #region Positive waterfall legend
                        legend = new Legend() { Index = series.Index };
                        legendBackgroundBinding = new MultiBinding { Converter = new PathBrushConverter() };
                        legendBackgroundBinding.Bindings.Add(new Binding("IsEnabled")
                        {
                            Source = this
                        });
                        legendBackgroundBinding.Bindings.Add(new Binding("MainBrush")
                        {
                            Source = series
                        });
                        legendBackgroundBinding.Bindings.Add(new Binding("DisabledBrush")
                        {
                            Source = this
                        });
                        legendBackgroundBinding.NotifyOnSourceUpdated = true;
                        legend.SetBinding(Legend.LegendBackgroundProperty, legendBackgroundBinding);

                        legendVisibilityBinding = new MultiBinding { Converter = new LegendVisibilityConverter(), ConverterParameter = false };
                        legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        legendVisibilityBinding.Bindings.Add(new Binding("Index")
                        {
                            Source = series
                        });
                        legendVisibilityBinding.NotifyOnSourceUpdated = true;
                        legend.SetBinding(Legend.VisibilityProperty, legendVisibilityBinding);

                        legend.SetBinding(Legend.TextProperty, new Binding("WaterfallLegends[0]") { Source = this });

                        _LegendCollection.Add(legend);
                        #endregion

                        #region Negative waterfall legend
                        legend = new Legend() { Index = series.Index };
                        legendBackgroundBinding = new MultiBinding { Converter = new PathBrushConverter() };
                        legendBackgroundBinding.Bindings.Add(new Binding("IsEnabled")
                        {
                            Source = this
                        });
                        legendBackgroundBinding.Bindings.Add(new Binding("SecondaryBrush")
                        {
                            Source = series
                        });
                        legendBackgroundBinding.Bindings.Add(new Binding("DisabledBrush")
                        {
                            Source = this
                        });
                        legendBackgroundBinding.NotifyOnSourceUpdated = true;
                        legend.SetBinding(Legend.LegendBackgroundProperty, legendBackgroundBinding);

                        legendVisibilityBinding = new MultiBinding { Converter = new LegendVisibilityConverter(), ConverterParameter = false };
                        legendVisibilityBinding.Bindings.Add(new Binding("ChartStyle")
                        {
                            Source = this
                        });
                        legendVisibilityBinding.Bindings.Add(new Binding("Index")
                        {
                            Source = series
                        });
                        legendVisibilityBinding.NotifyOnSourceUpdated = true;
                        legend.SetBinding(Legend.VisibilityProperty, legendVisibilityBinding);

                        legend.SetBinding(Legend.TextProperty, new Binding("WaterfallLegends[1]") { Source = this });

                        _LegendCollection.Add(legend);
                        #endregion

                        _canvas.Children.Add(series.Path);
                        _canvas.Children.Add(series.PositivePath);
                        _canvas.Children.Add(series.NegativePath);

                        rebuildPieLegends(series.Values, series);

                        SeriesCount++;
                        break;
                    }
                case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                    {
                        if (!(e.OldItems[0] is Series series)) break;

                        series.PropertyChanged -= Series_PropertyChanged;
                        //s.Values.CollectionChanged -= Values_CollectionChanged;
                        series.Path.MouseLeftButtonDown -= Path_MouseLeftButtonDown;
                        series.Path.MouseMove -= Path_MouseMove;
                        series.PieBrushes.BrushChanged -= PieBrushes_BrushChanged;

                        var index = _canvas.Children.IndexOf(series.Path);
                        if (index > -1)
                            _canvas.Children.RemoveAt(index);

                        _LegendCollection.RemoveAt(e.OldStartingIndex);
                        for (var i = _LegendCollection.Count - 1; i >= 0; i--)
                        {
                            if (((Legend)_LegendCollection[i]).Index == e.OldStartingIndex)
                                _LegendCollection.RemoveAt(i);
                        }
                        foreach (Legend lg in _LegendCollection.Where(l => ((Legend)l).Index > e.OldStartingIndex))
                        {
                            lg.Index--;
                        }
                        foreach (var sr in SeriesCollection.Where(sc => sc.Index > e.OldStartingIndex))
                        {
                            sr.Index--;
                            var b = BindingOperations.GetMultiBindingExpression(sr.Path, Path.DataProperty);
                            if (b != null)
                                b.UpdateTarget();
                            rebuildPieLegends(sr.Values, sr);
                        }

                        SeriesCount--;
                        break;
                    }
            }
            //OnPropertyChanged("SeriesCollection");
        }

        private void Series_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Values":
                    if (sender is Series series)
                    {
                        foreach (var sr in SeriesCollection.Where(s => s.Index != series.Index))
                        {
                            var b = BindingOperations.GetMultiBindingExpression(sr.Values.Path, Path.DataProperty);
                            if (b != null)
                                b.UpdateTarget();
                        }
                        rebuildPieLegends(series.Values, series);
                    }
                    break;
            }
            OnPropertyChanged("SeriesCollection");
        }

        private void PieBrushes_BrushChanged(object sender, BrushChangedEventArgs e)
        {
            if (e.Series == null || e.Index >= e.Series.PieBrushes.Length()) return;
            var legend = _PieLegends[e.Index];
            BindingOperations.ClearBinding(legend, Legend.LegendBackgroundProperty);
            var legendBackgroundBinding = new MultiBinding
            {
                Converter = new PieLegendBrushConverter(),
                ConverterParameter = e.Series.PieBrushes[e.Index]
            };
            legendBackgroundBinding.Bindings.Add(new Binding("IsEnabled")
            {
                Source = this
            });
            legendBackgroundBinding.Bindings.Add(new Binding("DisabledBrush")
            {
                Source = this
            });
            legendBackgroundBinding.NotifyOnSourceUpdated = true;
            legend.SetBinding(Legend.LegendBackgroundProperty, legendBackgroundBinding);
        }

        private void Path_MouseMove(object sender, MouseEventArgs e)
        {
            if (!(sender is Path path)) return;
            if (!(path.Tag is Series s)) return;
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
                    rc = s.RealRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
                    if (rc != default)
                    {
                        var index = s.RealRects.IndexOf(rc);
                        if (s.Values.Count <= index)
                        {
                            tooltip.Content = s.Name;
                            break;
                        }
                        tooltip.Content = s.Values[index].CustomValue != null
                            ? s.Values[index].CustomValue + " " + s.Values[index].Value.V1
                            : s.Name + " " + s.Values[index].Value.V1.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        tooltip.Content = s.Name;
                    }
                    break;
                case ChartStyle.Bars:
                case ChartStyle.StackedBars:
                case ChartStyle.FullStackedBars:
                    rc = s.RealRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
                    if (rc != default)
                    {
                        var index = s.RealRects.IndexOf(rc);
                        if (s.Values.Count <= index)
                        {
                            tooltip.Content = s.Name;
                            break;
                        }
                        tooltip.Content = s.Values[index].CustomValue != null
                            ? s.Values[index].CustomValue + " " + s.Values[index].Value.V1
                            : s.Name + " " + s.Values[index].Value.V1.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        tooltip.Content = s.Name;
                    }
                    break;
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
                        tooltip.Content = s.Values[index].CustomValue != null
                            ? s.Values[index].CustomValue + " " + s.Values[index].Value.V1
                            : s.Name + " " + s.Values[index].Value.V1.ToString(CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        tooltip.Content = s.Name;
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
            if (!(path.Tag is Series s)) return;
            if (!ChartStyle.In(ChartStyle.LinesWithMarkers, ChartStyle.StackedLinesWithMarkers,
                ChartStyle.FullStackedLinesWithMarkers, ChartStyle.SmoothLinesWithMarkers,
                ChartStyle.SmoothStackedLinesWithMarkers, ChartStyle.SmoothFullStackedLinesWithMarkers,
                ChartStyle.Bubbles, ChartStyle.Columns, ChartStyle.StackedColumns, ChartStyle.FullStackedColumns,
                ChartStyle.Bars, ChartStyle.StackedBars, ChartStyle.FullStackedBars) || e.ClickCount != 2) return;
            var rc = s.RealRects.FirstOrDefault(r => r.Contains(e.GetPosition(_canvas)));
            if (rc == default) return;
            var index = s.RealRects.IndexOf(rc);
            if (s.Values.Count <= index)
            {
                return;
            }
            RaiseMarkerLeftButtonDoubleClickEvent(s.Values[index], s);
        }

        //private void Values_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    var chartValues = sender as ChartValues;
        //    if (chartValues == null) return;
        //    var b = BindingOperations.GetMultiBindingExpression(chartValues.Path, Path.DataProperty);
        //    if (b != null)
        //        b.UpdateTarget();

        //    var series = chartValues.Path.Tag as Series;
        //    if (series == null) return;
        //    foreach (var sr in SeriesCollection.Where(s => s.Index != series.Index))
        //    {
        //        b = BindingOperations.GetMultiBindingExpression(sr.Values.Path, Path.DataProperty);
        //        if (b != null)
        //            b.UpdateTarget();
        //    }
        //    rebuildPieLegends(chartValues, series);
        //    OnPropertyChanged("SeriesCollection");
        //}
        #endregion

        #region Private functions

        private void rebuildPieLegends(ChartValues values, Series series)
        {
            if (series.Index > 0) return;
            _PieLegends.Clear();
            for (int i = 0, brushIndex = 0; i < values.Count; i++)
            {
                var v = values[i];
                if (brushIndex == series.PieBrushes.Length()) brushIndex = 0;
                var legend = new Legend();

                var legendBackgroundBinding = new MultiBinding
                {
                    Converter = new PieLegendBrushConverter(),
                    ConverterParameter = series.PieBrushes[brushIndex++]
                };
                legendBackgroundBinding.Bindings.Add(new Binding("IsEnabled")
                {
                    Source = this
                });
                legendBackgroundBinding.Bindings.Add(new Binding("DisabledBrush")
                {
                    Source = this
                });
                legendBackgroundBinding.NotifyOnSourceUpdated = true;
                legend.SetBinding(Legend.LegendBackgroundProperty, legendBackgroundBinding);

                var textBinding = new MultiBinding { Converter = new PieSectionTextConverter(), ConverterParameter = v };
                textBinding.Bindings.Add(new Binding("Values") { Source = series });
                textBinding.Bindings.Add(new Binding("AxesValuesFormat") { Source = this });
                legend.SetBinding(Legend.TextProperty, textBinding);

                _PieLegends.Add(legend);
            }
        }

        //private Tuple<double, double> getScalingFactors()
        //{
        //    var presentationSource = PresentationSource.FromVisual(Application.Current.MainWindow);
        //    if (presentationSource == null || presentationSource.CompositionTarget == null)
        //        return Tuple.Create(1.0, 1.0);
        //    var m = presentationSource.CompositionTarget.TransformToDevice;
        //    return Tuple.Create(m.M11, m.M22);// notice it's divided by 96 already
        //}
        #endregion

        #region Public properties
        /// <summary>
        /// Gets collection of <see cref="Legend"/> objects associated with chart series
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public ObservableCollection<FrameworkElement> LegendCollection
        {
            get { return _LegendCollection; }
        }
        /// <summary>
        /// Gets collection of <see cref="Legend"/> objects associated with chart series when chart style is set to <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/>
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public ObservableCollection<FrameworkElement> PieLegends
        {
            get { return _PieLegends; }
        }

        #endregion

        #region Dependency properties wrappers
        public string[] WaterfallLegends
        {
            get { return (string[])GetValue(WaterfallLegendsProperty); }
            set { SetValue(WaterfallLegendsProperty, value); }
        }
        /// <summary>
        /// Help property forced the control to repaintt
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
        public int SeriesCount
        {
            get { return (int)GetValue(SeriesCountProperty); }
            set { SetValue(SeriesCountProperty, value); }
        }
        /// <summary>
        /// Gets collection of <see cref="Series"/> objects
        /// </summary>
        [Category("ChartMisc"), Description("Collection of chart series")]
        public ObservableCollection<Series> SeriesCollection
        {
            get { return (ObservableCollection<Series>)GetValue(SeriesCollectionProperty); }
            set { SetValue(SeriesCollectionProperty, value); }
        }
        /// <summary>
        /// Specifies whether chart boundary is started on y-axes or with offfset from y-axes
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to any non-line style</remarks>
        [Category("ChartAppearance"), Description("Specifies whether chart boundary is started on y-axes or with offfset from y-axes")]
        public ChartBoundary ChartBoundary
        {
            get { return (ChartBoundary)GetValue(ChartBoundaryProperty); }
            set { SetValue(ChartBoundaryProperty, value); }
        }
        /// <summary>
        /// Specifies whether ticks are drawn on axes
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/></remarks>
        [Category("ChartAppearance"), Description("Specifies whether ticks are drawn on axes")]
        public bool ShowTicks
        {
            get { return (bool)GetValue(ShowTicksProperty); }
            set { SetValue(ShowTicksProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend font family
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets legend font family")]
        public FontFamily LegendFontFamily
        {
            get { return (FontFamily)GetValue(LegendFontFamilyProperty); }
            set { SetValue(LegendFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend font wweight
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets legend font wweight")]
        public FontWeight LegendFontWeight
        {
            get { return (FontWeight)GetValue(LegendFontWeightProperty); }
            set { SetValue(LegendFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend font style
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets legend font style")]
        public FontStyle LegendFontStyle
        {
            get { return (FontStyle)GetValue(LegendFontStyleProperty); }
            set { SetValue(LegendFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets or sets legend font size
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets legend font size")]
        public double LegendFontSize
        {
            get { return (double)GetValue(LegendFontSizeProperty); }
            set { SetValue(LegendFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the shape of legend. Can be one of <see cref="ag.WPF.Chart.LegendShape"/> enumeration members
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the shape of legend. Can be one of LegendShape enumeration members")]
        public LegendShape LegendShape
        {
            get { return (LegendShape)GetValue(LegendShapeProperty); }
            set { SetValue(LegendShapeProperty, value); }
        }
        /// <summary>
        /// Gets or sets the size of legend. Can be one of <see cref="ag.WPF.Chart.LegendSize"/> enumeration members
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets the size of legend. Can be one of LegendSize enumeration members")]
        public LegendSize LegendSize
        {
            get { return (LegendSize)GetValue(LegendSizeProperty); }
            set { SetValue(LegendSizeProperty, value); }
        }
        /// <summary>
        /// Specifies whether control will automatically adjust its max x- and y- values or they should be set explicitly
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/></remarks>
        [Category("ChartMisc"), Description("Specifies whether control will automatically adjust its max x- and y- values or they should be set explicitly")]
        public bool AutoAdjust
        {
            get { return (bool)GetValue(AutoAdjustProperty); }
            set { SetValue(AutoAdjustProperty, value); }
        }
        /// <summary>
        /// Gets or sets max numeric value for y-axis. The default value is 100
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="AutoAdjust"/> property is set to true</remarks>
        [Category("ChartMisc"), Description("Gets or sets max numeric value for y-axis. The default value is 100")]
        public double MaxY
        {
            get { return (double)GetValue(MaxYProperty); }
            set { SetValue(MaxYProperty, value); }
        }
        /// <summary>
        /// Gets or sets max numeric value for x-axis. The default value is 100
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="AutoAdjust"/> property is set to true</remarks>
        [Category("ChartMisc"), Description("Gets or sets max numeric value for x-axis. The default value is 100")]
        public double MaxX
        {
            get { return (double)GetValue(MaxXProperty); }
            set { SetValue(MaxXProperty, value); }
        }
        /// <summary>
        /// Gets or set chart opacity in range of 0.0 (fully transparent) to 1.0 (fully opaque)
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or set chart opacity in range of 0.0 (fully transparent) to 1.0 (fully opaque)")]
        public double ChartOpacity
        {
            get { return (double)GetValue(ChartOpacityProperty); }
            set { SetValue(ChartOpacityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the brush used for drawing the chart when control is disabled
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the brush used for drawing the chart when control is disabled")]
        public Brush DisabledBrush
        {
            get { return (Brush)GetValue(DisabledBrushProperty); }
            set { SetValue(DisabledBrushProperty, value); }
        }
        /// <summary>
        /// Gets or sets the sequence of values to be drawn next to x-axis instead of numeric values
        /// </summary>
        [Category("ChartMisc"), Description("Gets or sets the sequence of values to be drawn next to x-axis instead of numeric values")]
        public List<string> XAxisCustomValues
        {
            get { return (List<string>)GetValue(XAxisCustomValuesProperty); }
            set { SetValue(XAxisCustomValuesProperty, value); }
        }
        /// <summary>
        /// Gets or sets the sequence of values to be drawn next to y-axis instead of numeric values
        /// </summary>
        [Category("ChartMisc"), Description("Gets or sets the sequence of values to be drawn next to y-axis instead of numeric values")]
        public List<string> YAxisCustomValues
        {
            get { return (List<string>)GetValue(YAxisCustomValuesProperty); }
            set { SetValue(YAxisCustomValuesProperty, value); }
        }
        /// <summary>
        /// Gets or sets the visibility state of x- and y- axes numeric/custom values. Can be one of <see cref="ag.WPF.Chart.AxesValuesVisibility"/> enumeration members
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/></remarks>
        [Category("ChartAppearance"), Description("Gets or sets the visibility state of x- and y- axes numeric/custom values. Can be one of AxesValuesVisibility enumeration members")]
        public AxesValuesVisibility AxesValuesVisibility
        {
            get { return (AxesValuesVisibility)GetValue(AxesValuesVisibilityProperty); }
            set { SetValue(AxesValuesVisibilityProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart style. Can be one of <see cref="ag.WPF.Chart.ChartStyle"/> enumeration members
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets the chart style. Can be one of ChartStyle enumeration members")]
        public ChartStyle ChartStyle
        {
            get { return (ChartStyle)GetValue(ChartStyleProperty); }
            set { SetValue(ChartStyleProperty, value); }
        }
        /// <summary>
        /// Specifies whether chart legend should be shown
        /// </summary>
        [Category("ChartLegend"), Description("Specifies whether chart legend should be shown")]
        public bool ShowLegend
        {
            get { return (bool)GetValue(ShowLegendProperty); }
            set { SetValue(ShowLegendProperty, value); }
        }
        /// <summary>
        /// Gets or sets chart legend alignment. Can be one of <see cref="ag.WPF.Chart.LegendAlignment"/> enumeration members
        /// </summary>
        [Category("ChartLegend"), Description("Gets or sets chart legend alignment. Can be one of LegendAlignment enumeration members")]
        public LegendAlignment LegendAlignment
        {
            get { return (LegendAlignment)GetValue(LegendAlignmentProperty); }
            set { SetValue(LegendAlignmentProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font size of chart caption
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font size of chart caption")]
        public double CaptionFontSize
        {
            get { return (double)GetValue(CaptionFontSizeProperty); }
            set { SetValue(CaptionFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font style of chart caption
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font style of chart caption")]
        public FontStyle CaptionFontStyle
        {
            get { return (FontStyle)GetValue(CaptionFontStyleProperty); }
            set { SetValue(CaptionFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font weight of chart caption
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font weight of chart caption")]
        public FontWeight CaptionFontWeight
        {
            get { return (FontWeight)GetValue(CaptionFontWeightProperty); }
            set { SetValue(CaptionFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font family of chart caption
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font family of chart caption")]
        public FontFamily CaptionFontFamily
        {
            get { return (FontFamily)GetValue(CaptionFontFamilyProperty); }
            set { SetValue(CaptionFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font stretch of chart caption
        /// </summary>
        [Category("ChartCaption"), Description("Gets of sets the font stretch of chart caption")]
        public FontStretch CaptionFontStretch
        {
            get { return (FontStretch)GetValue(CaptionFontStretchProperty); }
            set { SetValue(CaptionFontStretchProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font family of chart axes
        /// </summary>
        [Category("ChartAppearance"), Description("Gets of sets the font family of chart axes")]
        public FontFamily AxesFontFamily
        {
            get { return (FontFamily)GetValue(AxesFontFamilyProperty); }
            set { SetValue(AxesFontFamilyProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font weight of chart axes
        /// </summary>
        [Category("ChartAppearance"), Description("Gets of sets the font weight of chart axes")]
        public FontWeight AxesFontWeight
        {
            get { return (FontWeight)GetValue(AxesFontWeightProperty); }
            set { SetValue(AxesFontWeightProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font size of chart axes
        /// </summary>
        [Category("ChartAppearance"), Description("Gets of sets the font size of chart axes")]
        public double AxesFontSize
        {
            get { return (double)GetValue(AxesFontSizeProperty); }
            set { SetValue(AxesFontSizeProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font style of chart axes
        /// </summary>
        [Category("ChartAppearance"), Description("Gets of sets the font style of chart axes")]
        public FontStyle AxesFontStyle
        {
            get { return (FontStyle)GetValue(AxesFontStyleProperty); }
            set { SetValue(AxesFontStyleProperty, value); }
        }
        /// <summary>
        /// Gets of sets the font stretch of chart axes
        /// </summary>
        [Category("ChartAppearance"), Description("Gets of sets the font stretch of chart axes")]
        public FontStretch AxesFontStretch
        {
            get { return (FontStretch)GetValue(AxesFontStretchProperty); }
            set { SetValue(AxesFontStretchProperty, value); }
        }
        /// <summary>
        /// Gets or sets the text which appears on the top/bottom of y-axis
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/></remarks>
        [Category("ChartAppearance")]
        public string YAxisText
        {
            get { return (string)GetValue(YAxisTextProperty); }
            set { SetValue(YAxisTextProperty, value); }
        }
        /// <summary>
        /// Gets or sets the text which appears on the right/left of x-axis
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/></remarks>
        [Category("ChartAppearance")]
        public string XAxisText
        {
            get { return (string)GetValue(XAxisTextProperty); }
            set { SetValue(XAxisTextProperty, value); }
        }
        /// <summary>
        /// Gets or sets the chart caption
        /// </summary>
        [Category("ChartCaption"), Description("Gets or sets the chart caption")]
        public string Caption
        {
            get { return (string)GetValue(CaptionProperty); }
            set { SetValue(CaptionProperty, value); }
        }
        /// <summary>
        /// Specifies whether secondary Y-axis dotted lines should be drawn on chart
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/></remarks>
        [Category("ChartAppearance"), Description("Specifies whether secondary Y-axis dotted lines should be drawn on chart")]
        public bool ShowSecondaryYLines
        {
            get { return (bool)GetValue(ShowSecondaryYLinesProperty); }
            set { SetValue(ShowSecondaryYLinesProperty, value); }
        }
        /// <summary>
        /// Specifies whether secondary X-axis dotted lines should be drawn on chart
        /// </summary>
        /// <remarks>This property will have no effect if <see cref="ChartStyle"/> property is set to <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/></remarks>
        [Category("ChartAppearance"), Description("Specifies whether secondary X-axis dotted lines should be drawn on chart")]
        public bool ShowSecondaryXLines
        {
            get { return (bool)GetValue(ShowSecondaryXLinesProperty); }
            set { SetValue(ShowSecondaryXLinesProperty, value); }
        }
        /// <summary>
        /// Gets or sets amount of sections on x-axis
        /// </summary>
        /// <remarks>
        /// This property has no effect if <see cref="ChartStyle"/> is set to <see cref="ag.WPF.Chart.ChartStyle.Columns"/>, <see cref="ag.WPF.Chart.ChartStyle.StackedColumns"/>, <see cref="ag.WPF.Chart.ChartStyle.FullStackedColumns"/>, 
        /// <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/>
        /// </remarks>
        [Category("ChartMisc"), Description("Gets or sets amount of sections on x-axis")]
        public int SectionsX
        {
            get { return (int)GetValue(SectionsXProperty); }
            set { SetValue(SectionsXProperty, value); }
        }
        /// <summary>
        /// Gets or sets amount of sections on y-axis
        /// </summary>
        /// <remarks>
        /// This property has no effect if <see cref="ChartStyle"/> is set to <see cref="ag.WPF.Chart.ChartStyle.Bars"/>, <see cref="ag.WPF.Chart.ChartStyle.StackedBars"/>, <see cref="ag.WPF.Chart.ChartStyle.FullStackedBars"/>, 
        /// <see cref="ag.WPF.Chart.ChartStyle.SolidPie"/> or <see cref="ag.WPF.Chart.ChartStyle.SlicedPie"/> or <see cref="ag.WPF.Chart.ChartStyle.Doughnut"/>
        /// </remarks>
        [Category("ChartMisc"), Description("Gets or sets amount of sections on y-axis")]
        public int SectionsY
        {
            get { return (int)GetValue(SectionsYProperty); }
            set { SetValue(SectionsYProperty, value); }
        }
        /// <summary>
        /// Gets or sets format for numeric values drawn next to x- and/or y- axes
        /// </summary>
        [Category("ChartAppearance"), Description("Gets or sets format for numeric values drawn next to x- and/or y- axes")]
        public string AxesValuesFormat
        {
            get { return (string)GetValue(AxesValuesFormatProperty); }
            set { SetValue(AxesValuesFormatProperty, value); }
        }

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
        private static void OnSeriesCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            //ch.SeriesCollection.CollectionChanged -= ch.SeriesCollection_CollectionChanged;
            //ch.SeriesCollection.CollectionChanged += ch.SeriesCollection_CollectionChanged;
            //ch.OnPropertyChanged("SeriesCollection");
            ch.OnSeriesCollectionChanged((ObservableCollection<Series>)e.OldValue, (ObservableCollection<Series>)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="SeriesCollectionChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnSeriesCollectionChanged(ObservableCollection<Series> oldValue, ObservableCollection<Series> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<ObservableCollection<Series>>(oldValue, newValue)
            {
                RoutedEvent = SeriesCollectionChangedEvent
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
            ch.OnLegendShapeChanged((LegendShape)e.OldValue, (LegendShape)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="LegendShapeChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnLegendShapeChanged(LegendShape oldValue, LegendShape newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<LegendShape>(oldValue, newValue)
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

        private static void OnAutoAdjustChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnAutoAdjustChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AutoAdjustChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAutoAdjustChanged(bool oldValue, bool newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = AutoAdjustChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnXAxisCustomValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnXAxisCustomValuesChanged((List<string>)e.OldValue, (List<string>)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="XAxisCustomValuesChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnXAxisCustomValuesChanged(List<string> oldValue, List<string> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<List<string>>(oldValue, newValue)
            {
                RoutedEvent = XAxisCustomValuesChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnYAxisCustomValuesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnYAxisCustomValuesChanged((List<string>)e.OldValue, (List<string>)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="YAxisCustomValuesChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnYAxisCustomValuesChanged(List<string> oldValue, List<string> newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<List<string>>(oldValue, newValue)
            {
                RoutedEvent = YAxisCustomValuesChangedEvent
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
            ch.OnAxesValuesVisibilityChanged((AxesValuesVisibility)e.OldValue, (AxesValuesVisibility)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="AxesValuesVisibilityChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnAxesValuesVisibilityChanged(AxesValuesVisibility oldValue, AxesValuesVisibility newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<AxesValuesVisibility>(oldValue, newValue)
            {
                RoutedEvent = AxesValuesVisibilityChangedEvent
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

        private static void OnShowSecondaryYLinesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnShowSecondaryYLinesChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ShowSecondaryYLinesChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnShowSecondaryYLinesChanged(bool oldValue, bool newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = ShowSecondaryYLinesChangedEvent
            };
            RaiseEvent(e);
        }

        private static void OnShowSecondaryXLinesChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (!(sender is Chart ch)) return;
            ch.OnShowSecondaryXLinesChanged((bool)e.OldValue, (bool)e.NewValue);
        }
        /// <summary>
        /// Invoked just before the <see cref="ShowSecondaryXLinesChangedEvent"/> event is raised on control
        /// </summary>
        /// <param name="oldValue">Old value</param>
        /// <param name="newValue">New value</param>
        protected void OnShowSecondaryXLinesChanged(bool oldValue, bool newValue)
        {
            var e = new RoutedPropertyChangedEventArgs<bool>(oldValue, newValue)
            {
                RoutedEvent = ShowSecondaryXLinesChangedEvent
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
        /// Occurs when the <see cref="SeriesCollection"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<ObservableCollection<Series>> SeriesCollectionChanged
        {
            add { AddHandler(SeriesCollectionChangedEvent, value); }
            remove { RemoveHandler(SeriesCollectionChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="SeriesCollectionChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent SeriesCollectionChangedEvent = EventManager.RegisterRoutedEvent("SeriesCollectionChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<ObservableCollection<Series>>), typeof(Chart));

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
        public event RoutedPropertyChangedEventHandler<LegendShape> LegendShapeChanged
        {
            add { AddHandler(LegendShapeChangedEvent, value); }
            remove { RemoveHandler(LegendShapeChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="LegendShapeChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent LegendShapeChangedEvent = EventManager.RegisterRoutedEvent("LegendShapeChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<LegendShape>), typeof(Chart));

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
        /// Occurs when the <see cref="YAxisCustomValues"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<List<string>> YAxisCustomValuesChanged
        {
            add { AddHandler(YAxisCustomValuesChangedEvent, value); }
            remove { RemoveHandler(YAxisCustomValuesChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="YAxisCustomValuesChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent YAxisCustomValuesChangedEvent = EventManager.RegisterRoutedEvent("YAxisCustomValuesChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<List<string>>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="XAxisCustomValues"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<List<string>> XAxisCustomValuesChanged
        {
            add { AddHandler(XAxisCustomValuesChangedEvent, value); }
            remove { RemoveHandler(XAxisCustomValuesChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="XAxisCustomValuesChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent XAxisCustomValuesChangedEvent = EventManager.RegisterRoutedEvent("XAxisCustomValuesChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<List<string>>), typeof(Chart));

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
        public event RoutedPropertyChangedEventHandler<AxesValuesVisibility> AxesValuesVisibilityChanged
        {
            add { AddHandler(AxesValuesVisibilityChangedEvent, value); }
            remove { RemoveHandler(AxesValuesVisibilityChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AxesValuesVisibilityChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AxesValuesVisibilityChangedEvent = EventManager.RegisterRoutedEvent("AxesValuesVisibilityChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<AxesValuesVisibility>), typeof(Chart));

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
        /// Occurs when the <see cref="AutoAdjust"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> AutoAdjustChanged
        {
            add { AddHandler(AutoAdjustChangedEvent, value); }
            remove { RemoveHandler(AutoAdjustChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="AutoAdjustChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent AutoAdjustChangedEvent = EventManager.RegisterRoutedEvent("AutoAdjustChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(Chart));

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
        /// Occurs when the <see cref="ShowSecondaryYLines"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> ShowSecondaryYLinesChanged
        {
            add { AddHandler(ShowSecondaryYLinesChangedEvent, value); }
            remove { RemoveHandler(ShowSecondaryYLinesChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ShowSecondaryYLinesChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ShowSecondaryYLinesChangedEvent = EventManager.RegisterRoutedEvent("ShowSecondaryYLinesChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(Chart));

        /// <summary>
        /// Occurs when the <see cref="ShowSecondaryXLines"/> property has been changed in some way
        /// </summary>
        public event RoutedPropertyChangedEventHandler<bool> ShowSecondaryXLinesChanged
        {
            add { AddHandler(ShowSecondaryXLinesChangedEvent, value); }
            remove { RemoveHandler(ShowSecondaryXLinesChangedEvent, value); }
        }
        /// <summary>
        /// Identifies the <see cref="ShowSecondaryXLinesChanged"/> routed event
        /// </summary>
        public static readonly RoutedEvent ShowSecondaryXLinesChangedEvent = EventManager.RegisterRoutedEvent("ShowSecondaryXLinesChanged",
            RoutingStrategy.Bubble, typeof(RoutedPropertyChangedEventHandler<bool>), typeof(Chart));

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

        private void RaiseMarkerLeftButtonDoubleClickEvent(ChartValue point, Series series)
        {
            var e = new ChartPointLeftButtonDoubleClickEventArgs(ChartPointLeftButtonDoubleClickEvent, point, series);
            RaiseEvent(e);
        }
        #endregion

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
    }

    /// <summary>
    /// Represents event args for <see cref="ag.WPF.Chart.Chart.ChartPointLeftButtonDoubleClick"/> routed event
    /// </summary>
    public class ChartPointLeftButtonDoubleClickEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// Gets current <see cref="ChartValue"/>
        /// </summary>
        public ChartValue Value { get; private set; }
        /// <summary>
        /// Gets current <see cref="Series"/>
        /// </summary>
        public Series Series { get; private set; }
        /// <summary>
        /// Initialzes a new instance of ChartPointLeftButtonDoubleClickEventArgs
        /// </summary>
        /// <param name="baseEvent">Base routed event</param>
        /// <param name="value">Current <see cref="ChartValue"/></param>
        /// <param name="series">Current <see cref="Series"/></param>
        public ChartPointLeftButtonDoubleClickEventArgs(RoutedEvent baseEvent, ChartValue value, Series series)
            : base(baseEvent)
        {
            Value = value;
            Series = series;
        }
    }
}
