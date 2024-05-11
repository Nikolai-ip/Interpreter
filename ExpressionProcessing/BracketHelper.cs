using System;
using System.Collections.Generic;
using System.Linq;

namespace ExpressionProcessing
{
    public class BracketHelper
    {
        public int GetCloseBracketIndex(string inputStr, int startIndex, char openBracket, char closeBracket)
        {
            var bracketStack = new Stack<char>();
            bracketStack.Push(openBracket);
            for (int i = startIndex; i < inputStr.Length; i++)
            {
                char symbol = inputStr[i];
                if (symbol.Equals(openBracket))
                {
                    bracketStack.Push(closeBracket);
                }

                if (symbol.Equals(closeBracket))
                {
                    bracketStack.Pop();
                    if (bracketStack.Count == 0) return i;
                }
            }

            return inputStr.Length;
        }
        public string GetExpressionInBracket(int startIndex, string expression)
        {
            int endIndex = GetCloseBracketIndex(expression, startIndex,'(',')');
            string result = new string(expression.Skip(startIndex).Take(endIndex - startIndex).ToArray());
            return result;
        }
        public string[] TakeArgumentsFromBrackets(string expression, int bracketIndex)
        {
            int finishBracketIndex = GetCloseBracketIndex(expression, bracketIndex+1,'(',')');
            string bracketExpression = new string(expression.Skip(bracketIndex+1).Take(finishBracketIndex - (bracketIndex+1)).ToArray());
            List<string> arguments = new List<string>();
            string argument = "";
            for (int i = 0; i < bracketExpression.Length; i++)
            {
                var symbol = bracketExpression[i];
                if (symbol.Equals('('))
                {
                    int closeBracketIndex = GetCloseBracketIndex(bracketExpression, i+1,'(',')'); //12
                    string bracketArgumentExpression = new string(bracketExpression.Skip(i).Take(closeBracketIndex - (i-1)).ToArray());
                    argument += bracketArgumentExpression;
                    i += bracketArgumentExpression.Length-1;
                    continue;
                }
                if (symbol.Equals(','))
                {
                    if (argument == "") throw new Exception("Argument can not be empty");
                    arguments.Add(argument);
                    argument = "";
                }   
                else
                {
                    argument += symbol;
                }
            }
            if (argument!="")
                arguments.Add(argument);
            return arguments.ToArray();
        }
        
    }
}