using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.Foundation.Diagnostics;



// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

namespace WinUI3Controls
{
    public class MapDataItem
    {
        public Nmea_Latitude_Fields Latitude;
        public Nmea_Longitude_Fields Longitude;
        public string Summary;
        public string Detail;

        public MapDataItem(GPRMC_Data nmea) 
        {
            Latitude = nmea.Latitude;
            Longitude = nmea.Longitude;
            Summary = nmea.SummaryString;
            Detail = nmea.DetailString;
        }
    }

    public sealed partial class SimpleMapControl : UserControl
    {
        public SimpleMapControl()
        {
            InitZoom();
            this.InitializeComponent();
            this.Loaded += SimpleMapControl_Loaded;
        }

        List<MapDataItem> MapData = new List<MapDataItem>();

        private void SimpleMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(uiMapItemCanvas, PADX);
            Canvas.SetTop(uiMapItemCanvas, PADY);

            //OnAddFakePoints(null, null);
        }
        private void OnMapCanvasSizeChanged(object sender, SizeChangedEventArgs e)
        {
            var rect = new RectangleGeometry();
            rect.Rect = new Rect(0, 0, uiMapCanvas.ActualWidth, uiMapCanvas.ActualHeight);
            uiMapCanvas.Clip = rect;
        }

        int NPointReleasedSuppress = 0;
        private void OnManipulationComplete(object sender, Microsoft.UI.Xaml.Input.ManipulationCompletedRoutedEventArgs e)
        {
            NPointReleasedSuppress++;

            var newscale = e.Cumulative.Scale;
            var startLeft = Canvas.GetLeft(uiMapItemCanvas);
            var startTop = Canvas.GetTop(uiMapItemCanvas);

            if (newscale != 1.0)
            {
                // Where is the final point?
                var touchPosition = e.Position; // "the" touch position which isn't obvious for pinchy gestures.
                var startLat = (touchPosition.X - startLeft) / ZoomFactor;
                var startLong = (touchPosition.Y - startTop) / ZoomFactor;

                // Zooming
                ZoomFactor *= newscale;

                var newStartLeft = touchPosition.X - (startLat * ZoomFactor);
                var newStartTop = touchPosition.Y - (startLong * ZoomFactor);

                UpdateZoom();
                Canvas.SetLeft(uiMapItemCanvas, newStartLeft);
                Canvas.SetTop(uiMapItemCanvas, newStartTop);

                Log($"Complete: scale={ZoomFactor:F2} delta={newscale:F2} lat={startLat:F2} start x={startLeft:F2} y={startTop:F2}");
            }
            else
            {
                var dx = e.Cumulative.Translation.X;
                var dy = e.Cumulative.Translation.Y;
                // Panning
                Canvas.SetLeft(uiMapItemCanvas, startLeft + dx);
                Canvas.SetTop(uiMapItemCanvas, startTop + dy);
                Log($"Complete: x={dx:F2} oldx={startLeft:F2} y={dy:F2} oldy={startTop:F2}");
            }
        }

        private void OnPointerReleased(object sender, Microsoft.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            if (NPointReleasedSuppress <= 0)
            {
                var startLeft = Canvas.GetLeft(uiMapItemCanvas);
                var startTop = Canvas.GetTop(uiMapItemCanvas);

                var touchPosition = e.GetCurrentPoint(uiZoom).Position; // "the" touch position which isn't obvious for pinchy gestures.

                // where is the touchPosition in lat/long?
                var lat = (touchPosition.X - startLeft) / ZoomFactor;



                Log($"Release: touch x={touchPosition.X:F2} start x={startLeft:F2} lat={lat:F2} ");
            }
            NPointReleasedSuppress--;
            if (NPointReleasedSuppress < 0) NPointReleasedSuppress = 0;
        }

        private void OnManipulationStarting(object sender, Microsoft.UI.Xaml.Input.ManipulationStartingRoutedEventArgs e)
        {
        }

