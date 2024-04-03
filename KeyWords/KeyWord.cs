namespace Lab5
{
    public abstract class KeyWord
    {
        public int CodeLineNumber { get; }

        protected KeyWord(int codeLineNumber)
        {
            CodeLineNumber = codeLineNumber;
        }
    }
}