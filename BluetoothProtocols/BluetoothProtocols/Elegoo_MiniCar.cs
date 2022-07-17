//From template: Protocol_Body
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;
using BluetoothDeviceController.Names;

namespace BluetoothProtocols
{
    /// <summary>
    /// Robot with a wooden shell. The interior robot is a typical Arduino bot. Communications are via pretend Serial port ffe1/ffe2..
    /// This class was automatically generated 6/10/2020 9:52 AM
    /// </summary>

    public partial class Elegoo_MiniCar : INotifyPropertyChanged
    {
        // Useful links for the device and protocol documentation
        // Link: https://www.elegoo.com/
        // Link: https://www.elegoo.com/product/elegoo-robotic-wooden-car-kit-with-nanoarduino-compatible-line-tracking-avoiding-obstacle-mobile-controlling-and-graphical-programming-intelligent-and-educational-toy-car-kitstem-toys-for-kids/
        // Link: https://www.elegoo.com/tutorial/Elegoo%20Robot%20miniCar%20Kit%20V1.0.2020.01.07.zip


        public BluetoothLEDevice ble { get; set; } = null;
        public BluetoothStatusEvent Status = new BluetoothStatusEvent();

        // For the INotifyPropertyChanged values
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        Guid[] ServiceGuids = new Guid[] {
            Guid.Parse("0000ffe0-0000-1000-8000-00805f9b34fb"),

        };
        String[] ServiceNames = new string[] {
            "Car",

        };
        GattDeviceService[] Services = new GattDeviceService[] {
            null,

        };
        Guid[] CharacteristicGuids = new Guid[] {
            Guid.Parse("0000ffe1-0000-1000-8000-00805f9b34fb"), // #0 is Result
            Guid.Parse("0000ffe2-0000-1000-8000-00805f9b34fb"), // #1 is Command

        };
        String[] CharacteristicNames = new string[] {
            "Result", // #0 is 0000ffe1-0000-1000-8000-00805f9b34fb
            "Command", // #1 is 0000ffe2-0000-1000-8000-00805f9b34fb

        };
        GattCharacteristic[] Characteristics = new GattCharacteristic[] {
            null,
            null,

        };
        List<HashSet<int>> MapServiceToCharacteristic = new List<HashSet<int>>() {
            new HashSet<int>(){ 0, 1,  },

        };


