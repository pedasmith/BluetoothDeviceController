using Microsoft.UI.Xaml.Shapes;
using Parsers.Nmea;
using System;
using System.Collections.Generic;
using System.Text;

#if NET8_0_OR_GREATER // Always true for this file
#nullable disable
#endif
namespace WinUI3Controls
{
    /// <summary>
    /// Contains a high-level view for a lat/long. The lat/long might be a single position
    /// with a bunch of individual points (held in GroupedNmea).
    /// </summary>
    public class MapDataItem
    {
        public Nmea_Latitude_Fields Latitude;
        public Nmea_Longitude_Fields Longitude;
        /// <summary>
        /// List of all points involved with a group. A group might only have one point.
        /// </summary>
        public List<Nmea_Data> GroupedNmea = new List<Nmea_Data>();
        public string SummaryString
        {
            get
            {
                if (GroupedNmea.Count == 0) return "No data";
                var retval = GroupedNmea[GroupedNmea.Count - 1].SummaryString;
                return retval;
            }
        }
        public Ellipse Dot = null;

        public MapDataItem(GPRMC_Data nmea)
        {
            Latitude = nmea.Latitude;
            Longitude = nmea.Longitude;
            GroupedNmea.Add(nmea);
        }

        /// <summary>
        /// Returns the distance between two points. The distance is simply
        /// the larger of the delta latitude and delta longitude. The value is always >= 0.
        /// This isn't scaled, and isn't calculated using the DS.Starting positions
        /// </summary>
        /// <param name="value2"></param>
        /// <returns></returns>
        public double Distance(MapDataItem value2)
        {
            double retval = 0.0;
            double lat = Math.Abs(value2.Latitude.AsDecimal - Latitude.AsDecimal);
            double lon = Math.Abs(value2.Longitude.AsDecimal - Longitude.AsDecimal);
            retval = Math.Max(lat, lon);
            return retval;
        }

        /// <summary>
        /// Reminder: latitude0 and longitude0 are 0..180 or 0..360 and are the regular
        /// latitude and longitude + 90 or + 180
        /// </summary>
        /// <param name="latitude0"></param>
        /// <param name="longitude0"></param>
        /// <returns></returns>
        public double Distance(double latitude0, double longitude0)
        {
            double retval = 0.0;
            double lat = Math.Abs(latitude0 - Latitude.AsDecimalFromZero);
            double lon = Math.Abs(longitude0 - Longitude.AsDecimalFromZero);
            retval = Math.Max(lat, lon);
            return retval;
        }

        public static int GetClosestIndex(IList<MapDataItem> MapData, double lat, double lon)
        {
            if (MapData.Count < 1)
            {
                return -1;
            }
            int minIndex = 0;
            double lat0 = lat + 90;
            double lon0 = lon + 180; // Add 90 and 180 to avoid negative numbers.
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
            return minIndex;

        }
    }
}
