using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

static class ExpressionConvertor
{
    static List<string> output = new List<string>();
    static Stack<string> operators = new Stack<string>();
    static Dictionary<string, int> precedence = new Dictionary<string, int>
    {
        { "+", 1 },
        { "-", 1 },
        { "*", 2 },
        { "/", 2 },
    };

    public static List<string> ConvertToRPN(List<string> tokens)
    {
        output.Clear();
        operators.Clear();
        foreach (var token in tokens)
        {
            // Если токен - число, добавляем его в выходную очередь
            if (double.TryParse(token.Replace(".", ","), out _) || IsVector(token))
            {
                output.Add(token);
            }
            // Если токен - открывающая скобка, помещаем ее в стек операторов
            else if (token == "(")
            {
                operators.Push(token);
            }
            // Если токен - закрывающая скобка, выталкиваем операторы из стека в выходную очередь
            else if (token == ")")
            {
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                if (operators.Count > 0 && operators.Peek() == "(")
                {
                    operators.Pop(); // Удаляем открывающую скобку
                }
            }
            else if (precedence.ContainsKey(token))
            {
                // Если токен - оператор
                while (operators.Count > 0 && precedence[token] <= GetOperatorPrecedence(operators.Peek()))
                {
                    output.Add(operators.Pop());
                }
                operators.Push(token);
            }
            else if (token == "pow")
            {
                operators.Push(token); // Пушим функцию log в стек операторов
            }
            else if (token == ",")
            {
                // Разделитель аргументов функции, переносим операторы из стека в выходную очередь
                while (operators.Count > 0 && operators.Peek() != "(")
                {
                    output.Add(operators.Pop());
                }
                if (operators.Count == 0 || operators.Peek() != "(")
                {
                    throw new Exception("Некорректное использование запятой в выражении.");
                }
            }
        }

        while (operators.Count > 0)
        {
            output.Add(operators.Pop());
        }

        return output;
    }

    private static int GetOperatorPrecedence(string op)
    {
        int precedenceValue;
        return precedence.TryGetValue(op, out precedenceValue) ? precedenceValue : 0;
    }

    private static bool IsVector(string str)
    {
        if (!str[0].Equals('<'))
            return false;
        if (!str.Last().Equals('>'))
            return false;
        if (!str.Contains(' '))
            return false;
        return true;
    }
}

