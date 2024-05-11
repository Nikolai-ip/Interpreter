using System.Linq;
using System.Text.RegularExpressions;
using ExpressionProcessing;
using Lab5;

namespace KeyWord
{
    public class ConditionKeyWord
    {
        private BooleanExpressionSolver _booleanExpressionSolver;
        private StringCleaner _stringCleaner = new StringCleaner();
        private BracketHelper _bracketHelper = new BracketHelper();
        
        public bool TryHandleCodeLine(string programText, ref int pc)
        {
            string currentCodeLine = GetCurrentCodeLine(programText, pc);
            currentCodeLine = _stringCleaner.RemoveSpaces(currentCodeLine);
            if (Regex.IsMatch(currentCodeLine, @"if\((?:.|[\r\n])*?\)"))
            {
                string bracketExpression = _bracketHelper.GetExpressionInBracket("if(".Length, currentCodeLine);
                if (!_booleanExpressionSolver.Solve(bracketExpression))
                {
                    JumpOverIfSection(programText, ref pc);
                }
                return true;
            }
            return false;
        }

        private string GetCurrentCodeLine(string programText, int pc)
        {
            string codeLine = "";
            for (int i = 0; i < programText.Length; i++)
            {
                if (programText[i].Equals('\n'))
                    pc--;
                
                if (pc == 0)
                {
                    codeLine = new string(programText.Skip(i).TakeWhile(c => c != '\n').ToArray());
                    break;
                }
            }

            return codeLine;
        }
        private string GetCurrentCodeLineLINQ(string programText, int pc)
        {
            var codeLines = programText.Split('\n');
            var codeLine = codeLines.SkipWhile(line => --pc > 0).FirstOrDefault(line => line.Contains(';'));
            return codeLine;
        }

        private void JumpOverIfSection(string programText, ref int pc)
        {
            int i;
            int codeLineAmount = 0;
            for ( i = 0; i < programText.Length; i++)
            {
                if (programText[i].Equals('\n'))
                {
                    codeLineAmount++;
                }
                if (pc == codeLineAmount)
                {
                    break;
                }
            }
            int openBracketIndex = programText.IndexOf('{', i);
            int closeBracketIndex = _bracketHelper.GetCloseBracketIndex(programText, openBracketIndex+1, '{', '}');
            pc = GetCodeLineBySymbolIndex(programText, closeBracketIndex);
        }
        private int GetCodeLineBySymbolIndex(string program, int symbolIndex)
        {
            int i;
            int codeLineAmount = 0;
            for ( i = 0; i < symbolIndex; i++)
            {
                if (program[i].Equals('\n'))
                {
                    codeLineAmount++;
                }
            }
            return codeLineAmount+1;
        }
        public ConditionKeyWord(BooleanExpressionSolver booleanExpressionSolver)
        {
            _booleanExpressionSolver = booleanExpressionSolver;
        }
    }
}