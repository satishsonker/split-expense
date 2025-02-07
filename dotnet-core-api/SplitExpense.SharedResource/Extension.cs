using System.ComponentModel;
using System.Reflection;
using System.Text;

namespace SplitExpense.SharedResource
{
    public static class Extension
    {
        /// <summary>
        /// Read the description attribute of enum
        /// </summary>
        /// <param name="value">enum description</param>
        /// <returns>description attribute value in string</returns>
        public static string GetDescription(this Enum value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var fieldInfo = value.GetType().GetField(value.ToString());

            if (fieldInfo == null) return value.ToString();

            var attribute = fieldInfo.GetCustomAttribute<DescriptionAttribute>();

            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// Add a whitespace before each uppercase letter except the first character.
        /// Convert the entire string to lowercase, except for the first character.
        /// </summary>
        /// <param name="input">String</param>
        /// <returns>Formatted string</returns>
        public static string ToFormattedString(this string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            StringBuilder result = new();
            result.Append(input[0]); // Keep the first character as is

            for (int i = 1; i < input.Length; i++)
            {
                if (char.IsUpper(input[i]))
                    result.Append(' '); // Add space before uppercase letters (except first)

                result.Append(char.ToLower(input[i])); // Convert to lowercase
            }

            return result.ToString();
        }
    }
}
