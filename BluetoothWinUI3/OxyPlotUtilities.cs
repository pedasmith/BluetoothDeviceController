using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3;


internal static class OxyPlotUtilities
{
    public static PlotModel MakeOxyPlotModel(string title)
    {
        PlotModel retval = new PlotModel
        {
            Title = title,
            PlotAreaBorderColor = OxyColors.Transparent,
            TextColor = OxyColors.Black,
            Axes =
            {
                new DateTimeAxis { Position = AxisPosition.Bottom },
            },
            Series =
            {
                new LineSeries
                    {
                        Color = OxyColor.FromAColor(0x80, OxyColors.Yellow),
                        StrokeThickness = 10,
                        MarkerType = MarkerType.None,
                        DataFieldX = "TimestampMostRecentDT", // All sensor data has a TimestampMostRecentDT
                    } 
            }
        };
        return retval;
    }

#if OLD_CODE_TO_BE_DELETED
    private static LineSeries MakeHighlightSeries()
    {
        var retval = new LineSeries
        {
            Color = OxyColor.FromAColor(0x80, OxyColors.Yellow),
            StrokeThickness = 10,
            MarkerType = MarkerType.None,
            DataFieldX = "TimestampMostRecentDT", // All sensor data has a TimestampMostRecentDT
        };
        return retval;
    }
#endif

    public static PlotModel MakeOxyPlotModelSimple(string title, int step, int range, string axisTitle, string propertyName)
    {
        PlotModel retval = MakeOxyPlotModel(title)
            .AddLine(step, range, axisTitle, propertyName);
#if ORIGINAL_OLD_CODE_TO_BE_DELETED
        PlotModel retval = new PlotModel
        {
            Title = title,
            PlotAreaBorderColor = OxyColors.Transparent,
            TextColor = OxyColors.Black,
            Axes =
            {
                new DateTimeAxis { Position = AxisPosition.Bottom },
                new LinearAxis
                {
                    Position = AxisPosition.Left,
                    PositionTier = 0, // PositionTier=0 is the innermost tier. //DOC:
                    MajorGridlineColor = OxyColors.Black,
                    MajorGridlineStyle = LineStyle.Solid,
                    MajorGridlineThickness = 1,
                    MajorStep = step, // Battery percentage run 0..100
                    MinimumRange= range, // Set this match your graphing needs
                    Title=axisTitle, // Set to something the user will recognize
                    Key=propertyName // Key has to match the YAxisKey in the Series
                },
            },
            Series =
            {
                MakeHighlightSeries(),
                new LineSeries
                {
                    Title = axisTitle,
                    Color = PreferredPlotColors[0], // OxyColors.DarkBlue
                    StrokeThickness = 0.75,
                    MarkerType = MarkerType.None,
                    DataFieldX = "TimestampMostRecentDT", // All sensor data has a TimestampMostRecentDT
                    DataFieldY = propertyName, // Must match the data in the sensor data class
                    YAxisKey= propertyName, // This key has to match the one in the Axis field.
                    // Suggestion is to set the YAxisKey to be the same as the DataFieldY
                },
            }
        };
#endif
        return retval;
    }

    public static List<OxyColor> PreferredPlotColors = new List<OxyColor>()
    {
        OxyColors.DarkBlue, OxyColors.DarkGreen, OxyColors.Violet, OxyColors.Black, OxyColors.Gray,
    };

    /// <summary>
    /// Stackable way to add additional lines to the oxyplot. 
    /// Returns the PlotModel, so you can do a MakeOxyPlotSimple().AddLine().AddLine()
    /// The step and range are only applied to the first line added
    /// </summary>
    public static PlotModel AddLine(this PlotModel retval, int step, int range, string lineTitle, string propertyName, double minimum = double.NaN, AxisPosition axisPosition = AxisPosition.Left, string axisKey = null, string axisTitle = null)
    {
        if (axisKey == null) axisKey = propertyName; // common case
        if (axisTitle == null) axisTitle = lineTitle; // common case

        var tier = retval.NInPosition(axisPosition);
        var colorIndex = (retval.Series.Count - 1) % PreferredPlotColors.Count;
        // subtract 1 because the first series is the highlight line.

        LinearAxis axis = null;
        bool hasAxis = false;
        foreach (var item in retval.Axes)
        {
            if (item.Key == axisKey)
            {
                hasAxis = true;
                break;
            }
        }
        if (!hasAxis)
        {
            axis = new LinearAxis()
            {
                Position = axisPosition,
                PositionTier = tier, // PositionTier=0 is the innermost tier. //DOC:
                Title = axisTitle,
                Key = axisKey,
            };
        }
        if (axis != null && retval.Axes.Count == 1) // Reminder: The first axis (index 0) is the X axis
        {
            axis.MajorGridlineColor = OxyColors.Black; // Not set for additional lines. Only the first axis gets a grid!
            axis.MajorGridlineStyle = LineStyle.Solid;
            axis.MajorGridlineThickness = 1;
            axis.MajorStep = step; // 1 hpa
            axis.MinimumRange = range;
        }
        if (axis != null && !double.IsNaN(minimum))
        {
            axis.Minimum = minimum;
        }
        var series = new LineSeries
        {
            Title = lineTitle,
            Tag = lineTitle,
            Color = PreferredPlotColors[colorIndex],
            StrokeThickness = 0.75,
            MarkerType = MarkerType.None,
            DataFieldX = "TimestampMostRecentDT",
            DataFieldY = propertyName,
            YAxisKey = axisKey,
        };

        if (axis != null)
        {
            retval.Axes.Add(axis);
        }
        retval.Series.Add(series);
        return retval;
    }

