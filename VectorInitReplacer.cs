using System;
using System.Numerics;
using lab1;

namespace Lab5
{
    public class VectorInitReplacer
    {
        private BracketHelper _bracketHelper = new BracketHelper();
        private ExpressionReplacer _expressionReplacer;
        private CalculatorProxy _calculator;
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
                    int closeBracketIndex = _bracketHelper.GetCloseBracketIndex(expression, startIndex);
                    expression = expression.Remove(closeBracketIndex, 1);
                    expression = expression.Insert(closeBracketIndex, ">");
                    string xExpression = GetStrWhile(expression, startIndex+1, ',');
                    string yExpression = GetStrWhile(expression, startIndex + xExpression.Length + 2, '>');
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

            return expression;
        }
        private string GetStrWhile(string str, int startIndex, char stopSymbol)
        {
            string result = "";
            for (int i = startIndex; i < str.Length; i++)
            {
                if (str[i].Equals(stopSymbol))
                    break;
                result += str[i];
            }

            return result;
        }
    }
}