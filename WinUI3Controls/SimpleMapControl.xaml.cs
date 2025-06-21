using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Parsers.Nmea;


// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif

namespace WinUI3Controls
{
    public sealed partial class SimpleMapControl : UserControl
    {
        public SimpleMapControl()
        {
            this.InitializeComponent();
            this.Loaded += SimpleMapControl_Loaded;
        }

        private void SimpleMapControl_Loaded(object sender, RoutedEventArgs e)
        {
            OnAddFakePoints(null, null);
        }

        Random r = new Random();
        void OnAddRandomPoints(object sender, RoutedEventArgs e)
        {
            var line = uiMapPositionPolyline.Points;
            Log($"random: npoints={line.Count}");
            for (int i = 0; i < 10; i++)
            {
                var lastPoint = line.Last();
                var x = Math.Min(360, Math.Max(0, lastPoint.X + ((r.NextDouble()-0.5) * 5)));
                var y = Math.Min(180, Math.Max(0, lastPoint.Y + ((r.NextDouble()-0.5) * 5)));
                line.Add(new Point(x, y));
            }
        }
        void OnAddFakePoints(object sender, RoutedEventArgs e)
        {
            var lines = FakeNmea.Split("\r\n");
            foreach (var line in lines)
            {
                var gprc = new GPRMC_Data(line);
                switch (gprc.ParseStatus )
                {
                    case Nmea_Gps_Parser.ParseResult.Ok:
                        AddLatitudeAndLongitude(gprc.Latitude.AsDecimal, gprc.Longitude.AsDecimal);
                        break;
                    default:
                        Log($"Error: FakePoints parse {gprc.ParseStatus} data {line}");
                        break;
                }
            }


        }


        private double StartingLatitude = double.MaxValue;
        private double StartingLongitude = double.MaxValue;

        private double OffsetX = 1.40;
        private double OffsetY = 80;

        /// <summary>
        /// Values are in decimal -180..180 and -90..90
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void AddLatitudeAndLongitude(double latitude, double longitude)
        {
            latitude += 90; // so it's 0...180
            longitude += 180; // so it's 0..360

            const double PADX = .100;
            const double PADY = .100;

            if (StartingLatitude == double.MaxValue)
            {
                StartingLatitude = latitude;
                StartingLongitude = longitude;
            }
            latitude = latitude - StartingLatitude;
            longitude = longitude - StartingLongitude;

            // "1" will be the minimum value allowed by the NMEA spec and is about .18 meters
            latitude *= 60 * 10000;
            longitude *= 60 * 10000;

            latitude += OffsetY;
            longitude += OffsetX;

            var newzoom = 0.5 / uiZoom.ZoomFactor;
            uiMapPositionPolyline.StrokeThickness = newzoom;
            var line = uiMapPositionPolyline.Points;
            line.Add(new Point(longitude, latitude));

            // Now throw in the adjustments
            var pd = new PointsData(line);
            if (pd.MaxX > uiMapCanvas.Width)
            {
                uiMapCanvas.Width += pd.MaxX;
            }
            if (pd.MaxY > uiMapCanvas.Height)
            {
                uiMapCanvas.Height += pd.MaxY;
            }
            if (pd.MinX < 0 || pd.MinY < 0)
            {
                var addx = (pd.MinX >= 0) ? 0 : -pd.MinX + PADX;
                var addy = (pd.MinY >= 0) ? 0 : -pd.MinY + PADY;
                OffsetX += addx > 0 ? addx : 0;
                OffsetY += addy > 0 ? addy : 0;
                PointCollection newpoints = new PointCollection();
                foreach (var oldpoint in line)
                {
                    var p = new Point(oldpoint.X + addx, oldpoint.Y + addy);
                    newpoints.Add(p);
                }

                Log($"SLIDE! {addx} {addy}  at {latitude:F2} {longitude:F2} zoom={uiZoom.ZoomFactor} thickness={newzoom}");
                line = newpoints; // just in case we want to reuse

                uiMapPositionPolyline.Points = newpoints;
                uiMapCanvas.Width += addx;
                uiMapCanvas.Height += addy;
            }

            const double RATIO_GOAL = 2.0;
            var ratio = uiMapCanvas.Width / uiMapCanvas.Height;
            if (ratio < 1.95)
            {
                // Too skinny. Make it wider
                uiMapCanvas.Width = uiMapCanvas.Height * RATIO_GOAL;
            }
            else if (ratio > 2.05) // Too wide. Make it taller
            {
                uiMapCanvas.Height = uiMapCanvas.Width / RATIO_GOAL;
            }

                Log($"{latitude:F2} {longitude:F2} zoom={uiZoom.ZoomFactor} thickness={newzoom}");
        }