        private void OnManipulationStart(object sender, Microsoft.UI.Xaml.Input.ManipulationStartedRoutedEventArgs e)
        {
        }
        public enum LogLevel {  Normal, None};
        private double ZoomFactor { get; set; } = 1.0;
        private double RegularLineThickness { get; } = 1;
        private void UpdateZoom()
        {
            // InitZoom();
            DS.Clear();

            //uiMapPositionPolyline.Points.Clear();
            uiMapPositionCanvas.Children.Clear();
            uiMapMarkerCanvas.Children.Clear();
            uiCursorMostRecent.Points.Clear();
            uiCursorStart.Points.Clear();

            int nsegments = 0;
            foreach (var data in MapData)
            {
                nsegments += DrawMapDataItem(data, LogLevel.None);
            }
            Log($"UpdateZoom: nsegment={nsegments}");
        }

        private void InitZoom()
        {
            ZoomFactor = 1.0;
        }

        void OnClear(object sender, RoutedEventArgs e)
        {
            uiMapPositionCanvas.Children.Clear();
            MapData.Clear();
        }

        void OnRedraw(object sender, RoutedEventArgs e)
        {
            uiMapPositionCanvas.Children.Clear();
            int nsegments = 0;
            foreach (var data in MapData)
            {
                nsegments += DrawMapDataItem(data, LogLevel.None);
            }
        }


        void OnAddFakePoints(object sender, RoutedEventArgs e)
        {
            int nsegments = 0;
            var lines = FakeNmea[CurrFakeNmea % FakeNmea.Length].Split("\r\n");
            foreach (var line in lines)
            {
                var gprc = new GPRMC_Data(line);
                gprc.Latitude.LatitudeMinutesDecimal += (CurrFakeNmea * 50);
                nsegments += AddNmea(gprc, LogLevel.None);
            }

            // Boop the index so next time the button is pressed we get the next set of data.
            CurrFakeNmea += 1;
            Log($"Add fake points: nsegments={nsegments}");
        }

        public int AddNmea(GPRMC_Data gprc, LogLevel logLevel = LogLevel.Normal)
        {
            int nsegments = 0;
            switch (gprc.ParseStatus)
            {
                case Nmea_Gps_Parser.ParseResult.Ok:
                    var data = new MapDataItem(gprc);
                    MapData.Add(data);
                    nsegments += DrawMapDataItem(data, logLevel);
                    break;
                default:
                    Log($"Error: FakePoints parse {gprc.ParseStatus} data {gprc.OriginalNmeaString}");
                    break;
            }
            return nsegments;
        }
        public int DrawMapDataItem(MapDataItem data, LogLevel logLevel)
        {
            var retval = AddLatitudeAndLongitude(data, logLevel);
            return retval;
        }


        class DrawingState
        {
            public DrawingState() { Clear(); }
            public void Clear()
            {
                StartingLatitude = double.MaxValue;
                StartingLongitude = double.MaxValue;
                IsFirstPoint = true;
            }
            public Line Line(double x1, double y1, double x2, double y2)
            {
                return new Line()
                {
                    Stroke = PositionLineBrush,
                    StrokeThickness = 1.0,
                    X1 = x1,
                    Y1 = y1,
                    X2 = x2,
                    Y2 = y2,
                };
            }
            public Line Line(Point p1, Point p2)
            {
                return new Line()
                {
                    Stroke = PositionLineBrush,
                    StrokeThickness = 1.0,
                    X1 = p1.X,
                    Y1 = p1.Y,
                    X2 = p2.X,
                    Y2 = p2.Y,
                };
            }
            public double StartingLatitude = double.MaxValue;
            public double StartingLongitude = double.MaxValue;
            public Point P1 { get; set; }
            public bool IsFirstPoint = true;

        }
        DrawingState DS = new DrawingState();

