using System;
using System.Collections.Generic;
using System.Linq;
using lab1;

namespace Lab5
{
    public class BooleanExpressionSolver
    {
        private Dictionary<string, Func<float, float, bool>> _comparisonFunctions =
            new Dictionary<string, Func<float, float, bool>>()
            {
                { ">", (a, b) => a > b },
                { "<", (a, b) => a < b },
                { "!=", (a, b) => a != b },
                { ">=", (a, b) => a >= b },
                { "<=", (a, b) => a <= b }
            };

        private List<char> _primarySymbolsOfComparisonFunc = new List<char>() { '>', '<', '!' };
        private ExpressionReplacer _expressionReplacer;
        private CalculatorProxy _calculatorProxy;
        public bool Solve(string expression)
        {
            int index = GetIndexOfComparisonOperator(expression, out string comparisonOperator);
            string leftSubExpression = new string(expression.Take(index).ToArray());
            string rightSubExpression = new string(expression.Skip(index + comparisonOperator.Length).ToArray());
            string replacedLeftExpression = _expressionReplacer.ReplaceTokensInExpression(leftSubExpression);
            string replacedRightExpression = _expressionReplacer.ReplaceTokensInExpression(rightSubExpression);
            float? leftExDigital = (float)_calculatorProxy.GetResult(replacedLeftExpression);
            float? rightExDigital = (float)_calculatorProxy.GetResult(replacedRightExpression);
            if (leftExDigital.HasValue && rightExDigital.HasValue)
            {
                return _comparisonFunctions[comparisonOperator](leftExDigital.Value, rightExDigital.Value);
            }
            throw new Exception("Выражение для сравнения составленно некорректно");

        }

        private int GetIndexOfComparisonOperator(string expression, out string comparisonOperator)
        {
            int index = 0; 
            comparisonOperator = "";
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                if (_primarySymbolsOfComparisonFunc.Contains(c))
                {
                    comparisonOperator += c;
                    if (expression[i + 1] == '=')
                    {
                        comparisonOperator += '=';
                    }
                    break;
                }

                index++;
            }

            return index;
        }

        public BooleanExpressionSolver(CalculatorProxy calculator, ExpressionReplacer expressionReplacer)
        {
            _expressionReplacer = expressionReplacer;
            _calculatorProxy = calculator;
        }
    }
}