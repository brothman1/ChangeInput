using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using ChangeInput.Core;

namespace ChangeInputTests
{
    [TestClass]
    public class MainMenuTests
    {
        [TestMethod]
        public void ExecuteCommand_WhenCommandIsHelp_ShouldReturnOutputAndOutStringWithValue()
        {
            Menu menu = new MainMenu();
            MenuCommand menuCommand = new MenuCommand(MainMenuCommand.Help, new string[] { });
            ExecutionType expectedValue = ExecutionType.Output;

            ExecutionType actualValue = menu.ExecuteCommand(menuCommand, out _, out _, out _, out string output, out _);

            Assert.AreEqual(expectedValue, actualValue);
            Assert.IsFalse(string.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void ExecuteCommand_WhenCommandIsOutputMonitorDetails_ShouldReturnOutputAndOutStringWithValue()
        {
            Menu menu = new MainMenu();
            MenuCommand menuCommand = new MenuCommand(MainMenuCommand.OutputMonitorDetails, new string[] { });
            ExecutionType expectedValue = ExecutionType.Output;

            ExecutionType actualValue = menu.ExecuteCommand(menuCommand, out _, out _, out _, out string output, out _);

            Assert.AreEqual(expectedValue, actualValue);
            Assert.IsFalse(string.IsNullOrEmpty(output));
        }

        [TestMethod]
        public void ExecuteCommand_WhenCommandIsSetMonitorInput_ShouldReturnOutputAndOutStringWithValue()
        {
            Menu menu = new MainMenu();
            MenuCommand menuCommand = new MenuCommand(MainMenuCommand.SetMonitorInput, new string[] { });
            ExecutionType expectedValue = ExecutionType.Output;

            ExecutionType actualValue = menu.ExecuteCommand(menuCommand, out _, out _, out _, out string output, out _);

            Assert.AreEqual(expectedValue, actualValue);
            Assert.IsFalse(string.IsNullOrEmpty(output));
        }
    }
}
