using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ChangeInput
{
    class Program
    {
        static void Main(string[] args)
        {
            //Ben1.MonitorCollection monitors = Ben1.GetDisplays();
            //foreach (Ben1.Monitor monitor in monitors)
            //{
            //    Console.WriteLine(string.Format("{0},{1},{2},{3}", monitor.Availability, monitor.DeviceName, monitor.ScreenHeight, monitor.ScreenWidth));
            //}
            //string possible = "(prot(monitor)type(LCD)model(VX2705)cmds(01 02 03 07 0C E3 F3)vcp(02 04 05 06 08 0B 0C 10 12 14(01 04 05 06 08 0B) 16 18 1A 52 60(0F 11 12) 62 87 8D(01 02) AC AE B2 B6 C6 C8 CA CC(01 02 03 04 05 06 07 09 0A 0B 0C 0D 12 16) D6(01 04 05) DF)mswhql(1)asset_eep(40)mccs_ver(2.2))";

            //foreach (string str in InputSelectCapabilities(possible))
            //{
            //    Console.WriteLine("HexadecimalKey: {0}|Value: {1}", str, InputSelectCodes[str]);
            //}


            MonitorInteraction.MonitorCollection monitors = MonitorInteraction.GetMonitors();

            foreach (KeyValuePair<string,MonitorInteraction.Monitor> monitor in monitors)
            {
                Console.WriteLine(monitor.Value.Name);
                Debug.WriteLine(monitor.Value.Name);
                Console.WriteLine(MonitorInteraction.TranslateInputSelectHex(monitor.Value.CurrentInput));
                foreach (string input in monitor.Value.PossibleInputs)
                {
                    Console.WriteLine(MonitorInteraction.TranslateInputSelectHex(input));
                }
            }

            try
            {
                ManagementObjectSearcher searcher =
                    new ManagementObjectSearcher("root\\WMI",
                    "SELECT * FROM WMIMonitorID");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("Win32_DesktopMonitor instance");
                    Console.WriteLine("-----------------------------------");
                    Console.WriteLine("Description: {0}", queryObj["InstanceName"]);
                    Console.WriteLine(queryObj["ManufacturerName"].ToString());
                    Console.WriteLine(queryObj["UserFriendlyName"].ToString());

                    UInt16[] manufacturerName = (UInt16[])queryObj["UserFriendlyName"];
                    byte[] byteArray = manufacturerName.SelectMany(BitConverter.GetBytes).ToArray();
                    Console.WriteLine(Encoding.ASCII.GetString(byteArray));

                    //Console.WriteLine(Encoding.ASCII.GetString((byte[])queryObj["ManufacturerName"]));
                }
            }
            catch (ManagementException e)
            {
                Console.WriteLine("An error occurred while querying for WMI data: " + e.Message);
            }

            string deviceName = "\\\\.\\DISPLAY1";
            byte index = 0;
            var match = Regex.Match(deviceName, @"DISPLAY(?<index>\d{1,2})\s*$");
            if (match.Success)
            {
                index = byte.Parse(match.Groups["index"].Value);
                Console.WriteLine("Yes");
            }
            Console.WriteLine(index);

            Console.WriteLine("Done");
            Console.ReadLine();
        }
        public static string ConvertToHexadecimal(int input)
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
        public static int ConvertFromHexadecimal(string input)
        {
            return Convert.ToInt32(input, 16);
        }
        public static List<string> InputSelectCapabilities(string MonitorCapabilities)
        {
            List<string> inputSelectCapabilities = new List<string>();
            
            string vcpCapabilities = BalancedSearch(MonitorCapabilities, "vcp", '(', ')');
            string vcpInputSelectCapabilities = BalancedSearch(vcpCapabilities, "60", '(', ')');

            foreach (string vcpInputSelectCapability in vcpInputSelectCapabilities.Split(' '))
            {
                inputSelectCapabilities.Add(string.Format("0x{0}", vcpInputSelectCapability));
            }
            return inputSelectCapabilities;
        }
        public static string BalancedSearch(string searchee, string searcher, char openToken, char closeToken)
        {
            //Find start position based on searcher
            int startPosition = searchee.IndexOf(searcher + openToken) + searcher.Length + 1;

            //Split expression to be searched into char array
            char[] searcheeSplit = searchee.ToCharArray();

            //Initialize search result, and open tokens
            string searchResult = string.Empty;
            int openTokens = 1;

            //Loop through expression to be searched start at searcher until searcher openToken is closed
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
            return searchResult;
        }
        public static readonly Dictionary<string, string> InputSelectCodes = new Dictionary<string, string>()
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
    }
}
