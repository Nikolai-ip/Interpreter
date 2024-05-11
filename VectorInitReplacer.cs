using System;
using System.Collections.Generic;
using System.Linq;
using CalcModule;
using ExpressionProcessing;

namespace Lab5
{
    public class VectorInitReplacer
    {
        private BracketHelper _bracketHelper = new BracketHelper();
        private ExpressionReplacer _expressionReplacer;
        private CalculatorProxy _calculator;
        private StringCleaner _stringCleaner = new StringCleaner();
        public VectorInitReplacer(ExpressionReplacer expressionReplacer, CalculatorProxy calculator)
        {
            _expressionReplacer = expressionReplacer;
            _calculator = calculator;
        }
        public string ReformatVectorInitExpression(string expression)
        {
            while (expression.Contains("new Vector2"))
            {
                int startIndex = expression.IndexOf("new Vector2", StringComparison.Ordinal);
                expression = expression.Remove(startIndex, "new Vector2".Length);
                
                if (expression[startIndex].Equals('('))
                {
                    expression = expression.Remove(startIndex, 1);
                    expression = expression.Insert(startIndex, "<");
                    int closeBracketIndex = _bracketHelper.GetCloseBracketIndex(expression, startIndex,'(',')');
                    expression = expression.Remove(closeBracketIndex, 1);
                    expression = expression.Insert(closeBracketIndex, ">");
                    string[] arguments = GetArguments(expression, startIndex + 1);
                    string xExpression = arguments[0];
                    string yExpression = arguments[1];
                    string xValueReplaced = _expressionReplacer.ReplaceTokensInExpression(xExpression);
                    string yValueReplaced = _expressionReplacer.ReplaceTokensInExpression(yExpression);
                    string xValueCalculated = _calculator.GetResult(xValueReplaced).ToString();
                    string yValueCalculated = _calculator.GetResult(yValueReplaced).ToString();
                    expression = expression
                        .Remove(startIndex + 1, xExpression.Length)
                        .Insert(startIndex + 1, xValueCalculated)
                        .Remove(startIndex + xValueCalculated.Length + 2, yExpression.Length)
                        .Insert(startIndex + xValueCalculated.Length + 2, yValueCalculated)
                        .Remove(startIndex + xValueCalculated.Length + 1, 1)
                        .Insert(startIndex + xValueCalculated.Length + 1, " ");
                }
                else
                {
                    throw new Exception("No found the bracket");
                }
                
            }

            return _stringCleaner.RemoveSpaces(expression);
        }
        private string[] GetArguments(string expression, int startIndex)
        {
            List<string> arguments = new List<string>();
            string argument = "";
            for (int i = startIndex; i < expression.Length; i++)
            {
                var symbol = expression[i];
                if (symbol == '>') break;
                if (symbol.Equals('('))
                {
                    int closeBracketIndex = _bracketHelper.GetCloseBracketIndex(expression, i+1,'(',')');
                    string bracketArgumentExpression = new string(expression.Skip(i).Take(closeBracketIndex - (i-1)).ToArray());
                    argument += bracketArgumentExpression;
                    i += bracketArgumentExpression.Length-1;
                    continue;
                }
                if (symbol.Equals(','))
                {
                    if (argument == "") throw new Exception("Vector member can not be empty");
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