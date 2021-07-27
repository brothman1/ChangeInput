using System;
using System.Runtime.InteropServices;

namespace ChangeInput.Core
{
    internal class SafeMonitorHandle : SafeHandle
    {
        public SafeMonitorHandle(IntPtr handle) : base(IntPtr.Zero, true)
        {
            this.handle = handle;
        }
        public override bool IsInvalid => false;
        protected override bool ReleaseHandle()
        {
            return MonitorInteraction.TryDestroyPhysicalMonitor(handle);
        }
    }
}
