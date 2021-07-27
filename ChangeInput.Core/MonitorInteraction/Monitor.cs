using System.Collections.Generic;

namespace ChangeInput.Core
{
    internal class Monitor
    {
        public Monitor(SafeMonitorHandle physicalMonitorHandle, string description, string currentInput, List<string> possibleInputs)
        {
            PhysicalMonitorHandle = physicalMonitorHandle;
            Description = description;
            CurrentInput = currentInput;
            PossibleInputs = possibleInputs;
        }
        public string Description { get; set; }
        public SafeMonitorHandle PhysicalMonitorHandle { get; set; }
        public string CurrentInput { get; set; }
        public List<string> PossibleInputs { get; set; }
    }
}
