using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeInput.Core
{
    public class MenuCommand
    {
        public MenuCommand(Enum command, string[] arguments)
        {
            Command = command;
            Arguments = arguments;
        }
        public Enum Command { get; private set; }
        public string[] Arguments { get; private set; }
    }
}
