using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace ChangeInput.Core
{
    public enum YesNo
    {
        None,
        Yes,
        No
    }
    public enum BreakType
    {
        None,
        Parenthesis,
        CurlyBrackets,
        SquareBrackets
    }
    public static class UtilityBelt
    {
        private static Regex _splitCommand = new Regex(@" (?=(?:[^""]*""[^""]*"")*(?![^""]*""))");

        #region TryParseEnum
        public static bool TryParseEnum<TEnum>(this string toBeParsed, out TEnum parsedEnum, out bool partialParse, params string[] excludedValues) where TEnum : struct, Enum
        {
            partialParse = false;
            if (toBeParsed.ToEnum(out parsedEnum) && !excludedValues.Contains(toBeParsed))
            {
                return true;
            }
            if (toBeParsed.ToEnumPartial(out parsedEnum) && !excludedValues.Contains(toBeParsed))
            {
                partialParse = true;
                return true;
            }
            return false;
        }
        public static bool TryParseEnumExact<TEnum>(this string toBeParsed, out TEnum parsedEnum, params string[] excludedValues) where TEnum : struct, Enum
        {
            if (toBeParsed.ToEnum(out parsedEnum) && !excludedValues.Contains(toBeParsed))
            {
                return true;
            }
            return false;
        }
        private static bool ToEnum<TEnum>(this string toBeParsed, out TEnum parsedEnum) where TEnum : struct, Enum
        {
            if (Enum.TryParse(toBeParsed, true, out parsedEnum))
            {
                return true;
            }
            parsedEnum = default;
            return false;
        }
        private static bool ToEnumPartial<TEnum>(this string toBeParsed, out TEnum parsedEnum) where TEnum : struct, Enum
        {
            List<TEnum> matchingEnums = Enum.GetNames(typeof(TEnum))
                                            .Select(enumName => enumName.ToLower())
                                            .Where(enumName => enumName.StartsWith(toBeParsed.ToLower()))
                                            .OrderBy(enumName => enumName) //Enum.Parse(typeof(Animal), str, true)
                                            .Select(enumName => (TEnum)Enum.Parse(typeof(TEnum), enumName, true))
                                            .ToList();
            if (matchingEnums.Count > 0)
            {
                parsedEnum = matchingEnums[0];
                return true;
            }
            parsedEnum = default;
            return false;
        }
        #endregion
        public static string GetEnumNames<TEnum>(string delimeter, string end, params string[] excludedValues) where TEnum : struct, Enum
        {
            List<string> enumNames = Enum.GetNames(typeof(TEnum))
                                            .Where(enumName => !excludedValues.Contains(enumName))
                                            .Select(enumName => $"\"{enumName.ToUserFriendlyString()}\"")
                                            .ToList();
            if (enumNames.Count > 1)
            {
                enumNames[enumNames.Count - 1] = $"{end} {enumNames[enumNames.Count - 1]}";
            }
            return string.Join($"{delimeter} ", enumNames);
        }
        #region ToUserFriendlyString
        public static string ToUserFriendlyString(this Enum value)
        {
            return Regex.Replace(value.ToString(), "(\\B[A-Z])", " $1");
        }
        public static string ToUserFriendlyString(this string value)
        {
            return Regex.Replace(value, "(\\B[A-Z])", " $1");
        }
        #endregion
        #region YesNo
        public static YesNo ToYesNo(this bool value)
        {
            if (value)
            {
                return YesNo.Yes;
            }
            return YesNo.No;
        }
        public static bool ToBool(this YesNo value)
        {
            if (value == YesNo.Yes)
            {
                return true;
            }
            return false;
        }
        #endregion
        #region SearchForBalancedEnd
        /// <summary>
        /// Searches through string to find a substring that starts at startSearchHere if indicated, or otherwise at the first instance of the breakType open.
        /// </summary>
        /// <param name="toBeSearched">String to be searched.</param>
        /// <param name="breakType">Type of break that we are searching for.</param>
        /// <param name="searchResult">Result of the search operation.</param>
        /// <param name="startSearchHere">Substring to optionally start the search from.</param>
        /// <returns>True if search succeeded, false if search failed.</returns>
        public static bool SearchForBalancedEnd(this string toBeSearched, BreakType breakType, out string searchResult, string startSearchHere = default)
        {
            searchResult = default;
            if (!TryGetSearchee(toBeSearched, breakType, startSearchHere, out char[] searchee))
            {
                return false;
            }
            int openTokens = 1;
            for (int i = 0; i < searchee.Length; i++)
            {
                char token = searchee[i];
                if(!ContinueSearch(token, breakType, ref openTokens, ref searchResult)) 
                {
                    return true;
                }
            }
            return false;
        }
        private static bool ContinueSearch(char token, BreakType breakType, ref int openTokens, ref string searchResult)
        {
            if (token == breakType.GetOpen())
            {
                openTokens++;
            }
            if (token == breakType.GetClose())
            {
                openTokens--;
            }
            if (openTokens > 0)
            {
                searchResult += token;
                return true;
            }
            return false;
        }
        private static bool TryGetSearchee(string toBeSearched, BreakType breakType, string startSearchHere, out char[] searchee)
        {
            searchee = default;
            int startPosition;
            if (IsValueSearchable(toBeSearched, breakType, startSearchHere))
            {
                if (startSearchHere == default)
                {
                    startPosition = toBeSearched.IndexOf(breakType.GetOpen());
                }
                else
                {
                    startPosition = toBeSearched.IndexOf($"{startSearchHere}{breakType.GetOpen()}") + startSearchHere.Length + 1;
                }
                searchee = toBeSearched.Substring(startPosition).ToArray();
                return true;
            }
            return false;
        }
        private static bool IsValueSearchable(string toBeSearched, BreakType breakType, string startSearchHere)
        {
            int startPosition;
            if (startSearchHere == default)
            {
                startPosition = toBeSearched.IndexOf(breakType.GetOpen());
                return startPosition >= 0 && toBeSearched.Substring(startPosition).Contains(breakType.GetClose());
            }
            else
            {
                startPosition = toBeSearched.IndexOf($"{startSearchHere}{breakType.GetOpen()}");
                return startPosition >= 0 && toBeSearched.Substring(startPosition).Contains(breakType.GetClose());
            }
        }
        private static char GetOpen(this BreakType breakType)
        {
            return _balancedTokens[breakType].openToken;
        }
        private static char GetClose(this BreakType breakType)
        {
            return _balancedTokens[breakType].closeToken;
        }
        private static readonly Dictionary<BreakType, (char openToken, char closeToken)> _balancedTokens = new Dictionary<BreakType, (char openToken, char closeToken)>
        {
            {BreakType.Parenthesis, ('(', ')') },
            {BreakType.CurlyBrackets, ('{', '}') },
            {BreakType.SquareBrackets, ('[', ']') }
        };
        #endregion
    }
}