        class PointsData
        {
            public double MinX, MinY;
            public double MaxX, MaxY;
            public double Width { get { return MaxX - MinX; } }
            public double Height { get { return MaxY - MinY; } }
            public PointsData (PointCollection lines)
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

                for (int i=1; i<lines.Count; i++)
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
            uiLog.Text= message;
            System.Diagnostics.Debug.WriteLine(message);
        }

        // All data here has been altered to improve privacy
        private string ZZZFakeNmea = @"$GPRMC,164512.000,A,5555.1118,N,05555.2120,W,000.0,126.8,200625,,,A
$GPRMC,164422.000,A,5555.1120,N,05555.2140,W,000.9,125.1,200625,,,A
$GPRMC,164524.000,A,5555.1122,N,05555.2150,W,000.0,126.8,200625,,,A
$GPRMC,164528.000,A,5555.1124,N,05555.2160,W,001.0,069.3,200625,,,A
$GPRMC,164529.000,A,5555.1126,N,05555.2170,W,001.1,067.8,200625,,,A
$GPRMC,164520.000,A,5555.1128,N,05555.2180,W,001.4,076.4,200625,,,A
$GPRMC,164521.000,A,5555.1130,N,05555.2190,W,001.8,050.0,200625,,,A
$GPRMC,164522.000,A,5555.1132,N,05555.2200,W,001.2,062.0,200625,,,A
$GPRMC,164523.000,A,5555.1134,N,05555.2210,W,000.9,065.0,200625,,,A
$GPRMC,164524.000,A,5555.1136,N,05555.2220,W,001.2,121.7,200625,,,A
$GPRMC,164525.000,A,5555.1138,N,05555.2230,W,002.5,070.8,200625,,,A
$GPRMC,164526.000,A,5555.1140,N,05555.2250,W,001.5,128.2,200625,,,A"; 
        
        
        private string FakeNmea = @"$GPRMC,184446.000,A,4739.6736,N,12207.8208,W,000.0,338.7,200625,,,A*72
$GPRMC,184451.000,A,4739.6733,N,12207.8209,W,000.9,125.1,200625,,,A*71
$GPRMC,184504.000,A,4739.6733,N,12207.8209,W,000.0,126.8,200625,,,A*73
$GPRMC,184508.000,A,4739.6732,N,12207.8209,W,001.0,069.3,200625,,,A*7E
$GPRMC,184509.000,A,4739.6732,N,12207.8208,W,001.1,067.8,200625,,,A*7A
$GPRMC,184510.000,A,4739.6732,N,12207.8203,W,001.4,076.4,200625,,,A*70
$GPRMC,184511.000,A,4739.6735,N,12207.8178,W,001.8,050.0,200625,,,A*75
$GPRMC,184512.000,A,4739.6739,N,12207.8167,W,001.2,062.0,200625,,,A*7F
$GPRMC,184513.000,A,4739.6741,N,12207.8163,W,000.9,065.0,200625,,,A*78
$GPRMC,184514.000,A,4739.6740,N,12207.8162,W,001.2,121.7,200625,,,A*73
$GPRMC,184515.000,A,4739.6735,N,12207.8157,W,002.5,070.8,200625,,,A*78
$GPRMC,184516.000,A,4739.6735,N,12207.8145,W,001.5,128.2,200625,,,A*7D
$GPRMC,184517.000,A,4739.6732,N,12207.8140,W,001.5,137.7,200625,,,A*75
$GPRMC,184518.000,A,4739.6729,N,12207.8136,W,001.5,151.7,200625,,,A*71
$GPRMC,184519.000,A,4739.6725,N,12207.8133,W,000.0,151.7,200625,,,A*7D
$GPRMC,184520.000,A,4739.6723,N,12207.8132,W,000.0,151.7,200625,,,A*70
$GPRMC,184521.000,A,4739.6721,N,12207.8133,W,000.0,151.7,200625,,,A*72
$GPRMC,184522.000,A,4739.6720,N,12207.8133,W,001.2,306.5,200625,,,A*71
$GPRMC,184523.000,A,4739.6722,N,12207.8137,W,000.0,306.5,200625,,,A*75";
        // #endif

    }
}