    private static int NInPosition(this PlotModel value, AxisPosition position)
    {
        int retval = 0;
        foreach (var item in value.Axes)
        {
            if (item.Position == position)
            {
                retval++;
            }
        }
        return retval;
    }

    private static int NLeft(this PlotModel value)
    {
        int retval = 0;
        foreach (var item in value.Axes)
        {
            if (item.Position == AxisPosition.Left)
            {
                retval++;
            }
        }
        return retval;
    }

    public static void InitializeLineNamesFromOxyPlotModel(List<string> dest, PlotModel oxyPlotModel)
    {
        foreach (var series in oxyPlotModel.Series)
        {
            var title = series.Title;
            if (!string.IsNullOrEmpty(title)) // Highlight line has no title
            {
                dest.Add(title);
            }
        }
    }


    /// <summary>
    /// Sets up the OxyModel Series. Reminder that each series is, e.g., "Temperature" or "Pressure"
    /// This is done in the control constructor. It can't be done at initialization time because of C#. 
    /// C# doesn't let me use a regular field when doing an initialization.
    /// </summary>
    public static void InitializeOxyPlotData(PlotView uiOxyPlot, PlotModel oxyPlotModel, System.Collections.IEnumerable data)
    {
        // Set up the OxyModel Series. Reminder that each series is, e.g., "Temperature" or "Pressure"
        // This can't be done at initialization time because of C#: it won't let me use a regular
        // field when doing an initialization.
        foreach (var series in oxyPlotModel.Series)
        {
            if (series is LineSeries lineSeries)
            {
                lineSeries.ItemsSource = data; //DOC:
            }
        }
        uiOxyPlot.Model = oxyPlotModel;

#if NEVER_EVER_DEFINED
        // There's seemingly no way to figure out if the user has clicked on a particular 
        // axis. Nuts!
        // Instead I'll do highlights via the menu system.

        //uiOxyPlot.Tapped += UiOxyPlot_Tapped;
        var controller = new PlotController();
        controller.BindMouseDown(OxyMouseButton.Left,
            new DelegateViewCommand<OxyMouseDownEventArgs>(OnMouseDown));
        uiOxyPlot.Controller = controller;
        //oxyPlotModel.MouseDown += OxyPlotModel_MouseDown;
#endif
    }

    private static void OnMouseDown(IView iview, IController controller, OxyMouseDownEventArgs e)
    {
        PlotView view = iview as PlotView;
        PlotModel model = view?.ActualModel;
        if (model == null) return;

        // I was going to use the ScreenRectangle that OxyPlot exposes, but
        // that only works for Wpf based version of OxyPlot.


#if NEVER_EVER_DEFINED
        // This code doesn't ever detect clicks on an axis
        HitTestArguments args = new HitTestArguments(e.Position, 10.0);
        var hits = model.HitTest(args);
        if (hits == null) return;
        foreach (var hit in hits)
        {
            if (hit.Element is Axis axis)
            {
                Log($"HIT: {axis.Title}");
            }
        }
#endif
    }

    private static void Log(string str)
    {
        System.Diagnostics.Debug.WriteLine(str);
        Console.WriteLine(str);
    }

    private static void UiOxyPlot_Tapped(object sender, Microsoft.UI.Xaml.Input.TappedRoutedEventArgs e)
    {
        var plot = (sender as PlotView);
        var model = plot?.Model;
        if (plot == null || model == null) return;
    }


    /// <summary>
    /// Set all of the axes to either visible or invisible. 
    /// </summary>
    public static void SetAxesVisibility(this PlotModel oxyPlotModel, PlotView uiOxyPlot, bool isVisible)
    {
        foreach (var axis in oxyPlotModel.Axes)
        {
            axis.IsAxisVisible = isVisible;
        }
        uiOxyPlot.InvalidatePlot(false); // false means just update for the axis
    }

    public static void DoHighlightGraphLine(this PlotModel oxyPlotModel, PlotView uiOxyPlot, string lineTag)
    {
        if (oxyPlotModel.Series.Count < 2) return; // it's not initialized yet

        var series = oxyPlotModel.Series[0] as LineSeries;
        bool clearHighlight = lineTag == "!CLEAR";

        if (clearHighlight)
        {
            // unhighlight it
            series.YAxisKey = null;
            series.DataFieldY = null;
            uiOxyPlot.InvalidatePlot(true); //DOC: Must be true to redraw the lines
        }
        else
        {
            LineSeries match = null;
            foreach (var item in oxyPlotModel.Series)
            {
                if (item.Tag as string == lineTag)
                {
                    match = item as LineSeries;
                }
            }
            if (match != null)
            {
                series.YAxisKey = match.YAxisKey;
                series.DataFieldY = match.DataFieldY;
                series.Selectable = false;
                series.ItemsSource = match.ItemsSource;
                uiOxyPlot.InvalidatePlot(true); //DOC: Must be true to redraw the lines
            }
        }
    }

}
