using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Diagnostics;
using System.Text.RegularExpressions;
using ChangeInput.Core;

namespace ChangeInput
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (!MonitorInteraction.TryGetMonitors())
            {
                throw new NotSupportedException();
            }
            if (args.Any())
            {
                MenuInteraction.ExecuteBulkCommands<MainMenuCommand>(new MainMenu(), args);
            }
            else
            {
                Console.WriteLine("Retrieved Monitor Information");
                MenuInteraction.StartMenu<MainMenuCommand>(new MainMenu());
            }
        }
    }
}
