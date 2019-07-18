#define WINDOWS 
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Chipkin
{
    unsafe class CASModbusAdapter
    {

#if WINDOWS
#if (DEBUG)
        const string BACNET_API_DLL_FILENAME = "CASModbusStack_x64_Debug.dll";
#else
        const string BACNET_API_DLL_FILENAME = "CASModbusStack_x64_Release.dll";
#endif // DEBUG
#else
        // Linux 
#if (DEBUG)
        const string BACNET_API_DLL_FILENAME = "libCASModbusDLL_x64_Debug.so";
#else
        const string BACNET_API_DLL_FILENAME = "libCASModbusDLL_x64_Release.so";
#endif // DEBUG
#endif // _WINDOWS


        // Modbus types 
        public const uint TYPE_RTU = 1;
        public const uint TYPE_TCP = 2;

        // Modbus API return codes 
        public const uint STATUS_ERROR_UNKNOWN = 0;
        public const uint STATUS_SUCCESS = 1;
        public const uint STATUS_ERROR_MODBUS_EXCEPTION = 6;

        // Modbus functions 
        public const byte FUNCTION_01_READ_COIL_STATUS = 1;
        public const byte FUNCTION_02_READ_INPUT_STATUS = 2;
        public const byte FUNCTION_03_READ_HOLDING_REGISTERS = 3;
        public const byte FUNCTION_04_READ_INPUT_REGISTERS = 4;
        public const byte FUNCTION_05_FORCE_SINGLE_COIL = 5;
        public const ushort FORCE_SINGLE_COIL_ON = 0xFF00;
        public const ushort FORCE_SINGLE_COIL_OFF = 0x0000;
        public const byte FUNCTION_06_PRESET_SINGLE_REGISTER = 6;
        public const byte FUNCTION_0F_FORCE_MULTIPLE_COILS = 15;
        public const byte FUNCTION_10_FORCE_MULTIPLE_REGISTERS = 16;

        // Exception codes
        public const byte EXCEPTION_01_ILLEGAL_FUNCTION = 0x01;
        public const byte EXCEPTION_02_ILLEGAL_DATA_ADDRESS = 0x02;
        public const byte EXCEPTION_03_ILLEGAL_DATA_VALUE = 0x03;
        public const byte EXCEPTION_04_SLAVE_DEVICE_FAILURE = 0x04;
        public const byte EXCEPTION_05_ACKNOWLEDGE = 0x05;
        public const byte EXCEPTION_06_SLAVE_DEVICE_BUSY = 0x06;
        public const byte EXCEPTION_07_NEGIATIVE_ACKOWLEDGE = 0x07;
        public const byte EXCEPTION_08_MEMORY_PARITY_ERROR = 0x08;
        public const byte EXCEPTION_0A_GATEWAY_PATH_UNAVAILABLE = 0x0A;
        public const byte EXCEPTION_0B_GATEWAY_TARGET_DEVICE_FAILED_TO_RESPOND = 0x0B;



        #region Callback functions delegate
        // Call back functions delegate
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool delegateSendMessage(System.UInt16 connectionId, System.Byte* payload, System.UInt16 payloadSize);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int delegateRecvMessage(System.UInt16* connectionId, System.Byte* payload, System.UInt16 maxPayloadSize);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate ulong delegateCurrentTime();
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool delegateSetModbusValue(System.Byte slaveAddress, System.Byte function, System.UInt16 startingAddress, System.UInt16 length, System.Byte* data, System.UInt16 dataSize, System.Byte* errorCode);
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate bool delegateGetModbusValue(System.Byte slaveAddress, System.Byte function, System.UInt16 startingAddress, System.UInt16 length, System.Byte* data, System.UInt16 maxDataSize, System.Byte* errorCode);
        #endregion

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_GetAPIMajorVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAPIMajorVersion();

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_GetAPIMinorVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAPIMinorVersion();

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_GetAPIPatchVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAPIPatchVersion();

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_GetAPIBuildVersion", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int GetAPIBuildVersion();

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_SetSlaveId", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool SetSlaveId(System.Byte slaveId);

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_Init", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Init(uint type, delegateSendMessage sendMessage, delegateRecvMessage recvMessage, delegateCurrentTime currentTime);

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_ReadRegisters", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint ReadRegisters(System.UInt16 connectionId, System.Byte address, System.Byte function, System.UInt16 startingAddress, System.UInt16 length, System.IntPtr data, System.UInt16 maxData, System.Byte* exceptionCode);

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_WriteRegisters", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint WriteRegisters(System.UInt16 connectionId, System.Byte address, System.Byte function, System.UInt16 startingAddress, System.IntPtr data, System.UInt16 valuesSize, System.Byte* exceptionCode);

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_Loop", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Loop();

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_Flush", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint Flush();

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_RegisterSetValue", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint RegisterSetValue(delegateSetModbusValue setModbusValueFn);

        [DllImport(BACNET_API_DLL_FILENAME, EntryPoint = "ModbusStack_RegisterGetValue", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern uint RegisterGetValue(delegateGetModbusValue getModbusValueFnk);

        // Sets up the function pointers for the stack
        bool LoadModbusFunctions()
        {
            return LoadModbusDLLFunctions();
        }

        // Called from the LoadBACnetFunctions function if the LIB TYPE is set to DLL
        bool LoadModbusDLLFunctions()
        {
            return false;
        }
    }
}
