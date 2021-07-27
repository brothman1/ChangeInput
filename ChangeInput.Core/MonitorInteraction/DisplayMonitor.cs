using System;
using System.Collections.Generic;

namespace ChangeInput.Core
{
    internal class DisplayMonitor
    {
        public DisplayMonitor(IntPtr handle, string name)
        {
            Handle = handle;
            Name = name;
        }
        public IntPtr Handle { get; set; }
        public string Name { get; set; }
        public List<Monitor> MonitorCollection { get; set; }
    }
}
