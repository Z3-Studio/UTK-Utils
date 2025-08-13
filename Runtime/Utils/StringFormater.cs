using System.Text.RegularExpressions;

namespace Z3.Utils
{
    public static class StringFormater
    {
        /// <remarks>
        /// Input Examples: 
        /// <para> myVariableTAGName0 </para>
        /// <para> _myVariableTAGName0 </para>
        /// <para> MyVariableTAGName0 </para>
        /// <para> _MyVariableTAGName0 </para>
        /// <para> MY_VARIABLE_TAG_NAME0 </para>
        /// 
        /// Output: 
        /// <para> variable-tag-name-0 </para>
        /// </remarks>
        public static string FormatToLowerCaseAndHyphenateWords(string input)
        {
            return Regex.Replace(input, @"(\B[A-Z])", "-$1").ToLower();
        }

        /// <summary>
        /// Similar than UnityEditor.ObjectNames.NicifyVariableName;
        /// </summary>
        public static string GetNiceString(string value)
        {
            string result = string.Empty;

            // Loop through our string
            for (int i = 0; i < value.Length; i++)
            {
                if (i == 0)
                {
                    result += char.ToUpper(value[0]);
                }
                else if (value[i] == '_')
                {
                    result += ' ';
                }
                else
                {
                    char last = value[i - 1];
                    if (char.IsUpper(value[i]) == true && last != ' ' && !char.IsUpper(last))
                    {
                        result += ' ';
                    }

                    result += value[i];
                }
            }

            return result;
        }
    }
}