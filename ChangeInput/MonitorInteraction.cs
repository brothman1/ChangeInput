using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Diagnostics;

namespace ChangeInput
{
    internal static class Error
    {
        #region Windows API Calls
        /// <summary>
        /// Formats a message string. The function requires a message definition as input. The message definition can come from a buffer passed into the function. It can come from a message table resource in an already-loaded module. Or the caller can ask the function to search the system's message table resource(s) for the message definition. The function finds the message definition in a message table resource based on a message identifier and a language identifier. The function copies the formatted message text to an output buffer, processing any embedded insert sequences if requested.
        /// </summary>
        /// <param name="formatOption">DWORD dwFlags : The formatting options, and how to interpret the lpSource parameter.</param>
        /// <param name="sourceLocation">LPCVOID lpSource : The location of the message definition.</param>
        /// <param name="messageId">DWORD dwMessageId : The message identifier for the requested message</param>
        /// <param name="languageId">DWORD dwLanguageId : The language identifier for the requested message.</param>
        /// <param name="buffer">LPTSTR lpBuffer : A pointer to a buffer that receives the null-terminated string that specifies the formatted message.</param>
        /// <param name="size">DWORD nSize : If the FORMAT_MESSAGE_ALLOCATE_BUFFER flag is not set, this parameter specifies the size of the output buffer, in TCHARs. If FORMAT_MESSAGE_ALLOCATE_BUFFER is set, this parameter specifies the minimum number of TCHARs to allocate for an output buffer.</param>
        /// <param name="arguments">va_list *Arguments : An array of values that are used as insert values in the formatted message.</param>
        /// <returns>If the function succeeds, the return value is the number of TCHARs stored in the output buffer, excluding the terminating null character.If the function fails, the return value is zero.</returns>
        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern uint FormatMessage(uint formatOption, IntPtr sourceLocation, uint messageId, uint languageId, StringBuilder buffer, int size, IntPtr arguments);
        #endregion
        #region Methods
        /// <summary>
        /// Translates last windows error to it's cooresponding windows error code and message.
        /// </summary>
        /// <returns>String representing error message</returns>
        public static string GetMessage()
        {
            int errorCode = Marshal.GetLastWin32Error();
            StringBuilder buffer = new StringBuilder(512);
            if (FormatMessage(FormatOption,IntPtr.Zero,(uint)errorCode,LanguageId,buffer,buffer.Capacity,IntPtr.Zero)>0)
            {
                return string.Format("({0}) {1}", errorCode, buffer.ToString());
            }
            else
            {
                return string.Format("({0}) {1}", errorCode, "Unable to retrieve error message.");
            }
        }
        #endregion
        #region Constants
        /// <summary>
        /// FORMAT_MESSAGE_FROM_SYSTEM
        /// </summary>
        private const uint FormatOption = 0x00001000;
        /// <summary>
        /// US (English)
        /// </summary>
        private const uint LanguageId = 0x0409;
        #endregion
    }
    public static class MonitorInteraction
    {
        #region Windows API Calls
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
        private static extern bool GetMonitorInfo(IntPtr monitorHandle, ref MonitorInfo monitorInfo);

