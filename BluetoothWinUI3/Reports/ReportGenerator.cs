using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothWinUI3.Reports;

static class ReportGenerator
{
    public static string MakeMarkdown(List<SingleTestResult> tests, BTCommon_Info.Common_Configuration_Data common_Configuration_Data, WatcherData advertisement)
    {
        var (testSummary, nFailTotal) = MakePassFailSummary(tests);
        var passFail = nFailTotal > 0 ? "**FAIL**" : "pass";

        var sb = new StringBuilder();
        sb.Append($"# Accessory Guidelines Report: {passFail}\n\n");
        sb.Append(MakeDeviceDescriptionSection(common_Configuration_Data, advertisement) + "\n\n");
        sb.Append($"## Result Summary: {passFail}\n\n{testSummary}\n\n");

        foreach (var test in tests)
        {
            var icon = MakeIcon(test.Result);
            sb.Append($"## {icon} Test {test.Section} {test.TestName}\n");
            sb.Append(MakeDeviceInfo(test.DeviceInfo));
            sb.Append($"- Result: {test.ResultAsString} \n");
            if (!string.IsNullOrEmpty(test.Comments))
            {
                sb.Append($"- Comments: {test.Comments} \n");
            }
            sb.Append("\n\n");
        }

        return sb.ToString();
    }

    private static (string, int nFailureTotal) MakePassFailSummary(List<SingleTestResult> tests)
    {
        int nOther = 0;
        OrderedDictionary<SingleTestResult.TestResult, int> counts = new();
        counts.Add(SingleTestResult.TestResult.Pass, 0);
        counts.Add(SingleTestResult.TestResult.FailShould, 0);
        counts.Add(SingleTestResult.TestResult.FailShall, 0);
        counts.Add(SingleTestResult.TestResult.FailUnofficial, 0);
        counts.Add(SingleTestResult.TestResult.CannotTest, 0);
        counts.Add(SingleTestResult.TestResult.NotTested, 0);
        OrderedDictionary<SingleTestResult.TestResult, string> list = new();
        list.Add(SingleTestResult.TestResult.Pass, "");
        list.Add(SingleTestResult.TestResult.FailShould, "");
        list.Add(SingleTestResult.TestResult.FailShall, "");
        list.Add(SingleTestResult.TestResult.FailUnofficial, "");
        list.Add(SingleTestResult.TestResult.CannotTest, "");
        list.Add(SingleTestResult.TestResult.NotTested, "");

        foreach (var test in tests)
        {
            counts[test.Result]++;
            if (list[test.Result] != "") list[test.Result] += ", ";
            list[test.Result] += test.Section;
        }
        var retval = "\n\n|Result|Count|Tests\n|----|----|----|\n";
        retval += $"|Pass|{counts[SingleTestResult.TestResult.Pass]}|{list[SingleTestResult.TestResult.Pass]}\n";
        retval += $"|Fail (shall)|{counts[SingleTestResult.TestResult.FailShall]}|{list[SingleTestResult.TestResult.FailShould]}\n";
        retval += $"|Fail (should)|{counts[SingleTestResult.TestResult.FailShould]}|{list[SingleTestResult.TestResult.FailShould]}\n";
        retval += $"|Fail (unofficial)|{counts[SingleTestResult.TestResult.FailUnofficial]}|{list[SingleTestResult.TestResult.FailUnofficial]}\n";

        if (counts[SingleTestResult.TestResult.CannotTest] != 0)
        {
            retval += $"|Cannot test|{counts[SingleTestResult.TestResult.CannotTest]}|{list[SingleTestResult.TestResult.CannotTest]}\n";
        }
        if (counts[SingleTestResult.TestResult.NotTested] != 0)
        {
            retval += $"|Not tested|{counts[SingleTestResult.TestResult.NotTested]}|{list[SingleTestResult.TestResult.NotTested]}\n";
        }
        int nFailTotal = counts[SingleTestResult.TestResult.FailShall]
            + counts[SingleTestResult.TestResult.FailShould]
            + counts[SingleTestResult.TestResult.FailUnofficial]
            + nOther;
        return (retval + "\n\n", nFailTotal);
    }

    private static string MakeIcon(SingleTestResult.TestResult value)
    {
        string icon = "";
        switch (value)
        {
            case SingleTestResult.TestResult.NotTested: icon = "┅"; break;
            case SingleTestResult.TestResult.CannotTest: icon = "┅"; break;
            case SingleTestResult.TestResult.Pass: icon = "pass"; break;
            case SingleTestResult.TestResult.FailUnofficial: icon = "FAIL"; break;
            case SingleTestResult.TestResult.FailShould: icon = "FAIL"; break;
            case SingleTestResult.TestResult.FailShall: icon = "FAIL"; break;
        }
        return icon;
    }
    private static string MakeDeviceDescriptionSection(BTCommon_Info.Common_Configuration_Data info, WatcherData advertisement)
    {
        StringBuilder sb = new();
        sb.Append("Microsoft Bluetooth Accessory Guidelines are at [learn.microsoft.com](https://learn.microsoft.com/en-us/windows-hardware/design/accessory-guidelines/bluetooth-accessory-guidelines/bluetooth-accessory-guidelines-overview) .  \n");
        sb.Append($"Device address: **{BluetoothAddress.AsString(advertisement.OriginalAdvertisement.BluetoothAddress)}**  \n");
        sb.Append($"Device name: **{info.Device_Name}**  \n");
        return sb.ToString();
    }

    private static string MakeDeviceInfo(string deviceInfo)
    {
        if (string.IsNullOrEmpty(deviceInfo)) return ""; // happens for CannotTest
        StringBuilder sb = new();
        if (!deviceInfo.Contains("\n"))
        {
            sb.Append($"- Device Info: {deviceInfo} \n");
        }
        else
        {
            sb.Append($"- Device Info: \n");
            var lines = deviceInfo.Split("\n");
            foreach (var line in lines)
            {
                sb.Append("  * " + line + " \n");
            }
        }
        return sb.ToString();
    }



}
