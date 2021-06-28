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
	//internal static class Error
 //   {
	//	[DllImport("Kernel32.dll", SetLastError = true)]
	//	private static extern uint FormatMessage(
	//		uint dwFlags,
	//		IntPtr lpSource,
	//		uint dwMessageId,
	//		uint dwLanguageId,
	//		StringBuilder lpBuffer,
	//		int nSize,
	//		IntPtr Arguments);

	//	private const uint FORMAT_MESSAGE_FROM_SYSTEM = 0x00001000;
	//	public static (int errorCode, string message) GetCodeMessage()
	//	{
	//		var errorCode = Marshal.GetLastWin32Error();
	//		return (errorCode, GetMessage(errorCode));
	//	}

	//	public static string GetMessage() => GetMessage(Marshal.GetLastWin32Error());

	//	public static string GetMessage(int errorCode)
	//	{
	//		var message = new StringBuilder($"Code: {errorCode}");

	//		var buffer = new StringBuilder(512); // This 512 capacity is arbitrary.
	//		if (FormatMessage(
	//			FORMAT_MESSAGE_FROM_SYSTEM,
	//			IntPtr.Zero,
	//			(uint)errorCode,
	//			0x0409, // US (English)
	//			buffer,
	//			buffer.Capacity,
	//			IntPtr.Zero) > 0)
	//		{
	//			message.Append($", Message: ").Append(buffer);
	//		}

	//		return message.ToString();
	//	}
	//}
    internal class Ben1
    {
		[DllImport("Dxva2.dll", SetLastError = true)]
		private static extern bool CapabilitiesRequestAndCapabilitiesReply(
			SafePhysicalMonitorHandle handle,
			[MarshalAs(UnmanagedType.LPStr)]
			[Out] StringBuilder pszASCIICapabilitiesString,
			uint dwCapabilitiesStringLengthInCharacters
			);
		[DllImport("Dxva2.dll", SetLastError = true)]
		private static extern bool GetCapabilitiesStringLength(
			SafePhysicalMonitorHandle handle,
			out uint pdwCapabilitiesStringLengthInCharacters
			);



		[DllImport("Dxva2.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetVCPFeatureAndVCPFeatureReply(
			SafePhysicalMonitorHandle hMonitor,
			byte bVCPCode,
			out LPMC_VCP_CODE_TYPE pvct,
			out uint pdwCurrentValue,
			out uint pdwMaximumValue);
		private enum LPMC_VCP_CODE_TYPE
		{
			MC_MOMENTARY,
			MC_SET_PARAMETER
		}
		[DllImport("Dxva2.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		public static extern bool DestroyPhysicalMonitor(
			IntPtr hMonitor);
		[DllImport("Dxva2.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetNumberOfPhysicalMonitorsFromHMONITOR(
			IntPtr hMonitor,
			out uint pdwNumberOfPhysicalMonitors);
		[DllImport("Dxva2.dll", SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetPhysicalMonitorsFromHMONITOR(
			IntPtr hMonitor,
			uint dwPhysicalMonitorArraySize,
			[Out] PHYSICAL_MONITOR[] pPhysicalMonitorArray);
		public struct PHYSICAL_MONITOR
		{
			public IntPtr hPhysicalMonitor;

			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 128)]
			public string szPhysicalMonitorDescription;
		}

		[DllImport("User32.dll")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool EnumDisplayMonitors(
			IntPtr hdc,
			IntPtr lprcClip,
			MonitorEnumProc lpfnEnum,
			IntPtr dwData);

		[return: MarshalAs(UnmanagedType.Bool)]
		private delegate bool MonitorEnumProc(
			IntPtr hMonitor,
			IntPtr hdcMonitor,
			IntPtr lprcMonitor,
			IntPtr dwData);
		[DllImport("User32.dll", EntryPoint = "GetMonitorInfoW")]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool GetMonitorInfo(
			IntPtr hMonitor,
			ref MONITORINFOEX lpmi);

		[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
		public struct MONITORINFOEX
		{
			public uint cbSize;
			public RECT rcMonitor;
			public RECT rcWork;
			public uint dwFlags;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
			public string szDevice;
		}
		[StructLayout(LayoutKind.Sequential)]
		internal struct RECT
        {
			public int Left;
			public int Top;
			public int Right;
			public int Bottom;
        }
		public class Monitor
		{
			public string Availability { get; set; }
			public string ScreenHeight { get; set; }
			public string ScreenWidth { get; set; }
			public RECT MonitorArea { get; set; }
			public RECT WorkArea { get; set; }
			public string DeviceName { get; set; }
		}

		private const byte InputSelect = 0x60; //vcp code for input select

		/// <summary>
		/// Collection of display information
		/// </summary>
		public class MonitorCollection : List<Monitor>
		{
		}
		public static MonitorCollection GetDisplays()
		{
			//DisplayInfoCollection col = new DisplayInfoCollection();
			MonitorCollection monitors = new MonitorCollection();

			if (EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero, Proc, IntPtr.Zero))
			{
				return monitors;
			}
			else return new MonitorCollection();
			bool Proc(IntPtr monitorHandle, IntPtr hdcMonitor, IntPtr lprcMonitor, IntPtr dwData)
			{
				MONITORINFOEX monitorInfo = new MONITORINFOEX { cbSize = (uint)Marshal.SizeOf<MONITORINFOEX>() };

				if (GetMonitorInfo(monitorHandle, ref monitorInfo))
				{
					Monitor monitor = new Monitor()
					{
						Availability = monitorInfo.dwFlags.ToString(),
						ScreenWidth = (monitorInfo.rcMonitor.Right - monitorInfo.rcMonitor.Left).ToString(),
						ScreenHeight = (monitorInfo.rcMonitor.Bottom - monitorInfo.rcMonitor.Top).ToString(),
						DeviceName = monitorInfo.szDevice
					};
					monitors.Add(monitor);
					if (GetNumberOfPhysicalMonitorsFromHMONITOR(monitorHandle, out uint count))
                    {
						Console.WriteLine(count);
						PHYSICAL_MONITOR[] monitorArr = new PHYSICAL_MONITOR[count];
						if (GetPhysicalMonitorsFromHMONITOR(monitorHandle, count, monitorArr))
                        {
							Console.WriteLine("woot");
							foreach (PHYSICAL_MONITOR physicalMonitor in monitorArr)
                            {
								SafePhysicalMonitorHandle safeHandle = new SafePhysicalMonitorHandle(physicalMonitor.hPhysicalMonitor);
								if (GetVCPFeatureAndVCPFeatureReply(safeHandle, InputSelect, out _, out uint currentval, out uint maxval))
								{
									Console.WriteLine(string.Format("{0},{1}", currentval.ToString(), maxval.ToString()));
								}
								else
								{
									Console.WriteLine($"Failed to get vcp feature. {Error.GetMessage()}");
								}

								
                                if (GetCapabilitiesStringLength(safeHandle, out uint strlen))
                                {
									Console.WriteLine(strlen.ToString());
                                }
								else 
                                {
                                    Console.WriteLine($"Failed to get string length. {Error.GetMessage()}");
                                }
								StringBuilder buffer = new StringBuilder((int)strlen);
                                if (CapabilitiesRequestAndCapabilitiesReply(safeHandle, buffer, strlen))
                                {
                                    Console.WriteLine(buffer.ToString());
									Debug.WriteLine(buffer.ToString());
								}
                                else
                                {
                                    Console.WriteLine($"Failed to get capability string. {Error.GetMessage()}");
                                }
                            }
							

                        }
						else
                        {
							Console.WriteLine($"Failed to get the number of physical monitors. {Error.GetMessage()}");
						}
                    }
					else 
					{
						Console.WriteLine($"Failed to get the number of physical monitors. {Error.GetMessage()}");
					}
				}
				return true;
			}
			//EnumDisplayMonitors(IntPtr.Zero, IntPtr.Zero,
			//	delegate (IntPtr hMonitor, IntPtr hdcMonitor, ref Rect lprcMonitor, IntPtr dwData)
			//	{
			//		MonitorInfoEx mi = new MonitorInfoEx();
			//		mi.Size = Marshal.SizeOf(mi);
			//		bool success = GetMonitorInfo(hMonitor, ref mi);
			//		if (success)
			//		{
			//			DisplayInfo di = new DisplayInfo();
			//			di.ScreenWidth = (mi.Monitor.Right - mi.Monitor.Left).ToString();
			//			di.ScreenHeight = (mi.Monitor.Bottom - mi.Monitor.Top).ToString();
			//			di.DeviceName = mi.DeviceName;
			//			//di.MonitorArea = mi.Monitor;
			//			//di.WorkArea = (Rect)mi.WorkArea;
			//			di.Availability = mi.Flags.ToString();

			//			col.Add(di);
			//		}
			//		return true;
			//	}, IntPtr.Zero);
			//return col;
		}
		private static bool TryGetDisplayIndex(string deviceName, out byte index)
		{
			// The typical format of device name is as follows:
			// EnumDisplayDevices (display), GetMonitorInfo : \\.\DISPLAY[index starting at 1]
			// EnumDisplayDevices (monitor)                 : \\.\DISPLAY[index starting at 1]\Monitor[index starting at 0]

			var match = Regex.Match(deviceName, @"DISPLAY(?<index>\d{1,2})\s*$");
			if (match.Success)
			{
				index = byte.Parse(match.Groups["index"].Value);
				return true;
			}
			index = 0;
			return false;
		}
		internal class SafePhysicalMonitorHandle : SafeHandle
		{
			public SafePhysicalMonitorHandle(IntPtr handle) : base(IntPtr.Zero, true)
			{
				this.handle = handle; // IntPtr.Zero may be a valid handle.
			}

			public override bool IsInvalid => false; // The validity cannot be checked by the handle.

			protected override bool ReleaseHandle()
			{
				return DestroyPhysicalMonitor(handle);
			}
		}
	}
}