        /// <summary>
        /// The EnumDisplayDevices function lets you obtain information about the display devices in the current session.
        /// </summary>
        /// <param name="deviceName">LPCSTR lpDevice : A pointer to the device name.</param>
        /// <param name="deviceIndex">DWORD iDevNum : An index value that specifies the display device of interest.</param>
        /// <param name="displayDevice">PDISPLAY_DEVICEA lpDisplayDevice : A pointer to a DISPLAY_DEVICE structure that receives information about the display device specified by iDevNum.</param>
        /// <param name="flags">DWORD dwFlags : Set this flag to EDD_GET_DEVICE_INTERFACE_NAME (0x00000001) to retrieve the device interface name for GUID_DEVINTERFACE_MONITOR, which is registered by the operating system on a per monitor basis. The value is placed in the DeviceID member of the DISPLAY_DEVICE structure returned in lpDisplayDevice. The resulting device interface name can be used with SetupAPI functions and serves as a link between GDI monitor devices and SetupAPI monitor devices.</param>
        /// <returns>If the function succeeds, the return value is nonzero.  If the function fails, the return value is zero.</returns>
        [DllImport("User32.dll", EntryPoint = "EnumDisplayDevicesA")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool EnumDisplayDevices(string deviceName, uint deviceIndex, ref DisplayDevice displayDevice, uint flags);

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
        /// <param name="monitorHandle">HMONITOR hMonitor : A monitor handle.</param>
        /// <param name="monitorCount">DWORD dwPhysicalMonitorArraySize : Number of elements in pPhysicalMonitorArray.</param>
        /// <param name="physicalMonitors">LPPHYSICAL_MONITOR pPhysicalMonitorArray : Pointer to an array of PHYSICAL_MONITOR structures.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicalMonitorsFromHMONITOR(IntPtr monitorHandle, uint monitorCount, [Out] PhysicalMonitor[] physicalMonitors);

        /// <summary>
        /// Closes a handle to a physical monitor.
        /// </summary>
        /// <param name="monitorHandle">HANDLE hMonitor : Handle to a physical monitor.</param>
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
        private static extern bool GetVCPFeatureAndVCPFeatureReply(MonitorHandle physicalMonitorHandle, byte vcpCode, out VcpCodeType vcpCodeType, out uint currentValue, out uint maximumValue);

        /// <summary>
        /// Retrieves the length of a monitor's capabilities string.
        /// </summary>
        /// <param name="physicalMonitorHandle">HANDLE hMonitor : Handle to a physical monitor.</param>
        /// <param name="capabilitiesStringLength">LPDWORD pdwCapabilitiesStringLengthInCharacters : Receives the length of the capabilities string, in characters, including the terminating null character.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool GetCapabilitiesStringLength(MonitorHandle physicalMonitorHandle, out uint capabilitiesStringLength);

        /// <summary>
        /// Retrieves a string describing a monitor's capabilities.
        /// </summary>
        /// <param name="physicalMonitorHandle">HANDLE hMonitor : Handle to a physical monitor.</param>
        /// <param name="buffer">LPSTR pszASCIICapabilitiesString : Pointer to a buffer that receives the monitor's capabilities string.</param>
        /// <param name="capabilitiesStringLength">DWORD dwCapabilitiesStringLengthInCharacters : Size of pszASCIICapabilitiesString in characters, including the terminating null character.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        [DllImport("Dxva2.dll", SetLastError = true)]
        private static extern bool CapabilitiesRequestAndCapabilitiesReply(MonitorHandle physicalMonitorHandle,[MarshalAs(UnmanagedType.LPStr)] [Out] StringBuilder buffer, uint capabilitiesStringLength);







        #endregion
        #region Windows API Structs
        /// <summary>
        /// Rectangle struct with only Left, Top, Right, Bottom to be used by Windows API Calls.
        /// </summary>
        private struct Rectangle
        {
            /// <summary>
            /// The x-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Left;
            /// <summary>
            /// The y-coordinate of the upper-left corner of the rectangle.
            /// </summary>
            public int Top;
            /// <summary>
            /// The x-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Right;
            /// <summary>
            /// The y-coordinate of the lower-right corner of the rectangle.
            /// </summary>
            public int Bottom;
        }

        /// <summary>
        /// MONITORINFOEX : The MONITORINFO structure contains information about a display monitor, and MONITORINFOEX structure is a superset of the MONITORINFO structure. The MONITORINFOEX structure adds a string member to contain a name for the display monitor.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct MonitorInfo
        {
            /// <summary>
            /// DWORD cbSize : The size of the structure, in bytes.
            /// </summary>
            public uint Size;
            /// <summary>
            /// RECT rcMonitor : A RECT structure that specifies the display monitor rectangle, expressed in virtual-screen coordinates.
            /// </summary>
            public Rectangle MonitorArea;
            /// <summary>
            /// RECT rcWork : A RECT structure that specifies the work area rectangle of the display monitor, expressed in virtual-screen coordinates.
            /// </summary>
            public Rectangle WorkArea;
            /// <summary>
            /// DWORD dwFlags : A set of flags that represent attributes of the display monitor.
            /// </summary>
            public uint Flags;
            /// <summary>
            /// CHAR szDevice : A string that specifies the device name of the monitor being used.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string MonitorName;
        }

        /// <summary>
        /// The DISPLAY_DEVICE structure receives information about the display device specified by the iDevNum parameter of the EnumDisplayDevices function.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        private struct DisplayDevice
        {
            /// <summary>
            /// DWORD cb : Size, in bytes, of the DISPLAY_DEVICE structure.
            /// </summary>
            public uint Size;
            /// <summary>
            /// CHAR DeviceName : An array of characters identifying the device name.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string DeviceName;
            /// <summary>
            /// CHAR DeviceString : An array of characters containing the device context string.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceString;
            /// <summary>
            /// DWORD StateFlags : Device state flags.
            /// </summary>
            public DisplayDeviceFlag StateFlags;
            /// <summary>
            /// CHAR DeviceID : Not used.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceID;
            /// <summary>
            /// CHAR DeviceKey : Reserved.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string DeviceKey;
        }

        /// <summary>
        /// Contains a handle and text description corresponding to a physical monitor.
        /// </summary>
        private struct PhysicalMonitor
        {
            /// <summary>
            /// HANDLE hPhysicalMonitor : Handle to the physical monitor.
            /// </summary>
            public IntPtr Handle;
            /// <summary>
            /// WCHAR szPhysicalMonitorDescription : Text description of the physical monitor.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
            public string Description;
        }
        #endregion
        #region Windows API Enums
        [Flags]
        private enum DisplayDeviceFlag : uint
        {
            AttachedToDesktop = 0x00000001,
            MultiDriver = 0x00000002,
            PrimaryDevice = 0x00000004,
            MirroringDriver = 0x00000008,
            VgaCompatible = 0x00000010,
            Removable = 0x00000020,
            AccDriver = 0x00000040,
            Rdpudd = 0x01000000,
            Disconnect = 0x02000000,
            Remote = 0x04000000,
            ModesPruned = 0x08000000,
            Active = 0x00000001,
            Attached = 0x00000002,
        }

        private enum VcpCodeType
        {
            Momentary,
            SetParameter
        }
        #endregion
        #region Sub-Classes
        internal class Monitor
        {
            public IntPtr Handle { get; set; }
            public MonitorHandle PhysicalHandle { get; set; }
            public string Name { get; set; }
            public string CurrentInput { get; set; }
            public List<string> PossibleInputs { get; set; }
        }
        internal class MonitorCollection : Dictionary<string,Monitor>
        {
        }
        /// <summary>
        /// Derive MonitorHandle class from SafeHandle.
        /// </summary>
        internal class MonitorHandle : SafeHandle
        {
            /// <summary>
            /// overload SafeHandle constructor to allow IntPtr.Zero to be a valid handle
            /// </summary>
            /// <param name="handle">Handle to a physical montior.</param>
            public MonitorHandle(IntPtr handle) : base(IntPtr.Zero, true)
            {
                this.handle = handle;
            }
            /// <summary>
            /// Validity cannot be checked by the handle, so override IsInvalid to false.
            /// </summary>
            public override bool IsInvalid => false;
            /// <summary>
            /// Override ReleaseHandle with windows API Call to DestroyPhysicalMonitor
            /// </summary>
            /// <returns></returns>
            protected override bool ReleaseHandle()
            {
                return DestroyPhysicalMonitor(handle);
            }
        }
        #endregion
        #region Methods
        internal static MonitorCollection GetMonitors()
        {
            MonitorCollection monitors = new MonitorCollection();

            if (EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, callback, IntPtr.Zero))
            {
                foreach (KeyValuePair<string,Monitor> monitor in monitors)
                {
                    uint iteration = 0;
                    uint size = (uint)Marshal.SizeOf<DisplayDevice>();
                    DisplayDevice display = new DisplayDevice { Size = size };
                    while (EnumDisplayDevices(null, iteration, ref display, GetDeviceInterfaceNameFlag))
                    {
                        iteration++;
                        if (display.StateFlags.HasFlag(DisplayDeviceFlag.MirroringDriver))
                        {
                            continue;
                        }
                        //if (!GetDisplayIndex(monitor.Key, out byte displayindex))
                        //{
                        //    continue;
                        //}
                        uint innerIteration = 0;
                        DisplayDevice displayMonitor = new DisplayDevice { Size = size };
                        while (EnumDisplayDevices(display.DeviceName, innerIteration, ref displayMonitor, GetDeviceInterfaceNameFlag))
                        {
                            innerIteration++;
                            if (!displayMonitor.StateFlags.HasFlag(DisplayDeviceFlag.Active))
                            {
                                continue;
                            }
                            Console.WriteLine(displayMonitor.DeviceName);
                            Console.WriteLine(displayMonitor.DeviceString);
                            Console.WriteLine(displayMonitor.DeviceID);
                            Console.WriteLine(displayMonitor.DeviceKey);
                        }
                    }
                }
                

                return monitors;
            }
            else
            {
                return new MonitorCollection();
            }

            bool callback(IntPtr monitorHandle, IntPtr deviceContextHandle, IntPtr monitorRect, IntPtr appData)
            {
                MonitorInfo monitorInfo = new MonitorInfo { Size = (uint)Marshal.SizeOf<MonitorInfo>() };

                if (GetMonitorInfo(monitorHandle, ref monitorInfo))
                {
                    Monitor monitor = new Monitor() 
                    { 
                        Handle = monitorHandle,
                        Name = monitorInfo.MonitorName
                    };
                    if (!GetPhysicalMonitorCount(monitorHandle, out uint monitorCount))
                    {
                        return true;
                    }
                    if (!GetPhysicalMonitors(monitorHandle, monitorCount, out PhysicalMonitor[] physicalMonitors))
                    {
                        return true;
                    }
                    foreach (PhysicalMonitor physicalMonitor in physicalMonitors)
                    {
                        monitor.PhysicalHandle = new MonitorHandle(physicalMonitor.Handle);

                        if (!GetInputSelectValues(monitor.PhysicalHandle, out uint currentValue, out uint maximumValue))
                        {
                            return true;
                        }
                        monitor.CurrentInput = ConvertToHex((int)currentValue);
                        if (!GetPossibleInputs(monitor.PhysicalHandle, out List<string> possibleInputs))
                        {
                            return true;
                        }
                        monitor.PossibleInputs = possibleInputs;
                    }

                    

                    monitors.Add(monitor.Name, monitor);
                }
                return true;
            }
        }

