using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Bluetooth.Background;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using static System.Runtime.InteropServices.JavaScript.JSType;


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
            this.InitializeComponent();
            this.Loaded += SimpleMapControl_Loaded;
        }

        List<MapDataItem> MapData = new List<MapDataItem>();

        private void SimpleMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            OnAddFakePoints(null, null);
        }

        private void OnViewChanged(object sender, ScrollViewerViewChangedEventArgs e)
        {
            if (e.IsIntermediate) return;
            SetZoom();
        }

        private void OnViewChanged(ScrollView sender, object args)
        {
            SetZoom();
        }
        private double RegularLineThickness {  
            get 
            { 
                var retval = 1.5 / uiZoom.ZoomFactor;
                retval = Math.Max(.2, retval);
                return retval;
            } 
        }
        private void SetZoom()
        {
            // If the zoom level changed, change the line size
            var newzoom = RegularLineThickness;

            uiMapPositionPolyline.StrokeThickness = newzoom;
            uiCursorMostRecent.StrokeThickness = newzoom;
            uiCursorStart.StrokeThickness = newzoom;
            Log($"Zoom: zoom={uiZoom.ZoomFactor} thickness={newzoom}");

        }

        void OnClear(object sender, RoutedEventArgs e)
        {
            uiMapPositionPolyline.Points.Clear();
            MapData.Clear();
        }

        void OnRedraw(object sender, RoutedEventArgs e)
        {
            uiMapPositionPolyline.Points.Clear();
            foreach (var data in MapData)
            {
                DrawMapDataItem(data);
            }
        }


        void OnAddFakePoints(object sender, RoutedEventArgs e)
        {
            var lines = FakeNmea[CurrFakeNmea].Split("\r\n");
            foreach (var line in lines)
            {
                var gprc = new GPRMC_Data(line);
                AddNmea(gprc);
            }

            // Boop the index so next time the button is pressed we get the next set of data.
            CurrFakeNmea += 1;
            if (CurrFakeNmea >= FakeNmea.Length)
            {
                CurrFakeNmea = 0;
            }
        }

        public void AddNmea(GPRMC_Data gprc)
        {
            switch (gprc.ParseStatus)
            {
                case Nmea_Gps_Parser.ParseResult.Ok:
                    var data = new MapDataItem(gprc);
                    MapData.Add(data);
                    DrawMapDataItem(data);
                    //AddLatitudeAndLongitude(mdi);
                    break;
                default:
                    Log($"Error: FakePoints parse {gprc.ParseStatus} data {gprc.OriginalNmeaString}");
                    break;
            }
        }
        public void DrawMapDataItem(MapDataItem data)
        {
            AddLatitudeAndLongitude(data);
        }


        class DrawingState
        {
            public double StartingLatitude = double.MaxValue;
            public double StartingLongitude = double.MaxValue;
            public double MinX = 0.0;
            public double MaxX = 00;
            public double MinY = 0.0;
            public double MaxY = 0.0;

            public double DeltaMinX = 0.0;
            public double DeltaMinY = 0.0;
            public bool UpdateMinMax(double x, double y)
            {
                bool retval = false;
                DeltaMinX = 0.0;
                DeltaMinY = 0.0;
                if (x < MinX) { DeltaMinX = MinX - x;  MinX = x; retval = true; }
                if (x > MaxX) { MaxX = x; retval = true; }
                if (y < MinY) { DeltaMinY = MinY - y;  MinY = y; retval = true; }
                if (y > MaxY) { MaxY = y; retval = true; }
                return retval;
            }
            public double Width {  get { return MaxX - MinX; } }
            public double Height {  get { return MaxY - MinY; } }
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
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        private Point LatitudeLongitudeToPoint(double latitude, double longitude)
        {
            var p = new Point(
                ((longitude - DS.StartingLongitude) * 60 * 10000),
                ((-(latitude - DS.StartingLatitude)) * 60 * 10000)
                );
            return p;
        }

        Brush MarkerBrush = new SolidColorBrush(Colors.Red);

        /// <summary>
        /// Takes in a MapDataItem and plots it. Also uses StartingLatitude which might be double.MaxValue for the first point.
        /// </summary>

        public void AddLatitudeAndLongitude(MapDataItem data)
        {
            var pline = uiMapPositionPolyline;
            var line = pline.Points;

            double latitude = data.Latitude.AsDecimal + 90; // so it's 0...180
            double longitude = data.Longitude.AsDecimal + 180; // so it's 0..360    


            if (line.Count == 0) // Is the first point
            {
                DS.StartingLatitude = latitude;
                DS.StartingLongitude = longitude;
                SetZoom(); // This is the ideal time to set the starting zoom level
            }
            var newp = LatitudeLongitudeToPoint(latitude , longitude);
            line.Add(newp);
            var sizeChanged = DS.UpdateMinMax(newp.X, newp.Y);

            // Now throw in the adjustments
            if (sizeChanged)
            {
                uiMapCanvas.Width += DS.Width + PADX + PADX;
                uiMapMarkerCanvas.Width = uiMapCanvas.Width;

                uiMapCanvas.Height = DS.Height + PADY + PADY;
                uiMapMarkerCanvas.Height = uiMapCanvas.Height;

                if (DS.DeltaMinX != 0)
                {
                    Canvas.SetLeft(pline, -DS.MinX + PADX);
                    foreach (var child in uiMapMarkerCanvas.Children)
                    {
                        Canvas.SetLeft(child, Canvas.GetLeft(child) + DS.DeltaMinX);
                    }
                }
                Canvas.SetTop(pline, -DS.MinY + PADY);
                if (DS.DeltaMinY != 0)
                {
                    foreach (var child in uiMapMarkerCanvas.Children)
                    {
                        Canvas.SetTop(child, Canvas.GetTop(child) + DS.DeltaMinY);
                    }
                }
            }

            // TODO: and move all the points in the uiMapMarkerCanvas?

            const double MarkerSize = 3;
            var marker = new Ellipse() { Height = MarkerSize, Width = MarkerSize, StrokeThickness = 0, Fill = MarkerBrush };
            uiMapMarkerCanvas.Children.Add(marker);
            Canvas.SetLeft(marker, newp.X - MarkerSize / 2.0 - DS.MinX + PADX);
            Canvas.SetTop(marker, newp.Y - MarkerSize / 2.0 - DS.MinY + PADY);

            // Move the cursor
            var lastp = line[line.Count - 1];
            var CS = 4;
            uiCursorMostRecent.Points.Clear();
            uiCursorMostRecent.Points.Add(lastp);
            uiCursorMostRecent.Points.Add(new Point(lastp.X, lastp.Y+CS));
            uiCursorMostRecent.Points.Add(new Point(lastp.X, lastp.Y-CS));
            uiCursorMostRecent.Points.Add(new Point(lastp.X, lastp.Y));
            uiCursorMostRecent.Points.Add(new Point(lastp.X-CS, lastp.Y));
            uiCursorMostRecent.Points.Add(new Point(lastp.X+CS, lastp.Y));
            uiCursorMostRecent.Points.Add(new Point(lastp.X, lastp.Y));
            Canvas.SetLeft(uiCursorMostRecent, -DS.MinX + PADX);
            Canvas.SetTop(uiCursorMostRecent, -DS.MinY + PADY);

            lastp = line[0];
            uiCursorStart.Points.Clear();
            uiCursorStart.Points.Add(lastp);
            uiCursorStart.Points.Add(new Point(lastp.X, lastp.Y + CS));
            uiCursorStart.Points.Add(new Point(lastp.X, lastp.Y - CS));
            uiCursorStart.Points.Add(new Point(lastp.X, lastp.Y));
            uiCursorStart.Points.Add(new Point(lastp.X - CS, lastp.Y));
            uiCursorStart.Points.Add(new Point(lastp.X + CS, lastp.Y));
            uiCursorStart.Points.Add(new Point(lastp.X, lastp.Y));
            Canvas.SetLeft(uiCursorStart, -DS.MinX + PADX);
            Canvas.SetTop(uiCursorStart, -DS.MinY + PADY);


            // Fix up the ratio
            var ratio = uiMapCanvas.Width / uiMapCanvas.Height;
            if (ratio < 1.95)
            {
                // Too skinny. Make it wider
                uiMapCanvas.Width = uiMapCanvas.Height * RATIO_GOAL;
                uiMapMarkerCanvas.Width = uiMapCanvas.Width;
            }
            else if (ratio > 2.05) // Too wide. Make it taller
            {
                uiMapCanvas.Height = uiMapCanvas.Width / RATIO_GOAL;
                uiMapCanvas.Height = uiMapCanvas.Height;
            }

            Log($"{newp.X:F2} {newp.Y:F2} ");
        }

        private void SetMostRecent()
        {

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
