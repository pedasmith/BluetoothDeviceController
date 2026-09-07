using BluetoothProtocols;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace BluetoothProtocolsDevicesCoreExtensions
{
    internal static class TI_SensorTag_1352_Extension
    {
        public static async Task WriteTemperatureEnable(this TI_SensorTag_1352 sensorTag, byte value)
        {
            var index = TI_SensorTag_1352.CharacteristicIndex.Temperature_Temperature_Conf_index;
            await sensorTag.Ensure_Characteristic_Async(TI_SensorTag_1352.ServiceIndex.Temperature_index, "Temperature", index, "Temperature Conf.");
            var ch = sensorTag.Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var writer = new DataWriter();
            writer.WriteByte(value); // enable
            var result = await ch.WriteValueAsync(writer.DetachBuffer());
            sensorTag.Status.ReportStatus("WriteTemperatureEnable", result);
        }

        public static async Task WriteHumidityEnable(this TI_SensorTag_1352 sensorTag, byte value)
        {
            var index = TI_SensorTag_1352.CharacteristicIndex.Humidity_Humidity_Conf_index;
            await sensorTag.Ensure_Characteristic_Async(TI_SensorTag_1352.ServiceIndex.Humidity_index, "Humidity", index, "Humidity Conf.");
            var ch = sensorTag.Characteristics[(int)index];
            if (ch == null)
            {
                return;
            }
            var writer = new DataWriter();
            writer.WriteByte(value); // enable
            var result = await ch.WriteValueAsync(writer.DetachBuffer());
            sensorTag.Status.ReportStatus("WriteHumidityEnable", result);
        }
    }
}