        internal static string TranslateInputSelectHex(string input)
        {
            return InputSelectOptions[input];
        }

        /// <summary>
        /// Windows API Call to get count of physical monitors, and writes errors to output.
        /// </summary>
        /// <param name="monitorHandle">A monitor handle.</param>
        /// <param name="monitorCount">Receives the number of physical monitors associated with the monitor handle.</param>
        /// <returns>True on success, False on failure.</returns>
        private static bool GetPhysicalMonitorCount(IntPtr monitorHandle, out uint monitorCount)
        {
            bool success = GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, out monitorCount);
            if (!success)
            {
                Debug.WriteLine(string.Format("Failed to get number of physical monitors.  {0}", Error.GetMessage()));
            }
            return success;
        }

        /// <summary>
        /// Windows API Call to get physical monitors into an array, and writes errors to output.
        /// </summary>
        /// <param name="monitorHandle">A monitor handle.</param>
        /// <param name="monitorCount">Number of elements in pPhysicalMonitorArray.</param>
        /// <param name="physicalMonitors">Pointer to an array of PHYSICAL_MONITOR structures.</param>
        /// <returns>True on success, False on failure.</returns>
        private static bool GetPhysicalMonitors(IntPtr monitorHandle, uint monitorCount, out PhysicalMonitor[] physicalMonitors)
        {
            physicalMonitors = new PhysicalMonitor[monitorCount];
            bool success = GetPhysicalMonitorsFromHMONITOR(monitorHandle, monitorCount, physicalMonitors);
            if (!success)
            {
                Debug.WriteLine(string.Format("Failed to get physical monitors.  {0}", Error.GetMessage()));
            }
            return success;
        }

