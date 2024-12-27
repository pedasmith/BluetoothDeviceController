using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothDeviceController.BluetoothDefinitionLanguage
{
    public class BluetoothAppearance
    {
        // Data from https://www.bluetooth.com/specifications/gatt/characteristics/
        // page 28 and up: https://www.bluetooth.com/wp-content/uploads/Files/Specification/HTML/Assigned_Numbers/out/en/Assigned_Numbers.pdf?v=1734987409486
        // 2024-12-23: no longer valid: https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Characteristics/org.bluetooth.characteristic.gap.appearance.xml
        // Characteristic 0x2A01, Appearance
        // Values are automatically converted from the XML (plus hand corrections)
        public enum Appearance
        {
            Unknown = 0,
            Phone = 64,
            Computer = 128,
            Watch = 192,
            Sports_Watch = 193,
            Clock = 256,
            Display = 320,
            Remote_Control = 384,
            Eyeglasses = 448,
            Tag = 512,
            Keyring = 576,
            Media_Player = 640,
            Barcode_Scanner = 704,
            Thermometer = 768,
            Thermometer_Ear = 769,
            Heart_rate_Sensor = 832,
            Heart_Rate_Belt = 833,
            Blood_Pressure = 896,
            Blood_Pressure_Arm = 897,
            Blood_Pressure_Wrist = 898,
            Human_Interface_Device = 960,
            Keyboard = 961,
            Mouse = 962,
            Joystick = 963,
            Gamepad = 964,
            Digitizer_Tablet = 965,
            Card_Reader = 966,
            Digital_Pen = 967,
            Barcode_Scanner_HID = 968,
            Glucose_Meter = 1024,
            Running_Walking_Sensor = 1088,
            Running_Walking_Sensor_In_Shoe = 1089,
            Running_Walking_Sensor_On_Shoe = 1090,
            Running_Walking_Sensor_On_Hip = 1091,
            Cycling = 1152,
            Cycling_Computer = 1153,
            Cycling_Speed_Sensor = 1154,
            Cycling_Cadence_Sensor = 1155,
            Cycling_Power_Sensor = 1156,
            Cycling_Speed_and_Cadence_Sensor = 1157,
            Pulse_Oximeter = 3136,
            Pulse_Oximeter_Fingertip = 3137,
            Pulse_Oximeter_Wrist_Worn = 3138,
            Weight_Scale = 3200,
            Personal_Mobility_Device = 3264,
            Powered_Wheelchair = 3265,
            Mobility_Scooter = 3266,
            Continuous_Glucose_Monitor = 3328,
            Insulin_Pump = 3392,
            Insulin_Pump_durable = 3393,
            Insulin_Pump_patch = 3396,
            Insulin_Pen = 3400,
            Medication_Delivery = 3456,
            Outdoor_Sports_Activity = 5184,
            Location_Display = 5185,
            Location_and_Navigation_Display = 5186,
            Location_Pod = 5187,
            Location_and_Navigation_Pod = 5188,
            Environmental_Sensor = 5696,
        };
        public static string AppearanceToString(UInt16 appearance)
        {
            try
            {
                // My "Gems" activity tracker has appearance 0x1234 (!)
                // which is not a valid appearance. Incorrect appearance
                // values can be caught by seeing if the resulting string
                // is in fact all-digits.

                Appearance a = (Appearance)appearance;
                var astr = a.ToString();
                if (IsAllDigits (astr))
                {
                    astr = $"?{appearance}";
                }
                else
                {
                    ; // just here for a nice debugger breakpoint.
                }
                return astr;
            }
            catch (Exception)
            {
                ;
            }
            return $"??{appearance}";
        }

        private static bool IsAllDigits(string str)
        {
            foreach (var ch in str)
            {
                if (ch < '0' || ch > '9') return false;
            }
            return true;
        }
#if NEVER_EER_DEFINED

<Enumeration description="Generic category" value="Generic Remote Control" key="384"/>

<Enumeration description="Generic category" value="Generic Eye-glasses" key="448"/>

<Enumeration description="Generic category" value="Generic Tag" key="512"/>

<Enumeration description="Generic category" value="Generic Keyring" key="576"/>

<Enumeration description="Generic category" value="Generic Media Player" key="640"/>

<Enumeration description="Generic category" value="Generic Barcode Scanner" key="704"/>

<Enumeration description="Generic category" value="Generic Thermometer" key="768"/>

<Enumeration description="Thermometer subtype" value="Thermometer: Ear" key="769"/>

<Enumeration description="Generic category" value="Generic Heart rate Sensor" key="832"/>

<Enumeration description="Heart Rate Sensor subtype" value="Heart Rate Sensor: Heart Rate Belt" key="833"/>

<!-- Added Blood pressure support on December 09, 2011 -->


<Enumeration description="Generic category" value="Generic Blood Pressure" key="896"/>

<Enumeration description="Blood Pressure subtype" value="Blood Pressure: Arm" key="897"/>

<Enumeration description="Blood Pressure subtype" value="Blood Pressure: Wrist" key="898"/>

<!-- Added HID Related appearance values on January 03, 2012 approved by BARB -->


<Enumeration description="HID Generic" value="Human Interface Device (HID)" key="960"/>

<Enumeration description="HID subtype" value="Keyboard" key="961"/>

<Enumeration description="HID subtype" value="Mouse" key="962"/>

<Enumeration description="HID subtype" value="Joystick" key="963"/>

<Enumeration description="HID subtype" value="Gamepad" key="964"/>

<Enumeration description="HID subtype" value="Digitizer Tablet" key="965"/>

<Enumeration description="HID subtype" value="Card Reader" key="966"/>

<Enumeration description="HID subtype" value="Digital Pen" key="967"/>

<Enumeration description="HID subtype" value="Barcode Scanner" key="968"/>

<!-- Added Generic Glucose Meter value on May 10, 2012 approved by BARB -->


<Enumeration description="Generic category" value="Generic Glucose Meter" key="1024"/>

<!-- Added additional appearance values on June 26th, 2012 approved by BARB -->


<Enumeration description="Generic category" value="Generic: Running Walking Sensor" key="1088"/>

<Enumeration description="Running Walking Sensor subtype" value="Running Walking Sensor: In-Shoe" key="1089"/>

<Enumeration description="Running Walking Sensor subtype" value="Running Walking Sensor: On-Shoe" key="1090"/>

<Enumeration description="Running Walking Sensor subtype" value="Running Walking Sensor: On-Hip" key="1091"/>

<Enumeration description="Generic category" value="Generic: Cycling" key="1152"/>

<Enumeration description="Cycling subtype" value="Cycling: Cycling Computer" key="1153"/>

<Enumeration description="Cycling subtype" value="Cycling: Speed Sensor" key="1154"/>

<Enumeration description="Cycling subtype" value="Cycling: Cadence Sensor" key="1155"/>

<Enumeration description="Cycling subtype" value="Cycling: Power Sensor" key="1156"/>

<Enumeration description="Cycling subtype" value="Cycling: Speed and Cadence Sensor" key="1157"/>

<!-- Added appearance values for Pulse Oximeter on July 30th, 2013 approved by BARB -->


<Enumeration description="Pulse Oximeter Generic Category" value="Generic: Pulse Oximeter" key="3136"/>

<Enumeration description="Pulse Oximeter subtype" value="Fingertip" key="3137"/>

<Enumeration description="Pulse Oximeter subtype" value="Wrist Worn" key="3138"/>

<!-- Added appearance values for Generic Weight Scale on May 21, 2014 approved by BARB -->


<Enumeration description="Weight Scale Generic Category" value="Generic: Weight Scale" key="3200"/>

<!-- Added additional appearance values on October 2nd, 2016 approved by BARB -->


<Enumeration description="Personal Mobility Device" value="Generic Personal Mobility Device" key="3264"/>

<Enumeration description="Personal Mobility Device" value="Powered Wheelchair" key="3265"/>

<Enumeration description="Personal Mobility Device" value="Mobility Scooter" key="3266"/>

<Enumeration description="Continuous Glucose Monitor" value="Generic Continuous Glucose Monitor" key="3328"/>

<!-- Added additional appearance values on February 1st, 2018 approved by BARB -->


<Enumeration description="Insulin Pump" value="Generic Insulin Pump" key="3392"/>

<Enumeration description="Insulin Pump" value="Insulin Pump, durable pump" key="3393"/>

<Enumeration description="Insulin Pump" value="Insulin Pump, patch pump" key="3396"/>

<Enumeration description="Insulin Pump" value="Insulin Pen" key="3400"/>

<Enumeration description="Medication Delivery" value="Generic Medication Delivery" key="3456"/>

<!-- Added appearance values for L&N on July 30th, 2013 approved by BARB -->


<Enumeration description="Outdoor Sports Activity Generic Category" value="Generic: Outdoor Sports Activity" key="5184"/>

<Enumeration description="Outdoor Sports Activity subtype" value="Location Display Device" key="5185"/>

<Enumeration description="Outdoor Sports Activity subtype" value="Location and Navigation Display Device" key="5186"/>

<Enumeration description="Outdoor Sports Activity subtype" value="Location Pod" key="5187"/>

<Enumeration description="Outdoor Sports Activity subtype" value="Location and Navigation Pod" key="5188"/>

<!-- Added appearance values for Generic Environmental Sensor on May 21, 2014 approved by BARB<Enumeration key="5696" value="Generic: Environmental Sensor" description="Environmental Sensor Generic Category" />-->
#endif
    }
}
