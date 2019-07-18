/**
 * Modbus RTU Slave Example CSharp
 * ----------------------------------------------------------------------------
 * Creates a simple Modbus RTU Slave application that polls specific Modbus 
 * registers. 
 *
 * More information https://github.com/chipkin/ModbusRTUSlaveExampleCSharp
 * 
 * Created by: Steven Smethurst 
 * Created on: June 16, 2019 
 * Last updated: July 16, 2019 
 */

using Chipkin;
using ModbusExample;
using System;
using System.Runtime.InteropServices;
using System.IO.Ports;


namespace ModbusRTUSlaveExampleCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ModbusSlave modbusSlave = new ModbusSlave();
            modbusSlave.Run();
        }

        unsafe class ModbusSlave
        {
            // Version 
            const string APPLICATION_VERSION = "0.0.1";
            
            // Configuration Options 
            public const byte SETTING_MODBUS_SERVER_SLAVE_ADDRESS = 1;
            const ushort SETTING_METER_COUNT = 25;
            const ushort SETTING_METER_REGISTERS_COUNT = 200;
            const ushort SETTING_MODBUS_DATABASE_MAX_SIZE = SETTING_METER_COUNT * SETTING_METER_REGISTERS_COUNT;
            const ushort SETTING_MODBUS_DATABASE_DEFAULT_VALUE = 0x0000;

            // Database to hold the current values. 
            UInt16[] database;

            // Serial port 
            SerialPort _serialPort;

            public void Run()
            {
                // Prints the version of the application and the CAS BACnet stack. 
                Console.WriteLine("Starting Modbus RTU Slave Example  version {0}.{1}", APPLICATION_VERSION, CIBuildVersion.CIBUILDNUMBER);
                Console.WriteLine("https://github.com/chipkin/ModbusTCPMasterExampleCSharp");
                Console.WriteLine("FYI: CAS Modbus Stack version: {0}.{1}.{2}.{3}",
                    CASModbusAdapter.GetAPIMajorVersion(),
                    CASModbusAdapter.GetAPIMinorVersion(),
                    CASModbusAdapter.GetAPIPatchVersion(),
                    CASModbusAdapter.GetAPIBuildVersion());

                // Set up the API and callbacks.
                uint returnCode = CASModbusAdapter.Init(CASModbusAdapter.TYPE_RTU, SendMessage, RecvMessage, CurrentTime);
                if (returnCode != CASModbusAdapter.STATUS_SUCCESS)
                {
                    Console.WriteLine("Error: Could not init the Modbus Stack, returnCode={0}", returnCode);
                    return;
                }

                // Set the modbus slave address. For Modbus TCP 0 and 255 are the only valid slave address. 
                CASModbusAdapter.SetSlaveId(ModbusSlave.SETTING_MODBUS_SERVER_SLAVE_ADDRESS);

                // Set up the call back functions for data. 
                CASModbusAdapter.RegisterGetValue(GetModbusValue);
                CASModbusAdapter.RegisterSetValue(SetModbusValue);


                // All done with the Modbus setup. 
                Console.WriteLine("FYI: CAS Modbus Stack Setup, successfuly");

                // Create the database and fill it with default data 
                this.database = new UInt16[SETTING_MODBUS_DATABASE_MAX_SIZE];
                for (UInt16 offset = 0; offset < SETTING_MODBUS_DATABASE_MAX_SIZE; offset++)
                {
                    this.database[offset] = SETTING_MODBUS_DATABASE_DEFAULT_VALUE;
                }

                // Connect and set up the serial ports 

                // Create a new SerialPort object with default settings.
                _serialPort = new SerialPort();

                // Allow the user to set the appropriate properties.
                _serialPort.PortName = SetPortName(_serialPort.PortName);
                _serialPort.BaudRate = SetPortBaudRate(_serialPort.BaudRate);
                _serialPort.Parity = SetPortParity(_serialPort.Parity);
                _serialPort.DataBits = SetPortDataBits(_serialPort.DataBits);
                _serialPort.StopBits = SetPortStopBits(_serialPort.StopBits);
                _serialPort.Handshake = SetPortHandshake(_serialPort.Handshake);

                // Set the read/write timeouts
                _serialPort.ReadTimeout = 10;
                _serialPort.WriteTimeout = 10;

                _serialPort.Open();

                // Program loop. 
                while (true)
                {
                    // Check for user input 
                    this.DoUserInput();

                    // Run the Modbus loop proccessing incoming messages.
                    CASModbusAdapter.Loop();

                    // Give some time to other applications. 
                    System.Threading.Thread.Sleep(1);
                }
            }

            public bool SendMessage(System.UInt16 connectionId, System.Byte* payload, System.UInt16 payloadSize)
            {
                if(!_serialPort.IsOpen)
                {
                    return false; // Serial port is not open, can't send any bytes.
                }
                Console.WriteLine("FYI: Sending {0} bytes", payloadSize);

                // Copy from the unsafe pointer to a Byte array. 
                byte[] message = new byte[payloadSize];
                Marshal.Copy((IntPtr)payload, message, 0, payloadSize);

                try
                {
                    _serialPort.Write(message, 0, payloadSize);

                    // Message sent 
                    Console.Write("    ");
                    Console.WriteLine(BitConverter.ToString(message).Replace("-", " ")); // Convert bytes to HEX string. 
                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return false; 
                }
            }
            public int RecvMessage(System.UInt16* connectionId, System.Byte* payload, System.UInt16 maxPayloadSize)
            {
                if (!_serialPort.IsOpen)
                {
                    return 0; // Serial port is not open, can't recive any bytes.
                }

                try
                {
                    byte[] incommingMessage = new byte[maxPayloadSize];
                    int incommingMessageSize = _serialPort.Read(incommingMessage, 0, maxPayloadSize);
                    if (incommingMessageSize <= 0)
                    {
                        // Nothing recived. 
                        return 0;
                    }

                    // Copy from the unsafe pointer to a Byte array. 
                    byte[] message = new byte[incommingMessageSize];
                    Marshal.Copy(incommingMessage, 0, (IntPtr)payload, incommingMessageSize);

                    // Debug Show the data on the console.  
                    Console.WriteLine("FYI: Recived {0} bytes", incommingMessageSize);
                    Console.Write("    ");
                    Console.WriteLine(BitConverter.ToString(incommingMessage).Replace("-", " ").Substring(0, incommingMessageSize * 3)); // Convert bytes to HEX string. 
                    return incommingMessageSize;
                }
                catch (TimeoutException) {
                    // Timeout while wating 
                    return 0; 
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                    return 0;
                }
            }
            public ulong CurrentTime()
            {
                // https://stackoverflow.com/questions/9453101/how-do-i-get-epoch-time-in-c
                return (ulong)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            }

            public bool SetModbusValue(System.Byte slaveAddress, System.Byte function, System.UInt16 startingAddress, System.UInt16 length, System.Byte* data, System.UInt16 dataSize, System.Byte* errorCode)
            {
                Console.WriteLine("FYI: SetModbusValue slaveAddress=[{0}], function=[{1}], startingAddress=[{2}], length=[{3}], dataSize=[{4}]", slaveAddress, function, startingAddress, length, dataSize);

                if (startingAddress > SETTING_MODBUS_DATABASE_MAX_SIZE || startingAddress + length > SETTING_MODBUS_DATABASE_MAX_SIZE)
                {
                    // Out of range
                    *errorCode = CASModbusAdapter.EXCEPTION_02_ILLEGAL_DATA_ADDRESS;
                }

                switch (function)
                {
                    case CASModbusAdapter.FUNCTION_06_PRESET_SINGLE_REGISTER:
                    case CASModbusAdapter.FUNCTION_10_FORCE_MULTIPLE_REGISTERS:
                        {
                            short[] value = new short[length];
                            Marshal.Copy((IntPtr)data, value, 0, length);
                            for (ushort offset = 0; offset < length; offset++)
                            {
                                this.database[startingAddress + offset] = (ushort)value[offset];
                            }

                            return true;
                        }
                    default:
                        break;
                }


                return false;
            }
            public bool GetModbusValue(System.Byte slaveAddress, System.Byte function, System.UInt16 startingAddress, System.UInt16 length, System.Byte* data, System.UInt16 maxDataSize, System.Byte* errorCode)
            {
                Console.WriteLine("FYI: GetModbusValue slaveAddress=[{0}], function=[{1}], startingAddress=[{2}], length=[{3}]", slaveAddress, function, startingAddress, length);

                if (startingAddress > SETTING_MODBUS_DATABASE_MAX_SIZE)
                {
                    // Out of range
                    *errorCode = CASModbusAdapter.EXCEPTION_02_ILLEGAL_DATA_ADDRESS;
                }

                switch (function)
                {
                    case CASModbusAdapter.FUNCTION_03_READ_HOLDING_REGISTERS:
                    case CASModbusAdapter.FUNCTION_04_READ_INPUT_REGISTERS:

                        // Convert the USHORT into BYTE 
                        byte[] dataAsBytes = new byte[length * 2];
                        Buffer.BlockCopy(this.database, startingAddress, dataAsBytes, 0, length * 2);
                        Marshal.Copy(dataAsBytes, 0, (IntPtr)data, length * 2);
                        return true;

                    default:
                        break;
                }
                return false;
            }

            public void PrintHelp()
            {
                Console.WriteLine("FYI: Modbus Stack version: {0}.{1}.{2}.{3}",
                    CASModbusAdapter.GetAPIMajorVersion(),
                    CASModbusAdapter.GetAPIMinorVersion(),
                    CASModbusAdapter.GetAPIPatchVersion(),
                    CASModbusAdapter.GetAPIBuildVersion());

                Console.WriteLine("Help:");
                Console.WriteLine(" Q          - Quit");
                Console.WriteLine(" UP Arror   - Increase 40,001 by 1 ");
                Console.WriteLine(" Down Arror - Decrease 40,001 by 1 ");
                Console.WriteLine("\n");
            }

            private void DoUserInput()
            {
                if (Console.KeyAvailable)
                {
                    ConsoleKeyInfo key = Console.ReadKey(true);
                    Console.WriteLine("");
                    Console.WriteLine("FYI: Key {0} pressed. ", key.Key);

                    switch (key.Key)
                    {
                        case ConsoleKey.Q:
                            Console.WriteLine("FYI: Quit");
                            Environment.Exit(0);
                            break;
                        case ConsoleKey.UpArrow:
                            Console.WriteLine("FYI: Increase 40001 by 1. Before={0}, After={1}", this.database[0], this.database[0] + 1);
                            this.database[0]++;
                            break;
                        case ConsoleKey.DownArrow:
                            Console.WriteLine("FYI: Decrease 40001 by 1. Before={0}, After={1}", this.database[0], this.database[0] - 1);
                            this.database[0]--;
                            break;
                        default:
                            this.PrintHelp();
                            break;
                    }
                }
            }



            // Display Port values and prompt user to enter a port.
            public static string SetPortName(string defaultPortName)
            {
                string portName;

                Console.WriteLine("Available Ports:");
                foreach (string s in SerialPort.GetPortNames())
                {
                    Console.WriteLine("   {0}", s);
                }

                Console.Write("Enter COM port value (Default: {0}): ", defaultPortName);
                portName = Console.ReadLine();

                if (portName == "" || !(portName.ToLower()).StartsWith("com"))
                {
                    portName = defaultPortName;
                }
                return portName;
            }
            // Display BaudRate values and prompt user to enter a value.
            public static int SetPortBaudRate(int defaultPortBaudRate)
            {
                string baudRate;

                Console.Write("Baud Rate(default:{0}): ", defaultPortBaudRate);
                baudRate = Console.ReadLine();

                if (baudRate == "")
                {
                    baudRate = defaultPortBaudRate.ToString();
                }

                return int.Parse(baudRate);
            }

            // Display PortParity values and prompt user to enter a value.
            public static Parity SetPortParity(Parity defaultPortParity)
            {
                string parity;

                Console.WriteLine("Available Parity options:");
                foreach (string s in Enum.GetNames(typeof(Parity)))
                {
                    Console.WriteLine("   {0}", s);
                }

                Console.Write("Enter Parity value (Default: {0}):", defaultPortParity.ToString(), true);
                parity = Console.ReadLine();

                if (parity == "")
                {
                    parity = defaultPortParity.ToString();
                }

                return (Parity)Enum.Parse(typeof(Parity), parity, true);
            }
            // Display DataBits values and prompt user to enter a value.
            public static int SetPortDataBits(int defaultPortDataBits)
            {
                string dataBits;

                Console.Write("Enter DataBits value (Default: {0}): ", defaultPortDataBits);
                dataBits = Console.ReadLine();

                if (dataBits == "")
                {
                    dataBits = defaultPortDataBits.ToString();
                }

                return int.Parse(dataBits.ToUpperInvariant());
            }

            // Display StopBits values and prompt user to enter a value.
            public static StopBits SetPortStopBits(StopBits defaultPortStopBits)
            {
                string stopBits;

                Console.WriteLine("Available StopBits options:");
                foreach (string s in Enum.GetNames(typeof(StopBits)))
                {
                    Console.WriteLine("   {0}", s);
                }

                Console.Write("Enter StopBits value (None is not supported and \n" +
                 "raises an ArgumentOutOfRangeException. \n (Default: {0}):", defaultPortStopBits.ToString());
                stopBits = Console.ReadLine();

                if (stopBits == "")
                {
                    stopBits = defaultPortStopBits.ToString();
                }

                return (StopBits)Enum.Parse(typeof(StopBits), stopBits, true);
            }
            public static Handshake SetPortHandshake(Handshake defaultPortHandshake)
            {
                string handshake;

                Console.WriteLine("Available Handshake options:");
                foreach (string s in Enum.GetNames(typeof(Handshake)))
                {
                    Console.WriteLine("   {0}", s);
                }

                Console.Write("Enter Handshake value (Default: {0}):", defaultPortHandshake.ToString());
                handshake = Console.ReadLine();

                if (handshake == "")
                {
                    handshake = defaultPortHandshake.ToString();
                }

                return (Handshake)Enum.Parse(typeof(Handshake), handshake, true);
            }

        }
    }
}
