using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothWinUI3;


internal static class OxyPlotUtilities
{
    public static PlotModel MakeOxyPlotModelSimple(string title, int step, int range, string axisTitle, string propertyName)
    {
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
                new LineSeries // CHANGE:
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
        return retval;
    }
    public static List<OxyColor> PreferredPlotColors = new List<OxyColor>()
    {
        OxyColors.DarkBlue, OxyColors.LightBlue, OxyColors.Violet, OxyColors.Black, OxyColors.Gray,
    };

    /// <summary>
    /// Stackable way to add additional lines to the oxyplot. Returns the PlotModel, so you can 
    /// </summary>
    public static PlotModel AddLine (this PlotModel retval, int step, int range, string axisTitle, string propertyName, double minimum = double.NaN, AxisPosition position = AxisPosition.Left)
    {
        var tier = retval.NInPosition(position);
        var colorIndex = (retval.Series.Count + 1) % PreferredPlotColors.Count;
        var axis = new LinearAxis()
        {
            Position = position,
            PositionTier = tier, // PositionTier=0 is the innermost tier. //DOC:
            //MajorGridlineColor = OxyColors.Black, // Not set for additional lines. Only the first axis gets a grid!
            //MajorGridlineStyle = LineStyle.Solid,
            //MajorGridlineThickness = 1,
            //MajorStep = step, // 1 hpa
            //MinimumRange = range,
            Title = axisTitle,
            Key = propertyName,
        };
        if (!double.IsNaN(minimum))
        {
            axis.Minimum = minimum;
        }
        var series = new LineSeries
        {
            Title = axisTitle,
            Color = PreferredPlotColors[colorIndex],
            StrokeThickness = 0.75,
            MarkerType = MarkerType.None,
            DataFieldX = "TimestampMostRecentDT",
            DataFieldY = propertyName,
            YAxisKey = axisTitle,
        };

        retval.Axes.Add(axis);
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
}
