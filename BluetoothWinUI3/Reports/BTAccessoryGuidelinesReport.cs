using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.Reports
{
    class UserSuppliedDeviceInformation
    {
        public enum DeviceType {  Unknown, Mouse, InputDevice, Other }
        public DeviceType CurrDeviceType { get; set; } = UserSuppliedDeviceInformation.DeviceType.Other;
        public bool HasBattery { get; set; } = true;
        public bool IsAccessoryNotJustLEBroadcaster { get; set; } = true; // Section 7.2, DIS "... does not apply to accessories that are just LE Broadcasters"
    }

    class SingleTestResult
    {
        public SingleTestResult(string section, string testName, string deviceInfo)
        {
            Section = section;
            TestName = testName;
            DeviceInfo = deviceInfo;
        }
        public SingleTestResult(string section, string testName, string deviceInfo, SingleTestResult.TestResult result, string comments)
        {
            Section = section;
            TestName = testName;
            DeviceInfo = deviceInfo;
            Result = result;
            Comments = comments;
        }
        public string Section = "";
        public string TestName = "";
        public enum TestResult { NotTested, CannotTest, Pass, FailShould, FailMust, FailUnofficial };

        public TestResult Result = TestResult.NotTested;
        public string ResultAsString
        {
            get
            {
                switch (Result)
                {
                    case TestResult.NotTested:
                        return "Not Tested";
                    case TestResult.CannotTest:
                        return "Cannot Test this guideline";
                    case TestResult.Pass:
                        return "Pass";
                    case TestResult.FailShould:
                        return "A SHOULD guideline is not satisfied";
                    case TestResult.FailMust:
                        return "A MUST guideline is not satisfied";
                    case TestResult.FailUnofficial:
                        return "The device has an issue that's not part of the official guidelines";
                    default:
                        return "Unknown Result";
                }
            }
        }
        public string DeviceInfo = "";
        public string Comments = "";
    }

    class BTAccessoryGuidelinesReport
    {
        public void SetCommon_Info(BTCommon_Info.Common_Configuration_Data info)
        {
            Common_Configuration_Data = info;
        }

        public void SetAdvertisement_Info(WatcherData info)
        {
            Advertisement = info;
        }

        public void SetBattery_Info(BTCommon_Info.Battery_Data info)
        {
            Battery_Data = info;
        }

        public void SetDevice_Info(BTCommon_Info.Device_Info_Data info)
        {
            Device_Info_Data = info;
        }

        private BTCommon_Info.Common_Configuration_Data Common_Configuration_Data;
        private BTCommon_Info.Battery_Data Battery_Data;
        private BTCommon_Info.Device_Info_Data Device_Info_Data;
        private UserSuppliedDeviceInformation UserSuppliedDeviceInformation = new();
        private WatcherData Advertisement = null;

        public string MakeReportMarkdown()
        {
            List<SingleTestResult> tests = new List<SingleTestResult>();
            tests.Add(Test412(Common_Configuration_Data));
            tests.Add(Test44A(Common_Configuration_Data));
            tests.Add(Test44A2(Common_Configuration_Data, UserSuppliedDeviceInformation));
            tests.Add(Test44B(Common_Configuration_Data, UserSuppliedDeviceInformation));
            tests.Add(Test44C(Common_Configuration_Data));
            tests.Add(Test612(Battery_Data, UserSuppliedDeviceInformation));
            tests.Add(Test72x(Device_Info_Data, Advertisement, UserSuppliedDeviceInformation));

            var sb = new StringBuilder();
            sb.Append("# Accessory Guidelines Report\n\n");
            foreach (var test in tests)
            {
                var icon = "";
                switch (test.Result)
                {
                    case SingleTestResult.TestResult.NotTested: icon = "┅"; break;
                    case SingleTestResult.TestResult.CannotTest: icon = "┅"; break;
                    case SingleTestResult.TestResult.Pass: icon = "👌"; break;
                    case SingleTestResult.TestResult.FailUnofficial: icon = "☹"; break;
                    case SingleTestResult.TestResult.FailShould: icon = "☹"; break;
                    case SingleTestResult.TestResult.FailMust: icon = "☹"; break;
                }

                sb.Append($"## {icon} Test {test.Section} {test.TestName}\n");
                if (!test.DeviceInfo.Contains("\n"))
                {
                    sb.Append($"- Device Info: {test.DeviceInfo} \n");
                }
                else
                {
                    sb.Append($"- Device Info: \n");
                    var lines = test.DeviceInfo.Split("\n");
                    foreach (var line in lines)
                    {
                        sb.Append("  * " + line + " \n");
                    }
                }
                sb.Append($"- Result: {test.ResultAsString} \n");
                if (!string.IsNullOrEmpty(test.Comments))
                {
                    sb.Append($"- Comments: {test.Comments} \n");
                }
                sb.Append("\n\n");
            }
            ;

            return sb.ToString();
        }

        private SingleTestResult Test412(BTCommon_Info.Common_Configuration_Data info)
        {
            var section = "4.1.2";
            var testname = "Appearance SHOULD be set";
            var deviceInfo = $"Appearance: {info.Appearance}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            switch (info.Appearance)
            {
                case 0:
                    return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailUnofficial, "Appearance value is 0, which is not a useful value to set.");
                case 65535:
                    return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailShould, "Appearance value was not set. The appearance SHOULD be set.");
                default:
                    return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Appearance value is valid.");
            }
        }

        private SingleTestResult Test44A(BTCommon_Info.Common_Configuration_Data info)
        {
            var section = "4.4A";
            var testname = "Connection interval SHOULD be a multiple of 7.5 ms";
            var deviceInfo = $"Connection min interval: {info.Interval_Min}\nConnection max interval: {info.Interval_Max}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (info.Interval_Min < 0)
            {
                // TODO: Required by the spec?
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailMust, "Device connection intervals are not set.");
            }
            if (!DivisibleBy75(info.Interval_Min))
            {
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailShould, "Connection min interval is not a multiple of 7.5 ms.");
            }
            if (!DivisibleBy75(info.Interval_Max))
            {
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailShould, "Connection max interval is not a multiple of 7.5 ms.");
            }
            return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Connection min and max intervals are divisible by 7.5 ms.");
        }

        private SingleTestResult Test44A2(BTCommon_Info.Common_Configuration_Data info, UserSuppliedDeviceInformation userInfo)
        {
            double allowedMin = 30; // ms
            switch (userInfo.CurrDeviceType)
            {
                case UserSuppliedDeviceInformation.DeviceType.Mouse: allowedMin = 7.5; break;
                case UserSuppliedDeviceInformation.DeviceType.InputDevice: allowedMin = 15; break;
                default: allowedMin = 30; break;
            }

            var section = "4.4A2";
            var testname = $"Connection min interval SHOULD be {allowedMin} ms or higher";
            var deviceInfo = $"Connection min interval: {info.Interval_Min}\nConnection max interval: {info.Interval_Max}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (info.Interval_Min < 0)
            {
                // TODO: Required by the spec?
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailMust, "Device connection intervals are not set.");
            }


            if (info.Interval_Min < allowedMin)
            {
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailShould, $"Connection min interval must be at least {allowedMin} ms.");
            }
            return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Connection min >= {allowedMin} ms");
        }

        private bool DivisibleBy75(double value)
        {
            var retval = (value % 7.5) == 0;
            return retval;
        }

        private SingleTestResult Test44B(BTCommon_Info.Common_Configuration_Data info, UserSuppliedDeviceInformation userInfo)
        {
            var section = "4.4B";
            var testname = "Connection min, max range SHOULD contain at least one of 7.5, 15, 30 etc. ms";
            var deviceInfo = $"Connection min interval: {info.Interval_Min}\nConnection max interval: {info.Interval_Max}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (info.Interval_Min < 0)
            {
                // TODO: Required by the spec?
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailMust, "Device connection intervals are not set.");
            }
            if (!RangeContains(7.5, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(15, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(30, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(60, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(120, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(240, info.Interval_Min, info.Interval_Max))
            {
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailShould, "Connection interval range does not contain any of the recommended values (7.5 ms, 15 ms, 30 ms, 60 ms, 120 ms or 240 ms).");
            }

            return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Connection min, max contains one of the recommended values");
        }

        private SingleTestResult Test44C(BTCommon_Info.Common_Configuration_Data info)
        {
            var section = "4.4C";
            var testname = "Connection min, max values SHOULD be set";
            var deviceInfo = $"Connection min interval: {info.Interval_Min}\nConnection max interval: {info.Interval_Max}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (info.Interval_Min < 0)
            {
                // TODO: Required by the spec?
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailShould, "Device connection intervals are not set.");
            }

            return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Connection min, max are set");
        }

        private bool RangeContains(double value, double min, double max)
        {
            var retval = value >= min && value <= max;
            return retval;
        }
        private SingleTestResult Test612(BTCommon_Info.Battery_Data info, UserSuppliedDeviceInformation userInfo)
        {
            var section = "6.1.2";
            var testname = "Devices with a battery SHOULD support battery level reporting";
            var battery = info?.BatteryLevel.ToString() ?? "no battery data";
            var deviceInfo = $"Device has battery: {userInfo.HasBattery} \nBattery level: {battery}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (!userInfo.HasBattery)
            {
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Device does not have a battery, so battery level reporting is not required.");
            }
            if (info == null || info.BatteryLevel < 0)
            {
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailShould, "Devices with batteries should support battery level reporting.");
            }

            return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Connection min, max contains one of the recommended values");
        }

        private SingleTestResult Test72x(BTCommon_Info.Device_Info_Data info, WatcherData Advertisement, UserSuppliedDeviceInformation userInfo)
        {
            var section = "7.2x";
            var testname = "Devices shall support the Device Information Service";
            ushort vendorID = (ushort)info.VendorID;
            var pnpManufacturer = BluetoothConversions.BluetoothCompanyIdentifier.GetBluetoothCompanyIdentifier(vendorID);
            switch (info.VendorIDSource)
            {
                case 0: pnpManufacturer = $"{vendorID:X4} source={info.VendorIDSource}"; break;
                case 1:
                    if (pnpManufacturer.StartsWith("CompanyId="))
                    {
                        pnpManufacturer = $"{vendorID:X4} source=Bluetooth SIG (1)";
                    }
                    break;
                case 2: pnpManufacturer = $"{vendorID:X4} source= USB Forum {info.VendorIDSource}"; break;
                default: pnpManufacturer = $"{vendorID:X4} source={info.VendorIDSource} Reserved"; break;
            }

            var deviceInfo = $"Manufacturer: {info.ManufacturerName}\nModel Number: {info.ModelNumber}\nFirmware Revision: {info.FirmwareRevision}\nSoftware Revision: {info.SoftwareRevision}\nPNP ID: {info.VendorIDSource} {vendorID:X4} {info.ProductID} {info.ProductVersion}\nPNP Manufacturer: {pnpManufacturer}";
            var retval = new SingleTestResult(section, testname, deviceInfo);

            return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.NotTested, $"TODO: still making the tests");
            //return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Connection min, max contains one of the recommended values");
        }

    }
}
