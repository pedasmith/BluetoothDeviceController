using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Shapes;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestNmeaGpsParserWinUI;
using Windows.Foundation;
using static WinUI3Controls.SimpleMapControl;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

namespace WinUI3Controls
{

    public sealed partial class SimpleMapControl : UserControl, IDoGpsMap
    {
        Random R = new Random(); // I need a way to distinguish one map control from another. Would use the "address", but that's not a thing in C#.
        int RandomValue = 0;
        public SimpleMapControl()
        {
            RandomValue = R.Next();
            this.InitializeComponent();
            ConstantlyShowCurrentFocus = new WinUI3Controls.ConstantlyShowCurrentFocusTask(MainWindow.MainWindowWindow, uiFocus);

            this.Loaded += SimpleMapControl_Loaded;
            this.Unloaded += SimpleMapControl_Unloaded;
        }


        /// <summary>
        /// Logging levels used by some of the drawing routines. handy because drawing a single line often
        /// needs more logging and drawing a bunch of lines needs less.
        /// </summary>
        public enum LogLevel { Normal, None };
        /// <summary>
        /// Internal enum; used when the "middle" of a map is removed so that there's fewer segments. 
        /// </summary>
        private enum LineType { StartingLine, EndingLine, ConnectingLine }
        [Flags] private enum PointTypeFlag { Normal = 0, IsFirstPoint = 1, IsLastPoint = 2 }


        // Setting segment limits per eyeballed measurements on laptop 2025-07-05
        const int MAX_STARTING_SEGMENTS = 2500;
        const int MAX_ENDING_SEGMENTS = 2500;
        const double MAX_SEGMENT_LENGTH = 100.0;
        const int MAX_SEGMENTS_PER_LINE = 100;
        const int MAX_SEGMENTS_HYSTERESIS = 2 * (MAX_STARTING_SEGMENTS + MAX_ENDING_SEGMENTS);
        const double MIN_MARKER_SIZE = 3;
        const double MAX_MARKER_SIZE = 10;
        const double CURSOR_RADIUS = 4;


        // Efficiency hints:
        // Measurements 2025-07-05: 5K segments works fine; panning is reasonably smooth. 10K segments gets jittery.
        // When the MAX_*_SEGMENTS is set at less than MAX_SEGMENTS_PER_LINE we can get into a bad state where
        // every new line results in a redraw. This is then super slow

        const double SCALEFACTOR_INIT = 3.0;

        static Brush MarkerBrush = new SolidColorBrush(Colors.Red);
        static Brush PositionLineStartingBrush = new SolidColorBrush(Colors.DarkGreen);
        static Brush PositionLineEndingBrush = new SolidColorBrush(Colors.DarkCyan);
        static Brush PositionLineBrush3 = new SolidColorBrush(Colors.Red);

        public List<MapDataItem> MapData = null;
        DrawingState DS = new DrawingState();

        // Used internally
        Task FocusReporterTask;
        CancellationTokenSource UnloadingCts;
        bool FirstCallToLoaded = true;
        WinUI3Controls.ConstantlyShowCurrentFocusTask ConstantlyShowCurrentFocus; // Created in constructor; used in loaded/unloading
        private void SimpleMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Log($"SimpleMapControl: Loaded called: Random={RandomValue}");
            this.Focus(FocusState.Programmatic);

            UnloadingCts = new CancellationTokenSource();
            FocusReporterTask = ConstantlyShowCurrentFocus.CreateTask(UnloadingCts.Token);
            FocusReporterTask.Start();

            // When a control is embedded in a tab control, it will be loaded and unloaded
            if (!FirstCallToLoaded) return;
            FirstCallToLoaded = false;

            DS.Clear();
            DS.ResetScale();
            Canvas.SetLeft(uiMapItemCanvas, uiMapCanvas.ActualWidth / 2.0);
            Canvas.SetTop(uiMapItemCanvas, uiMapCanvas.ActualHeight / 2.0);
        }

        private void SimpleMapControl_Unloaded(object sender, RoutedEventArgs e)
        {
            Log($"SimpleMapControl: Unloaded called: Random={RandomValue}");
            UnloadingCts.Cancel();
        }


        #region UX_MANIPULATION
        private void OnMapCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rect = new RectangleGeometry();
            rect.Rect = new Rect(0, 0, uiMapCanvas.ActualWidth, uiMapCanvas.ActualHeight);
            uiMapCanvas.Clip = rect;

