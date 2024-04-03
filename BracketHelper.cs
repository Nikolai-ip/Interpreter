using System.Collections.Generic;
using System.Linq;

namespace Lab5
{
    public class BracketHelper
    {
        public int GetCloseBracketIndex(string inputStr, int startIndex)
        {
            var bracketStack = new Stack<char>();
            bracketStack.Push('(');
            for (int i = startIndex; i < inputStr.Length; i++)
            {
                char symbol = inputStr[i];
                if (symbol.Equals('('))
                {
                    bracketStack.Push('(');
                }

                if (symbol.Equals(')'))
                {
                    bracketStack.Pop();
                    if (bracketStack.Count == 0) return i;
                }
            }

            return inputStr.Length;
        }
        public string GetExpressionInBracket(int startIndex, string expression)
        {
            int endIndex = GetCloseBracketIndex(expression, startIndex);
            string result = new string(expression.Skip(startIndex).Take(endIndex - startIndex).ToArray());
            return result;
        }
    }
}