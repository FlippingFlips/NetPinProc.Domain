#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
using NetPinProc.Domain.Json;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text.Json.Serialization;
// The MIT License
// 
// Copyright (c) 2010 Adam Preble, Gerry Stellenberg
// 
// Permission is hereby granted, free of charge, to any person obtaining a Copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, Copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

namespace NetPinProc.Domain.PinProc
{
    /// <inheritdoc/>
    public static class PinProcConstants
    {
        /// <inheritdoc/>
        public const int kPRDriverCount = 256;
        /// <inheritdoc/>
        public const int kPRSwitchCount = 256;
    }

    /// <summary>
    /// Class to decode P-ROC stuff
    /// </summary>
    public static class PinProcDecoder
    {
        /// <summary>
        /// Decodes a machine item string into P-ROC number. This is used by the fake pinproc device <para/>
        /// Conversion from libpinproc / pinproc.cpp
        /// </summary>
        /// <param name="machineType"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static ushort PRDecode(MachineType machineType, string str)
        {
            ushort x;

            if (str == null)
                return 0;

            if (str.Length == 3)
                x = (ushort)((str[1] - '0') * 10 + (str[2] - '0'));
            else if (str.Length == 4)
                x = (ushort)((str[2] - '0') * 10 + (str[3] - '0'));
            else return ushort.Parse(str);

            if ((machineType == MachineType.WPC) ||
                (machineType == MachineType.WPC95) ||
                (machineType == MachineType.WPCAlphanumeric))
            {
                switch (str[0])
                {
                    case 'F':
                    case 'f':
                        switch (str[1])
                        {
                            case 'L':
                            case 'l':
                                switch (str[2])
                                {
                                    case 'R':
                                    case 'r':
                                        switch (str[3])
                                        {
                                            case 'M':
                                            case 'm':
                                                return 32;
                                            default:
                                                return 33;
                                        }
                                    default:
                                        switch (str[3])
                                        {
                                            case 'M':
                                            case 'm':
                                                return 34;
                                            default:
                                                return 35;
                                        }
                                }
                            default:
                                switch (str[2])
                                {
                                    case 'R':
                                    case 'r':
                                        switch (str[3])
                                        {
                                            case 'M':
                                            case 'm':
                                                return 36;
                                            default:
                                                return 37;
                                        }
                                    default:
                                        switch (str[3])
                                        {
                                            case 'M':
                                            case 'm':
                                                return 38;
                                            default:
                                                return 39;
                                        }
                                }
                        }

                    case 'L':
                    case 'l':
                        return (ushort)(80 + 8 * ((x / 10) - 1) + ((x % 10) - 1));
                    case 'C':
                    case 'c':
                        if (x <= 28)
                            return (ushort)(x + 39);
                        else if (x <= 36)
                            return (ushort)(x + 3);
                        else if (x <= 44)
                        {
                            if (machineType == MachineType.WPC95)
                                //return x + 7;
                                return (ushort)(x + 31);
                            else
                                return (ushort)(x + 107); // WPC 37-44 use 8-driver board (mapped to drivers 144-151)
                        }
                        else return (ushort)(x + 108);
                    case 'G':
                    case 'g':
                        return (ushort)(x + 71);
                    case 'S':
                    case 's':
                        {
                            switch (str[1])
                            {
                                case 'D':
                                case 'd':
                                    return (ushort)(8 + ((str[2] - '0') - 1));
                                case 'F':
                                case 'f':
                                    return (ushort)((str[2] - '0') - 1);
                                default:
                                    return (ushort)(32 + 16 * ((x / 10) - 1) + ((x % 10) - 1));
                            }
                        }
                }
            }
            else if (machineType == MachineType.SternSAM)
            {
                switch (str[0])
                {
                    case 'L':
                    case 'l':
                        return (ushort)(80 + 16 * (7 - ((x - 1) % 8)) + (x - 1) / 8);
                    case 'C':
                    case 'c':
                        return (ushort)(x + 31);
                    case 'S':
                    case 's':
                        {
                            switch (str[1])
                            {
                                case 'D':
                                case 'd':
                                    if (str.Length == 3)
                                        return (ushort)((str[2] - '0') + 7);
                                    else return (ushort)(x + 7);
                                default:
                                    if ((x - 1) % 16 < 8)
                                        return (ushort)(32 + 8 * ((x - 1) / 8) + (7 - ((x - 1) % 8)));
                                    else
                                        return (ushort)(32 + (x - 1));
                            }
                        }
                }
            }
            else if (machineType == MachineType.SternWhitestar)
            {
                switch (str[0])
                {
                    case 'L':
                    case 'l':
                        return (ushort)(80 + 16 * (7 - ((x - 1) % 8)) + (x - 1) / 8);
                    case 'C':
                    case 'c':
                        return (ushort)(x + 31);
                    case 'S':
                    case 's':
                        {
                            switch (str[1])
                            {
                                case 'D':
                                case 'd':
                                    if (str.Length == 3)
                                        return (ushort)((str[2] - '0') + 7);
                                    else return (ushort)(x + 7);
                                default:
                                    return (ushort)(32 + 16 * (((x - 1) / 8)) + (7 - ((x - 1) % 8)));
                            }
                        }
                }
            }
            return ushort.Parse(str);
        }
    }

