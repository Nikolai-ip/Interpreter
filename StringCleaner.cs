using System;
using System.Globalization;
using System.Numerics;

namespace Lab5
{
    public class StringCleaner
    {
        public string RemoveSpaces(string expression)
        {
            bool isVector = false;
            for (int i = 0; i < expression.Length; i++)
            {
                if (expression[i].Equals(' ') && !isVector)
                {
                    expression = expression.Remove(i, 1);
                    if(i>0)
                        i--;
                }

                if (expression[i].Equals('<'))
                    isVector = true;
                if (expression[i].Equals('>'))
                    isVector = false;
            }

            return expression;
        }

        public string ReplaceValueToString(object value)
        {
            if (value is Vector2 vector2)
            {
                return $"<{vector2.X} {vector2.Y}>";
            }

            return Convert.ToString(value, CultureInfo.InvariantCulture);
        }
    }
}