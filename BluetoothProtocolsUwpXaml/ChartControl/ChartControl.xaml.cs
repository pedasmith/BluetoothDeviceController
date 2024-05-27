using BluetoothDeviceController.Names;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.Charts
{
    public sealed partial class ChartControl : UserControl
    {
        public UISpecifications UISpec = new UISpecifications();
        public IList<string> Names = new List<string>();
        private bool MinMaxBeenInit { get; set; } = false;
        public double XMin { get; set; } = -1000.0;
        public double XMax { get; set; } = 1000.0;

        // Min and Max are just a little weird. Sorry about that :-)
        // Some devices the difference lines need a combined min/max. Other devices
        // need seperate min/max. For the first: accelerometers have x,y,z values
        // which are all +-2g (or whatever). For the second, a device with temperature,
        // pressure, and humidity, which are all different.
        // Default is that they are all the same.
        // The X values, on the other hand, are always combined.


        private List<bool> YMinMaxBeenInit = new List<bool>();
        private List<double> YMins = new List<double>();
        private List<double> YMaxs = new List<double>();
        private const double DEFAULT_YMIN = 0.0;
        private const double DEFAULT_YMAX = 100.0;


        public static Windows.UI.Color ConvertColor(string color)
        {
            foreach (var item in typeof(Windows.UI.Colors).GetProperties())
            {
                if (color == item.Name)
                {
                    return (Windows.UI.Color)item.GetValue(null, null);
                    ;
                }
            }
            return Windows.UI.Colors.Aquamarine;
        }
        public double GetYMin(int index) 
        {
            switch (UISpec?.chartYAxisCombined ?? UISpecifications.YMinMaxCombined.Combined)
            {
                case UISpecifications.YMinMaxCombined.Combined:
                    if (YMins.Count == 0) return DEFAULT_YMIN;
                    return YMins.Min();
                case UISpecifications.YMinMaxCombined.Separate:
                    return YMins[index];
            }
            return DEFAULT_YMIN;
        }
        public double GetYMax(int index) 
        {
            switch (UISpec?.chartYAxisCombined ?? UISpecifications.YMinMaxCombined.Combined)
            {
                case UISpecifications.YMinMaxCombined.Combined:
                    if (YMins.Count == 0) return DEFAULT_YMAX;
                    return YMaxs.Max();
                case UISpecifications.YMinMaxCombined.Separate:
                    return YMaxs[index];
            }
            return DEFAULT_YMAX; // Bigger than YMin
        }



        /// <summary>
        /// There's a gap in the C# Generics support. This ChartControl isn't a generic class; it's not a chart 
        /// for any particular data. That's because there's no way to do that. 
        /// Instead
        /// 1. The chart is set up with a list of PropertyInfo and saved as DataProperties
        /// 2. When the chart is updated, a DataCollection of T is passed in. The dataCollection is enumerated
        /// and the individual property values are pulled out thanks to the saved DataProperties
        /// The actual data is copied into a Data[] array which is an array of doubles and corresponds to the original data
        /// 
        /// When the line cursor is shown, the data is pulled from the DataCollection. This only works because the DataCollection
        /// is also an ISummarizeValues; it can take a ratio (0..1) and return a string summary of the data.
        /// </summary>
        private List<List<Point>> UnderlyingData = new List<List<Point>>();
        private List<Polyline> Lines = new List<Polyline>();
        private IList<PropertyInfo> DataProperties = null;
        private PropertyInfo TimeProperty = null;
        private DateTime StartTime;
        ISummarizeValue Values = null;

        public ChartControl()
        {
            this.InitializeComponent();
            this.Loaded += ChartControl_Loaded;
        }

        private void ChartControl_Loaded(object sender, RoutedEventArgs e)
        {
            this.uiStatus.Text = $"H={this.ActualHeight} W={this.ActualWidth}";
            uiBackgroundRect.Width = this.ActualWidth;
            uiBackgroundRect.Height = this.ActualHeight;
        }

        public void SetDataProperties(IList<PropertyInfo> dataProperties, PropertyInfo timeProperty, IList<string> names)
        {
            DataProperties = dataProperties;
            TimeProperty = timeProperty;
            Names = names;
        }

        public void SetTitle(string title)
        {
            uiTitle.Text = title;
        }

        private double X(double x)
        {
            var retval = (XMax == XMin) ? 0 : ((x - XMin) / (XMax - XMin)) * this.ActualWidth;
            return retval;
        }
        private double Y(int line, double y)
        {
            var ymin = UISpec?.ChartMinY(line, GetYMin(line)) ?? GetYMin(line);
            var ymax = UISpec?.ChartMaxY(line, GetYMax(line)) ?? GetYMax(line);
            var retval = (ymax==ymin) ? 0 : ((y - ymin) / (ymax - ymin)) * this.ActualHeight;
            retval = this.ActualHeight - retval;
            if (retval > this.ActualHeight)
            {
                ;
            }
            return retval;
        }

        Color[] DefaultLineColors = new Color[]
        {
            Colors.Black,
            Colors.DarkBlue,
            Colors.DarkGreen,
            Colors.DarkRed,
            Colors.DarkCyan,
            Colors.DarkGoldenrod,
            Colors.DarkMagenta,
        };
        Color GetDefaultLineColor(int i)
        {
            var index = (i % DefaultLineColors.Length);
            return DefaultLineColors[index];
        }


        private void EnsureLineExists (int lineIndex)
        {
            // Create both the set of XAML lines (which are polylines) and the data.
            while (Lines.Count <= lineIndex)
            {
                var newIndex = Lines.Count; // if 1 line already exists, then this new line will be index [2]
                var name = (Names != null && Names.Count > newIndex) ? Names[newIndex] : "";
                var lineDefault = UISpec.chartLineDefaults.ContainsKey(name) ? UISpec.chartLineDefaults[name] : null;
                var color = (lineDefault == null) ? GetDefaultLineColor(Lines.Count) : ConvertColor(lineDefault.stroke);
                var polyline = new Polyline()
                {
                    Fill = null,
                    Stroke = new SolidColorBrush(color),
                    StrokeThickness = 1.0,
                };
                Lines.Add(polyline);
                uiCanvas.Children.Add(polyline); // Actually add the polyline!
            }
            while (UnderlyingData.Count <= lineIndex)
            {
                UnderlyingData.Add(new List<Point>());
            }
            EnsureYExists(lineIndex);
        }

        private void EnsureYExists(int lineIndex)
        {
            while (YMins.Count <= lineIndex)
            {
                YMins.Add(DEFAULT_YMIN);
            }
            while (YMaxs.Count <= lineIndex)
            {
                YMaxs.Add(DEFAULT_YMAX);
            }
            while (YMinMaxBeenInit.Count <= lineIndex)
            {
                YMinMaxBeenInit.Add(false); // not initialized.
            }
        }

        private void RedrawAllLines()
        {
            for (int i=0; i<UnderlyingData.Count; i++)
            {
                var data = UnderlyingData[i];
                var line = Lines[i];
                line.Points.Clear();
                //System.Diagnostics.Debug.WriteLine($"\n\nDBG: REDRAW ALL LINES: npoints {data.Count}");
                foreach (var point in data)
                {
                    var x = X(point.X);
                    var y = Y(i, point.Y);
                    //System.Diagnostics.Debug.WriteLine($"    ({point.X},{point.Y}) ==> ({x},{y})");
                    line.Points.Add(new Point(x, y));
                }
            }
        }


        /// <summary>
        /// Uses DataProperties to pull data from the DataCollection<record> list
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void RedrawYTime<T>(DataCollection<T> list)
        {
            if (DataProperties == null) return;
            ResetMinMax(list);
            if (list.Count > 0)
            {
                for (int i = 0; i < DataProperties.Count; i++)
                {
                    // Act like everything is all different.
                    AddYTime<T>(i, AddResult.AddReplace, list, DataProperties[i], TimeProperty);
                }
            }
        }


        /// <summary>
        /// Most common entry point! Uses DataProperties to pull data from the DataCollection<record> list
        /// </summary>

        public void AddYTime<T>(AddResult addResult, DataCollection<T> list)
        {
            if (DataProperties == null) return;
            if (addResult == AddResult.AddReplace)
            {
                ResetMinMax(list);
            }
            for (int i=0; i<DataProperties.Count; i++)
            {
                // Each data point can have multiple values -- e.g., temp, pressure, humidity.
                // Add in each value, all at the same time.
                AddYTime<T>(i, addResult, list, DataProperties[i], TimeProperty);
            }
        }

        // TODO: adding new method for oscilloscope-type updates. here!here
        // Hey -- how is this really any different from RedrawLineYTime()???
        public void ZZZSetLineData<T>(DateTime startTime, int lineIndex, DataCollection<T> list)
        {
            if (lineIndex != 0)
            {
                throw new Exception ("NIY: Sorry, the oscilloscope stuff only works for single traces");
            }
            StartTime = startTime;
            var yProperty = DataProperties[lineIndex];
            var timeProperty = TimeProperty;

            UnderlyingData[lineIndex] = new List<Point>(list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                var record = list[i];
                var x = Convert.ToDateTime(timeProperty.GetValue(record));
                var y = Convert.ToDouble(yProperty.GetValue(record));
                double xtime = (x.Subtract(StartTime)).TotalSeconds;
                UnderlyingData[lineIndex].Add(new Point(xtime, y));
            }
            RedrawAllLines();
        }

        /// <summary>
        /// Internal routine to handle the messy cases of AddReplace versus AddSimple
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lineIndex"></param>
        /// <param name="addResult"></param>
        /// <param name="list"></param>
        /// <param name="yProperty"></param>
        /// <param name="timeProperty"></param>
        private void AddYTime<T>(int lineIndex, AddResult addResult, DataCollection<T> list, PropertyInfo yProperty, PropertyInfo timeProperty)
        {
            Values = list;
            if (list.Count == 0) return; // no data means nothing to do.
            switch (addResult)
            {
                case AddResult.AddReplace:
                    {
                        // Note: must call ResetMinMax if the data is really updated.
                        UnderlyingData[lineIndex] = new List<Point>(list.Count);
                        for (int i = 0; i < list.Count; i++)
                        {
                            var record = list[i];
                            var x = Convert.ToDateTime(timeProperty.GetValue(record));
                            var y = Convert.ToDouble(yProperty.GetValue(record));
                            double xtime = (x.Subtract(StartTime)).TotalSeconds;
                            UnderlyingData[lineIndex].Add(new Point(xtime, y));
                        }
                        RedrawAllLines();
                    }
                    break;
                case AddResult.AddSimple: // just add the most recent value
                    {
                        var record = list[list.Count() - 1];
                        var x = Convert.ToDateTime(timeProperty.GetValue(record));
                        var y = Convert.ToDouble(yProperty.GetValue(record));
                        AddXYPoint(lineIndex, x, y);
                    }
                    break;
                case AddResult.NotAdded:
                    // Collection wasn't changed; don't have to do anything.
                    break;
            }
        }

        /// <summary>
        /// Uses DataProperties to pull data from the DataCollection<record> list
        /// BUT requires that the DataProperties be in the weird line index / actual value order like for BBC microBit
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void RedrawLineYTime<T>(DataCollection<T> list)
        {
            if (DataProperties == null) return;
            ResetMinMaxLine(list);
            if (list.Count > 0)
            {
                // Act like everything is all different.
                AddLineYTime<T>(AddResult.AddReplace, list);
            }
            RedrawAllLines();
        }

        /// <summary>
        /// Uses DataProperties to pull data from the DataCollection<record> list
        /// BUT requires that the DataProperties be in the weird line index / actual value order like for BBC microBit
        /// TODO: delete, never used
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        public void ZZZRedrawLineYTimeOscilloscope<T>(DataCollection<T> list)
        {
            if (DataProperties == null) return;
            ResetMinMaxLine(list); //TODO: replace with correct item?
            if (list.Count > 0)
            {
                // Act like everything is all different.
                AddLineYTime<T>(AddResult.AddReplace, list);
            }
            RedrawAllLines();
        }
        /// <summary>
        /// Most common entry point! Uses DataProperties to pull data from the DataCollection<record> list
        /// My new version just for the bbc micro:bit. Let me just say that their 
        /// pin/value mechanism is a little hard to handle!
        /// 
        /// When addResult is AddSimple, just the last item is added.
        /// When addResult is AddReplace, the entire list is replaced.
        /// </summary>
        public void AddLineYTime<T>(AddResult addResult, DataCollection<T> list)
        {
            if (addResult == AddResult.AddReplace)
            {
                ResetMinMaxLine(list);
            }
            switch (addResult)
            {
                case AddResult.AddSimple:
                    {
                        var record = list[list.Count - 1];
                        AddLineYTimeSimple(record);
                    }
                    break;
                case AddResult.AddReplace:
                    // Wipe out the old lines
                    for (int lineIndex = 0; lineIndex < UnderlyingData.Count; lineIndex++)
                    {
                        UnderlyingData[lineIndex] = new List<Point>(list.Count);
                    }
                    // Add the data back in
                    foreach (var record in list)
                    {
                        AddLineYTimeSimple(record);
                    }
                    RedrawAllLines();

                    break;
                case AddResult.NotAdded:
                    ; // do nothing.
                    break;
            }
        }

        public void AddLineYTimeSimple<T>(T record)
        {
            if (DataProperties.Count % 2 != 0) return; // The data format is <line> <data> <line> <data> and must be divisible by 2.
            for (int j = 0; j < DataProperties.Count; j += 2)
            {
                var lineIndexProperty = DataProperties[j];
                var yProperty = DataProperties[j + 1];
                var lineIndex = Convert.ToInt32(lineIndexProperty.GetValue(record));
                if (lineIndex >= 0 && lineIndex <= MAX_LINE_INDEX) // Because the junk values are big-ish integers
                {
                    var x = Convert.ToDateTime(TimeProperty.GetValue(record));
                    var y = Convert.ToDouble(yProperty.GetValue(record));
                    AddXYPoint(lineIndex, x, y);
                }
            }
        }
        const int MAX_LINE_INDEX = 99;

        /// <summary>
        /// Uses the **Line** format where the data properties is line index / value
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        private void ResetMinMaxLine<T>(DataCollection<T> list)
        {
            XMin = double.MaxValue;
            XMax = double.MinValue;
            for (int i = 0; i < YMins.Count; i++)
            {
                YMins[i] = double.MaxValue;
                YMaxs[i] = double.MinValue;
            }
            for (int i = 0; i < list.Count; i++)
            {
                var record = list[i];
                for (int j=0; j<DataProperties.Count; j+=2)
                {
                    var lineIndexProperty = DataProperties[j];
                    var yProperty = DataProperties[j + 1];
                    var lineIndex = Convert.ToDouble(lineIndexProperty.GetValue(record));
                    if (lineIndex >= 0 && lineIndex <= MAX_LINE_INDEX)
                    {
                        EnsureLineExists((int)lineIndex);
                        var y = Convert.ToDouble(yProperty.GetValue(record));
                        YMins[j] = Math.Min(YMins[j], y);
                        YMaxs[j] = Math.Max(YMaxs[j], y);
                    }
                }
                var x = Convert.ToDateTime(TimeProperty.GetValue(record));
                if (i == 0)
                {
                    StartTime = x;
                }
                double xtime = (x.Subtract(StartTime)).TotalSeconds;
                XMin = Math.Min(XMin, xtime);
                XMax = Math.Max(XMax, xtime);
            }
        }

        /// <summary>
        /// Untemplated common routine to add data to the lines without reference to a DataRecord
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="MustRedraw"></param>
        private void AddXYPoint (int lineIndex, DateTime x, double y, bool MustRedraw = false)
        {
            if (!MinMaxBeenInit)
            {
                StartTime = x;
            }

            EnsureLineExists(lineIndex);
            var line = Lines[lineIndex];
            double xtime = (x.Subtract (StartTime)).TotalSeconds;
            UnderlyingData[lineIndex].Add(new Point(xtime, y));

            if (xtime < XMin || xtime > XMax || y < GetYMin(lineIndex) || y > GetYMax(lineIndex) || !MinMaxBeenInit)
            {
                if (!MinMaxBeenInit)
                {
                    MinMaxBeenInit = true;
                    StartTime = x;
                    XMin = xtime;
                    XMax = xtime;
                    YMins[lineIndex] = y;
                    YMaxs[lineIndex] = y;
                    YMinMaxBeenInit[lineIndex] = true;
                }
                else if (!YMinMaxBeenInit[lineIndex])
                {
                    YMins[lineIndex] = y;
                    YMaxs[lineIndex] = y;
                    YMinMaxBeenInit[lineIndex] = true;
                }
                else
                {
                    XMin = Math.Min(XMin, xtime);
                    XMax = Math.Max(XMax, xtime);
                    YMins[lineIndex] = Math.Min(YMins[lineIndex], y);
                    YMaxs[lineIndex] = Math.Max(YMaxs[lineIndex], y);

                    var xdelta = XMax - XMin;
                    if (xdelta < 5) XMax = XMin + 5; // At least 5 seconds of data at all times?
                }
                RedrawAllLines();
            }
            else
            {
                var point = new Point(X(xtime), Y(lineIndex, y));
                Lines[lineIndex].Points.Add(point);
                if (MustRedraw)
                {
                    RedrawAllLines();
                }
            }
        }

        /// <summary>
        /// Uses DataProperties to pull data from the DataCollection<record> list
        /// </summary>

        private void ResetMinMax<T>(DataCollection<T> list)
        {
            for (int i = 0; i < DataProperties.Count; i++)
            {
                EnsureLineExists(i);
            }

            XMin = double.MaxValue;
            XMax = double.MinValue;
            for (int i = 0; i < YMins.Count; i++)
            {
                YMins[i] = double.MaxValue;
                YMaxs[i] = double.MinValue;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var record = list[i];
                for (int j = 0; j < DataProperties.Count; j ++)
                {
                    var yProperty = DataProperties[j];
                    var y = Convert.ToDouble(yProperty.GetValue(record));
                    YMins[j] = Math.Min(YMins[j], y);
                    YMaxs[j] = Math.Max(YMaxs[j], y);
                }
                var x = Convert.ToDateTime(TimeProperty.GetValue(record));
                if (i==0)
                {
                    StartTime = x;
                }
                double xtime = (x.Subtract(StartTime)).TotalSeconds;
                XMin = Math.Min(XMin, xtime);
                XMax = Math.Max(XMax, xtime);
            }
        }

        /// <summary>
        /// Used by e.g. Kano Wand to set the wand position
        /// </summary>
        /// <param name="lineIndex"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void SetCursorPosition (double x, double y)
        {
            const int defaultLineIndex = 0;
            var left = X(x) - uiCursor.ActualWidth / 2;
            var top = Y(defaultLineIndex, y) - uiCursor.ActualHeight / 2; 
            if (left < 0) left = 0;
            if (left > this.ActualWidth) left = this.ActualWidth;
            if (top < 0) top = 0;
            if (top > this.ActualHeight) top = this.ActualHeight;
            Canvas.SetLeft(uiCursor, left);
            Canvas.SetTop(uiCursor, top);
        }


        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var position = e.GetCurrentPoint(uiCanvas).Position;
            uiThin.Visibility = Visibility.Visible;
            uiThinTextBorder.Visibility = Visibility.Visible;
            uiThin.X1 = position.X;
            uiThin.X2 = position.X;
            uiThin.Y1 = 0;
            uiThin.Y2 = uiCanvas.ActualHeight;
            Canvas.SetLeft(uiThinTextBorder, position.X);
            e.Handled = true;

            // What should the value box say?
            var ratio = position.X / uiCanvas.ActualWidth;
            var summary = Values?.GetSummary(ratio);
            if (String.IsNullOrEmpty(summary))
            {
                uiThinTextBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                uiThinTextValue.Text = summary;
            }
        }

        private void OnPointerExit(object sender, PointerRoutedEventArgs e)
        {
            uiThin.Visibility = Visibility.Collapsed;
            uiThinTextBorder.Visibility = Visibility.Collapsed;
            e.Handled = true;
        }

        private void OnPointerPress(object sender, PointerRoutedEventArgs e)
        {

        }
    }
}
