using System.Linq;
using System.Text.RegularExpressions;

namespace Lab5
{
    public class VectorExpressionHelper
    {
        public bool ExpressionHasVector(string expression)
        {
            int i = -1;
            do
            {
                i++;
                i = expression.IndexOf('<', i);
                string token = new string(expression.Skip(i).TakeWhile(c => c != '>').ToArray()) + '>';
                if (Regex.IsMatch(token, "<[-+]?\\d+(\\,\\d+)?\\s+[-+]?\\d+(\\,\\d+)?>"))
                    return true;
            } while (i != -1);

            return false;
        }
        // public bool ExpressionHasVector(string expression)
        // {
        //     int i = 0;
        //     while (i != -1)
        //     {
        //         i = expression.IndexOf('<',i);
        //         string firstToken = TakeWord(expression, i);
        //         char spaceSymbol = expression[i + firstToken.Length];
        //         string secondToken = TakeWord(expression, i + firstToken.Length + 1);
        //         if (double.TryParse(firstToken, out double res) && spaceSymbol==' ' && double.TryParse(secondToken, out double res2))
        //         {
        //             return true;
        //         }
        //         
        //         i++;
        //     }
        //
        //     return false;
        // }

        private string TakeWord(string expression, int startIndex)
        {
            return new string(expression.Skip(startIndex+1).TakeWhile(c => c != ' ' ).ToArray());
        }
        private bool TokenIsDigit(string expression, int i)
        {
            return char.IsDigit(expression[i]) || (expression[i] == '-' && char.IsDigit(expression[i + 1]));
        }
    }
}