    /// <inheritdoc/>
    public enum EventType
    {
        /// <inheritdoc/>
        Invalid = 0,
        /// <inheritdoc/>
        SwitchClosedDebounced = 1,
        /// <inheritdoc/>
        SwitchOpenDebounced = 2,
        /// <inheritdoc/>
        SwitchClosedNondebounced = 3,
        /// <inheritdoc/>
        SwitchOpenNondebounced = 4,
        /// <inheritdoc/>
        DMDFrameDisplayed = 5,
        /// <inheritdoc/>
        None = 6
    };

    /// <inheritdoc/>
    public enum LogLevel
    {
        /// <inheritdoc/>
        Verbose = 0,
        /// <inheritdoc/>
        Debug = 1,
        /// <inheritdoc/>
        Info = 2,
        /// <inheritdoc/>
        Warning = 3,
        /// <inheritdoc/>
        Error = 4
    };

    /// <summary>
    /// Machine type the P-ROC is working with
    /// </summary>
    [JsonConverter(typeof(StringEnumConverter<MachineType>))]
    public enum MachineType
    {
        /// <inheritdoc/>
        Invalid = 0,
        /// <summary>
        /// Custom machine type
        /// </summary>
        Custom = 1,
        /// <summary>
        /// Alpha display
        /// </summary>
        WPCAlphanumeric = 2,
        /// <summary>
        /// Williams
        /// </summary>
        WPC = 3,
        /// <summary>
        /// Williams 95
        /// </summary>
        WPC95 = 4,
        /// <summary>
        /// Data East
        /// </summary>
        SternWhitestar = 5,
        /// <summary>
        /// Later stern machines
        /// </summary>
        SternSAM = 6,
        /// <summary>
        /// P3-ROC machine board
        /// </summary>
        PDB = 7
    };

    /// <summary>
    /// Success or Fail
    /// </summary>
    public enum Result { Success = 1, Failure = 0 };

