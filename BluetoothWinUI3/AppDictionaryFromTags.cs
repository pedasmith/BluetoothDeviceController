using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.ApplicationModel.Activation;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3;

internal static class AppDictionaryFromTags
{
    static OrderedDictionary<string, string> tagToResourceKeyMap = new OrderedDictionary<string, string>
    {
        { "#cycling", "sDeviceBorder_Background_Cycling" },
        { "#heartrate", "sDeviceBorder_Background_Health" },
        { "#pulseoximeter", "sDeviceBorder_Background_PulseOximeter" },
        { "#health", "sDeviceBorder_Background_Health" },

        { "#environment", "sDeviceBorder_Background_Environment" },
        { "#security", "sDeviceBorder_Background_Security" },
        { "#entertainment", "sDeviceBorder_Background_Entertainment" },
        { "#productivity", "sDeviceBorder_Background_Productivity" },
        { "#report", "sDeviceBorder_Background_Report" },
        { "#utility", "sDeviceBorder_Background_Utility" },
        { "#other", "sDeviceBorder_Background_Other" },
    };
    /// <summary>
    /// Given a control's Tags (like "#environment #health"), return the appropriate background brush from the 
    /// app's resource dictionary.
    /// An index of -1 means find a new index to use and add it to the list of used indexes.
    /// An index of -2 means 
    /// </summary>
    public static (Brush brush, int selectedIndex) GetBackgroundBrushFromTags(string tags, int preferredIndex = -1)
    {
        foreach (var item in tagToResourceKeyMap)
        {
            if (tags.Contains(item.Key))
            {
                // What number?
                preferredIndex = GetNextAvailableIndex(item.Value, preferredIndex);
                var brushname = item.Value + "_" + preferredIndex.ToString();
                return (GetBrush(brushname), preferredIndex);
            }
        }
        // #other will always exist!
        return GetBackgroundBrushFromTags("#other", preferredIndex);
    }

    const int MAXCOLOR = 5; // 0..4
    const int DEFAULTINDEX = 0;
    private static Dictionary<string, HashSet<Int16>> usedIndexes = new ();
    private static int GetNextAvailableIndex(string resourceKey, int preferredIndex)
    {
        if (!usedIndexes.ContainsKey(resourceKey))
        {
            usedIndexes[resourceKey] = new HashSet<Int16>();
        }
        if (preferredIndex >= 0)
        {
            usedIndexes[resourceKey].Add((Int16)preferredIndex);
            return preferredIndex;
        }
        for (Int16 i = 0; i < MAXCOLOR; i++)
        {
            if (!usedIndexes[resourceKey].Contains(i))
            {
                usedIndexes[resourceKey].Add(i);
                return i;
            }
        }
        return DEFAULTINDEX; // Always return something!
    }

    private static Brush GetBrush(string resourceKey)
    {
        if (Application.Current.Resources.TryGetValue(resourceKey, out object value))
        {
            return value as SolidColorBrush;
        }
        return null;
    }
}
