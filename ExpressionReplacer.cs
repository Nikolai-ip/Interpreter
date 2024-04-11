using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using lab1;

namespace Lab5
{
    public class ExpressionReplacer
    {
        private InterpreterContext _interpreterContext;
        private readonly char[] _allowableOperations = { '+', '/', '*', '-' };
        private StringCleaner _stringCleaner = new StringCleaner();
        private BracketHelper _bracketHelper = new BracketHelper();
        private ReplacerHelper _replacerHelper = new ReplacerHelper();
        private CharChecker _charChecker = new CharChecker();
        private MemberReplacer _memberReplacer;

        public string ReplaceTokensInExpression(string expression)
        {
            expression = _stringCleaner.RemoveSpaces(expression);
            expression = _memberReplacer.ReplaceMembers(expression);
            expression = ReplaceVariablesInExpression(expression);
            return expression;
        }
        private string ReplaceVariablesInExpression(string expression)
        {
            string token = "";
            for (var i = 0; i < expression.Length; i++)
            {
                var c = expression[i];
                if (IsToken(c,token))
                {
                    token += c;
                }
                else if (!string.IsNullOrEmpty(token))
                {
                    if (_interpreterContext.Variables.TryGetValue(token, out object value))
                    {
                        expression = _replacerHelper.ReplaceVariableWithValue(expression, token, value, i-token.Length);
                        string valueStr = _stringCleaner.ReplaceValueToString(value);
                        i += valueStr.Length - (token.Length+1);
                    }
                    else if (_interpreterContext.SupportFunctions.TryGetValue(token, out var function))
                    {
                        string expressionInBracket = _bracketHelper.GetExpressionInBracket(i, expression);
                        i += expressionInBracket.Length + 1;
                        string replacedExpression = ReplaceTokensInExpression(expressionInBracket);
                        var result = function(new object[] { replacedExpression });
                        if (result != null)
                            expression = expression.Replace(token + "(" + expressionInBracket + ")", result.ToString());
                    }
                    else
                    {
                        throw new Exception("Некорректный токен:" + token);
                    }

                    token = "";
                }
            }


            if (token != "")
            {
                if (_interpreterContext.Variables.TryGetValue(token, out object value)) 
                {
                    expression = _replacerHelper.ReplaceVariableWithValue(expression, token, value, expression.Length-token.Length);
                }
            }
            return expression;
        }
        
        private bool IsToken(char c, string token)
        {
            return !char.IsDigit(c) && !c.Equals('.') && !c.Equals(',') && !_allowableOperations.Contains(c) &&
                   !_charChecker.IsBracket(c) && !_charChecker.IsCompareOperator(c) && !c.Equals(' ') ||
                   _charChecker.NumberIsPartOfVariableName(c, token);
        }
        
        public ExpressionReplacer(InterpreterContext interpreterContext, CalculatorProxy calc)
        {
            _interpreterContext = interpreterContext;
            _memberReplacer = new MemberReplacer(interpreterContext, calc);
            _memberReplacer.InitExpressionReplacer(this);
        }
        
    }
}