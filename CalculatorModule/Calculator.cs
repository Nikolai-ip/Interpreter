using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace lab1
{
    static class Calculator
    {

        static public object CalculateRPN(List<string> output)
        {

            Stack<object> stack = new Stack<object>();
            try
            {
                foreach (var token in output)
                {
                    if (double.TryParse(token.Replace(".", ","), out double number))
                    {
                        stack.Push(number);
                    }
                    else if (token.First().Equals('<'))
                    {
                        stack.Push(ParseVector2(token)); 
                    }
                    else
                    {
                        object b = stack.Pop();
                        object a = stack.Pop();
                        stack.Push(ExecuteOperationWithOperands(a, b, token));
                    }
                   
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
                Environment.Exit(1);
            }
            return stack.Pop();
        }

        private static Vector2 ParseVector2(string str)
        {
            str = str.Replace("<", "").Replace(">", "").Replace('.',','); //5 2
            string[] parts = str.Split(new char[] { ' ', '\u00A0' }); // Разделение по пробелу или неразрывному пробелу
            float x = (float)Convert.ToDouble(parts[0]);
            float y = (float)Convert.ToDouble(parts[1]);
            return new Vector2(x, y);
        }

        private static object ExecuteOperationWithOperands(object a, object b, string operation)
        {
            if (a is double aNumber && b is double bNumber)
            {
                return ExecuteNumericOperation(aNumber, bNumber, operation);
            }
            else if (a is Vector2 inputVectorA && b is Vector2 inputVectorB)
            {
                return ExecuteVectorOperation(inputVectorA, inputVectorB, operation);
            }
            else if (a is Vector2 vector && double.TryParse(b.ToString(), out double scalar))
            {
                return ExecuteVectorScalarOperation(vector, scalar, operation);
            }
            else if (double.TryParse(a.ToString(), out double scalarValue) && b is Vector2 vectorValue)
            {
                return ExecuteScalarVectorOperation(scalarValue, vectorValue, operation);
            }

            return null;
        }

        private static object ExecuteNumericOperation(double a, double b, string operation)
        {
            switch (operation)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                case "/": return b != 0 ? (double?)(a / b) : throw new Exception("Division by zero");
                default: return null;
            }
        }

        private static object ExecuteVectorOperation(Vector2 a, Vector2 b, string operation)
        {
            switch (operation)
            {
                case "+": return a + b;
                case "-": return a - b;
                case "*": return a * b;
                default: return null;
            }
        }

        private static object ExecuteVectorScalarOperation(Vector2 vector, double scalar, string operation)
        {
            switch (operation)
            {
                case "*": return vector * (float)scalar;
                case "/": return vector / (float)scalar;
                default: return null;
            }
        }

        private static object ExecuteScalarVectorOperation(double scalar, Vector2 vector, string operation)
        {
            return ExecuteVectorScalarOperation(vector, scalar, operation);
        }
    }
}
