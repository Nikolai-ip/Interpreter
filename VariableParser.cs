using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text.RegularExpressions;
using lab1;

namespace Lab5
{
    public class VariableParser
    {
        private CalculatorProxy _calculator;
        private ExpressionReplacer _expressionReplacer;
        private VectorInitReplacer _vectorInitReplacer;
        public string GetVariable(string codeLine, string typeStr, out object value)
        {
            string variableName;
            typeStr = typeStr.Trim();
            codeLine = codeLine.Remove(0, typeStr.Length); // a =
            Type variableType = GetVariableType(typeStr);
            value = InitializeVariable(variableType);
           
            if (codeLine.Contains("="))
            {
                variableName = new string(codeLine.TakeWhile(c => c != '=').ToArray()).Trim();
                value = GetVariableValue(codeLine, variableType);
                value = Convert.ChangeType(value, variableType);
            }
            else
            {
                variableName = new string(codeLine.TakeWhile(c => c != ';').ToArray());
                variableName = variableName.Trim();
            }
            return variableName;
        }

        private Type GetVariableType(string typeStr)
        {
            switch (typeStr)
            {
                case "int": return typeof(int);
                case "string": return typeof(string);
                case "Vector2": return typeof(Vector2);
                case "float": return typeof(float);
                case "double": return typeof(double);
                case "bool": return typeof(bool);
                default: return null;
            }
            
        }

        private object InitializeVariable(Type variableType)
        {
            if (variableType == typeof(string))
            {
                return string.Empty;
            }
            return Activator.CreateInstance(variableType);
        }
        private object GetVariableValue(string codeLine, Type type)
        {
            int startIndex = codeLine.IndexOf('=') + 1;
            int endIndex = codeLine.IndexOf(';'); 
            int length = endIndex - startIndex;    
            string expression = codeLine.Substring(startIndex, length).Trim();
            if (TypeIsDigital(type))
            {
                expression = _expressionReplacer.ReplaceTokensInExpression(expression);
                return _calculator.GetResult(expression);
            }

            if (type == typeof(string))
            {
                expression = expression.Replace("\"", "");
                return expression;
            }

            if (type == typeof(Vector2))
            {
                Vector2? vector2 = ParseVectorInitExpression(expression);
                if (vector2 == null)
                {
                    try
                    {
                        expression = _expressionReplacer.ReplaceTokensInExpression(expression);
                        return _calculator.GetResult(expression);
                    }
                    catch (Exception e)
                    {
                        throw new Exception("Syntax error: not correct instantiate Vector2");
                    }
                }
                else
                    return vector2;
            }

            return null;
        }

        private bool TypeIsDigital(Type type)
        {
            return type == typeof(int) || type == typeof(float) || type == typeof(double);
        }
        private Vector2? ParseVectorInitExpression(string expression)
        {
            Vector2? result;
            expression = _vectorInitReplacer.ReformatVectorInitExpression(expression);
            expression = _expressionReplacer.ReplaceTokensInExpression(expression);
            result = (Vector2) _calculator.GetResult(expression);
            return result;
        }

       

        public VariableParser(ExpressionReplacer expressionReplacer, CalculatorProxy calculator)
        {
            _expressionReplacer = expressionReplacer;
            _calculator = calculator;
            _vectorInitReplacer = new VectorInitReplacer(_expressionReplacer, _calculator);
        }
    }
}