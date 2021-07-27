using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChangeInput.Core
{
    public enum MainMenuCommand
    {
        None,
        Help,
        OutputMonitorDetails,
        SetMonitorInput,
        Quit
    }
    public class MainMenu : Menu
    {
        public MainMenu() : base(false, "None")
        {
        }

        public override ExecutionType ExecuteCommand(MenuCommand menuCommand, out Menu menu, out Enum menuEnum, out Type menuEnumType, out string output, out int cascadeQuit)
        {
            menu = default;
            menuEnum = default;
            menuEnumType = default;
            output = default;
            cascadeQuit = 0;
            switch (menuCommand.Command)
            {
                case MainMenuCommand.Help:
                    return Help(menuCommand, out output);
                case MainMenuCommand.OutputMonitorDetails:
                    return OutputMonitorDetails(menuCommand, out output);
                case MainMenuCommand.SetMonitorInput:
                    return SetMonitorInput(menuCommand, out output);
            }
            return default;
        }
        public ExecutionType Help(MenuCommand menuCommand, out string output)
        {
            if (menuCommand.Arguments.Any())
            {
                output = "Command must be passed with appropriate arguments! For available commands and their syntax try \"/Help\"\n\n";
                return ExecutionType.Output;
            }
            StringBuilder helpMessage = new StringBuilder();
            helpMessage.Append($"\nTips:\n");
            helpMessage.Append($"\tCommands must be prefixed with a forward slash \'/\'.\n");
            helpMessage.Append($"\tCommand arguments are separated with spaces.\n");
            helpMessage.Append($"\tCommand must be passed with appropriate arguments.\n");
            helpMessage.Append($"Functions:\n");
            helpMessage.Append($"\t{MainMenuCommand.Quit} - Quits the application.\n");
            helpMessage.Append($"\t{MainMenuCommand.Help} - Output available tips and functions.\n");
            helpMessage.Append($"\t{MainMenuCommand.OutputMonitorDetails} - Output user monitors fetched at launch with their current input and possible inputs.\n");
            helpMessage.Append($"\t{MainMenuCommand.SetMonitorInput} \"value\" - Sets all monitor inputs to specified value.\n");
            helpMessage.Append($"\t{MainMenuCommand.SetMonitorInput} \"monitor\" \"value\" - Sets specified monitor to specified value.\n");
            helpMessage.AppendLine();
            output = helpMessage.ToString();
            return ExecutionType.Output;
        }
        public ExecutionType OutputMonitorDetails(MenuCommand menuCommand, out string output)
        {
            if (menuCommand.Arguments.Any())
            {
                output = "Command must be passed with appropriate arguments! For available commands and their syntax try \"/Help\"\n\n";
                return ExecutionType.Output;
            }
            output = $"{string.Join("\n", MonitorInteraction.GetMonitorDetails())}\n";
            return ExecutionType.Output;
        }
        public ExecutionType SetMonitorInput(MenuCommand menuCommand, out string output)
        {
            switch(menuCommand.Arguments.Count())
            {
                case 1:
                    return SetAllMonitorInput(menuCommand.Arguments[0], out output);
                case 2:
                    return SetOneMonitorInput(menuCommand.Arguments[0], menuCommand.Arguments[1], out output);
                default:
                    output = "Command must be passed with appropriate arguments! For available commands and their syntax try \"/Help\"\n\n";
                    return ExecutionType.Output;
            }
        }
        private ExecutionType SetAllMonitorInput(string newInput, out string output)
        {
            if (MonitorInteraction.SetMonitorInput(newInput))
            {
                output = $"All monitors changed to \"{newInput}\" successfully.\n\n";
            }
            else
            {
                output = $"Unable to change all monitors to \"{newInput}\"!\n\n";
            }
            return ExecutionType.Output;
        }
        private ExecutionType SetOneMonitorInput(string name, string newInput, out string output)
        {
            if (MonitorInteraction.SetMonitorInput(name, newInput))
            {
                output = $"\"{name}\" changed to \"{newInput}\" successfully.\n\n";
            }
            else
            {
                output = $"Unable to change \"{name}\" to {newInput}!\n\n";
            }
            return ExecutionType.Output;
        }
    }
}