        /// <summary>
        /// Windows API Call to get current value and maximum value for VCP input select and writes errors to output.
        /// </summary>
        /// <param name="physicalMonitorHandle">Handle to a physical monitor.</param>
        /// <param name="currentValue">Receives the current value of the VCP code.</param>
        /// <param name="maximumValue">If bVCPCode specifies a continuous VCP code, this parameter receives the maximum value of the VCP code.</param>
        /// <returns>If the function succeeds, the return value is TRUE. If the function fails, the return value is FALSE.</returns>
        private static bool GetInputSelectValues(MonitorHandle physicalMonitorHandle, out uint currentValue, out uint maximumValue)
        {
            bool success = GetVCPFeatureAndVCPFeatureReply(physicalMonitorHandle, InputSelectVcpCode, out _, out currentValue, out maximumValue);
            if (!success)
            {
                Debug.WriteLine(string.Format("Failed to get input select values.  {0}", Error.GetMessage()));
            }
            return success;
        }

        /// <summary>
        /// Windows API Call to get monitor capabilities, and upon retrieving capabilities, parse them
        /// </summary>
        /// <param name="physicalMonitorHandle"></param>
        /// <param name="possibleInputs"></param>
        /// <returns></returns>
        private static bool GetPossibleInputs(MonitorHandle physicalMonitorHandle, out List<string> possibleInputs)
        {
            //Initlaize possible input list
            possibleInputs = new List<string>();

            //Attempt to get capabilities string length
            bool success = GetCapabilitiesStringLength(physicalMonitorHandle, out uint capabilitiesStringLength);
            if (success)
            {
                //Initlaizer buffer to hold capabilities
                StringBuilder buffer = new StringBuilder((int)capabilitiesStringLength);

                //Attempt to get capabilities
                success = CapabilitiesRequestAndCapabilitiesReply(physicalMonitorHandle, buffer, capabilitiesStringLength);
                if (success)
                {
                    string capabilities = buffer.ToString();
                    success = ParseInputSelectCapabilities(capabilities, out possibleInputs);
                }
                else 
                {
                    //Writes error if unable to retrieve capabilities string
                    Debug.WriteLine(string.Format("Failed to get capabilities string.  {0}", Error.GetMessage()));
                }
            }
            else 
            { 
                //Writes error if unable to retrieve capabilities string length
                Debug.WriteLine(string.Format("Failed to get capabilities string length.  {0}", Error.GetMessage()));
            }

            return success;
        }

