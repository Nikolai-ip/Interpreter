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
        private CalculatorProxy _calc;
        public string ReplaceTokensInExpression(string expression)
        {
            string token = "";
            expression = _stringCleaner.RemoveSpaces(expression);
            expression = ReplaceMembers(expression);
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

        public string ReplaceMembers(string expression)
        {
            string token = "";
            int countOfDots = expression.Count(c => c.Equals('.'));
            int dotIndex = 0;
            for (int i = 0; i < countOfDots; i++)
            {
                dotIndex = expression.IndexOf('.',dotIndex);
                string entityStr = GetWordByDotSide(dotIndex, expression, true);
                string memberName = GetWordByDotSide(dotIndex, expression, false);
                if (TryParseEntity(entityStr, out object entityObj, out Type entityType))
                {
                    if (MemberIsMethod(expression,dotIndex,memberName))
                    {
                        int openBracketIndex = dotIndex + memberName.Length+1;
                        MethodInfo methodInfo = entityType.GetMethod(memberName);
                        if (methodInfo == null) throw new Exception($"Failed to invoke the method {memberName} on an {entityStr}");
                        string[] arguments = _bracketHelper.TakeArgumentsFromBrackets(expression, openBracketIndex).
                            Select(ReplaceTokensInExpression).ToArray();
                        object returnValue = InvokeMethod(arguments,methodInfo);
                        expression = ReplaceMethodByReturnValue(expression, returnValue, entityStr, memberName, openBracketIndex);
                    }
                    else if (TryGetFieldValue(entityObj,entityType,memberName,out var value) || TryGetPropertyValue(entityObj,entityType,memberName,out value))
                    {
                        string fullName = entityStr + "." + memberName;
                        expression = _replacerHelper.ReplaceVariableWithValue(expression, fullName, value, dotIndex-entityStr.Length);
                    }
                    else
                    {
                        throw new Exception("Failed to attempt identify the member");
                    }

                }

                dotIndex++;
            }
            return expression;
        }

        private bool TryGetFieldValue(object entityObj, Type entityType, string fieldName, out object value)
        {
            FieldInfo fieldInfo = entityType.GetField(fieldName);
            value = null;
            if (fieldInfo != null)
                value = fieldInfo.GetValue(entityObj);
            return value != null;
        }
        private bool TryGetPropertyValue(object entityObj, Type entityType, string propName, out object value)
        {
            PropertyInfo propertyInfo = entityType.GetProperty(propName);
            value = null;
            if (propertyInfo != null)
                value = propertyInfo.GetValue(entityObj);
            return value != null;
        }
        private string GetWordByDotSide(int dotIndex, string expression, bool takeWordOnLeft)
        {
            string word = "";
            if (takeWordOnLeft)
            {
                for (int i = dotIndex-1; i >= 0; i--)
                {
                    if (!char.IsLetter(expression[i]) && !char.IsDigit(expression[i]))
                    {
                        break;
                    }
                    word += expression[i];
                    
                }
            }
            else
            {
                for (int i = dotIndex+1; i < expression.Length; i++)
                {
                    if (!char.IsLetter(expression[i]) && !char.IsDigit(expression[i]))
                        break;
                    word += expression[i];
                }
            }

            if (takeWordOnLeft) word = new string(word.Reverse().ToArray());
            return word;
        }

        private bool TryParseEntity(string entity, out object result, out Type resultType)
        {
            result = null;
            resultType = null;
            if (_interpreterContext.Variables.TryGetValue(entity, out object value))
            {
                result = value;
                resultType = result.GetType();
            } 
            if (_interpreterContext.SupportedVariableTypesMap.TryGetValue(entity, out Type type))
            {
                resultType = type;
            }

            if (result != null || resultType != null) 
                return true;
            return false;
        }

        private bool MemberIsMethod(string expression, int i, string memberName)
        {
            int bracketIndex = i + memberName.Length + 1;
            if (bracketIndex >= expression.Length) return false;
            return expression[i + memberName.Length + 1].Equals('(');
        }
        private object InvokeMethod(string[] argumentsStr, MethodInfo methodInfo)
        {
            object[] arguments = new object[argumentsStr.Length];
            for (int i = 0; i < argumentsStr.Length; i++)
            {
                arguments[i] = _calc.GetResult(argumentsStr[i]);
            }
            return methodInfo.Invoke(null,arguments);
        }

        private string ReplaceMethodByReturnValue(string expression, object returnValue, string entityStr,
            string methodName, int openBracketIndex)
        {
            int closeBracketIndex = _bracketHelper.GetCloseBracketIndex(expression, openBracketIndex+1);
            string bracketExpression = new string(expression.Skip(openBracketIndex)
                .Take(closeBracketIndex - (openBracketIndex-1)).ToArray());
            string entityWithMember = entityStr + "." + methodName;
            string fullReplacedObjectName = entityWithMember + bracketExpression;
            expression = _replacerHelper.ReplaceVariableWithValue(expression, fullReplacedObjectName, returnValue,openBracketIndex-entityWithMember.Length);
            return expression;
        }
      
        public ExpressionReplacer(InterpreterContext interpreterContext, CalculatorProxy calc)
        {
            _interpreterContext = interpreterContext;
            //_memberReplacer = new MemberReplacer(interpreterContext, calc);
            _calc = calc;
        }
        
    }
}