        // Constants for the size and shape of the map area
        const double RATIO_GOAL = 2.0; // E.g, 2.0 means twice as wide as it is high
        const double PADX = 200;
        const double PADY = PADX / RATIO_GOAL;


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
                ZoomFactor * ((longitude - DS.StartingLongitude) * 60 * 10000),
                ZoomFactor * ((-(latitude - DS.StartingLatitude)) * 60 * 10000)
                );
            return p;
        }

        Brush MarkerBrush = new SolidColorBrush(Colors.Red);
        static Brush PositionLineBrush = new SolidColorBrush(Colors.Blue);

        /// <summary>
        /// Takes in a MapDataItem and plots it. Also uses StartingLatitude which might be double.MaxValue for the first point.
        /// </summary>
        public int AddLatitudeAndLongitude(MapDataItem data, LogLevel logLevel)
        {
            int retval = 0;
            var holdercanvas = uiMapCanvas;

            double latitude = data.Latitude.AsDecimal + 90; // so it's 0...180
            double longitude = data.Longitude.AsDecimal + 180; // so it's 0..360    


            if (DS.IsFirstPoint) // current_points.Count == 0) // Is the first point
            {
                DS.StartingLatitude = latitude;
                DS.StartingLongitude = longitude;
                // InitZoom(); // This is the ideal time to set the starting zoom level
            }
            var newp = LatitudeLongitudeToPoint(latitude, longitude);

            if (!DS.IsFirstPoint)
            {

                var maxDelta = Math.Max(Math.Abs(DS.P1.X - newp.X), Math.Abs(DS.P1.Y - newp.Y));
                const double MAX_SEGMENT_LENGTH = 100.0;
                if (maxDelta < MAX_SEGMENT_LENGTH)
                {
                    var line = DS.Line(DS.P1, newp);
                    uiMapPositionCanvas.Children.Add(line);
                    retval += 1;
                }
                else
                {
                    var x1 = DS.P1.X;
                    var y1 = DS.P1.Y;
                    var nsegment = Math.Round(maxDelta / MAX_SEGMENT_LENGTH);
                    var dx = (newp.X - DS.P1.X) / nsegment;
                    var dy = (newp.Y - DS.P1.Y) / nsegment;
                    for (double i = 1; i <= nsegment; i++)
                    {
                        var line = DS.Line(x1, y1, x1+dx, y1+dy);
                        uiMapPositionCanvas.Children.Add(line);

                        x1 = x1 + dx;
                        y1 = y1 + dy;
                        retval += 1;
                    }
                }
            }
            DS.P1 = newp;

            const double MarkerSize = 3;
            var marker = new Ellipse() { Height = MarkerSize, Width = MarkerSize, StrokeThickness = 0, Fill = MarkerBrush };
            uiMapMarkerCanvas.Children.Add(marker);
            Canvas.SetLeft(marker, newp.X - MarkerSize / 2.0);
            Canvas.SetTop(marker, newp.Y - MarkerSize / 2.0);

            // Move the cursor
            var lastp = newp; //  current_points[current_points.Count - 1];
            var CS = 4;
            uiCursorMostRecent.Points.Clear();
            uiCursorMostRecent.Points.Add(lastp);
            uiCursorMostRecent.Points.Add(new Point(lastp.X, lastp.Y + CS));
            uiCursorMostRecent.Points.Add(new Point(lastp.X, lastp.Y - CS));
            uiCursorMostRecent.Points.Add(new Point(lastp.X, lastp.Y));
            uiCursorMostRecent.Points.Add(new Point(lastp.X - CS, lastp.Y));
            uiCursorMostRecent.Points.Add(new Point(lastp.X + CS, lastp.Y));
            uiCursorMostRecent.Points.Add(new Point(lastp.X, lastp.Y));

            if (uiCursorStart.Points.Count == 0)
            {
                lastp = newp; // current_points[0];
                uiCursorStart.Points.Clear();
                uiCursorStart.Points.Add(lastp);
                uiCursorStart.Points.Add(new Point(lastp.X, lastp.Y + CS));
                uiCursorStart.Points.Add(new Point(lastp.X, lastp.Y - CS));
                uiCursorStart.Points.Add(new Point(lastp.X, lastp.Y));
                uiCursorStart.Points.Add(new Point(lastp.X - CS, lastp.Y));
                uiCursorStart.Points.Add(new Point(lastp.X + CS, lastp.Y));
                uiCursorStart.Points.Add(new Point(lastp.X, lastp.Y));
            }
            DS.IsFirstPoint = false;
            switch (logLevel)
            {
                case LogLevel.Normal:
                    Log($"{newp.X:F2} {newp.Y:F2} ");
                    break;
            }
            return retval;
        }

        class PointsData
        {
            public double MinX, MinY;
            public double MaxX, MaxY;
            public double Width { get { return MaxX - MinX; } }
            public double Height { get { return MaxY - MinY; } }
            public PointsData(PointCollection lines)
            {
                if (lines.Count < 1)
                {
                    MinX = 0; MinY = 0; MaxX = 0; MaxY = 0;
                    return;
                }
                MinX = lines[0].X;
                MinY = lines[0].Y;
                MaxX = lines[0].X;
                MaxY = lines[0].Y;

                for (int i = 1; i < lines.Count; i++)
                {
                    MinX = Math.Min(MinX, lines[i].X);
                    MinY = Math.Min(MinY, lines[i].Y);
                    MaxX = Math.Max(MaxX, lines[i].X);
                    MaxY = Math.Max(MaxY, lines[i].Y);
                }
            }
        }

        private void Log(string message)
        {
            uiLog.Text = message;
            System.Diagnostics.Debug.WriteLine(message);
        }


        private int CurrFakeNmea = 0;
        private string[] FakeNmea = new string[] {

            @"$GPRMC,184446.000,A,4700.6036,N,12200.8008,W,000.0,338.7,200625,,,A
$GPRMC,184451.000,A,4700.6033,N,12200.8012,W,000.9,125.1,200625,,,A
$GPRMC,184504.000,A,4700.6030,N,12200.8008,W,000.0,126.8,200625,,,A
$GPRMC,184508.000,A,4700.6033,N,12200.8004,W,001.0,069.3,200625,,,A
$GPRMC,184509.000,A,4700.6036,N,12200.8008,W,001.1,067.8,200625,,,A
$GPRMC,184510.000,A,4700.6032,N,12200.8003,W,001.4,076.4,200625,,,A
$GPRMC,184511.000,A,4700.6035,N,12200.8078,W,001.8,050.0,200625,,,A
$GPRMC,184512.000,A,4700.6039,N,12200.8067,W,001.2,062.0,200625,,,A
$GPRMC,184513.000,A,4700.6041,N,12200.8063,W,000.9,065.0,200625,,,A
$GPRMC,184514.000,A,4700.6040,N,12200.8062,W,001.2,121.7,200625,,,A
$GPRMC,184515.000,A,4700.6035,N,12200.8057,W,002.5,070.8,200625,,,A
$GPRMC,184516.000,A,4700.6035,N,12200.8045,W,001.5,128.2,200625,,,A
$GPRMC,184517.000,A,4700.6032,N,12200.8040,W,001.5,137.7,200625,,,A
$GPRMC,184518.000,A,4700.6029,N,12200.8036,W,001.5,151.7,200625,,,A
$GPRMC,184519.000,A,4700.6025,N,12200.8033,W,000.0,151.7,200625,,,A
$GPRMC,184520.000,A,4700.6023,N,12200.8032,W,000.0,151.7,200625,,,A
$GPRMC,184521.000,A,4700.6021,N,12200.8033,W,000.0,151.7,200625,,,A
$GPRMC,184522.000,A,4700.6020,N,12200.8033,W,001.2,306.5,200625,,,A
$GPRMC,184523.000,A,4700.6022,N,12200.8037,W,000.0,306.5,200625,,,A",

            @"$GPRMC,184446.000,A,4700.4036,N,12200.8008,W,000.0,338.7,200625,,,A
$GPRMC,184451.000,A,4700.4033,N,12200.8012,W,000.9,125.1,200625,,,A
$GPRMC,184504.000,A,4700.4030,N,12200.8008,W,000.0,126.8,200625,,,A
$GPRMC,184508.000,A,4700.4033,N,12200.8004,W,001.0,069.3,200625,,,A
$GPRMC,184509.000,A,4700.4036,N,12200.8008,W,001.1,067.8,200625,,,A
$GPRMC,184510.000,A,4700.4032,N,12200.8003,W,001.4,076.4,200625,,,A
$GPRMC,184511.000,A,4700.4035,N,12200.8078,W,001.8,050.0,200625,,,A
$GPRMC,184512.000,A,4700.4039,N,12200.8067,W,001.2,062.0,200625,,,A
$GPRMC,184513.000,A,4700.4041,N,12200.8063,W,000.9,065.0,200625,,,A
$GPRMC,184514.000,A,4700.4040,N,12200.8062,W,001.2,121.7,200625,,,A
$GPRMC,184515.000,A,4700.4035,N,12200.8057,W,002.5,070.8,200625,,,A
$GPRMC,184516.000,A,4700.4035,N,12200.8045,W,001.5,128.2,200625,,,A
$GPRMC,184517.000,A,4700.4032,N,12200.8040,W,001.5,137.7,200625,,,A
$GPRMC,184518.000,A,4700.4029,N,12200.8036,W,001.5,151.7,200625,,,A
$GPRMC,184519.000,A,4700.4025,N,12200.8033,W,000.0,151.7,200625,,,A
$GPRMC,184520.000,A,4700.4023,N,12200.8032,W,000.0,151.7,200625,,,A
$GPRMC,184521.000,A,4700.4021,N,12200.8033,W,000.0,151.7,200625,,,A
$GPRMC,184522.000,A,4700.4020,N,12200.8033,W,001.2,306.5,200625,,,A
$GPRMC,184523.000,A,4700.4022,N,12200.8037,W,000.0,306.5,200625,,,A",

            // Same figure, but to the left
            @"$GPRMC,184446.000,A,4700.4036,N,12201.8008,W,000.0,338.7,200625,,,A
$GPRMC,184451.000,A,4700.4033,N,12201.8012,W,000.9,125.1,200625,,,A
$GPRMC,184504.000,A,4700.4030,N,12201.8008,W,000.0,126.8,200625,,,A
$GPRMC,184508.000,A,4700.4033,N,12201.8004,W,001.0,069.3,200625,,,A
$GPRMC,184509.000,A,4700.4036,N,12201.8008,W,001.1,067.8,200625,,,A
$GPRMC,184510.000,A,4700.4032,N,12201.8003,W,001.4,076.4,200625,,,A
$GPRMC,184511.000,A,4700.4035,N,12201.8078,W,001.8,050.0,200625,,,A
$GPRMC,184512.000,A,4700.4039,N,12201.8067,W,001.2,062.0,200625,,,A
$GPRMC,184513.000,A,4700.4041,N,12201.8063,W,000.9,065.0,200625,,,A
$GPRMC,184514.000,A,4700.4040,N,12201.8062,W,001.2,121.7,200625,,,A
$GPRMC,184515.000,A,4700.4035,N,12201.8057,W,002.5,070.8,200625,,,A
$GPRMC,184516.000,A,4700.4035,N,12201.8045,W,001.5,128.2,200625,,,A
$GPRMC,184517.000,A,4700.4032,N,12201.8040,W,001.5,137.7,200625,,,A
$GPRMC,184518.000,A,4700.4029,N,12201.8036,W,001.5,151.7,200625,,,A
$GPRMC,184519.000,A,4700.4025,N,12201.8033,W,000.0,151.7,200625,,,A
$GPRMC,184520.000,A,4700.4023,N,12201.8032,W,000.0,151.7,200625,,,A
$GPRMC,184521.000,A,4700.4021,N,12201.8033,W,000.0,151.7,200625,,,A
$GPRMC,184522.000,A,4700.4020,N,12201.8033,W,001.2,306.5,200625,,,A
$GPRMC,184523.000,A,4700.4022,N,12201.8037,W,000.0,306.5,200625,,,A",

        };


    }
}
