using System;
using System.Linq;
using System.Reflection;
using CalcModule;
using Lab5;

namespace ExpressionProcessing
{
    public class MemberReplacer
    {
        private InterpreterContext _interpreterContext;
        private ReplacerHelper _replacerHelper = new ReplacerHelper();
        private BracketHelper _bracketHelper = new BracketHelper();
        private CalculatorProxy _calc;
        private ExpressionReplacer _expressionReplacer;
        public string ReplaceMembers(string expression)
        {
            string token = "";
            int countOfDots = expression.Count(c => c.Equals('.'));
            int dotIndex = 0;
            for (int i = 0; i < countOfDots; i++)
            {
                if (dotIndex>=expression.Length) break;
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
                        string[] arguments = _bracketHelper.TakeArgumentsFromBrackets(expression, openBracketIndex);
                        arguments = arguments.Select(_expressionReplacer.ReplaceTokensInExpression).ToArray();
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
            int closeBracketIndex = _bracketHelper.GetCloseBracketIndex(expression, openBracketIndex+1, '(',')');
            string bracketExpression = new string(expression.Skip(openBracketIndex)
                .Take(closeBracketIndex - (openBracketIndex-1)).ToArray());
            string entityWithMember = entityStr + "." + methodName;
            string fullReplacedObjectName = entityWithMember + bracketExpression;
            expression = _replacerHelper.ReplaceVariableWithValue(expression, fullReplacedObjectName, returnValue,openBracketIndex-entityWithMember.Length);
            return expression;
        }
        public MemberReplacer(InterpreterContext interpreterContext, CalculatorProxy calc)
        {
            _interpreterContext = interpreterContext;
            _calc = calc; 
        }

        public void InitExpressionReplacer(ExpressionReplacer expressionReplacer)
        {
            _expressionReplacer = expressionReplacer;
        }
    }
}
