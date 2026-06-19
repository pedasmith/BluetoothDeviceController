using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothConversions
{
    static class BluetoothServiceUuid16Bit
    {
        public static string Encode(UInt16 value)
        {
            // 00000000-0000-1000-8000-00805F9B34FB
            var valuestr = $"{value:X4}";
            var retval = "0000" + valuestr + "-0000-1000-8000-00805F9B34FB";
            return retval;
        }

		private static void Log(string str)
		{
			System.Diagnostics.Debug.WriteLine(str);
			Console.WriteLine(str);
		}
		private static int TestOneEncode(UInt16 input, string expected)
		{
			int nerror = 0;
			string actual = Encode(input);
			if (actual != expected)
			{
				Log($"ERROR: Guid16: {input:X4} expected={expected} actual={actual}");
			}
			return nerror;
		}
		public static int Test()
		{
			int nerror = 0;
			nerror += TestOneEncode(0x1802, "00001802-0000-1000-8000-00805F9B34FB");
			return nerror;
		}
        public static string Encode(UInt32 value)
        {
            // 00000000-0000-1000-8000-00805F9B34FB
            var valuestr = $"{value:X8}";
            var retval = "" + valuestr + "-0000-1000-8000-00805F9B34FB";
            return retval;
        }

        // From https://btprodspecificationrefs.blob.core.windows.net/assigned-values/16-bit%20UUID%20Numbers%20Document.pdf
        // the items marked GATT Service
        // Maybe https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/service_uuids.yaml
        public static string Decode (UInt16 value)
        {
            switch (value)
            {
                // updatefile:
                // url:https://bitbucket.org/bluetooth-SIG/public/raw/main/assigned_numbers/uuids/service_uuids.yaml
                // file:service_uuids.yaml
                // startupdatefile:
				case 0x1800: return "GAP"; // org.bluetooth.service.gap
				case 0x1801: return "GATT"; // org.bluetooth.service.gatt
				case 0x1802: return "Immediate Alert"; // org.bluetooth.service.immediate_alert
				case 0x1803: return "Link Loss"; // org.bluetooth.service.link_loss
				case 0x1804: return "Tx Power"; // org.bluetooth.service.tx_power
				case 0x1805: return "Current Time"; // org.bluetooth.service.current_time
				case 0x1806: return "Reference Time Update"; // org.bluetooth.service.reference_time_update
				case 0x1807: return "Next DST Change"; // org.bluetooth.service.next_dst_change
				case 0x1808: return "Glucose"; // org.bluetooth.service.glucose
				case 0x1809: return "Health Thermometer"; // org.bluetooth.service.health_thermometer
				case 0x180A: return "Device Information"; // org.bluetooth.service.device_information
				case 0x180D: return "Heart Rate"; // org.bluetooth.service.heart_rate
				case 0x180E: return "Phone Alert Status"; // org.bluetooth.service.phone_alert_status
				case 0x180F: return "Battery"; // org.bluetooth.service.battery_service
				case 0x1810: return "Blood Pressure"; // org.bluetooth.service.blood_pressure
				case 0x1811: return "Alert Notification"; // org.bluetooth.service.alert_notification
				case 0x1812: return "Human Interface Device"; // org.bluetooth.service.human_interface_device
				case 0x1813: return "Scan Parameters"; // org.bluetooth.service.scan_parameters
				case 0x1814: return "Running Speed and Cadence"; // org.bluetooth.service.running_speed_and_cadence
				case 0x1815: return "Automation IO"; // org.bluetooth.service.automation_io
				case 0x1816: return "Cycling Speed and Cadence"; // org.bluetooth.service.cycling_speed_and_cadence
				case 0x1818: return "Cycling Power"; // org.bluetooth.service.cycling_power
				case 0x1819: return "Location and Navigation"; // org.bluetooth.service.location_and_navigation
				case 0x181A: return "Environmental Sensing"; // org.bluetooth.service.environmental_sensing
				case 0x181B: return "Body Composition"; // org.bluetooth.service.body_composition
				case 0x181C: return "User Data"; // org.bluetooth.service.user_data
				case 0x181D: return "Weight Scale"; // org.bluetooth.service.weight_scale
				case 0x181E: return "Bond Management"; // org.bluetooth.service.bond_management
				case 0x181F: return "Continuous Glucose Monitoring"; // org.bluetooth.service.continuous_glucose_monitoring
				case 0x1820: return "Internet Protocol Support"; // org.bluetooth.service.internet_protocol_support
				case 0x1821: return "Indoor Positioning"; // org.bluetooth.service.indoor_positioning
				case 0x1822: return "Pulse Oximeter"; // org.bluetooth.service.pulse_oximeter
				case 0x1823: return "HTTP Proxy"; // org.bluetooth.service.http_proxy
				case 0x1824: return "Transport Discovery"; // org.bluetooth.service.transport_discovery
				case 0x1825: return "Object Transfer"; // org.bluetooth.service.object_transfer
				case 0x1826: return "Fitness Machine"; // org.bluetooth.service.fitness_machine
				case 0x1827: return "Mesh Provisioning"; // org.bluetooth.service.mesh_provisioning
				case 0x1828: return "Mesh Proxy"; // org.bluetooth.service.mesh_proxy
				case 0x1829: return "Reconnection Configuration"; // org.bluetooth.service.reconnection_configuration
				case 0x183A: return "Insulin Delivery"; // org.bluetooth.service.insulin_delivery
				case 0x183B: return "Binary Sensor"; // org.bluetooth.service.binary_sensor
				case 0x183C: return "Emergency Configuration"; // org.bluetooth.service.emergency_configuration
				case 0x183D: return "Authorization Control"; // org.bluetooth.service.authorization_control
				case 0x183E: return "Physical Activity Monitor"; // org.bluetooth.service.physical_activity_monitor
				case 0x183F: return "Elapsed Time"; // org.bluetooth.service.elapsed_time
				case 0x1840: return "Generic Health Sensor"; // org.bluetooth.service.generic_health_sensor
				case 0x1843: return "Audio Input Control"; // org.bluetooth.service.audio_input_control
				case 0x1844: return "Volume Control"; // org.bluetooth.service.volume_control
				case 0x1845: return "Volume Offset Control"; // org.bluetooth.service.volume_offset
				case 0x1846: return "Coordinated Set Identification"; // org.bluetooth.service.coordinated_set_identification
				case 0x1847: return "Device Time"; // org.bluetooth.service.device_time
				case 0x1848: return "Media Control"; // org.bluetooth.service.media_control
				case 0x1849: return "Generic Media Control"; // org.bluetooth.service.generic_media_control
				case 0x184A: return "Constant Tone Extension"; // org.bluetooth.service.constant_tone_extension
				case 0x184B: return "Telephone Bearer"; // org.bluetooth.service.telephone_bearer
				case 0x184C: return "Generic Telephone Bearer"; // org.bluetooth.service.generic_telephone_bearer
				case 0x184D: return "Microphone Control"; // org.bluetooth.service.microphone_control
				case 0x184E: return "Audio Stream Control"; // org.bluetooth.service.audio_stream_control
				case 0x184F: return "Broadcast Audio Scan"; // org.bluetooth.service.broadcast_audio_scan
				case 0x1850: return "Published Audio Capabilities"; // org.bluetooth.service.published_audio_capabilities
				case 0x1851: return "Basic Audio Announcement"; // org.bluetooth.service.basic_audio_announcement
				case 0x1852: return "Broadcast Audio Announcement"; // org.bluetooth.service.broadcast_audio_announcement
				case 0x1853: return "Common Audio"; // org.bluetooth.service.common_audio
				case 0x1854: return "Hearing Access"; // org.bluetooth.service.hearing_access
				case 0x1855: return "Telephony and Media Audio"; // org.bluetooth.service.telephony_and_media_audio
				case 0x1856: return "Public Broadcast Announcement"; // org.bluetooth.service.public_broadcast_announcement
				case 0x1857: return "Electronic Shelf Label"; // org.bluetooth.service.electronic_shelf_label
				case 0x1858: return "Gaming Audio"; // org.bluetooth.service.gaming_audio
				case 0x1859: return "Mesh Proxy Solicitation"; // org.bluetooth.service.mesh_proxy_solicitation
				case 0x185A: return "Industrial Measurement Device"; // org.bluetooth.service.industrial_measurement_device
				case 0x185B: return "Ranging"; // org.bluetooth.service.ranging
				case 0x185C: return "HID ISO"; // org.bluetooth.service.hid_iso
				case 0x185D: return "Cookware"; // org.bluetooth.service.cookware
				case 0x185E: return "Voice Assistant"; // org.bluetooth.service.voice_assistant
				case 0x185F: return "Generic Voice Assistant"; // org.bluetooth.service.generic_voice_assistant
                    // endupdatefile:
            }
            return $"";
        }
    }
}
