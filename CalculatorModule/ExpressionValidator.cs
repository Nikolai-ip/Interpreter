using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

static class ExpressionValidator
{
    private static readonly string[] Functions = { "pow" };



    public static bool IsValid(string expression)
    {
        string patternMissingMultiplication = @"\d\(";
        string patternConsecutiveOperators = @"[\+\-\*/]{2,}";
        bool containsMissingMultiplication = Regex.IsMatch(expression, patternMissingMultiplication);
        bool containsConsecutiveOperators = Regex.IsMatch(expression, patternConsecutiveOperators);
        try
        {

            if(string.IsNullOrEmpty(expression))
                throw new Exception("Выражение не может быть пустым.");


            int openBracketCount = expression.Count(c => c == '(');
            int closeBracketCount = expression.Count(c => c == ')');
            if(openBracketCount != closeBracketCount)
                throw new Exception("Несоответствие количества открывающих и закрывающих скобок.");

            if (containsMissingMultiplication)
                throw new Exception("В выражении есть отсутствие умножения между числом и скобкой.");

            if (containsConsecutiveOperators)
                throw new Exception("В выражении есть несколько операций подряд без чисел.");



            int i = 0;
            while (i<expression.Length)
            {
                if (char.IsLetter(expression[i]))
                {
                    string functionName = "";
                    while (i < expression.Length && char.IsLetter(expression[i]))
                    {
                        functionName += expression[i];
                        i++;
                    }

                    if (!Functions.Contains(functionName))
                        throw new Exception("Неизвестная функция " + functionName);
                }
                i++;
                   
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return false;
        }
    }


    private static bool IsValidFunctionName(string expression, ref int i)
    {
        string functionName = "";
        while (i < expression.Length && char.IsLetter(expression[i]))
        {
            functionName += expression[i];
            i++;
        }

        if(!Functions.Contains(functionName) )
        {
            Console.WriteLine("Ошибка: Неизвестная функция "+ functionName);
            return false;
        }
        return true;
    }
}

