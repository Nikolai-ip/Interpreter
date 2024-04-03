namespace Lab5
{
    public class ForKeyWord:KeyWord
    {
        public int Index { get; }
        public int StartValue { get; }
        public string NameOfCounter { get; }
        public string AddedExpression { get; }
        public string Condition { get; }
        public ForKeyWord(int codeLineNumber, string nameOfCounter, int startValue, string condition, string addedExpression) : base(codeLineNumber)
        {
            NameOfCounter = nameOfCounter;
            AddedExpression = addedExpression;
            Index = startValue;
            Condition = condition;
            StartValue = startValue;
        }
    }
}