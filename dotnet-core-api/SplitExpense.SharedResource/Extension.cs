using System.ComponentModel;
using System.Reflection;

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
    }
}
