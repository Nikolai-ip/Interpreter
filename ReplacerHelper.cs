using System.Linq;

namespace Lab5
{
    public class ReplacerHelper
    {
        private StringCleaner _stringCleaner = new StringCleaner();

        public string ReplaceVariableWithValue(string expression, string variableName, object value, int startIndex)
        {
            string insertValue = _stringCleaner.ReplaceValueToString(value);
            string result = expression.Remove(startIndex, variableName.Length).Insert(startIndex, insertValue);
            return result;
        }
        // public string ReplaceVariableWithValue(string expression,string variableName, object value, int startIndex)
        // {
        //     string replaceValue = _stringCleaner.ReplaceValueToString(value);
        //     expression = expression.Replace(variableName, replaceValue);
        //     return expression;
        // }


    }
}