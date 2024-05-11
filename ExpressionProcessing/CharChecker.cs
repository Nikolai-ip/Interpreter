using System.Linq;

namespace ExpressionProcessing
{
    public class CharChecker
    {
        public bool NumberIsPartOfVariableName(char c, string token)
        {
            return char.IsDigit(c) && token.Length > 0 && char.IsLetter(token.Last());
        }
        public bool IsBracket(char c)
        {
            return c.Equals('(') || c.Equals(')');
        }

        public bool IsCompareOperator(char c)
        {
            return c.Equals('<') || c.Equals('>');
        }
    }
}