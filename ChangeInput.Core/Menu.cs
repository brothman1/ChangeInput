using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeInput.Core
{
    public enum ExecutionType
    {
        None = 0,
        LaunchMenu,
        Output,
        OutputAndQuit,
        Quit
    }
    public abstract class Menu
    {
        public string[] ExcludedValues { get; private set; }
        public bool PrintRequests { get; private set; }

        public Menu(bool printRequests, params string[] excludedValues)
        {
            PrintRequests = printRequests;
            ExcludedValues = excludedValues;
        }

        public abstract ExecutionType ExecuteCommand(MenuCommand menuCommand, out Menu menu, out Enum menuEnum, out Type menuEnumType, out string output, out int cascadeQuit);
    }
}
