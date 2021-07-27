using System;
using System.Text;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace ChangeInput.Core
{
    internal static class WindowsApiErrorHandler
    {
        private static List<string> _errors = new List<string>();
        #region Windows API Calls
        /// <summary>
        /// Formats a message string. The function requires a message definition as input. The message definition can come from a buffer passed into the function. It can come from a message table resource in an already-loaded module. Or the caller can ask the function to search the system's message table resource(s) for the message definition. The function finds the message definition in a message table resource based on a message identifier and a language identifier. The function copies the formatted message text to an output buffer, processing any embedded insert sequences if requested.
        /// </summary>
        /// <param name="formatOption">DWORD dwFlags : The formatting options, and how to interpret the lpSource parameter.</param>
        /// <param name="sourceLocation">LPCVOID lpSource : The location of the message definition.</param>
        /// <param name="messageId">DWORD dwMessageId : The message identifier for the requested message</param>
        /// <param name="languageId">DWORD dwLanguageId : The language identifier for the requested message.</param>
        /// <param name="buffer">LPTSTR lpBuffer : A pointer to a buffer that receives the null-terminated string that specifies the formatted message.</param>
        /// <param name="size">DWORD nSize : If the FORMAT_MESSAGE_ALLOCATE_BUFFER flag is not set, this parameter specifies the size of the output buffer, in TCHARs. If FORMAT_MESSAGE_ALLOCATE_BUFFER is set, this parameter specifies the minimum number of TCHARs to allocate for an output buffer.</param>
        /// <param name="arguments">va_list *Arguments : An array of values that are used as insert values in the formatted message.</param>
        /// <returns>If the function succeeds, the return value is the number of TCHARs stored in the output buffer, excluding the terminating null character.If the function fails, the return value is zero.</returns>
        [DllImport("Kernel32.dll", SetLastError = true)]
        private static extern uint FormatMessage(uint formatOption, IntPtr sourceLocation, uint messageId, uint languageId, StringBuilder buffer, int size, IntPtr arguments);
        #endregion
        #region Methods
        /// <summary>
        /// Translates last windows error to it's cooresponding windows error code and message.
        /// </summary>
        /// <returns>String representing error message</returns>
        public static string GetErrorMessage()
        {
            int errorCode = Marshal.GetLastWin32Error();
            StringBuilder buffer = new StringBuilder(512);
            if (FormatMessage(FormatOption,IntPtr.Zero,(uint)errorCode,LanguageId,buffer,buffer.Capacity,IntPtr.Zero)>0)
            {
                return string.Format("({0}) {1}", errorCode, buffer.ToString());
            }
            else
            {
                return string.Format("({0}) {1}", errorCode, "Unable to retrieve error message.");
            }
        }
        public static void RecordError()
        {
            _errors.Add(GetErrorMessage());
        }
        public static string GetLastErrorMessage()
        {
            return _errors[_errors.Count - 1];
        }
        #endregion
        #region Constants
        /// <summary>
        /// FORMAT_MESSAGE_FROM_SYSTEM
        /// </summary>
        private const uint FormatOption = 0x00001000;
        /// <summary>
        /// US (English)
        /// </summary>
        private const uint LanguageId = 0x0409;
        #endregion
    }
}
