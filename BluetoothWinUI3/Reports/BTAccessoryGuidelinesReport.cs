using BluetoothProtocols;
using BluetoothWatcher.AdvertismentWatcher;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Input.Preview;

#if NET8_0_OR_GREATER
#nullable disable
#endif

namespace BluetoothWinUI3.Reports
{
    class UserSuppliedAccessoryInformation
    {
        public enum AccessoryType {  Unknown, Mouse, InputAccessory, AudioAccessory, Other }
        public AccessoryType CurrAccessoryType { get; set; } = UserSuppliedAccessoryInformation.AccessoryType.AudioAccessory;
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

        public SingleTestResult Update(SingleTestResult.TestResult result, string comments)
        {
            Result = result;
            Comments = comments;
            return this;
        }
        public string Section = "";
        public string TestName = "";
        public enum TestResult { NotTested, CannotTest, Pass, FailShould, FailShall, FailUnofficial };

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
                    case TestResult.FailShall:
                        return "A MUST guideline is not satisfied";
                    case TestResult.FailUnofficial:
                        return "The accessory has an issue that's not part of the official guidelines";
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
        public static async Task<string> CreateFromAdvertisement(WatcherData advertisement)
        {
            var Report = new BTAccessoryGuidelinesReport();
            BTCommon_Info Device = new()
            {
                ble = await BluetoothLEDevice.FromBluetoothAddressAsync(advertisement.Addr),
            };
            BluetoothCacheMode DefaultCacheMode = BluetoothCacheMode.Cached;
            if (Device.ble == null)
            {
                return $"Error: unable to make report for {BluetoothAddress.AsString(advertisement.Addr)}";
            }

            Report.StatusDIS = await Device.ble.GetGattServicesForUuidAsync(BTCommon_Info.ServiceGuid_Device_Info);

            await Device.ReadDevice_Name(DefaultCacheMode);
            await Device.ReadAppearance(DefaultCacheMode);
            await Device.ReadConnection_Parameter(DefaultCacheMode);
            await Device.ReadBatteryLevel(DefaultCacheMode);
            await Device.ReadManufacturer_Name(DefaultCacheMode);
            await Device.ReadModel_Number(DefaultCacheMode);
            await Device.ReadFirmware_Revision(DefaultCacheMode);
            await Device.ReadSoftware_Revision(DefaultCacheMode);
            await Device.ReadPnP_ID(DefaultCacheMode);

            Report.Common_Configuration_Data = Device.CurrCommon_Configuration_Data;
            Report.Advertisement = advertisement;
            Report.Battery_Data = Device.CurrBattery_Data;
            Report.Device_Info_Data = Device.CurrDevice_Info_Data;

            var retval = Report.MakeReportMarkdown();
            return retval;
        }


        private BTCommon_Info.Common_Configuration_Data Common_Configuration_Data;
        private BTCommon_Info.Battery_Data Battery_Data;
        private BTCommon_Info.Device_Info_Data Device_Info_Data;
        GattDeviceServicesResult StatusDIS;
        private UserSuppliedAccessoryInformation UserSuppliedAccessoryInformation = new();
        private WatcherData Advertisement = null;