        bool readCharacteristics = false;
        public async Task<bool> EnsureCharacteristicAsync(bool forceReread = false)
        {
            if (Characteristics.Length == 0) return false;
            if (ble == null) return false; // might not be initialized yet

            GattCharacteristicsResult lastResult = null;
            if (forceReread)
            {
                readCharacteristics = false;
            }
            if (readCharacteristics == false)
            {
                for (int serviceIndex = 0; serviceIndex < MapServiceToCharacteristic.Count; serviceIndex++)
                {
                    var serviceStatus = await ble.GetGattServicesForUuidAsync(ServiceGuids[serviceIndex]);
                    if (serviceStatus.Status != GattCommunicationStatus.Success)
                    {
                        Status.ReportStatus($"Unable to get service {ServiceNames[serviceIndex]}", serviceStatus);
                        return false;
                    }
                    if (serviceStatus.Services.Count != 1)
                    {
                        Status.ReportStatus($"Unable to get valid service count ({serviceStatus.Services.Count}) for {ServiceNames[serviceIndex]}", serviceStatus);
                        continue; //return false;
                    }
                    var service = serviceStatus.Services[0];
                    var characteristicIndexSet = MapServiceToCharacteristic[serviceIndex];
                    foreach (var characteristicIndex in characteristicIndexSet)
                    {
                        var characteristicsStatus = await service.GetCharacteristicsForUuidAsync(CharacteristicGuids[characteristicIndex]);
                        if (characteristicsStatus.Status != GattCommunicationStatus.Success)
                        {
                            Status.ReportStatus($"unable to get characteristic for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            return false;
                        }
                        if (characteristicsStatus.Characteristics.Count == 0)
                        {
                            Status.ReportStatus($"unable to get any characteristics for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else if (characteristicsStatus.Characteristics.Count != 1)
                        {
                            Status.ReportStatus($"unable to get correct characteristics count ({characteristicsStatus.Characteristics.Count}) for {CharacteristicNames[characteristicIndex]}", characteristicsStatus);
                            Characteristics[characteristicIndex] = null;
                        }
                        else
                        {
                            Characteristics[characteristicIndex] = characteristicsStatus.Characteristics[0];
                            lastResult = characteristicsStatus;
                        }
                        lastResult = characteristicsStatus;
                    }
                }
                // Do not call ReportStatus on OK -- the actual read/write/etc. call will
                // call ReportStatus for them. It's important that for any one actual call
                // (public method) that there's only one ReportStatus.
                //Status.ReportStatus("OK: Connected to device", lastResult);
                readCharacteristics = true;
            }
            return readCharacteristics;
        }

        /// <summary>
        /// Primary method used to for any bluetooth characteristic WriteValueAsync() calls.
        /// There's only one characteristic we use, so just use the one global.
        /// </summary>
        /// <param name="method" ></param>
        /// <param name="command" ></param>
        /// <returns></returns>
        private async Task WriteCommandAsync(int characteristicIndex, string method, byte[] command, GattWriteOption writeOption)
        {
            GattCommunicationStatus result = GattCommunicationStatus.Unreachable;
            try
            {
                result = await Characteristics[characteristicIndex].WriteValueAsync(command.AsBuffer(), writeOption);
            }
            catch (Exception)
            {
                result = GattCommunicationStatus.Unreachable;
            }
            Status.ReportStatus(method, result);
            if (result != GattCommunicationStatus.Success)
            {
                // NOTE: should add a way to reset
            }
        }
        /// <summary>
        /// Generic read method; takes in a cache mode which defaults to uncached.
        /// Calls ReportStatus on either sucess or failure
        /// </summary>
        /// <param name="characteristicIndex">Index number of the characteristic</param>
        /// <param name="method" >Name of the actual method; is just used for logging</param>
        /// <param name="cacheMode" >Type of caching</param>
        /// <returns></returns>
        private async Task<IBuffer> ReadAsync(int characteristicIndex, string method, BluetoothCacheMode cacheMode = BluetoothCacheMode.Uncached)
        {
            GattReadResult readResult;
            IBuffer buffer = null;
            try
            {
                readResult = await Characteristics[characteristicIndex].ReadValueAsync(cacheMode);
                if (readResult.Status == GattCommunicationStatus.Success)
                {
                    buffer = readResult.Value;
                }
                else
                {
                    // NOTE: reset the characteristics array?
                }
                Status.ReportStatus(method, readResult.Status);
            }
            catch (Exception)
            {
                Status.ReportStatus(method, GattCommunicationStatus.Unreachable);
                // NOTE: reset the characteristics array?
            }
            return buffer;
        }

        /// <summary>
        /// Delegate for all Notify events
        /// </summary>
        /// <param name="data"></param>
        public delegate void BluetoothDataEvent(BluetoothDeviceController.BleEditor.ValueParserResult data);




        private string _Result = "";
        private bool _Result_set = false;
        public string Result
        {
            get { return _Result; }
            internal set { if (_Result_set && value == _Result) return; _Result = value; _Result_set = true; OnPropertyChanged(); }
        }

        // Returns a string with the status; starts with OK for good status.
        /// <summary>
        /// Event for notifications; ResultEvent += _my function_
        /// </summary>
        public event BluetoothDataEvent ResultEvent = null;
        /// <summary>
        /// We only want to set the internal callback once, and never need to remove it.
        /// </summary>

        private bool NotifyResult_ValueChanged_Set = false;

        /// <summary>
        /// Sets up the notifications; 
        /// Will call Status
        /// </summary>
        /// <param name="notifyType"></param>
        /// <returns>true if the notify was set up. </returns>

        public async Task<bool> NotifyResultAsync(GattClientCharacteristicConfigurationDescriptorValue notifyType = GattClientCharacteristicConfigurationDescriptorValue.Notify)
        {
            if (!await EnsureCharacteristicAsync()) return false;
            var ch = Characteristics[0];
            if (ch == null) return false;
            GattCommunicationStatus result = GattCommunicationStatus.ProtocolError;
            try
            {
                result = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(notifyType);
                if (!NotifyResult_ValueChanged_Set)
                {
                    // Only set the event callback once
                    NotifyResult_ValueChanged_Set = true;
                    ch.ValueChanged += (sender, args) =>
                    {
                        var datameaning = "STRING|ASCII|Result";
                        var parseResult = BluetoothDeviceController.BleEditor.ValueParser.Parse(args.CharacteristicValue, datameaning);

                        Result = parseResult.ValueList.GetValue("Result").AsString;

                        ResultEvent?.Invoke(parseResult);
                    };
                }

            }
            catch (Exception e)
            {
                Status.ReportStatus($"NotifyResult: {e.Message}", result);
                return false;
            }
            Status.ReportStatus($"NotifyResult: set notification", result);

            return true;
        }
        //From template: Protocol_WriteMethodTemplate
        /// <summary>
        /// Writes data for Command
        /// </summary>
        /// <param name="Period"></param>
        /// <returns></returns>
        public async Task WriteCommand(String Command)
        {
            if (!await EnsureCharacteristicAsync()) return;

            var dw = new DataWriter();
            // Bluetooth standard: From v4.2 of the spec, Vol 3, Part G (which covers GATT), page 523: Bleutooth is normally Little Endian
            dw.ByteOrder = ByteOrder.LittleEndian;
            dw.UnicodeEncoding = UnicodeEncoding.Utf8;
            dw.WriteString(Command);

            var command = dw.DetachBuffer().ToArray();
            const int MAXBYTES = 20;
            for (int i = 0; i < command.Length; i += MAXBYTES)
            {
                // So many calculations and copying just to get a slice
                var maxCount = Math.Min(MAXBYTES, command.Length - i);
                var subcommand = new ArraySegment<byte>(command, i, maxCount).ToArray();
                await WriteCommandAsync(1, "Command", subcommand, GattWriteOption.WriteWithoutResponse);
            }
            // original: await DoWriteAsync(data);
        }

        //From template:Protocol_FunctionTemplate

        Command command_Command_ObstacleAvoidance = null;
        public Command Command_ObstacleAvoidance_Init()
        {
            if (command_Command_ObstacleAvoidance == null)
            {
                var command = new Command();

                command.InitVariables();
                command.Compute = "${OA[?]}";
                command_Command_ObstacleAvoidance = command;
            }
            return command_Command_ObstacleAvoidance;
        }
        public async Task Command_ObstacleAvoidance()
        {
            var command = Command_ObstacleAvoidance_Init();

            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_LineTrack_Sensor
        {
            Left = 0,
            Right = 1,
        }
        Command command_Command_LineTrack = null;
        public Command Command_LineTrack_Init()
        {
            if (command_Command_LineTrack == null)
            {
                var command = new Command();

                command.Parameters.Add("Sensor",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.InitVariables();
                command.Compute = "${LT[ $Sensor_GN $][?]}";
                command_Command_LineTrack = command;
            }
            return command_Command_LineTrack;
        }
        public async Task Command_LineTrack(Command_LineTrack_Sensor Sensor)
        {
            var command = Command_LineTrack_Init();

            command.Parameters["Sensor"].CurrValue = (double)Sensor;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_Sport_Direction
        {
            Stop = 0,
            Forward = 1,
            Backward = 2,
            Left = 3,
            Right = 4,
        }
        Command command_Command_Sport = null;
        public Command Command_Sport_Init()
        {
            if (command_Command_Sport == null)
            {
                var command = new Command();

                command.Parameters.Add("Direction",
                    new VariableDescription()
                    {
                        Init = 1,
                    });
                command.Parameters.Add("Speed",
                    new VariableDescription()
                    {
                        Init = 255,
                    });
                command.InitVariables();
                command.Compute = "${TURN[ $Direction_GN $][ $Speed_GN $]}";
                command_Command_Sport = command;
            }
            return command_Command_Sport;
        }
        public async Task Command_Sport(Command_Sport_Direction Direction, byte Speed)
        {
            var command = Command_Sport_Init();

            command.Parameters["Direction"].CurrValue = (double)Direction;
            command.Parameters["Speed"].CurrValue = (double)Speed;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_Sport2_Direction
        {
            Stop = 0,
            Forward = 1,
            Backward = 2,
            Left = 3,
            Right = 4,
        }
        public enum Command_Sport2_Light
        {
            Off = 0,
            White = 1,
            Purple_A020F0 = 2,
        }
        Command command_Command_Sport2 = null;
        public Command Command_Sport2_Init()
        {
            if (command_Command_Sport2 == null)
            {
                var command = new Command();

                command.Parameters.Add("Direction",
                    new VariableDescription()
                    {
                        Init = 1,
                    });
                command.Parameters.Add("Speed",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.Parameters.Add("Light",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.InitVariables();
                command.Compute = "${TURNS[ $Direction_GN $][ $Speed_GN $][ $Light_GN $]}";
                command_Command_Sport2 = command;
            }
            return command_Command_Sport2;
        }
        public async Task Command_Sport2(Command_Sport2_Direction Direction, byte Speed, Command_Sport2_Light Light)
        {
            var command = Command_Sport2_Init();

            command.Parameters["Direction"].CurrValue = (double)Direction;
            command.Parameters["Speed"].CurrValue = (double)Speed;
            command.Parameters["Light"].CurrValue = (double)Light;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_Move_Motor
        {
            All = 0,
            Left = 1,
            Right = 2,
        }
        public enum Command_Move_Direction
        {
            Stop = 0,
            Forward = 1,
            Reverse = 2,
            No_Execution = 3,
        }
        Command command_Command_Move = null;
        public Command Command_Move_Init()
        {
            if (command_Command_Move == null)
            {
                var command = new Command();

                command.Parameters.Add("Motor",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.Parameters.Add("Direction",
                    new VariableDescription()
                    {
                        Init = 1,
                    });
                command.Parameters.Add("Speed",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.InitVariables();
                command.Compute = "${MOVE[ $Motor_GN $][ $Direction_GN $][ $Speed_GN $]}";
                command_Command_Move = command;
            }
            return command_Command_Move;
        }
        public async Task Command_Move(Command_Move_Motor Motor, Command_Move_Direction Direction, byte Speed)
        {
            var command = Command_Move_Init();

            command.Parameters["Motor"].CurrValue = (double)Motor;
            command.Parameters["Direction"].CurrValue = (double)Direction;
            command.Parameters["Speed"].CurrValue = (double)Speed;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_Move2_LeftDirection
        {
            Stop = 0,
            Forward = 1,
            Reverse = 2,
            No_Execution = 3,
        }
        public enum Command_Move2_RightDirection
        {
            Stop = 0,
            Forward = 1,
            Reverse = 2,
            No_Execution = 3,
        }
        Command command_Command_Move2 = null;
        public Command Command_Move2_Init()
        {
            if (command_Command_Move2 == null)
            {
                var command = new Command();

                command.Parameters.Add("LeftDirection",
                    new VariableDescription()
                    {
                        Init = 1,
                    });
                command.Parameters.Add("LeftSpeed",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.Parameters.Add("RightDirection",
                    new VariableDescription()
                    {
                        Init = 1,
                    });
                command.Parameters.Add("RightSpeed",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.InitVariables();
                command.Compute = "${MOVES[ $LeftDirection_GN $][ $LeftSpeed_GN $][ $RightDirection_GN $][ $RightSpeed_GN $]}";
                command_Command_Move2 = command;
            }
            return command_Command_Move2;
        }
        public async Task Command_Move2(Command_Move2_LeftDirection LeftDirection, byte LeftSpeed, Command_Move2_RightDirection RightDirection, byte RightSpeed)
        {
            var command = Command_Move2_Init();

            command.Parameters["LeftDirection"].CurrValue = (double)LeftDirection;
            command.Parameters["LeftSpeed"].CurrValue = (double)LeftSpeed;
            command.Parameters["RightDirection"].CurrValue = (double)RightDirection;
            command.Parameters["RightSpeed"].CurrValue = (double)RightSpeed;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_Beep_Song
        {
            Off = 0,
            Frère_Jacques = 1,
        }
        Command command_Command_Beep = null;
        public Command Command_Beep_Init()
        {
            if (command_Command_Beep == null)
            {
                var command = new Command();

                command.Parameters.Add("Song",
                    new VariableDescription()
                    {
                        Init = 1,
                    });
                command.InitVariables();
                command.Compute = "${BEEP[ $Song_GN $]}";
                command_Command_Beep = command;
            }
            return command_Command_Beep;
        }
        public async Task Command_Beep(Command_Beep_Song Song)
        {
            var command = Command_Beep_Init();

            command.Parameters["Song"].CurrValue = (double)Song;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        Command command_Command_Beep2 = null;
        public Command Command_Beep2_Init()
        {
            if (command_Command_Beep2 == null)
            {
                var command = new Command();

                command.Parameters.Add("Tone",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.Parameters.Add("Duration",
                    new VariableDescription()
                    {
                        Init = 250,
                    });
                command.InitVariables();
                command.Compute = "${BEEPS[ $Tone_GN $][ $Duration_GN_1000_/ $]}";
                command_Command_Beep2 = command;
            }
            return command_Command_Beep2;
        }
        public async Task Command_Beep2(byte Tone, ushort Duration)
        {
            var command = Command_Beep2_Init();

            command.Parameters["Tone"].CurrValue = (double)Tone;
            command.Parameters["Duration"].CurrValue = (double)Duration;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_RGB_Lights
        {
            Both = 0,
            Left = 1,
            Right = 2,
        }
        public enum Command_RGB_Mode
        {
            Solid = 0,
            Flashing = 1,
        }
        Command command_Command_RGB = null;
        public Command Command_RGB_Init()
        {
            if (command_Command_RGB == null)
            {
                var command = new Command();

                command.Parameters.Add("R",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.Parameters.Add("G",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.Parameters.Add("B",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.Parameters.Add("Lights",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.Parameters.Add("Duration",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.Parameters.Add("Mode",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.InitVariables();
                command.Compute = "${RGB[ $R_GN $][ $G_GN $][ $B_GN $][ $Lights_GN $][ $Duration_GN $][ $Mode_GN $]}";
                command_Command_RGB = command;
            }
            return command_Command_RGB;
        }
        public async Task Command_RGB(byte R, byte G, byte B, Command_RGB_Lights Lights, byte Duration, Command_RGB_Mode Mode)
        {
            var command = Command_RGB_Init();

            command.Parameters["R"].CurrValue = (double)R;
            command.Parameters["G"].CurrValue = (double)G;
            command.Parameters["B"].CurrValue = (double)B;
            command.Parameters["Lights"].CurrValue = (double)Lights;
            command.Parameters["Duration"].CurrValue = (double)Duration;
            command.Parameters["Mode"].CurrValue = (double)Mode;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_RGBSet_LeftMode
        {
            Solid = 0,
            Flashing = 1,
        }
        public enum Command_RGBSet_RightMode
        {
            Solid = 0,
            Flashing = 1,
        }
        Command command_Command_RGBSet = null;
        public Command Command_RGBSet_Init()
        {
            if (command_Command_RGBSet == null)
            {
                var command = new Command();

                command.Parameters.Add("LeftRGB",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.Parameters.Add("LeftMode",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.Parameters.Add("RightRGB",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.Parameters.Add("RightMode",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.InitVariables();
                command.Compute = "${RGBS[ $LeftRGB_GS $][ $LeftMode_GN $][ $RightRGB_GS $][ $RightMode_GN $]}";
                command_Command_RGBSet = command;
            }
            return command_Command_RGBSet;
        }
        public async Task Command_RGBSet(byte LeftRGB, Command_RGBSet_LeftMode LeftMode, byte RightRGB, Command_RGBSet_RightMode RightMode)
        {
            var command = Command_RGBSet_Init();

            command.Parameters["LeftRGB"].CurrValue = (double)LeftRGB;
            command.Parameters["LeftMode"].CurrValue = (double)LeftMode;
            command.Parameters["RightRGB"].CurrValue = (double)RightRGB;
            command.Parameters["RightMode"].CurrValue = (double)RightMode;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        Command command_Command_Brightness = null;
        public Command Command_Brightness_Init()
        {
            if (command_Command_Brightness == null)
            {
                var command = new Command();

                command.Parameters.Add("Brightness",
                    new VariableDescription()
                    {
                        Init = 100,
                    });
                command.InitVariables();
                command.Compute = "${RGBB[ $Brightness_GN $]}";
                command_Command_Brightness = command;
            }
            return command_Command_Brightness;
        }
        public async Task Command_Brightness(byte Brightness)
        {
            var command = Command_Brightness_Init();

            command.Parameters["Brightness"].CurrValue = (double)Brightness;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_Key_KeyMode
        {
            Standby = 0,
            Line_Tracking = 1,
            Obstacle_Avoidance = 2,
            Auto_follow = 3,
            Explorer = 4,
        }
        Command command_Command_Key = null;
        public Command Command_Key_Init()
        {
            if (command_Command_Key == null)
            {
                var command = new Command();

                command.Parameters.Add("KeyMode",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.InitVariables();
                command.Compute = "${KEY[ $KeyMode_GN $]}";
                command_Command_Key = command;
            }
            return command_Command_Key;
        }
        public async Task Command_Key(Command_Key_KeyMode KeyMode)
        {
            var command = Command_Key_Init();

            command.Parameters["KeyMode"].CurrValue = (double)KeyMode;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
        //From template:Protocol_FunctionTemplate

        public enum Command_Clear_ClearMode
        {
            All = 0,
            Lights_Off = 1,
            Stop = 2,
            Mute = 3,
        }
        Command command_Command_Clear = null;
        public Command Command_Clear_Init()
        {
            if (command_Command_Clear == null)
            {
                var command = new Command();

                command.Parameters.Add("ClearMode",
                    new VariableDescription()
                    {
                        Init = 0,
                    });
                command.InitVariables();
                command.Compute = "${CLEAR[ $ClearMode_GN $]}";
                command_Command_Clear = command;
            }
            return command_Command_Clear;
        }
        public async Task Command_Clear(Command_Clear_ClearMode ClearMode)
        {
            var command = Command_Clear_Init();

            command.Parameters["ClearMode"].CurrValue = (double)ClearMode;
            var computed_string = command.DoCompute();
            await WriteCommand(computed_string);
        }
    }
}
