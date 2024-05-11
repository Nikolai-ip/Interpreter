using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lab5;

namespace CalcModule
{
    internal static class ExpressionTokenizer
    {
        private static readonly char[] Operators = { '+', '-', '*', '/', '(', ')' };
        private static StringCleaner _stringCleaner = new StringCleaner();
       
        public static List<string> TokenizeExpression(string expression)
        {
            expression = _stringCleaner.RemoveSpaces(expression);
            List<string> tokens = new List<string>();
            int i = 0;
            bool isNegativeNumber = false;
            
            while (i < expression.Length)
            {
                if (char.IsDigit(expression[i]) || expression[i] == '.')
                {
                    tokens.Add(ExtractNumber(expression, ref i));
                    if (isNegativeNumber)
                    {
                        tokens.Add(")"); 
                        isNegativeNumber = false;
                    }
                }
                else if (expression[i].Equals('<'))
                {
                    tokens.Add(ExtractVector(expression, ref i));
                }
                else if (Operators.Contains(expression[i]))
                {
                    if(IsNegativeNumber(expression,i))
                    {
                        tokens.Add("(");
                        tokens.Add("0");
                        isNegativeNumber = true;
                    }
                    tokens.Add(expression[i].ToString());
                    i++;
                }
                else
                {
                    Console.WriteLine("Некорректный ввод " + expression[i]);
                    break;
                }
            }

            return tokens;
        }
        
        private static string ExtractNumber(string expression, ref int i)
        {
            string number = "";
            bool hasDecimalPoint = false;

            // Добавляем ноль в целую часть, если она отсутствует
            if (expression[i] == '.')
            {
                number += "0";
            }

            while (i < expression.Length && (char.IsDigit(expression[i]) || (!hasDecimalPoint && expression[i] == '.')))
            {
                if (expression[i] == '.')
                {
                    hasDecimalPoint = true;
                }
                number += expression[i];
                i++;
            }
            return number;
        }

        private static string ExtractVector(string expression, ref int i)
        {
            expression = expression.Remove(0, i);
            string result = new string(expression.TakeWhile(c => !c.Equals('>')).ToArray()) + '>';
            i += result.Length;
            return result;
        }

        private static bool IsNegativeNumber(string expression, int i)
        {
            char[] symbols = { '(', ',', '*', '+', '/' };
            return (i != 0 && symbols.Contains(expression[i-1]) && expression[i] == '-' ) || (i == 0 && expression[i] == '-');
        }
        private static List<string> ExtractFunction(string expression, ref int i)
        {
            
            string functionName = "";
            while (i < expression.Length && char.IsLetter(expression[i]))
            {
                functionName += expression[i];
                i++;
            }
            if ((i < expression.Length && expression[i] == '('))
            {
                List<string> tokens = new List<string> { functionName, "(" };
                i++;
                List<string> functionArgs = ExtractFunctionArguments(expression, ref i);
                try
                {
                   

                    if (functionArgs.Count != 2)
                    {
                       throw new Exception("Функция имеет некорректное число аргументов");
                    }
                    else
                    {
                        tokens.AddRange(TokenizeExpression(functionArgs[0]));
                        tokens.Add(",");
                        tokens.AddRange(TokenizeExpression(functionArgs[1]));
                    }
                    
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"Ошибка: {ex.Message}");
                    Environment.Exit(1);
                }

                return tokens;
            }
            else
            {
                Console.WriteLine("Некорректная функция: " + functionName);
                return new List<string>();
            }
        }

        private static List<string> ExtractFunctionArguments(string expression, ref int i)
        {
            int openParenCount = 1;
            int closeParenCount = 0;
            string functionArg = "";
            List<string> functionArgs = new List<string>();

            while (i < expression.Length && openParenCount != closeParenCount)
            {
                if (expression[i] == '(')
                {
                    openParenCount++;
                }
                else if (expression[i] == ')')
                {
                    closeParenCount++;
                }
                //получим первый аргумент
                else if (expression[i] == ',' && openParenCount == closeParenCount + 1)
                {
                    functionArgs.Add(functionArg);
                    functionArg = "";
                    i++;
                    continue;
                }

                functionArg += expression[i];
                i++;
            }
            //получим второй аргумент
            functionArgs.Add(functionArg);
            foreach (var arg in functionArgs)
            {
                if (arg == "")
                {
                    Console.WriteLine("Отсутствует аргумент функции");
                }
            }
           

            return functionArgs;
        }


    }
}