        /// <summary>
        /// Parses the raw capabilities string to first find VCP capbilities, then to find the input select VCP capabilities.  Upon retrieval 
        /// of the input select VCP capabilities, split them out and format as hexadecimal with 0x prefix.
        /// </summary>
        /// <param name="capabilities">String that we will search for input select capabilities.</param>
        /// <param name="inputSelectCapabilitiesList">Receives the result of parsing the capabilities string.</param>
        /// <returns>Returns true if balanced searches completed successfully, otherwise returns false.</returns>
        private static bool ParseInputSelectCapabilities(string capabilities, out List<string> inputSelectCapabilitiesList)
        {
            //Inialize input select list
            inputSelectCapabilitiesList = new List<string>();

            //Attempt to get VCP capabilities from capabilities
            bool success = BalancedSearch(capabilities, "vcp", '(', ')', out string vcpCapabilities);
            if (!success)
            {
                return success;
            }

            //Attempt to get input select capabilities from VCP capabilities
            success = BalancedSearch(vcpCapabilities, "60", '(', ')', out string inputSelectCapabilities);
            if (!success)
            {
                return success;
            }

            //Split input select capabilities by space and loop through array
            foreach (string inputSelectCapability in inputSelectCapabilities.Split(' '))
            {
                inputSelectCapabilitiesList.Add(string.Format("0x{0}", inputSelectCapability.ToUpper()));
            }

            return success;
        }