        public string MakeReportMarkdown()
        {
            List<SingleTestResult> tests = new List<SingleTestResult>();
            tests.Add(Test412(Common_Configuration_Data));
            tests.Add(Test44A(Common_Configuration_Data));
            tests.Add(Test44A2(Common_Configuration_Data, UserSuppliedAccessoryInformation));
            tests.Add(Test44B(Common_Configuration_Data, UserSuppliedAccessoryInformation));
            tests.Add(Test44C(Common_Configuration_Data));
            tests.Add(Test612(Battery_Data, UserSuppliedAccessoryInformation));
            tests.Add(Test72DIS(Device_Info_Data, StatusDIS, Advertisement, UserSuppliedAccessoryInformation));
            tests.Add(Test72x(Device_Info_Data, Advertisement, UserSuppliedAccessoryInformation));

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
                    case SingleTestResult.TestResult.FailShall: icon = "☹"; break;
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
                    return retval.Update (SingleTestResult.TestResult.FailUnofficial, "Appearance value is 0, which is not a useful value to set.");
                case 65535:
                    return retval.Update(SingleTestResult.TestResult.FailShould, "Appearance value was not set. The appearance SHOULD be set.");
                default:
                    return retval.Update(SingleTestResult.TestResult.Pass, $"Appearance value is valid.");
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
                return retval.Update(SingleTestResult.TestResult.FailShall, "Device connection intervals are not set.");
            }
            if (!DivisibleBy75(info.Interval_Min))
            {
                return retval.Update(SingleTestResult.TestResult.FailShould, "Connection min interval is not a multiple of 7.5 ms.");
            }
            if (!DivisibleBy75(info.Interval_Max))
            {
                return retval.Update(SingleTestResult.TestResult.FailShould, "Connection max interval is not a multiple of 7.5 ms.");
            }
            return retval.Update(SingleTestResult.TestResult.Pass, $"Connection min and max intervals are divisible by 7.5 ms.");
        }

        private SingleTestResult Test44A2(BTCommon_Info.Common_Configuration_Data info, UserSuppliedAccessoryInformation userInfo)
        {
            double allowedMin = 30; // ms
            switch (userInfo.CurrAccessoryType)
            {
                case UserSuppliedAccessoryInformation.AccessoryType.Mouse: allowedMin = 7.5; break;
                case UserSuppliedAccessoryInformation.AccessoryType.InputAccessory: allowedMin = 15; break;
                default: allowedMin = 30; break;
            }

            var section = "4.4A2";
            var testname = $"Connection min interval SHOULD be {allowedMin} ms or higher";
            var deviceInfo = $"Connection min interval: {info.Interval_Min}\nConnection max interval: {info.Interval_Max}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (info.Interval_Min < 0)
            {
                // TODO: Required by the spec?
                return retval.Update(SingleTestResult.TestResult.FailShall, "Device connection intervals are not set.");
            }


            if (info.Interval_Min < allowedMin)
            {
                return retval.Update(SingleTestResult.TestResult.FailShould, $"Connection min interval must be at least {allowedMin} ms.");
            }
            return retval.Update(SingleTestResult.TestResult.Pass, $"Connection min >= {allowedMin} ms");
        }

        private bool DivisibleBy75(double value)
        {
            var retval = (value % 7.5) == 0;
            return retval;
        }

        private bool RangeDivisibleBy30(double minvalue, double maxvalue)
        {
            var value = (maxvalue - minvalue);
            var retval = (value % 30.0) == 0;
            return retval;
        }