            PositionuiPointInfoBorder();
        }

        private void OnPointInfoBorderSizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Position the point info border in the bottom right corner of the map canvas.
            PositionuiPointInfoBorder();
        }

        private void PositionuiPointInfoBorder()
        {
            Canvas.SetLeft(uiPointInfoBorder, uiMapCanvas.ActualWidth - uiPointInfoBorder.ActualWidth);
            Canvas.SetTop(uiPointInfoBorder, uiMapCanvas.ActualHeight - uiPointInfoBorder.ActualHeight);
        }

        int NPointReleasedSuppress = 0;
        bool InManipulation = false;
        // Tap: no manipulation started and get a pointer released
        // Pan: manipation started + end and get a pointer released which should be ignored
        // Scale: manipulation started + pointer released + manipulation end and get a pointer released. Both pointer
        //      released need to be ignored, but there's no way to "know" that there's two (just because there was a 
        //      scale doesn't mean there must have been two points; scaling might be done differently in the future).
        // So: always disallow pointer released during a manipulation, and then disallow one after an end.

        // Note: when the user just presses and releases with no panning or pinching, none of the manipulation
        // events is triggered at all. Since I need to support panning, scaling (pinching) and selection (pointer released),
        // I have to add interlocks and nonsense.

        private void OnManipulationStarting(object sender, Microsoft.UI.Xaml.Input.ManipulationStartingRoutedEventArgs e)
        {
        }

        private void OnManipulationStart(object sender, Microsoft.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
            Log($"Manipulation started");
            InManipulation = true;
        }
        private void OnManipulationComplete(object sender, Microsoft.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {

            e.Handled = true;
            NPointReleasedSuppress++;
            InManipulation = false;

            var newscaledelta = e.Cumulative.Scale;
            var startLeft = Canvas.GetLeft(uiMapItemCanvas);
            var startTop = Canvas.GetTop(uiMapItemCanvas);

            if (newscaledelta != 1.0)
            {
                // Where is the final point?
                var touchPosition = e.Position; // "the" touch position which isn't obvious for pinchy gestures.
                touchPosition = new Point(uiMapCanvas.ActualWidth / 2.0, uiMapCanvas.ActualHeight / 2.0); // always zoom around the center of the screen.
                var startLat = (touchPosition.X - startLeft) / DS.ScaleFactor;
                var startLong = (touchPosition.Y - startTop) / DS.ScaleFactor;

                // Zooming
                DS.ScaleFactor *= newscaledelta;

                var newStartLeft = touchPosition.X - (startLat * DS.ScaleFactor);
                var newStartTop = touchPosition.Y - (startLong * DS.ScaleFactor);

                DoClear(); // Also does DS.Clear() but not DS.ResetScale() or remove mapdata points
                var nsegment = RedrawAllLines(LogLevel.None);
                Canvas.SetLeft(uiMapItemCanvas, newStartLeft);
                Canvas.SetTop(uiMapItemCanvas, newStartTop);
                // ?? the highlighted item should not be updated here? DisplayHighlightByIndex(DS.HighlightedMapDataItemIndex, DS.HighlightedMapDataItemGroupIndex);
                Log($"Manipulation Complete: scale={DS.ScaleFactor:F2} delta={newscaledelta:F2} start lat={startLat:F2} x={startLeft:F2} newX={newStartLeft:F2} nsegment={nsegment}");
            }
            else
            {
                var dx = e.Cumulative.Translation.X;
                var dy = e.Cumulative.Translation.Y;
                // Panning
                Canvas.SetLeft(uiMapItemCanvas, startLeft + dx);
                Canvas.SetTop(uiMapItemCanvas, startTop + dy);
                Log($"Manipulation Complete: pan x={dx:F2} oldx={startLeft:F2} y={dy:F2} oldy={startTop:F2}");
            }
        }

        private void OnPointerWheelChange(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            var point = e.GetCurrentPoint(uiMapPositionCanvas);
            var mwd = point.Properties.MouseWheelDelta; // is, e.g., "120" for a single click
            var keym = e.KeyModifiers;
            if (!keym.HasFlag(Windows.System.VirtualKeyModifiers.Control))
            {
                return; // only zoom for ctrl-wheel
            }
            e.Handled = true;
            var newscaledelta = (1.0 + (double)mwd * .005);
            //Log($"DBG: mwd: {mwd} delta={newscaledelta} DS.ScaleFactor:{DS.ScaleFactor} new:{DS.ScaleFactor*newscaledelta}");
            

            var startLeft = Canvas.GetLeft(uiMapItemCanvas);
            var startTop = Canvas.GetTop(uiMapItemCanvas);

            // Where is the final point?
            var touchPosition = new Point(uiMapCanvas.ActualWidth / 2.0, uiMapCanvas.ActualHeight / 2.0); // always zoom around the center of the screen.
            var startLat = (touchPosition.X - startLeft) / DS.ScaleFactor;
            var startLong = (touchPosition.Y - startTop) / DS.ScaleFactor;

            // Zooming
            DS.ScaleFactor *= newscaledelta;

            var newStartLeft = touchPosition.X - (startLat * DS.ScaleFactor);
            var newStartTop = touchPosition.Y - (startLong * DS.ScaleFactor);

            DoClear(); // Also does DS.Clear() but not DS.ResetScale() or remove mapdata points
            var nsegment = RedrawAllLines(LogLevel.None);
            Canvas.SetLeft(uiMapItemCanvas, newStartLeft);
            Canvas.SetTop(uiMapItemCanvas, newStartTop);
            // the selected item isn't changed by this method! DisplayHighlightByIndex(DS.HighlightedMapDataItemIndex, DS.HighlightedMapDataItemGroupIndex);
            Log($"Pointer Complete: mwd={mwd} scale={DS.ScaleFactor:F2} delta={newscaledelta:F2} start lat={startLat:F2} x={startLeft:F2} newX={newStartLeft:F2} nsegment={nsegment}");
        }


        private void OnPointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            Log($"Pointer released: in manipulation={InManipulation} supress={NPointReleasedSuppress}");
            e.Handled = true;
            if (InManipulation) return;

            if (NPointReleasedSuppress <= 0)
            {
                var touchPosition = e.GetCurrentPoint(uiMapPositionCanvas).Position;

                // touchPosition.X matches the calculated value for moving the highlighted.
                // Comments shows how the reverse calculation was derived from the known-good forward calculations
                //
                // X=DS.ScaleFactor * ((longitude - DS.StartingLongitude) * 60 * 10000),
                // x/(DS.ScaleFactor* 60 * 10000) = longitude - DS.StartingLongitude
                // x/(DS.ScaleFactor* 60 * 10000) + DS.StartingLongitude = longitude

                // y = DS.ScaleFactor * ((-(latitude - DS.StartingLatitude)) * 60 * 10000)
                // y/(DS.ScaleFactor * 60 * 10000) = (-(latitude - DS.StartingLatitude))
                // y/(DS.ScaleFactor * 60 * 10000) = -(latitude - DS.StartingLatitude)
                // -y/(DS.ScaleFactor * 60 * 10000) = latitude - DS.StartingLatitude
                // DS.StartingLatitude-y/(DS.ScaleFactor * 60 * 10000) = latitude


                if (MapData.Count >= 1)
                {

                    var lon0 = touchPosition.X / (DS.ScaleFactor * 60 * 10000) + DS.StartingLongitude;
                    var lat0 = -(touchPosition.Y / (DS.ScaleFactor * 60 * 10000)) + DS.StartingLatitude;
                    int minIndex = 0;
                    double minDistance = MapData[minIndex].Distance(lat0, lon0);
                    for (int i = 1; i < MapData.Count; i++)
                    {
                        double d = MapData[i].Distance(lat0, lon0);
                        if (d < minDistance)
                        {
                            minIndex = i;
                            minDistance = d;
                        }
                    }
                    DisplayHighlightByIndex(minIndex, 0);
                    Log($"Release: move highlight: closest={minIndex} touch x={touchPosition.X:F2} lon0={lon0}");
                }
            }
            NPointReleasedSuppress--;
            if (NPointReleasedSuppress < 0) NPointReleasedSuppress = 0;
        }


        private void OnPointInfoMultiLeft(object sender, RoutedEventArgs e)
        {
            DisplayHighlightByIndex(DS.HighlightedMapDataItemIndex, DS.HighlightedMapDataItemGroupIndex-1);
        }
        private void OnPointInfoMultiRight(object sender, RoutedEventArgs e)
        {
            DisplayHighlightByIndex(DS.HighlightedMapDataItemIndex, DS.HighlightedMapDataItemGroupIndex + 1);
        }


        private void OnCharacter(UIElement sender, Microsoft.UI.Xaml.Input.CharacterReceivedRoutedEventArgs args)
        {
            switch (args.Character)
            {
                default:
                    Log($"Character: Character={args.Character}");
                    break;
                case '.':
                    Log($"Character: Character=DOT");
                    if (DS.HighlightedMapDataItemIndex >= 0)
                    {
                        var data = MapData[DS.HighlightedMapDataItemIndex];
                        var p = ToPoint(data);
                        Canvas.SetLeft(uiMapItemCanvas, uiMapCanvas.ActualWidth / 2.0 - p.X);
                        Canvas.SetTop(uiMapItemCanvas, uiMapCanvas.ActualHeight / 2.0 - p.Y);
                        Log($"KEY: '.' will set center to highlighted {DS.HighlightedMapDataItemIndex} left={p.X}");
                    }
                    break;
                case '<':
                    Log($"Character: Character=LT");
                    DisplayHighlightByIndexDelta(0, -1);
                    break;
                case '>':
                    Log($"Character: Character=GT");
                    DisplayHighlightByIndexDelta(0, 1);
                    break;
            }
        }

        private void OnKeyUp(object sender, Microsoft.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            switch (e.Key)
            {
                default:
                    Log($"KEY: Key={e.Key}");
                    e.Handled = false;
                    break;
                case Windows.System.VirtualKey.Left:
                    DisplayHighlightByIndexDelta(-1, 0);
                    e.Handled = true;
                    break;
                case Windows.System.VirtualKey.Right:
                    DisplayHighlightByIndexDelta(1, 0);
                    e.Handled = true;
                    break;
            }
        }

        /// <summary>
        /// You can set either the indexDelta or the groupIndexDelta, but not both. And you can only set the
        /// values to -1, 0, or 1.
        /// </summary>
        private void DisplayHighlightByIndexDelta(int indexDelta, int groupIndexDelta)
        {
            int index = DS.HighlightedMapDataItemIndex;
            int groupIndex = DS.HighlightedMapDataItemGroupIndex;
            if (index == -1)
            {
                // We're just initializing.
                if (indexDelta < 0 || groupIndexDelta < 0)
                {
                    // If we're going backwards, then we need to start at the end.
                    index = MapData.Count - 1;
                    groupIndex = MapData[index].GroupedNmea.Count - 1; // wrap around to the last group.
                }
                else
                {
                    index = 0; // start at the first item.
                    groupIndex = 0; // start at the first group.
                }
            }
            else if (groupIndexDelta != 0)
            {
                groupIndex += groupIndexDelta;
                if (groupIndex < 0)
                {
                    // we're going earlier and did an underflow. 
                    index--;
                    if (index < 0)
                    {
                        index = MapData.Count - 1; // wrap around to the end.
                    }
                    groupIndex = MapData[index].GroupedNmea.Count - 1; // wrap around to the last group.
                }
                else if (groupIndex >= MapData[index].GroupedNmea.Count)
                {
                    index++;
                    if (index > MapData.Count - 1)
                    {
                        index = 0; // wrap around to the start.
                    }
                    groupIndex = 0; // wrap around to the first group.
                }
            }
            else if (indexDelta != 0)
            {
                index += indexDelta;
                if (index < 0)
                {
                    index = MapData.Count - 1;
                    groupIndex = MapData[index].GroupedNmea.Count - 1; // wrap around to the last group.
                }
                else if (index >= MapData.Count)
                {
                    index = 0; // wrap around to the start.
                    groupIndex = 0; // wrap around to the first group.
                }
            }
            DisplayHighlightByIndex(index, groupIndex);
        }

        private void DisplayHighlightByIndex(int newIndex, int newGroupIndex)
        {
            var ischanged = DS.HighlightedMapDataItemIndex != newIndex;

            if (newIndex >= MapData.Count)
            {
                newIndex = MapData.Count - 1;
            }
            if (newIndex < 0)
            {
                newIndex = 0;
            }

            var data = MapData[newIndex];

            if (newGroupIndex >= data.GroupedNmea.Count)
            {
                newGroupIndex = data.GroupedNmea.Count - 1;
            }
            if (newGroupIndex < 0)
            {
                newGroupIndex = 0;
            }
            var nmea = data.GroupedNmea[newGroupIndex];

            DS.HighlightedMapDataItemIndex = newIndex;
            DS.HighlightedMapDataItemGroupIndex = newGroupIndex;


            var p = ToPoint(data);
            if (ischanged)
            {
                Log($"Highlight: setting highlighted x={p.X} long0={data.Longitude.AsDecimalFromZero} index={DS.HighlightedMapDataItemIndex}");
            }

            // Move the little circle highlight to be over the point.
            uiCursorHighlight.Visibility = Visibility.Visible;
            Canvas.SetLeft(uiCursorHighlight, p.X - uiCursorHighlight.Width / 2.0);
            Canvas.SetTop(uiCursorHighlight, p.Y - uiCursorHighlight.Height / 2.0);


            // The whole point of this one method is to display this text :-)
            uiPointInfo.Text = nmea.DetailString;

            if (data.GroupedNmea.Count <= 1)
            {
                uiPointInfoMultiPanel.Visibility = Visibility.Collapsed;
                DS.HighlightedMapDataItemGroupIndex = 0;
            }
            else
            {
                uiPointInfoMultiPanel.Visibility = Visibility.Visible;
                uiPointInfoMultiIndex.Text = (DS.HighlightedMapDataItemGroupIndex + 1).ToString(); // Users prefer 1-based indexes.
                uiPointMultiCount.Text = data.GroupedNmea.Count.ToString();
            }
            if (uiPointInfoBorder.Visibility == Visibility.Collapsed)
            {
                uiPointInfoBorder.Visibility = Visibility.Visible;
            }
        }
        void OnClear(object sender, RoutedEventArgs e)
        {
            // TODO: these needs to be top-down, from the GpsControl
            DoClear();
            MapData.Clear();
            DS.ResetScale();
            Canvas.SetLeft(uiMapItemCanvas, uiMapCanvas.ActualWidth / 2.0);
            Canvas.SetTop(uiMapItemCanvas, uiMapCanvas.ActualHeight / 2.0);
            uiPointInfoBorder.Visibility = Visibility.Collapsed;
            uiCursorHighlight.Visibility = Visibility.Collapsed;
        }

        void OnRedraw(object sender, RoutedEventArgs e)
        {
            DoClear(); // Also does DS.Clear() but not DS.ResetScale() and will not remove mapdata
            RedrawAllLines(LogLevel.None);
        }




        private void OnCancelFocus(object sender, RoutedEventArgs e)
        {
            UnloadingCts.Cancel();
        }



        #endregion

        private int RedrawAllLines(LogLevel logLevel)
        {
            int nstartsegments = 0;
            int nendsegments = 0;
            int i;

            if (MapData.Count == 0) return 0; // nothing to redraw.

            MapDataItem data;

            // Set the StartingLatitude and StartingLongitude in a consistant way.
            data = MapData[0];
            DS.StartingLatitude = data.Latitude.AsDecimalFromZero; // Value is 0...180
            DS.StartingLongitude = data.Longitude.AsDecimalFromZero; // Value is 0..360  
            DS.IsFirstPointOfMap = false; // otherwise the first point that we set will be the starting point, which is wrong.

            var nmapdataitem = 0;
            for (i = MapData.Count - 1; i >= 0 && nendsegments < MAX_ENDING_SEGMENTS; i--)
            {
                data = MapData[i];
                var pflag = PointTypeFlag.Normal;
                if (i == MapData.Count - 1) pflag |= PointTypeFlag.IsLastPoint;
                if (i == 0) pflag |= PointTypeFlag.IsFirstPoint;
                nendsegments += DrawMapDataItem(data, LineType.EndingLine, pflag, logLevel);
                nmapdataitem++;
                if (nendsegments > MAX_ENDING_SEGMENTS) break;
            }
            var EarliestEndMapItemIndex = i; // TODO: off by one?
            DS.IsFirstPointOfSegment = true;
            DS.EarliestPointOfLastSegment = DS.LastPoint;
            for (i = 0; i<= EarliestEndMapItemIndex; i++)
            {
                data = MapData[i];
                var pflag = PointTypeFlag.Normal;
                if (i == 0) pflag |= PointTypeFlag.IsFirstPoint;
                nstartsegments += DrawMapDataItem(data, LineType.StartingLine, pflag, logLevel);
                nmapdataitem++;
                if (nstartsegments > MAX_STARTING_SEGMENTS) break;
            }
            // Draw the connector
            if (nmapdataitem < MapData.Count)
            {
                DS.LastPoint = DS.EarliestPointOfLastSegment; // reset the "last point"
                nstartsegments += DrawMapDataItem(MapData[i], LineType.ConnectingLine, PointTypeFlag.Normal, logLevel);
            }

            return nendsegments + nstartsegments;
        }



        /// <summary>
        /// Reinitializes the drawing entirely. When connected to a real GPS unit, will restart the position, clear the cursors, etc.
        /// BUT will not reset the scale or remove MapData values or do any kind of panning
        /// </summary>
        void DoClear()
        {
            DS.Clear(); // but do not reset the scale

            uiMapPositionCanvas.Children.Clear();
            uiMapMarkerCanvas.Children.Clear();
            uiCursorMostRecent.Points.Clear();
            uiCursorStart.Points.Clear();
        }


        /// <summary>
        /// Called when the given MapDataItem has been updated so that there' a new value for the marker size.
        /// </summary>
        /// <param name="prev"></param>
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

        public async Task MapDataUpdatedGroup(MapDataItem prev, LogLevel logLevel)
        {
            var oldw = prev.Dot.Width;
            var marker_size = CalculateMarkerSize(prev);

            // Update drawing
            var delta = (marker_size - oldw) / 2.0;
            if (delta > 0.0)
            {
                Log($"AddNmea: resize dot; move left from {Canvas.GetLeft(prev.Dot)} by {delta}");
                Canvas.SetLeft(prev.Dot, Canvas.GetLeft(prev.Dot) - delta);
                Canvas.SetTop(prev.Dot, Canvas.GetTop(prev.Dot) - delta);
                prev.Dot.Width = marker_size;
                prev.Dot.Height = marker_size;
            }
            else
            {
                Log($"Addnmea: do not resize dot. marker_size={marker_size} oldw={oldw}");
            }
        }

        public async Task MapDataAddedFirstItem(MapDataItem data, LogLevel logLevel)
        {
            DrawMapDataItem(data, LineType.EndingLine, PointTypeFlag.IsFirstPoint, logLevel);
        }
        public async Task MapDataAddedNewItem(MapDataItem data, LogLevel logLevel)
        {
            if (DS.NSegments > MAX_SEGMENTS_HYSTERESIS)
            {
                Log($"AddNmea: must redraw:  DS.NSegments={DS.NSegments} HYS={MAX_SEGMENTS_HYSTERESIS}");
                DoClear(); // Also does DS.Clear() but not DS.ResetScale() and will not remove mapdata
                RedrawAllLines(LogLevel.None);
            }
            DrawMapDataItem(data, LineType.EndingLine, PointTypeFlag.IsLastPoint, logLevel);
        }
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously

        #region DRAWING
        private int DrawMapDataItem(MapDataItem data, LineType lineType, PointTypeFlag pflag, LogLevel logLevel)
        {
            var retval = DrawLatitudeAndLongitude(data, lineType, pflag, logLevel);
            return retval;
        }

        /// <summary>
        /// DrawingState contains all of the values that need to be saved from one call to AddNmea to another.
        /// This does not include the actual data or e.g. the manipulation state.
        /// </summary>
        class DrawingState
        {
            public DrawingState() 
            {
                Clear();
                ScaleFactor = SCALEFACTOR_INIT;
            }
            public void Clear()
            {
                StartingLatitude = double.MaxValue;
                StartingLongitude = double.MaxValue;
                IsFirstPointOfMap = true;
                IsFirstPointOfSegment = true;
                NSegments = 0;
            }
            public void ResetScale()
            {
                ScaleFactor = SCALEFACTOR_INIT;
            }

            /// <summary>
            /// UX value (set by the user) for how much the map is zoomed in.
            /// </summary>
            public double ScaleFactor = SCALEFACTOR_INIT;
            /// <summary>
            /// Says which of the NMEA sample data sets is to be drawn. It's allowed to go beyond the number of data sets, 
            /// so when it's used, it has to be modulo the Nmea sample size.
            /// </summary>
            public int CurrSampleNmea = 0;
            public int HighlightedMapDataItemIndex = -1;
            public int HighlightedMapDataItemGroupIndex = 0; // Which item to display in the highlight panel.


            private DoubleCollection GetDash(LineType lineType)
            {
                // See https://learn.microsoft.com/en-us/windows/windows-app-sdk/api/winrt/microsoft.ui.xaml.shapes.shape.strokedasharray?view=windows-app-sdk-1.7
                // for an explanation of what these do.
                // Weirdly, you can't reuse one of these.

                switch (lineType)
                {
                    case LineType.ConnectingLine:
                        var retval = new DoubleCollection();
                        retval.Add(3);
                        retval.Add(1);
                        retval.Add(1);
                        retval.Add(1);
                        retval.Add(1);
                        retval.Add(1);
                        return retval;
                }
                return null;
            }
            private Brush GetLineBrush(LineType lineType)
            {
                switch (lineType)
                {
                    case LineType.StartingLine: return PositionLineStartingBrush; // As of 2025-06-29, Green
                    default:
                    case LineType.EndingLine: return PositionLineEndingBrush;
                    case LineType.ConnectingLine: return PositionLineBrush3;
                }
            }
            public Line CreateLine(LineType lineType, double x1, double y1, double x2, double y2)
            {
                return new Line()
                {
                    Stroke = GetLineBrush(lineType),
                    StrokeThickness = 1.0,
                    StrokeDashArray = GetDash(lineType),
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                };
            }
            public Line CreateLine(LineType lineType, Point p1, Point p2)
            {
                return new Line()
                {
                    Stroke = GetLineBrush(lineType),
                    StrokeThickness = 1.0,
                    StrokeDashArray = GetDash(lineType),
                    X1 = p1.X,
                    Y1 = p1.Y,
                    X2 = p2.X,
                    Y2 = p2.Y,
                };
            }
            public double StartingLatitude = double.MaxValue;
            public double StartingLongitude = double.MaxValue;
            public Point LastPoint { get; set; }
            public Point EarliestPointOfLastSegment;
            public bool IsFirstPointOfMap = true;
            public bool IsFirstPointOfSegment = true;

            public int NSegments = 0;
        }


        /// <summary>
        /// The resulting point will be a value where x runs from 0 to (large), where a jump of 1 is a smallest value allowed by NMEA
        /// (aka: 00011.2345 and 00011.2346 are different by 1) and is 1/10,000th of a minute, or about .18 meters.
        /// The Y value 0...large where 90N will be 0 and 90S a very large number. Like the X value, the difference between 4511.2345N 
        /// and 4511.2346N is "1". The weird Y system is needed because in XAML, lower Y values are at the top.
        /// AND the lat and longitude are all relative to the starting lat and long
        /// 
        /// Note: point X and Y values can also be negative; they aren't always positive.
        /// </summary>

        private Point LatitudeLongitudeToPoint(double latitude, double longitude)
        {
            var p = new Point(
                DS.ScaleFactor * ((longitude - DS.StartingLongitude) * 60 * 10000),
                DS.ScaleFactor * ((-(latitude - DS.StartingLatitude)) * 60 * 10000)
                );
            return p;
        }

        private double CalculateMarkerSize(MapDataItem data)
        {
            var marker_size = MIN_MARKER_SIZE * data.GroupedNmea.Count;
            marker_size = Math.Min(marker_size, MAX_MARKER_SIZE);
            return marker_size;
        }

        private Point ToPoint(MapDataItem data)
        {
            double latitude = data.Latitude.AsDecimalFromZero; // Value is 0...180
            double longitude = data.Longitude.AsDecimalFromZero; // Value is 0..360    
            var newp = LatitudeLongitudeToPoint(latitude, longitude);
            return newp;

        }

        /// <summary>
        /// Takes in a MapDataItem and plots it. Also uses StartingLatitude which might be double.MaxValue for the first point.
        /// Returns the number of segments drawn (and also adds to the DS.NSegments value)
        /// </summary>
        private int DrawLatitudeAndLongitude(MapDataItem data, LineType lineType, PointTypeFlag pflag, LogLevel logLevel)
        {
            int retval = 0;
            var holdercanvas = uiMapCanvas;
            double latitude = data.Latitude.AsDecimalFromZero; // Value is 0...180
            double longitude = data.Longitude.AsDecimalFromZero; // Value is 0..360    

            if (DS.IsFirstPointOfMap)  // Everything is relative to the first point
            {
                DS.StartingLatitude = latitude;
                DS.StartingLongitude = longitude;
            }
            var newp = LatitudeLongitudeToPoint(latitude, longitude);

            if (!DS.IsFirstPointOfSegment)
            {
                var maxDelta = Math.Max(Math.Abs(DS.LastPoint.X - newp.X), Math.Abs(DS.LastPoint.Y - newp.Y));
                if (maxDelta < MAX_SEGMENT_LENGTH)
                {
                    var line = DS.CreateLine(lineType, DS.LastPoint, newp);
                    uiMapPositionCanvas.Children.Add(line);
                    retval += 1;
                }
                else
                {
                    var x1 = DS.LastPoint.X;
                    var y1 = DS.LastPoint.Y;
                    var nsegment = Math.Round(maxDelta / MAX_SEGMENT_LENGTH);
                    nsegment = Math.Min(nsegment, MAX_SEGMENTS_PER_LINE); // don't go crazy with the segments
                    var dx = (newp.X - DS.LastPoint.X) / nsegment;
                    var dy = (newp.Y - DS.LastPoint.Y) / nsegment;
                    for (double i = 1; i <= nsegment; i++)
                    {
                        var line = DS.CreateLine(lineType, x1, y1, x1+dx, y1+dy);
                        uiMapPositionCanvas.Children.Add(line);

                        x1 = x1 + dx;
                        y1 = y1 + dy;
                        retval += 1;
                    }
                }
            }
            DS.LastPoint = newp;

            var marker_size = CalculateMarkerSize(data);
            var marker = new Ellipse() { Height = marker_size, Width = marker_size, StrokeThickness = 0, Fill = MarkerBrush };
            uiMapMarkerCanvas.Children.Add(marker);
            Canvas.SetLeft(marker, newp.X - marker_size / 2.0);
            Canvas.SetTop(marker, newp.Y - marker_size / 2.0);
            data.Dot = marker; // Note that this is always the first time this is set, and it's always set to the min size

            // Move the cursor
            if (pflag.HasFlag(PointTypeFlag.IsLastPoint))
            {
                uiCursorMostRecent.Points.Clear();
                uiCursorMostRecent.Points.Add(newp);
                uiCursorMostRecent.Points.Add(new Point(newp.X, newp.Y + CURSOR_RADIUS));
                uiCursorMostRecent.Points.Add(new Point(newp.X, newp.Y - CURSOR_RADIUS));
                uiCursorMostRecent.Points.Add(new Point(newp.X, newp.Y));
                uiCursorMostRecent.Points.Add(new Point(newp.X - CURSOR_RADIUS, newp.Y));
                uiCursorMostRecent.Points.Add(new Point(newp.X + CURSOR_RADIUS, newp.Y));
                uiCursorMostRecent.Points.Add(new Point(newp.X, newp.Y));
            }

            if (pflag.HasFlag(PointTypeFlag.IsFirstPoint))
            {
                uiCursorStart.Points.Clear();
                uiCursorStart.Points.Add(newp);
                uiCursorStart.Points.Add(new Point(newp.X, newp.Y + CURSOR_RADIUS));
                uiCursorStart.Points.Add(new Point(newp.X, newp.Y - CURSOR_RADIUS));
                uiCursorStart.Points.Add(new Point(newp.X, newp.Y));
                uiCursorStart.Points.Add(new Point(newp.X - CURSOR_RADIUS, newp.Y));
                uiCursorStart.Points.Add(new Point(newp.X + CURSOR_RADIUS, newp.Y));
                uiCursorStart.Points.Add(new Point(newp.X, newp.Y));
            }
            DS.IsFirstPointOfMap = false;
            DS.IsFirstPointOfSegment = false;
            switch (logLevel)
            {
                case LogLevel.Normal:
                    Log($"{newp.X:F2} {newp.Y:F2} ");
                    break;
            }
            DS.NSegments += retval;
            return retval;
        }
        #endregion


        private void Log(string message)
        {
            uiLogSMC.Text = message;
            System.Diagnostics.Debug.WriteLine(message);
        }

        public async Task PrivacyUpdated()
        {
            await Task.Delay(0); // This is needed to make this method async, but it doesn't do anything.
            return;
        }
    }
}
