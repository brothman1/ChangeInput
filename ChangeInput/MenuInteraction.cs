using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ChangeInput.Core;
using static System.Console;

namespace ChangeInput
{
    public static class MenuInteraction
    {
        private static int _cascadeQuit = 0;
        private static Regex _splitCommandPattern = new Regex(@" (?=(?:[^""]*""[^""]*"")*(?![^""]*""))");

        #region ExecuteBulkCommands
        public static void ExecuteBulkCommands<TEnum>(Menu menu, string[] arguments) where TEnum : struct, Enum
        {
            TEnum quit = GetQuit<TEnum>();
            if (TryParseCommands<TEnum>(string.Join(" ", arguments), out List<MenuCommand> menuCommands, menu.ExcludedValues))
            {
                ExecuteCommands(menu, quit, menuCommands);
                if (!menuCommands.Any(menuCommand => Equals((TEnum)menuCommand.Command, quit)))
                {
                    StartMenu<TEnum>(menu);
                }
            }
        }
        private static void ExecuteCommands(Menu menu, Enum quit, List<MenuCommand> menuCommands)
        {
            foreach (MenuCommand menuCommand in menuCommands)
            {
                if (menuCommand.Command == quit)
                {
                    return;
                }
                BranchMenuCommand(menu, menuCommand);
            }
        }
        #endregion
        #region StartMenu
        public static void StartMenu<TEnum>(Menu menu) where TEnum : struct, Enum
        {
            TEnum quit = GetQuit<TEnum>();
            TEnum command = default;
            while (!command.Equals(quit))
            {
                if (_cascadeQuit > 0)
                {
                    _cascadeQuit--;
                    return;
                }
                string message = $"Choose from commands: either {UtilityBelt.GetEnumNames<TEnum>(",", "or", menu.ExcludedValues)}";
                MenuCommand menuCommand = RequestCommand<TEnum>(message, menu.PrintRequests, menu.ExcludedValues)[0];
                command = (TEnum)menuCommand.Command;
                BranchMenuCommand(menu, menuCommand);
            }
        }
        private static List<MenuCommand> RequestCommand<TEnum>(string requestMessage, bool printRequests, params string[] excludedValues) where TEnum : struct, Enum
        {
            List<MenuCommand> menuCommands = default;
            while (menuCommands == default || !menuCommands.Any())
            {
                string userResponse = RequestUserResponse(requestMessage, printRequests);
                if (!TryParseCommands<TEnum>(userResponse, out menuCommands, excludedValues) || menuCommands.Count > 1)
                {
                    Write($"\"{userResponse}\" is invalid! For available commands try \"/Help\"\n\n");
                }
            }
            return menuCommands;
        }
        #endregion
        private static TEnum GetQuit<TEnum>() where TEnum : struct, Enum
        {
            if (!Enum.TryParse("Quit", out TEnum quit))
            {
                throw new ArgumentException("Quit must be defined!");
            }
            return quit;
        }
        private static void BranchMenuCommand(Menu menu, MenuCommand menuCommand)
        {
            switch (menu.ExecuteCommand(menuCommand, out Menu outputMenu, out Enum outputMenuEnum, out Type outputMenuEnumType, out string output, out int cascadeQuit))
            {
                case ExecutionType.LaunchMenu:
                    typeof(MenuInteraction).GetMethod("StartMenu").MakeGenericMethod(outputMenuEnumType).Invoke(null, new object[] { outputMenu, outputMenuEnum });
                    break;
                case ExecutionType.Output:
                    Write(output);
                    break;
                case ExecutionType.OutputAndQuit:
                    Write(output);
                    _cascadeQuit = cascadeQuit;
                    break;
                case ExecutionType.Quit:
                    Write(output);
                    _cascadeQuit = cascadeQuit;
                    break;
            }
        }
        private static bool TryParseCommands<TEnum>(string commandInput, out List<MenuCommand> menuCommands, params string[] excludedValues) where TEnum : struct, Enum
        {
            menuCommands = new List<MenuCommand>();
            if (!commandInput.Contains('/'))
            {
                return false;
            }
            foreach (string menuCommand in commandInput.Split('/'))
            {
                string[] menuCommandArguments = _splitCommandPattern.Split(menuCommand);
                if (menuCommandArguments[0].TryParseEnumExact(out TEnum parsedEnum, excludedValues))
                {
                    menuCommands.Add(new MenuCommand(parsedEnum, menuCommandArguments.Skip(1).ToArray()));
                }
            }
            return menuCommands.Any();
        }
        private static string RequestUserResponse(string message, bool printRequests, bool readOnSameLine = true)
        {
            if (readOnSameLine && printRequests)
            {
                Write(message);
            }
            else if (printRequests)
            {
                WriteLine(message);
            }
            return ReadLine();
        }
    }
}