        private SingleTestResult Test44B(BTCommon_Info.Common_Configuration_Data info, UserSuppliedAccessoryInformation userInfo)
        {
            var section = "4.4B";
            var testname = "Connection min, max range SHOULD contain at least one of 7.5, 15, 30 etc. ms";
            var deviceInfo = $"Connection min interval: {info.Interval_Min}\nConnection max interval: {info.Interval_Max}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (info.Interval_Min < 0)
            {
                // TODO: Required by the spec?
                return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.FailShall, "Device connection intervals are not set.");
            }
            if (!RangeContains(7.5, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(15, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(30, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(60, info.Interval_Min, info.Interval_Max) &&
                !RangeContains(120, info.Interval_Min, info.Interval_Max) &&
                !RangeDivisibleBy30(info.Interval_Min, info.Interval_Max)  // Question: why? For example, the C1-A Cyclemeter has a range of 400..800. It's an enormous range, but technically fails.
                )
            {
                return retval.Update(SingleTestResult.TestResult.FailShould, "Connection interval range does not contain any of the recommended values (7.5 ms, 15 ms, 30 ms, 60 ms, 120 ms or 240 ms).");
            }

            return retval.Update(SingleTestResult.TestResult.Pass, $"Connection min, max contains one of the recommended values");
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
                return retval.Update(SingleTestResult.TestResult.FailShould, "Device connection intervals are not set.");
            }

            return retval.Update(SingleTestResult.TestResult.Pass, $"Connection min, max are set");
        }

        private bool RangeContains(double value, double min, double max)
        {
            var retval = value >= min && value <= max;
            return retval;
        }
        private SingleTestResult Test612(BTCommon_Info.Battery_Data info, UserSuppliedAccessoryInformation userInfo)
        {
            var section = "6.1.2";
            var testname = "Accessories with a battery SHOULD support battery level reporting";
            var battery = info?.BatteryLevel.ToString() ?? "no battery data";
            var deviceInfo = $"Accessory has battery: {userInfo.HasBattery} \nBattery level: {battery}";
            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (!userInfo.HasBattery)
            {
                return retval.Update(SingleTestResult.TestResult.Pass, $"Accessory does not have a battery, so battery level reporting is not required.");
            }
            if (info == null || info.BatteryLevel < 0)
            {
                return retval.Update(SingleTestResult.TestResult.FailShould, "Accessoryies with batteries should support battery level reporting.");
            }

            return retval.Update(SingleTestResult.TestResult.Pass, $"Accessory has a valid battery level");
        }
        private static string GetPnpManufacturer(BTCommon_Info.Device_Info_Data info)
        {
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
            return pnpManufacturer;
        }

        private bool ValidateManufacturerString(SingleTestResult retval, string str, string field, string invalidValue = "Manufacturer Name")
        {
            if (str == "")
            {
                retval.Update(SingleTestResult.TestResult.FailShall, $"{field} shall not be blank");
                return false;
            }
            if (str.EndsWith("\\0"))
            {
                retval.Update(SingleTestResult.TestResult.FailShall, $"{field} shall not end with a NUL char");
                return false;
            }
            if (str == invalidValue)
            {
                retval.Update(SingleTestResult.TestResult.FailShall, $"{field} shall not be {invalidValue}");
                return false;
            }
            if (str.Contains("\\0"))
            {
                retval.Update(SingleTestResult.TestResult.FailUnofficial, $"{field} shall not contain any NUL chars");
                return false;
            }
            return true;
        }
        private bool ValidateOtherString(SingleTestResult retval, string str, string field, string invalidValue)
        {
            if (str == "")
            {
                retval.Update(SingleTestResult.TestResult.FailUnofficial, $"{field} shall not be blank");
                return false;
            }
            if (str.EndsWith("\\0"))
            {
                retval.Update(SingleTestResult.TestResult.FailUnofficial, $"{field} shall not end with a NUL char");
                return false;
            }
            if (str == invalidValue)
            {
                retval.Update(SingleTestResult.TestResult.FailUnofficial, $"{field} shall not be {invalidValue}");
                return false;
            }
            if (str.Contains("\\0"))
            {
                retval.Update(SingleTestResult.TestResult.FailUnofficial, $"{field} shall not contain any NUL chars");
                return false;
            }
            return true;
        }
        private SingleTestResult Test72DIS(BTCommon_Info.Device_Info_Data info, GattDeviceServicesResult statusDIS, WatcherData Advertisement, UserSuppliedAccessoryInformation userInfo)
        {
            var section = "7.2Supported";
            var testname = "Accessories shall support the Device Information Service";
            var pnpManufacturer = GetPnpManufacturer(info);
            ushort vendorID = (ushort)info.VendorID;
            bool pnpRead = info.VendorIDSource != 255 || vendorID != 65535;
            string pnpstring = pnpRead ? $"PNP ID: {info.VendorIDSource} {vendorID:X4} {info.ProductID} {info.ProductVersion}\nPNP Manufacturer: {pnpManufacturer}" : "PNP ID: not set";
            var deviceInfo = $"Manufacturer: {info.ManufacturerName}\nModel Number: {info.ModelNumber}\nFirmware Revision: {info.FirmwareRevision}\nSoftware Revision: {info.SoftwareRevision}\n{pnpstring}";

            var retval = new SingleTestResult(section, testname, deviceInfo);

            if (statusDIS.Status != GattCommunicationStatus.Success)
            {
                retval.Update(SingleTestResult.TestResult.FailShall, $"... shall implement the Device Information Service. Attempted connection status was {statusDIS.Status}");
                return retval;
            }
            if (statusDIS.Services.Count != 1)
            {
                retval.Update(SingleTestResult.TestResult.FailShall, $"... shall implement the Device Information Service. Service count was {StatusDIS.Services.Count}");
                return retval;
            }
            var hasGuidInAdvert = Advertisement.ServiceUuids.Contains(BTCommon_Info.ServiceGuid_Device_Info);

            if (userInfo.IsAccessoryNotJustLEBroadcaster && hasGuidInAdvert)
            {
                retval.Update(SingleTestResult.TestResult.FailShould, $"... service GUID should not be advertised in the advertising data");
                return retval;
            }

            return retval.Update(SingleTestResult.TestResult.Pass, $"Accessory includes the Device Information Service");
        }

        private SingleTestResult Test72x(BTCommon_Info.Device_Info_Data info, WatcherData Advertisement, UserSuppliedAccessoryInformation userInfo)
        {
            var section = "7.2x";
            var testname = "Device Information Service values are set correctly";
            var pnpManufacturer = GetPnpManufacturer(info);
            ushort vendorID = (ushort)info.VendorID;
            bool pnpRead = info.VendorIDSource != 255 || vendorID != 65535;
            string pnpstring = pnpRead ? $"PNP ID: {info.VendorIDSource} {vendorID:X4} {info.ProductID} {info.ProductVersion}\nPNP Manufacturer: {pnpManufacturer}" : "PNP ID: not set";
            var deviceInfo = $"Manufacturer: {info.ManufacturerName}\nModel Number: {info.ModelNumber}\nFirmware Revision: {info.FirmwareRevision}\nSoftware Revision: {info.SoftwareRevision}\n{pnpstring}";

            var retval = new SingleTestResult(section, testname, deviceInfo);
            if (!ValidateManufacturerString(retval, info.ManufacturerName, "Manufacturer Name")) return retval;
            if (!ValidateOtherString(retval, info.ModelNumber, "Model Number", "Model Number")) return retval;
            if (!ValidateOtherString(retval, info.FirmwareRevision, "Firmware Revision", "Firmware Revision")) return retval;
            if (!ValidateOtherString(retval, info.SoftwareRevision, "Software Revision", "Software Revision")) return retval;

            if (pnpRead)
            {
                switch (info.VendorIDSource)
                {
                    case 1:
                    case 2:
                        break;
                    case 0:
                        retval.Update(SingleTestResult.TestResult.FailShall, "The PNP Vendor ID Source must set set, not 0");
                        return retval;
                    case 255:
                        if (userInfo.CurrAccessoryType == UserSuppliedAccessoryInformation.AccessoryType.AudioAccessory)
                        {
                            retval.Update(SingleTestResult.TestResult.FailShall, "Audio accessories shall implements the PNP ID");
                        }
                        else
                        {
                            retval.Update(SingleTestResult.TestResult.FailShall, "Accessories should support [the PNP ID]");
                        }
                        return retval;
                    default:
                        retval.Update(SingleTestResult.TestResult.FailShall, "The PNP Vendor ID Source must be either 1 (Bluetooth) or 2 (USB)");
                        return retval;
                }
                if (info.VendorID == 0)
                {
                    retval.Update(SingleTestResult.TestResult.FailShall, "The PNP Vendor ID Source must set set, not 0");
                    return retval;
                }
                // 2026-10-07 Valid values are 0..4398. The "invalid point" is set to higher than that so the code isn't instantly out of date.
                if (info.VendorID > 8000)
                {
                    retval.Update(SingleTestResult.TestResult.FailShall, $"The PNP Vendor ID must be valid. As of 2026-10-07 valid IDs are 0..4398");
                    return retval;
                }
            }

            return retval.Update(SingleTestResult.TestResult.NotTested, $"TODO: still making the tests");
            //return new SingleTestResult(section, testname, deviceInfo, SingleTestResult.TestResult.Pass, $"Connection min, max contains one of the recommended values");
        }
    }
}
