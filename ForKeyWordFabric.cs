using System;
using System.Linq;

namespace Lab5
{
    public class ForKeyWordFabric
    {
        private VariableParser _variableParser;
        
        public ForKeyWord GetKeyWord(string codeLine, int pc)
        {
            codeLine = new string(codeLine.SkipWhile(c => c!='(').ToArray()) ;
            codeLine = codeLine.Trim();
            codeLine = codeLine.Remove(codeLine.Length - 1);
            codeLine = codeLine.Remove(0,1);
            string[] partsOfForConstruction = codeLine.Split(';');
            string nameOfCounter = _variableParser.GetVariable(partsOfForConstruction[0]+';', "int", out object value);
            int startValue = Convert.ToInt32(value);

            string condition = partsOfForConstruction[1];
            string addedExpression = partsOfForConstruction[2];
            return new ForKeyWord(pc, nameOfCounter, startValue, condition, addedExpression);
        }
        public ForKeyWordFabric(VariableParser variableParser)
        {
            _variableParser = variableParser;
        }
        
    }
}