        /// <summary>
        /// Searches through searchee to find a substring that starts with searchee and the openToken and then retrieves everything through the closeToken.
        /// </summary>
        /// <param name="searchee">String that we will search.</param>
        /// <param name="searcher">String that we will search for in searchee.</param>
        /// <param name="openToken">Char that indicates the open of a balanced substring like (, [, {, etc.</param>
        /// <param name="closeToken">Char that indicates the close of a balanced substring like ), ], {, etc.</param>
        /// <param name="searchResult">Receives the result of the balanced search.</param>
        /// <returns>Returns true if balanced search completed successfully, otherwise returns false.</returns>
        private static bool BalancedSearch(string searchee, string searcher, char openToken, char closeToken, out string searchResult)
        {
            //Initialize search result and open tokens
            searchResult = string.Empty;
            int openTokens = 1;

            //Find start position based on searcher
            int searchindex = searchee.IndexOf(searcher + openToken);
            int startPosition = searchindex + searcher.Length + 1;

            //Write error if searcher + openToken is not found in searchee
            if (searchindex == -1)
            {
                Debug.Print("Balanced search failed due to searcher + openToken not being found in searchee");
                return false;
            }

            //Split expression to be searched into char array
            char[] searcheeSplit = searchee.ToCharArray();

            //Loop through expression to be searched starting at searcher until searcher openToken is closed
            for (int i = startPosition; i < searcheeSplit.Length; i++)
            {
                char token = searcheeSplit[i];
                if (token == openToken)
                {
                    openTokens++;
                }
                if (token == closeToken)
                {
                    openTokens--;
                }
                if (openTokens > 0)
                {
                    searchResult += token;
                }
                else
                {
                    break;
                }
            }
            
            //Write error if token is never closed
            if (openTokens > 0)
            {
                searchResult = string.Empty;
                Debug.Print("Balanced search failed due to not closing openToken.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// Convert input integer into Hex string.
        /// </summary>
        /// <param name="input">Integer to convert.</param>
        /// <returns>Hex String representation of input integer.</returns>
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
        /// <summary>
        /// Convert input Hex string into integer.
        /// </summary>
        /// <param name="input">Hex string to convert.</param>
        /// <returns>Integer representation of input Hex String.</returns>
        private static int ConvertFromHex(string input)
        {
            return Convert.ToInt32(input, 16);
        }

        

        private static bool GetDisplayIndex(string deviceName, out byte index)
        {
            Match match = Regex.Match(deviceName, @"DISPLAY(?<index>\d{1,2})\s*$");
            if (match.Success)
            {
                index = byte.Parse(match.Groups["index"].Value);
                return true;
            }
            index = 0;
            return false;
        }
        #endregion


        #region Constants
        private const uint GetDeviceInterfaceNameFlag = 0x00000001;
        private const byte InputSelectVcpCode = 0x60;
        private static readonly Dictionary<string, string> InputSelectOptions = new Dictionary<string, string>()
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
        #endregion
    }
}