    [JsonConverter(typeof(StringEnumConverter<SwitchType>))]
    public enum SwitchType
    {
        NO = 0,
        NC = 1
    };
    /// <summary>
    /// Status: Partial test passed (only checked DeHighCycles)
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Size = 60), Serializable]
    public struct DMDConfig
    {
        public byte NumRows;
        public UInt16 NumColumns;
        public byte NumSubFrames;
        public byte NumFrameBuffers;
        public bool AutoIncBufferWrPtr;
        public bool EnableFrameEvents;
        public bool Enable;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public Byte[] RclkLowCycles;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public Byte[] LatchHighCycles;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U2, SizeConst = 8)]
        public UInt16[] DeHighCycles;
        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.U1, SizeConst = 8)]
        public Byte[] DotclkHalfPeriod;

        public DMDConfig(int columns, int rows)
        {
            this.AutoIncBufferWrPtr = true;
            this.NumRows = (byte)rows;
            this.NumColumns = (UInt16)columns;
            this.NumSubFrames = 4;
            this.NumFrameBuffers = 3;
            this.Enable = true;
            this.EnableFrameEvents = true;
            this.RclkLowCycles = new byte[8];
            this.LatchHighCycles = new byte[8];
            this.DeHighCycles = new UInt16[8];
            this.DotclkHalfPeriod = new byte[8];
            for (int i = 0; i < 8; i++)
            {
                this.RclkLowCycles[i] = 15;
                this.LatchHighCycles[i] = 15;
                this.DotclkHalfPeriod[i] = 1;
            }
            // 60 fps timing:
            this.DeHighCycles[0] = 90;
            this.DeHighCycles[1] = 190;
            this.DeHighCycles[2] = 50;
            this.DeHighCycles[3] = 377;
        }
    };

    /// <summary>
    /// 
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct DriverGlobalConfig
    {
        public bool EnableOutputs;
        public bool GlobalPolarity;
        public bool UseClear;
        public bool StrobeStartSelect;
        public byte StartStrobeTime;
        public byte MatrixRowEnableIndex1;
        public byte MatrixRowEnableIndex0;
        public bool ActiveLowMatrixRows;
        public bool EncodeEnables;
        public bool TickleSternWatchdog;
        public bool WatchdogExpired;
        public bool WatchdogEnable;
        public UInt16 WatchdogResetTime;
    };

    /// <summary>
    /// DriverGroupConfig
    /// </summary>
    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct DriverGroupConfig
    {
        public byte GroupNum;
        public UInt16 SlowTime;
        public byte EnableIndex;
        public byte RowActivateIndex;
        public byte RowEnableSelect;
        public bool Matrixed;
        public bool Polarity;
        public bool Active;
        public bool DisableStrobeAfter;
    };

    [StructLayout(LayoutKind.Sequential, Size = 28), Serializable]
    public struct DriverState
    {
        public UInt16 DriverNum;
        public byte OutputDriveTime;
        public bool Polarity;
        public bool State;
        public bool WaitForFirstTimeSlot;
        public UInt32 Timeslots;
        public byte PatterOnTime;
        public byte PatterOffTime;
        public bool PatterEnable;
        public bool futureEnable;

        public override string ToString() { return string.Format("DriverState num={0}", DriverNum); }
    };

    // Status: Tested good.
    [StructLayout(LayoutKind.Sequential, Size = 12), Serializable]
    public struct Event
    {
        public EventType Type;
        public UInt32 Value;
        public UInt32 Time;

        public override string ToString() { return string.Format("Event type={0} value={1}", Type, Value); }
    };

    [StructLayout(LayoutKind.Sequential)]
    public struct PRLED
    {
        public byte boardAddr;
        public byte LEDIndex;
    };

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct PRLEDRGB
    {
        public PRLED pRedLED { get; set; }
        public PRLED pGreenLED { get; set; }
        public PRLED pBlueLED { get; set; }
    };

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct SwitchConfig
    {
        public bool Clear;
        public bool HostEventsEnable;
        public bool UseColumn9;
        public bool UseColumn8;
        public byte DirectMatrixScanLoopTime;
        public byte PulsesBeforeCheckingRX;
        public byte InactivePulsesAfterBurst;
        public byte PulsesPerBurst;
        public byte PulseHalfPeriodTime;
    };

    [StructLayout(LayoutKind.Sequential), Serializable]
    public struct SwitchRule
    {
        public bool ReloadActive;
        public bool NotifyHost;
    };

    public struct FakeSwitchRule
    {
        public bool NotifyHost;
        public bool ReloadActive;
        public List<IDriver> Drivers;
    }
}
