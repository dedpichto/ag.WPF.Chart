// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.

using ag.WPF.Chart.Values;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ag.WPF.Chart
{
    public interface ISeries
    {
        int Index { get; set; }
        string Name { get; set; }
        Brush MainBrush { get; set; }
        Brush SecondaryBrush { get; set; }
        BrushesCollection PieBrushes { get; }
        ChartValues Values { get; }
        Path Path { get;  }
        Path PositivePath { get;  }
        Path NegativePath { get;  }
        List<Rect> RealRects { get; }
        List<Point> RealPoints { get; }

        event PropertyChangedEventHandler PropertyChanged;
    }
}