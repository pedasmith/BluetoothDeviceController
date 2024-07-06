using BluetoothDeviceController.Names;
using BluetoothDeviceController.SpecialtyPagesCustom;
using BluetoothProtocolsUwpXaml.ChartControl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace BluetoothDeviceController.Charts
{


    /// <summary>
    /// Simplified version of the ChartControl that is just for line-oriented data (like the BBC Micro:bit)
    /// </summary>
    public interface IChartControlOscilloscope
    {
        void SetDataProperties(IList<PropertyInfo> dataProperties, PropertyInfo timeProperty, IList<string> names);
        void SetTitle(string title);
        void SetUISpec(UISpecifications uiSpec);
        double GetPan();
        double GetMaxPan();
        void SetPan(double value);
        void SetZoom(double value);
        double GetZoom();
        void RedrawOscilloscopeYTime(int line, DataCollection<OscDataRecord> list, List<int> triggerIndex);
        int GetNextOscilloscopeLine();
        //TODO: int GetNMaxLines();
        void ClearLine(int lineIndex);
    }

    public class PointerPositionArgs : EventArgs
    {
        public PointerPositionArgs(string message, double value)
        {
            Message = message;
            Ratio = value;
        }
        public string Message { get; internal set; } = "";
        public double Ratio { get; set; } = 0.0;
    }

    public class ChartZoom
    {
        /// <summary>
        /// Zoom is 1 for no zoom, 10 for zoom in 10x. 
        /// </summary>
        public double Zoom = 1.0;
        /// <summary>
        /// Offset in ratio units; ratio is 0 to 1 for all data. e.g., setting 
        /// xratiooffset to .5 will jump to the middle of the traces.
        /// </summary>
        public double XRatioOffset = 0.0;
    }

    public sealed partial class ChartControl : UserControl, IChartControlOscilloscope
    {
        public UISpecifications UISpec = new UISpecifications();
        public IList<string> Names = new List<string>();
        public ChartZoom CurrZoom { get; } = new ChartZoom();



        public event EventHandler<PointerPositionArgs> OnPointerPosition;

        private bool MinMaxBeenInit { get; set; } = false;
        /// <summary>
        /// Data min/max values
        /// </summary>
        public double XMin { get; set; } = -1000.0;
        /// <summary>
        /// Data min/max values
        /// </summary>
        public double XMax { get; set; } = 1000.0;

        public void SetZoom(double value)
        {
            var oldzoom = CurrZoom.Zoom;
            CurrZoom.Zoom = value;
            RedrawAllLines();
        }

        /// <summary>
        /// Number from 1 to infinity. 1.0=no zoom 10=zoom in 10x. OK to be < 1 as long as it's > 0
        /// </summary>
        /// <returns></returns>
        public double GetZoom()
        {
            return CurrZoom.Zoom;
        }

        public double GetPan()
        {
            return CurrZoom.XRatioOffset;
        }

        public double GetMaxPan()
        {
            // How wide the the screen? Answer: in panning unit, "1.0".
            var screenwidth = 1.0 / CurrZoom.Zoom;
            var retval = 1.0 - screenwidth;
            return retval;
        }

        public void SetPan(double value)
        {
            var oldzoom = CurrZoom.XRatioOffset;
            CurrZoom.XRatioOffset = value;
            RedrawAllLines();
        }

        public void SetXRatioOffset(double value)
        {
            CurrZoom.XRatioOffset = value;
            RedrawAllLines();
        }

        public void SetUISpec(UISpecifications uiSpec)
        {
            UISpec = uiSpec;
        }

        // Y Min and Max are just a little weird. Sorry about that :-)
        // Some devices the difference lines need a combined min/max. Other devices
        // need seperate min/max. For the first: accelerometers have x,y,z values
        // which are all +-2g (or whatever). For the second, a device with temperature,
        // pressure, and humidity, which are all different.
        // Default is that they are all the same.
        // The X values, on the other hand, are always combined.

        private const double DEFAULT_YMIN = 0.0;
        private const double DEFAULT_YMAX = 100.0;


        public double GetYMin(int lineIndex)
        {
            switch (UISpec?.chartYAxisCombined ?? UISpecifications.YMinMaxCombined.Combined)
            {
                case UISpecifications.YMinMaxCombined.Combined:
                    return LineData.GetYMin(AllLineData);
                case UISpecifications.YMinMaxCombined.Separate:
                    var linedata = AllLineData[lineIndex];
                    return linedata.YMin;
            }
            return DEFAULT_YMIN;
        }
        public double GetYMax(int lineIndex)
        {
            switch (UISpec?.chartYAxisCombined ?? UISpecifications.YMinMaxCombined.Combined)
            {
                case UISpecifications.YMinMaxCombined.Combined:
                    return LineData.GetYMax(AllLineData);
                case UISpecifications.YMinMaxCombined.Separate:
                    var linedata = AllLineData[lineIndex];
                    return linedata.YMax;
            }
            return DEFAULT_YMAX; // Bigger than YMin
        }

        /// <summary>
        /// Per-line data
        /// </summary>
        class LineData
        {
            /// <summary>
            /// Color is the line color 
            /// </summary>
            /// <param name="color"></param>
            public LineData(Color color)
            {
                var polyline = new Polyline()
                {
                    Fill = null,
                    Stroke = new SolidColorBrush(color),
                    StrokeThickness = 1.0,
                };
                LLLine = polyline;
                LLMarkers = new List<Polyline>();
            }
            public List<Point> UnderlyingData = new List<Point>();
            public double PerLineXOffsetInSeconds = 0.0;
            public List<Point> MarkerList = new List<Point>();
            public Polyline LLLine = new Polyline();
            public List<Polyline> LLMarkers = new List<Polyline>();

            public bool YMinMaxBeenInit = false;
            public bool IsValid = false;
            public double YMin = DEFAULT_YMIN;
            public double YMax = DEFAULT_YMAX;

            public static double GetYMax(IList<LineData> list)
            {
                if (list.Count == 0) return DEFAULT_YMAX;
                var retval = list[0].YMax;
                foreach (var linedata in list)
                {
                    retval = Math.Max(retval, linedata.YMax);
                }
                return retval;
            }
            public static double GetYMin(IList<LineData> list)
            {
                if (list.Count == 0) return DEFAULT_YMIN;
                var retval = list[0].YMin;
                foreach (var linedata in list)
                {
                    retval = Math.Min(retval, linedata.YMin);
                }
                return retval;
            }
        }

        List<LineData> AllLineData = new List<LineData>();
        LineData CurrLineData {  get { return AllLineData[CurrOscilloscopeLine]; } }
        bool CurrLineDataExists {  get { return CurrOscilloscopeLine >= 0 && CurrOscilloscopeLine < AllLineData.Count; } }

        private int CurrOscilloscopeLine = -1;
        const int MAX_OSCILLOSCOPE_LINE = 3;
        public int GetNextOscilloscopeLine()
        {
            // Phase 1: return the first blank line from the existing set of lines
            for (int lineIndex=0; lineIndex<AllLineData.Count; lineIndex++)
            {
                var linedata = AllLineData[lineIndex];
                if (linedata == null) continue;
                if (!linedata.IsValid) return lineIndex;
            }

            // Phase 2: let the array be expanded up to the max
            if (AllLineData.Count < MAX_OSCILLOSCOPE_LINE)
            {
                return AllLineData.Count;// New line will be automatically created.
            }

            // Phase 3: return an old line. Don't delete the data; we don't know what the
            // caller will do with this info (not really true: they are about to draw a 
            // new line, of course!)
            var retval = (CurrOscilloscopeLine + 1) % MAX_OSCILLOSCOPE_LINE;
            return retval;
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
        private double CurrXLineOffsetInSeconds
        {
            get { return CurrLineData.PerLineXOffsetInSeconds; }
            set { CurrLineData.PerLineXOffsetInSeconds = value; }
        }

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
            this.uiStatus.Text = $"H={uiCanvas.ActualHeight} W={uiCanvas.ActualWidth}";
            uiBackgroundRect.Width = uiCanvas.ActualWidth;
            uiBackgroundRect.Height = uiCanvas.ActualHeight;
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

        #region POSITION
        /// <summary>
        /// Given an X time in seconds, return the 0..1 value of where it is between
        /// the XMin and XMax. Handles the case of XMin == XMax by return 0.
        /// </summary>
        private double XToRatio(double x)
        {
            var ratio = (XMax == XMin) ? 0 : ((x - XMin) / (XMax - XMin));
            return ratio;
        }

        /// <summary>
        /// Convert an X time in seconds into a pixel position offset by the current zoom and
        /// pan but not by the per-line offsets.
        /// </summary>
        private double X(double x)
        {
            var ratio = XToRatio(x);
            // 2024-06-09: var retval = (XMax == XMin) ? 0 : ((x - XMin) / (XMax - XMin)) * uiCanvas.ActualWidth;
            ratio = ratio - CurrZoom.XRatioOffset;
            ratio = ratio * CurrZoom.Zoom;
            var retval = ratio * uiCanvas.ActualWidth;
            return retval;
        }

        /// <summary>
        /// Given a physical location (e.g., from a pointer move), return a 0..1 value that adjusts for
        /// the current screen zoom and pan but not the-line adjustment. Not quite the reverse of the X
        /// method because that method takes in an X time and this one returns an X ratio.
        /// </summary>
        private double XRatioReverse(double xpos)
        {
            double reverse = xpos / uiCanvas.ActualWidth;
            reverse = reverse / CurrZoom.Zoom;
            reverse = reverse + CurrZoom.XRatioOffset; // reverse is now the 0..1 ratio but is missing the per-line offset.
            return reverse;
        }

        private double Y(int line, double y)
        {
            var ymin = UISpec?.ChartMinY(line, GetYMin(line)) ?? GetYMin(line);
            var ymax = UISpec?.ChartMaxY(line, GetYMax(line)) ?? GetYMax(line);
            var retval = (ymax==ymin) ? 0 : ((y - ymin) / (ymax - ymin)) * uiCanvas.ActualHeight;
            retval = uiCanvas.ActualHeight - retval;
            if (retval > uiCanvas.ActualHeight)
            {
                ;
            }
            return retval;
        }
        #endregion

        #region COLOR
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
        Color GetDefaultLineColor(int lineIndex)
        {
            var index = (lineIndex % DefaultLineColors.Length);
            return DefaultLineColors[index];
        }

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
        #endregion


        private void EnsureLineExists (int lineIndex)
        {
            while (AllLineData.Count <= lineIndex)
            {
                int newIndex = AllLineData.Count;

                // Kind of paintful to get the color...
                var name = (Names != null && Names.Count > newIndex) ? Names[newIndex] : "";
                var lineDefault = UISpec.chartLineDefaults.ContainsKey(name) ? UISpec.chartLineDefaults[name] : null;
                var color = (lineDefault == null) ? GetDefaultLineColor(AllLineData.Count) : ConvertColor(lineDefault.stroke);

                AllLineData.Add(new LineData(color));

                uiCanvas.Children.Add(AllLineData[newIndex].LLLine); // Actually add the polyline!

            }
        }
        private Polyline MakeEmptyMarker()
        {
            var retval = new Polyline() { Fill = null, Stroke = new SolidColorBrush(Colors.Red), StrokeThickness = 2.0 };
            return retval;
        }

        int nRedrawAllLines = 0;
        private void RedrawAllLines()
        {
            nRedrawAllLines++;
            for (int lineIndex=0; lineIndex<AllLineData.Count; lineIndex++)
            {
                var linedata = AllLineData[lineIndex];
                if (!linedata.IsValid) continue; // line might have been cleared
                RemoveLLMarkers(lineIndex);
                DrawMarkers(lineIndex);

                var data = linedata.UnderlyingData;
                linedata.LLLine.Points.Clear();
                foreach (var datapoint in data)
                {
                    var xtime = datapoint.X;
                    var y = datapoint.Y;
                    var point = new Point(X(xtime + linedata.PerLineXOffsetInSeconds), Y(lineIndex, y));
                    // Only add points that are visible on screen.
                    if (point.X >= 0 && point.X <= uiCanvas.ActualWidth)
                    {
                        linedata.LLLine.Points.Add(point);
                    }
                }
            }

            DrawReticule();
        }

        private LineData GetFirstLine()
        {
            for (int i=0; i<AllLineData.Count; i++)
            {
                if (AllLineData[i].IsValid) return AllLineData[i];
            }
            return null;
        }

        // goal is to set xdelta to a nice even number
        // e.g., if xdelta is 1.2, switch to be just 1; if it's 1.6, bump
        // it to 2. 
        private static double MakeNiceReticuleSpace(double xdelta)
        {
            var retval = xdelta;
            var ndigits = Math.Log10(xdelta); // e.g., 3.4 ==> 0.xx, and 34.2==>1.xx 0.034 ==> -2.xx
            var normdig = Math.Floor(ndigits);
            var normalized = retval / Math.Pow(10, normdig);  // Now 

            var rounded = Math.Round(normalized); // e.g., 0.000034 is now 3
            if (rounded == 4) rounded = 5.0;
            if (rounded == 8 || rounded == 9) rounded = 10; // bump up.

            xdelta = rounded * Math.Pow(10, normdig); // convert back to expected 

            return xdelta;
        }
        private static void Log(string text)
        {
            System.Diagnostics.Debug.Write(text);
        }
        private static int TestMakeNiceReticuleSpaceOne(double xdelta, double expected)
        {
            int nerror = 0;
            double actual = MakeNiceReticuleSpace(xdelta);
            var diff = Math.Abs(actual - expected);   
            if (diff > 0.0000000001) // little bit of rounding slop is OK
            {
                Log($"ERROR: MakeNiceReticuleSpace ({xdelta}) expected={expected} actual={actual}");
                nerror++;
            }

            return nerror;
        }
        public static int TestMakeNiceReticuleSpace()
        {
            int nerror = 0;
            nerror += TestMakeNiceReticuleSpaceOne(0.00037, 0.0004);


            nerror += TestMakeNiceReticuleSpaceOne(1.2, 1.0);
            nerror += TestMakeNiceReticuleSpaceOne(0.12, 0.1);
            nerror += TestMakeNiceReticuleSpaceOne(0.0012, 0.001);


            nerror += TestMakeNiceReticuleSpaceOne(3.2, 3.0);
            nerror += TestMakeNiceReticuleSpaceOne(0.32, 0.3);
            nerror += TestMakeNiceReticuleSpaceOne(0.0032, 0.003);

            nerror += TestMakeNiceReticuleSpaceOne(4.2, 5.0);
            nerror += TestMakeNiceReticuleSpaceOne(0.42, 0.5);
            nerror += TestMakeNiceReticuleSpaceOne(0.0042, 0.005);

            return nerror;
        }

        public static class Reticule_Settings
        {
            public const int N_RETICULE_LINES = 10; // when zoomed all the way
            public const int N_MINOR_LINES = 10; // 10 lines per division

            public const double STROKE_MAJOR_THICKNESS = 3.0;
            public static Color MAJOR_COLOR = Colors.DarkGreen;
            public static Brush MAJOR_BRUSH = new SolidColorBrush(MAJOR_COLOR);

            public const double STROKE_MINOR_THICKNESS = 1.0;
            public static Color MINOR_COLOR = Colors.DarkBlue;
            public static Brush MINOR_BRUSH = new SolidColorBrush(MINOR_COLOR);

        }

        public double ReticuleSizeInSeconds { get; internal set; } = 0.0;
        public string GetReticuleSpace()
        {
            string retval = "??";
            double x = ReticuleSizeInSeconds;
            string units = "s";
            if (x < 1)
            {
                x = x * 1000.0;
                units = "ms";
            }

            if (x < 1)
            {
                x = x * 1000.0;
                units = "µs";
            }
            retval = $"{x:F1}{units}";
            return retval;
        }
        private void DrawReticule() // The background grid
        {
            uiReticule.Children.Clear();
            if (uiReticule.ActualWidth < 10) return; // too narrow or uninitialized.
            var linedata = GetFirstLine();
            if (linedata == null) return; // no line means no grid
            var data = linedata.UnderlyingData;
            if (data.Count < 2) return; // need two points to make lines
            var xstart = data[0].X;
            var xend = data[data.Count-1].X;
            var xdelta = xend - xstart;
            if (xdelta < 0.000001) return; // need two different points to make lines

            double zoom = GetZoom();
            if (zoom < 1.0) zoom = 1.0;

            // Now start the calculations proper
            var xspace = MakeNiceReticuleSpace (xdelta / (Reticule_Settings.N_RETICULE_LINES * zoom));
            var xspace_minor = xspace / Reticule_Settings.N_MINOR_LINES;
            ReticuleSizeInSeconds = xspace;

            for (double xtime = xstart; xtime <= xend; xtime+=xspace)
            {
                var llx = X(xtime + linedata.PerLineXOffsetInSeconds);
                if (llx >= 0 && llx <= uiReticule.ActualWidth)
                {
                    Line line = new Line()
                    {
                        X1 = llx,
                        X2 = llx,
                        Y1 = 0,
                        Y2 = uiReticule.ActualHeight,

                        Stroke = Reticule_Settings.MAJOR_BRUSH,
                        StrokeThickness = Reticule_Settings.STROKE_MAJOR_THICKNESS,
                    };
                    uiReticule.Children.Add(line);
                }

                for (int minor = 0; minor < Reticule_Settings.N_MINOR_LINES; minor++)
                {
                    double xtimeminor = xtime + (minor * xspace_minor);
                    var llxminor = X(xtimeminor + linedata.PerLineXOffsetInSeconds);
                    if (llxminor >= 0 && llxminor <= uiReticule.ActualWidth)
                    {
                        Line lineminor = new Line()
                        {
                            X1 = llxminor,
                            X2 = llxminor,
                            Y1 = 0,
                            Y2 = uiReticule.ActualHeight,

                            Stroke = Reticule_Settings.MINOR_BRUSH,
                            StrokeThickness = Reticule_Settings.STROKE_MINOR_THICKNESS,
                        };
                        uiReticule.Children.Add(lineminor);
                    }
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
            var linedata = AllLineData[lineIndex];
            switch (addResult)
            {
                case AddResult.AddReplace:
                    {
                        // Note: must call ResetMinMax if the data is really updated.
                        linedata.UnderlyingData = new List<Point>(list.Count);
                        for (int i = 0; i < list.Count; i++)
                        {
                            var record = list[i];
                            var x = Convert.ToDateTime(timeProperty.GetValue(record));
                            var y = Convert.ToDouble(yProperty.GetValue(record));
                            double xtime = (x.Subtract(StartTime)).TotalSeconds;
                            linedata.UnderlyingData.Add(new Point(xtime, y));
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

        public void ClearLine(int lineIndex)
        {
            var linedata = AllLineData[lineIndex];
            linedata.IsValid = false;
            linedata.LLLine.Points.Clear();
            linedata.MarkerList.Clear();
            RemoveLLMarkers(lineIndex);
        }


        double line0markerSeconds = 0;
        /// <summary>
        /// Primary method used to push data from the OscilloscopeControl into the embedded ChartControl
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lineIndex"></param>
        /// <param name="list"></param>
        public void RedrawOscilloscopeYTime(int lineIndex, DataCollection<OscDataRecord> list, List<int> markerIndexList)
        {
            if (DataProperties == null) return;
            if (DataProperties.Count != 1) return; // NOTE: Always exactly 1 item
            if (lineIndex < 0 || lineIndex > MAX_LINE_INDEX) return; // Bad line index = fail

            EnsureLineExists(lineIndex);
            var linedata = AllLineData[lineIndex];
            Values = list; // Reset the "Values" so we get a summary item

            ClearLine(lineIndex);
            linedata.IsValid = true;
            ResetMinMaxOscilloscope(lineIndex, list); // Reset XMIN XMAX YMIN YMAX StartTime
            CurrOscilloscopeLine = lineIndex;

            if (list.Count > 0)
            {
                if (markerIndexList.Count > 0)
                {
                    if (lineIndex == 0)
                    {
                        var record = list[markerIndexList[0]];
                        var time = Convert.ToDateTime(TimeProperty.GetValue(record));
                        line0markerSeconds = time.Subtract(StartTime).TotalSeconds;
                        CurrXLineOffsetInSeconds = 0.0;
                    }
                    else
                    {
                        var firstMarkerRecord = list[markerIndexList[0]];
                        var time = Convert.ToDateTime(TimeProperty.GetValue(firstMarkerRecord));
                        var markerSeconds = time.Subtract(StartTime).TotalSeconds;
                        CurrXLineOffsetInSeconds = line0markerSeconds - markerSeconds;
                    }
                }
                else
                {
                    CurrXLineOffsetInSeconds = 0.0;
                }


                linedata.UnderlyingData = new List<Point>(list.Count);
                var yProperty = DataProperties[0];
                foreach (var record in list)
                {
                    var x = Convert.ToDateTime(TimeProperty.GetValue(record));
                    var y = Convert.ToDouble(yProperty.GetValue(record));
                    AddXYPoint(lineIndex, x, y);
                }


                if (markerIndexList.Count > 0)
                {
                    foreach (var markerIndex in markerIndexList)
                    {
                        var markerRecord = list[markerIndex];
                        var x = Convert.ToDateTime(TimeProperty.GetValue(markerRecord));
                        var y = Convert.ToDouble(yProperty.GetValue(markerRecord));
                        double xtime = (x.Subtract(StartTime)).TotalSeconds;
                        var markerPoint = new Point(xtime, y);
                        linedata.MarkerList.Add(markerPoint);
                    }
                    DrawMarkers(lineIndex);
                }
            }
            if (lineIndex == 0)
            {
                DrawReticule();
            }
        }
        private void RemoveLLMarkers(int lineIndex)
        {
            var linedata = AllLineData[lineIndex];
            foreach (var marker in linedata.LLMarkers)
            {
                uiCanvas.Children.Remove(marker);
            }
        }

        /// <summary>
        /// Draw the makers in the Markerlist
        /// </summary>
        /// <param name="lineIndex"></param>
        private void DrawMarkers(int lineIndex)
        {
            var linedata = AllLineData[lineIndex];
            foreach (var point in linedata.MarkerList)
            {
                double xpos = X(point.X + linedata.PerLineXOffsetInSeconds);
                double ypos = Y(lineIndex, point.Y);
                if (xpos >= 0 && xpos <= uiCanvas.ActualWidth)
                {
                    var marker = MakeEmptyMarker();
                    marker.Stroke = linedata.LLLine.Stroke; // Make it be the same color as the line
                    linedata.LLMarkers.Add(marker);
                    uiCanvas.Children.Add(marker);
                    SetupMarker(marker.Points, xpos, ypos);
                }
            }
        }

        private void SetupMarker(PointCollection points, double xpos, double ypos)
        {
            points.Clear();
            points.Add(new Point(xpos, ypos));
            points.Add(new Point(xpos - 5, ypos - 5));

            points.Add(new Point(xpos, ypos));
            points.Add(new Point(xpos + 5, ypos - 5));

            points.Add(new Point(xpos, ypos));
            points.Add(new Point(xpos - 5, ypos + 5));

            points.Add(new Point(xpos, ypos));
            points.Add(new Point(xpos + 5, ypos + 5));

            points.Add(new Point(xpos, ypos));
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
                    for (int lineIndex = 0; lineIndex < AllLineData.Count; lineIndex++)
                    {
                        var linedata = AllLineData[lineIndex];
                        linedata.UnderlyingData = new List<Point>(list.Count);
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
            for (int lineIndex = 0; lineIndex < AllLineData.Count; lineIndex++)
            {
                var linedata = AllLineData[lineIndex];
                linedata.YMin = double.MaxValue;
                linedata.YMax = double.MinValue;
            }
            for (int i = 0; i < list.Count; i++)
            {
                var record = list[i];
                for (int j=0; j<DataProperties.Count; j+=2)
                {
                    var lineIndexProperty = DataProperties[j];
                    var yProperty = DataProperties[j + 1];
                    var lineIndex = (int)Convert.ToDouble(lineIndexProperty.GetValue(record));
                    if (lineIndex >= 0 && lineIndex <= MAX_LINE_INDEX)
                    {
                        EnsureLineExists(lineIndex);
                        var linedata = AllLineData[lineIndex];
                        var y = Convert.ToDouble(yProperty.GetValue(record));
                        linedata.YMin = Math.Min(linedata.YMin, y);
                        linedata.YMax = Math.Max(linedata.YMax, y);
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
        /// Uses the **Oscilloscope** data format where the line is given and the data has a single property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="lineIndex"></param>
        /// <param name="list"></param>
        private void ResetMinMaxOscilloscope<T>(int lineIndex, DataCollection<T> list)
        {
            if (lineIndex < 0 && lineIndex > MAX_LINE_INDEX) return;
            if (DataProperties.Count != 1) return;
            var linedata = AllLineData[lineIndex];
            XMin = double.MaxValue;
            XMax = double.MinValue;
            EnsureLineExists(lineIndex);
            linedata.YMin = double.MaxValue;
            linedata.YMax = double.MinValue;
            var yProperty = DataProperties[0];
            for (int i = 0; i < list.Count; i++)
            {
                var record = list[i];
                var y = Convert.ToDouble(yProperty.GetValue(record));
                linedata.YMin = Math.Min(linedata.YMin, y);
                linedata.YMax = Math.Max(linedata.YMax, y);
                var x = Convert.ToDateTime(TimeProperty.GetValue(record));
                if (i == 0)
                {
                    StartTime = x;
                }
                double xtime = (x.Subtract(StartTime)).TotalSeconds;
                XMin = Math.Min(XMin, xtime);
                XMax = Math.Max(XMax, xtime);
            }

            linedata.YMinMaxBeenInit = true;
            MinMaxBeenInit = true;
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
            var linedata = AllLineData[lineIndex];
            double xtime = (x.Subtract (StartTime)).TotalSeconds;
            linedata.UnderlyingData.Add(new Point(xtime, y));

            var xoutofrange = xtime < XMin || xtime > XMax;
            var youtofrange = y < GetYMin(lineIndex) || y > GetYMax(lineIndex);
            if (xoutofrange || youtofrange || !MinMaxBeenInit)
            {
                if (!MinMaxBeenInit)
                {
                    MinMaxBeenInit = true;
                    StartTime = x;
                    XMin = xtime;
                    XMax = xtime;
                    linedata.YMin = y;
                    linedata.YMax = y;
                    linedata.YMinMaxBeenInit = true;
                }
                else if (!linedata.YMinMaxBeenInit)
                {
                    linedata.YMin = y;
                    linedata.YMax = y;
                    linedata.YMinMaxBeenInit = true;
                }
                else
                {
                    XMin = Math.Min(XMin, xtime);
                    XMax = Math.Max(XMax, xtime);
                    linedata.YMin = Math.Min(linedata.YMin, y);
                    linedata.YMax = Math.Max(linedata.YMax, y);

                    var xdelta = XMax - XMin;
                    if (xdelta < 5) XMax = XMin + 5; // At least 5 seconds of data at all times?
                }
                RedrawAllLines();
            }
            else
            {
                var point = new Point(X(xtime + linedata.PerLineXOffsetInSeconds), Y(lineIndex, y));
                linedata.LLLine.Points.Add(point);
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
            for (int lineIndex = 0; lineIndex < AllLineData.Count; lineIndex++)
            {
                var linedata = AllLineData[lineIndex];
                linedata.YMin = double.MaxValue;
                linedata.YMax = double.MinValue;
            }

            for (int i = 0; i < list.Count; i++)
            {
                var record = list[i];
                for (int lineIndex = 0; lineIndex < DataProperties.Count; lineIndex ++)
                {
                    var linedata = AllLineData[lineIndex];
                    var yProperty = DataProperties[lineIndex];
                    var y = Convert.ToDouble(yProperty.GetValue(record));
                    linedata.YMin = Math.Min(linedata.YMin, y);
                    linedata.YMax = Math.Max(linedata.YMax, y);
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
            if (left > uiCanvas.ActualWidth) left = uiCanvas.ActualWidth;
            if (top < 0) top = 0;
            if (top > uiCanvas.ActualHeight) top = uiCanvas.ActualHeight;
            Canvas.SetLeft(uiCursor, left);
            Canvas.SetTop(uiCursor, top);
        }

        public bool HandlePointerEvents { get; set; } = true;

        public void PointerSetCursorVisible(bool visible)
        {
            Visibility v = visible ? Visibility.Visible : Visibility.Collapsed;
            uiThin.Visibility = v;
            uiThinTextBorder.Visibility = v;
        }

        public void PointerSetCursorVisibleOnLeft(bool visible)
        {
            if (visible)
            {
                DoPointerMove(new Point() { X = uiCanvas.ActualWidth * 0.15, Y = uiCanvas.ActualHeight - 20.0 });
            }
            PointerSetCursorVisible(visible);
        }

        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            if (!HandlePointerEvents) return;
            var position = e.GetCurrentPoint(uiCanvas).Position;
            e.Handled = true;
            DoPointerMove(position);
        }

        public void DoPointerMove(Point position)
        { 
            uiThin.Visibility = Visibility.Visible;
            uiThinTextBorder.Visibility = Visibility.Visible;
            uiThin.X1 = position.X;
            uiThin.X2 = position.X;
            uiThin.Y1 = 0;
            uiThin.Y2 = uiCanvas.ActualHeight;
            Canvas.SetLeft(uiThinTextBorder, position.X);

            // What should the value box say?
            if (!CurrLineDataExists)
            {
                uiThinTextBorder.Visibility = Visibility.Collapsed;
                return; // no oscilloscope data yet
            }
            var ratio = XRatioReverse(position.X) - XToRatio(CurrXLineOffsetInSeconds);
            var summary = Values?.GetSummary(ratio); // ratio is unzoomed, with no offsets. Just 0..1.
            if (String.IsNullOrEmpty(summary))
            {
                uiThinTextBorder.Visibility = Visibility.Collapsed;
            }
            else
            {
                uiThinTextValue.Text = summary;
            }
            // Per https://learn.microsoft.com/en-us/dotnet/csharp/programming-guide/events/how-to-publish-events-that-conform-to-net-framework-guidelines, 
            // avoid a race condition by making a temp variable first.
            var tmp = OnPointerPosition;
            tmp?.Invoke(this, new PointerPositionArgs(summary, ratio));
        }

        private void OnPointerExit(object sender, PointerRoutedEventArgs e)
        {
            if (!HandlePointerEvents) return;
            PointerSetCursorVisible(false);

            e.Handled = true;
        }

        private void OnPointerPress(object sender, PointerRoutedEventArgs e)
        {
            if (!HandlePointerEvents) return;
        }

        public void SetPersonalization(BluetoothProtocolsUwpXaml.ChartControl.UserPersonalization pref)
        {
            uiReticule.Background = pref.GetBrush(UserPersonalization.Item.ChartBackground);
            uiThin.Stroke = pref.GetBrush(UserPersonalization.Item.ThinCursor);
            uiThin.StrokeThickness = pref.GetThickness(UserPersonalization.Item.ThinCursor);
        }
    }
}
