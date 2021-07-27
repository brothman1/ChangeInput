using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace ChangeInput.Core
{
    public static class MonitorInteraction
    {
        private static List<DisplayMonitor> _displayMonitorCollection = new List<DisplayMonitor>();
        private static readonly byte _inputSelectVcpCode = 0x60;
        private static readonly CircularDictionary<string, string> _inputSelectOptions = new CircularDictionary<string, string>()
        {
            { "0x01", "Analog video (R/G/B) 1" },
            { "0x02", "Analog video (R/G/B) 2" },
            { "0x03", "Digital video (TMDS) 1 DVI 1" },
            { "0x04", "Digital video (TMDS) 2 DVI 2" },
            { "0x05", "Composite video 1" },
            { "0x06", "Composite video 2" },
            { "0x07", "S-video 1" },
            { "0x08", "S-video 2" },
            { "0x09", "Tuner 1" },
            { "0x0A", "Tuner 2" },
            { "0x0B", "Tuner 3" },
            { "0x0C", "Component video (YPbPr / YCbCr) 1" },
            { "0x0D", "Component video (YPbPr / YCbCr) 2" },
            { "0x0E", "Component video (YPbPr / YCbCr) 3" },
            { "0x0F", "DisplayPort 1" },
            { "0x10", "DisplayPort 2" },
            { "0x11", "Digital Video (TMDS) 3 HDMI 1" },
            { "0x12", "Digital Video (TMDS) 4 HDMI 2" }
        };

        #region Windows API
        private struct Rectangle
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorInfoEx
        {
            public uint Size;
            public Rectangle MonitorArea;
            public Rectangle WorkArea;
            public uint Flags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string MonitorName;
        }
        private struct PhysicalMonitor
        {
            public IntPtr Handle;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Description;
        }
        private enum VcpCodeType
        {
            None = -1,
            Momentary,
            SetParameter
        }
        /// <summary>
        /// The EnumDisplayMonitors function enumerates display monitors (including invisible pseudo-monitors associated with the mirroring drivers) that intersect a region formed by the intersection of a specified clipping rectangle and the visible region of a device context. EnumDisplayMonitors calls an application-defined MonitorEnumProc callback function once for each monitor that is enumerated.
        /// </summary>
        /// <param name="monitorHandle">HDC hdc : A handle to a display device context that defines the visible region of interest.</param>
        /// <param name="pointer">LPCRECT lprcClip : A pointer to a RECT structure that specifies a clipping rectangle.</param>
        /// <param name="callback">MONITORENUMPROC lpfnEnum : A pointer to a MonitorEnumProc application-defined callback function.</param>
        /// <param name="appData">LPARAM dwData : Application-defined data that EnumDisplayMonitors passes directly to the MonitorEnumProc function.</param>
        /// <returns>If the function succeeds, the return value is nonzero.  If the function fails, the return value is zero.</returns>
        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumDisplayMonitors(IntPtr monitorHandle, IntPtr pointer, EnumDisplayMonitorsCallBack callback, IntPtr appData);

        /// <summary>
        /// A MonitorEnumProc function is an application-defined callback function that is called by the EnumDisplayMonitors function.
        /// </summary>
        /// <param name="monitorHandle">HMONITOR unnamedParam1 : A handle to the display monitor. This value will always be non-NULL.</param>
        /// <param name="deviceContextHandle">HDC unnamedParam2 : A handle to a device context.</param>
        /// <param name="monitorRect">LPRECT unnamedParam3 : A pointer to a RECT structure.</param>
        /// <param name="appData">LPARAM unnamedParam4 : Application-defined data that EnumDisplayMonitors passes directly to the enumeration function.</param>
        /// <returns>To continue the enumeration, return TRUE.  To stop the enumeration, return FALSE.</returns>
        [return: MarshalAs(UnmanagedType.Bool)]
        private delegate bool EnumDisplayMonitorsCallBack(IntPtr monitorHandle, IntPtr deviceContextHandle, IntPtr monitorRect, IntPtr appData);

        /// <summary>
        /// The GetMonitorInfo function retrieves information about a display monitor.
        /// </summary>
        /// <param name="monitorHandle">HMONITOR hMonitor : A handle to the display monitor of interest.</param>
        /// <param name="monitorInfo">LPMONITORINFO lpmi : A pointer to a MONITORINFO or MONITORINFOEX structure that receives information about the specified display monitor.</param>
        /// <returns>If the function succeeds, the return value is nonzero.  If the function fails, the return value is zero.</returns>
        [DllImport("User32.dll", EntryPoint = "GetMonitorInfoW")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(IntPtr monitorHandle, ref MonitorInfoEx monitorInfo);

        /// <summary>
        /// Retrieves the number of physical monitors associated with an HMONITOR monitor handle.
        /// </summary>
        /// <param name="monitorHandle">HMONITOR hMonitor : A monitor handle.</param>
        /// <param name="monitorCount">LPDWORD pdwNumberOfPhysicalMonitors : Receives the number of physical monitors associated with the monitor handle.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(IntPtr monitorHandle, out uint monitorCount);

        /// <summary>
        /// Retrieves the physical monitors associated with an HMONITOR monitor handle.
        /// </summary>
        /// <param name="handle">HMONITOR hMonitor : A monitor handle.</param>
        /// <param name="monitorCount">DWORD dwPhysicalMonitorArraySize : Number of elements in pPhysicalMonitorArray.</param>
        /// <param name="physicalMonitors">LPPHYSICAL_MONITOR pPhysicalMonitorArray : Pointer to an array of PHYSICAL_MONITOR structures.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr monitorHandle, uint monitorCount, [Out] PhysicalMonitor[] physicalMonitors);

        /// <summary>
        /// Closes a handle to a physical monitor.
        /// </summary>
        /// <param name="handle">HANDLE hMonitor : Handle to a physical monitor.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool DestroyPhysicalMonitor(IntPtr monitorHandle);

        /// <summary>
        /// Retrieves the current value, maximum value, and code type of a Virtual Control Panel (VCP) code for a monitor.
        /// </summary>
        /// <param name="physicalMonitorHandle">HANDLE hMonitor : Handle to a physical monitor.</param>
        /// <param name="vcpCode">BYTE vVCPCode : VCP code to query.</param>
        /// <param name="vcpCodeType"> LPMC_VCP_CODE_TYPE pvct : Receives the VCP code type enum.</param>
        /// <param name="currentValue">LPDWORD pdwCurrentValue : Receives the current value of the VCP code.</param>
        /// <param name="maximumValue">LPDWORD pdwMaximumValue : If bVCPCode specifies a continuous VCP code, this parameter receives the maximum value of the VCP code.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetVCPFeatureAndVCPFeatureReply(SafeMonitorHandle physicalMonitorHandle, byte vcpCode, out VcpCodeType vcpCodeType, out uint currentValue, out uint maximumValue);

        /// <summary>
        /// Retrieves the length of a monitor's capabilities string.
        /// </summary>
        /// <param name="physicalMonitorHandle">HANDLE hMonitor : Handle to a physical monitor.</param>
        /// <param name="capabilitiesStringLength">LPDWORD pdwCapabilitiesStringLengthInCharacters : Receives the length of the capabilities string, in characters, including the terminating null character.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool GetCapabilitiesStringLength(SafeMonitorHandle physicalMonitorHandle, out uint capabilitiesStringLength);

        /// <summary>
        /// Retrieves a string describing a monitor's capabilities.
        /// </summary>
        /// <param name="physicalMonitorHandle">HANDLE hMonitor : Handle to a physical monitor.</param>
        /// <param name="buffer">LPSTR pszASCIICapabilitiesString : Pointer to a buffer that receives the monitor's capabilities string.</param>
        /// <param name="capabilitiesStringLength">DWORD dwCapabilitiesStringLengthInCharacters : Size of pszASCIICapabilitiesString in characters, including the terminating null character.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool CapabilitiesRequestAndCapabilitiesReply(SafeMonitorHandle physicalMonitorHandle, [MarshalAs(UnmanagedType.LPStr)][Out] StringBuilder buffer, uint capabilitiesStringLength);

        /// <summary>
        /// Sets the value of a Virtual Control Panel (VCP) code for a monitor.
        /// </summary>
        /// <param name="physicalMonitorHandle">HANDLE hMonitor : Handle to a physical monitor.</param>
        /// <param name="VCPCode">BYTE bVCPCode : VCP code to set.</param>
        /// <param name="NewValue">DWORD dwNewValue : Value to set VCP code to.</param>
        /// <returns></returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetVCPFeature(SafeMonitorHandle physicalMonitorHandle, byte VCPCode, uint NewValue);
        #endregion
        internal static bool TryDestroyPhysicalMonitor(IntPtr monitorHandle)
        {
            if (!DestroyPhysicalMonitor(monitorHandle))
            {
                WindowsApiErrorHandler.RecordError();
                return false;
            }
            return true;
        }
        #region TryGetMonitors
        public static bool TryGetMonitors()
        {
            if (!TryGetDisplayMonitorCollection())
            {
                return false;
            }
            if (!TryUpdateDisplayMonitorCollection())
            {
                return false;
            }
            return true;
        }
        #region TryGetMonitorCollection
        private static bool TryGetDisplayMonitorCollection()
        {
            return EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, TryGetMonitor, IntPtr.Zero);
        }
        private static bool TryGetMonitor(IntPtr monitorHandle, IntPtr deviceContextHandle, IntPtr monitorRect, IntPtr appData)
        {
            MonitorInfoEx monitorInfoEx = new MonitorInfoEx { Size = (uint)Marshal.SizeOf<MonitorInfoEx>() };
            if (GetMonitorInfo(monitorHandle, ref monitorInfoEx))
            {
                DisplayMonitor displayMonitor = new DisplayMonitor(monitorHandle, monitorInfoEx.MonitorName);
                _displayMonitorCollection.Add(displayMonitor);
            }
            return true;
        }
        #endregion
        #region TryGetPhysicalMonitors
        private static bool TryGetPhysicalMonitors(IntPtr monitorHandle, out PhysicalMonitor[] physicalMonitors)
        {
            physicalMonitors = default;
            if (!TryGetPhysicalMonitorCount(monitorHandle, out uint monitorCount))
            {
                return false;
            }
            physicalMonitors = new PhysicalMonitor[monitorCount];
            if (!GetPhysicalMonitorsFromHMONITOR(monitorHandle, monitorCount, physicalMonitors))
            {
                WindowsApiErrorHandler.RecordError();
                return false;
            }
            return true;
        }
        private static bool TryGetPhysicalMonitorCount(IntPtr monitorHandle, out uint monitorCount)
        {
            if (!GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, out monitorCount))
            {
                WindowsApiErrorHandler.RecordError();
                return false;
            }
            return true;
        }
        #endregion
        #region TryUpdateDisplayMonitorCollection
        private static bool TryUpdateDisplayMonitorCollection()
        {
            foreach (DisplayMonitor displayMonitor in _displayMonitorCollection)
            {
                if (!TryGetMonitorCollection(displayMonitor.Handle, out List<Monitor> monitorCollection))
                {
                    return false;
                }
                displayMonitor.MonitorCollection = monitorCollection;
            }
            return true;
        }
        private static bool TryGetMonitorCollection(IntPtr handle, out List<Monitor> monitorCollection)
        {
            monitorCollection = default;
            if (!TryGetPhysicalMonitors(handle, out PhysicalMonitor[] physicalMonitors))
            {
                return false;
            }
            if (!TryGetMonitorCollectionDetails(physicalMonitors, out monitorCollection))
            {
                return false;
            }
            return true;
        }
        private static bool TryGetMonitorCollectionDetails(PhysicalMonitor[] physicalMonitors, out List<Monitor> monitorCollection)
        {
            monitorCollection = new List<Monitor>();
            foreach (PhysicalMonitor physicalMonitor in physicalMonitors)
            {
                SafeMonitorHandle physicalMonitorHandle = new SafeMonitorHandle(physicalMonitor.Handle);
                if (!TryGetInputDetails(physicalMonitorHandle, out string currentInput, out List<string> possibleInputs))
                {
                    return false;
                }
                Monitor monitor = new Monitor(physicalMonitorHandle, physicalMonitor.Description, currentInput, possibleInputs);
                monitorCollection.Add(monitor);
            }
            return true;
        }
        #endregion
        #region TryGetInputDetails
        private static bool TryGetInputDetails(SafeMonitorHandle physicalMonitorHandle, out string currentInput, out List<string> possibleInputs)
        {
            if (!TryGetCurrentInput(physicalMonitorHandle, out currentInput))
            {
                possibleInputs = default;
                return false;
            }
            if (!TryGetPossibleInputs(physicalMonitorHandle, out possibleInputs))
            {
                return false;
            }
            return true;
        }
        private static bool TryGetCurrentInput(SafeMonitorHandle physicalMonitorHandle, out string currentInput)
        {
            if (!GetVCPFeatureAndVCPFeatureReply(physicalMonitorHandle, _inputSelectVcpCode, out _, out uint currentValue, out _))
            {
                WindowsApiErrorHandler.RecordError();
                currentInput = default;
                return false;
            }
            currentInput = ConvertToHex((int)currentValue);
            return true;
        }
        private static bool TryGetPossibleInputs(SafeMonitorHandle physicalMonitorHandle, out List<string> possibleInputs)
        {
            possibleInputs = default;
            if (!TryGetCapabilities(physicalMonitorHandle, out string capabilities))
            {
                return false;
            }
            if (!TryParseInputSelectCapabilities(capabilities, out possibleInputs))
            {
                return false;
            }
            return true;
        }
        private static bool TryGetCapabilities(SafeMonitorHandle physicalMonitorHandle, out string capabilities)
        {
            capabilities = default;
            if (!GetCapabilitiesStringLength(physicalMonitorHandle, out uint capabilitiesStringLength))
            {
                WindowsApiErrorHandler.RecordError();
                return false;
            }
            StringBuilder capabilitiesBuffer = new StringBuilder((int)capabilitiesStringLength);
            if (!CapabilitiesRequestAndCapabilitiesReply(physicalMonitorHandle, capabilitiesBuffer, capabilitiesStringLength))
            {
                WindowsApiErrorHandler.RecordError();
                return false;
            }
            capabilities = capabilitiesBuffer.ToString();
            return true;
        }
        private static bool TryParseInputSelectCapabilities(string capabilities, out List<string> inputSelectCapabilitiesList)
        {
            inputSelectCapabilitiesList = default;
            if (!TrySearchForVcpCapabilities(capabilities, out string vcpCapabilities))
            {
                return false;
            }
            if (!TrySearchForInputSelectCapabilities(vcpCapabilities, out string inputSelectCapabilities))
            {
                return false;
            }
            inputSelectCapabilitiesList = inputSelectCapabilities.Split(' ').Select(x => $"0x{x.ToUpper()}").ToList();
            return true;
        }
        private static bool TrySearchForVcpCapabilities(string capabilities, out string vcpCapabilities)
        {
            return capabilities.SearchForBalancedEnd(BreakType.Parenthesis, out vcpCapabilities, "vcp");
        }
        private static bool TrySearchForInputSelectCapabilities(string vcpCapabilities, out string inputSelectCapabilities)
        {
            return vcpCapabilities.SearchForBalancedEnd(BreakType.Parenthesis, out inputSelectCapabilities, "60");
        }
        private static string ConvertToHex(int input)
        {
            string output = Convert.ToString(input, 16).ToUpper();
            if (output.Length == 1)
            {
                return "0x0" + output;
            }
            else
            {
                return "0x" + output;
            }
        }
        #endregion
        #endregion
        public static IEnumerable<string> GetMonitorDetails()
        {
            return _displayMonitorCollection
                    .SelectMany(displayMonitor => displayMonitor.MonitorCollection, (displayMonitor, monitor) => new { displayMonitor, monitor })
                    .Select(item => FormatMonitorDetails(item.displayMonitor, item.monitor));
        }
        private static string FormatMonitorDetails(DisplayMonitor displayMonitor, Monitor monitor)
        {
            StringBuilder monitorDetails = new StringBuilder();
            monitorDetails.Append($"Monitor Name: {displayMonitor.Name}\n");
            monitorDetails.Append($"Current Input: {_inputSelectOptions[monitor.CurrentInput]}\n");
            monitorDetails.Append($"Possible Inputs: {string.Join(",", monitor.PossibleInputs.Select(input => _inputSelectOptions[input]))}\n");
            return monitorDetails.ToString();
        }
    }
}
