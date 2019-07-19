# Modbus RTU Slave Example CSharp

A basic Modbus RTU Slave example written in CSharp using the [CAS Modbus Stack](https://store.chipkin.com/services/stacks/modbus-stack)

## User input

- **Q** - Quit
- **UP Arror** - Increase 40,001 by 1
- **Down Arror** - Decrease 40,001 by 1

## Example Ouput

```txt

Starting Modbus RTU Slave Example  version 0.0.1.0
https://github.com/chipkin/ModbusTCPMasterExampleCSharp
FYI: CAS Modbus Stack version: 2.3.11.0
FYI: CAS Modbus Stack Setup, successfuly
Available Ports:
   COM1
   COM4
   COM5
Enter COM port value (Default: COM1): com4
Baud Rate(default:9600):
Available Parity options:
   None
   Odd
   Even
   Mark
   Space
Enter Parity value (Default: None):
Enter DataBits value (Default: 8):
Available StopBits options:
   None
   One
   Two
   OnePointFive
Enter StopBits value (None is not supported and
raises an ArgumentOutOfRangeException.
 (Default: One):
Available Handshake options:
   None
   XOnXOff
   RequestToSend
   RequestToSendXOnXOff
Enter Handshake value (Default: None):

FYI: Key Enter pressed.
FYI: Modbus Stack version: 2.3.11.0
Help:
 Q          - Quit
 UP Arror   - Increase 40,001 by 1
 Down Arror - Decrease 40,001 by 1


FYI: Recived 1 bytes
    01
FYI: Recived 7 bytes
    03 00 00 00 0A C5 CD
FYI: GetModbusValue slaveAddress=[1], function=[3], startingAddress=[0], length=[10]
FYI: Sending 25 bytes
    01 03 14 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 A3 67

FYI: Key UpArrow pressed.
FYI: Increase 40001 by 1. Before=0, After=1

FYI: Key UpArrow pressed.
FYI: Increase 40001 by 1. Before=1, After=2

FYI: Key UpArrow pressed.
FYI: Increase 40001 by 1. Before=2, After=3

FYI: Key UpArrow pressed.
FYI: Increase 40001 by 1. Before=3, After=4

FYI: Key UpArrow pressed.
FYI: Increase 40001 by 1. Before=4, After=5

FYI: Key UpArrow pressed.
FYI: Increase 40001 by 1. Before=5, After=6

FYI: Key UpArrow pressed.
FYI: Increase 40001 by 1. Before=6, After=7
FYI: Recived 1 bytes
    01
FYI: Recived 7 bytes
    03 00 00 00 0A C5 CD
FYI: GetModbusValue slaveAddress=[1], function=[3], startingAddress=[0], length=[10]
FYI: Sending 25 bytes
    01 03 14 00 07 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 16 13
FYI: Recived 1 bytes
    01
FYI: Recived 4 bytes
    06 00 00 00
FYI: Recived 1 bytes
    63
FYI: Recived 2 bytes
    C9 E3
FYI: SetModbusValue slaveAddress=[1], function=[6], startingAddress=[0], length=[1], dataSize=[2]
FYI: Sending 8 bytes
    01 06 00 00 00 63 C9 E3
FYI: Recived 8 bytes
    01 03 00 00 00 0A C5 CD
FYI: GetModbusValue slaveAddress=[1], function=[3], startingAddress=[0], length=[10]
FYI: Sending 25 bytes
    01 03 14 00 63 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 00 79 2B

FYI: Key Q pressed.
FYI: Quit

```

## Building

1. Copy *CASModbusStack_Win32_Debug.dll*, *CASModbusStack_Win32_Release.dll*, *CASModbusStack_x64_Debug.dll*, and *CASModbusStack_x64_Release.dll* from the [CAS Modbus Stack](https://store.chipkin.com/services/stacks/modbus-stack) project  into the /bin/ folder.
2. Use [Visual Studios 2019](https://visualstudio.microsoft.com/vs/) to build the project. The solution can be found in the */ModbusRTUSlaveExampleCSharp/* folder.

Note: The project is automaticly build on every checkin using GitlabCI.
