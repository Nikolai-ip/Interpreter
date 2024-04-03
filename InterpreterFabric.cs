using lab1;

namespace Lab5
{
    public class InterpreterFabric
    {
        public static Interpreter GetInstance()
        {
            var interpreterContext = new InterpreterContext();
            var interpreter = new Interpreter(interpreterContext);
            var replacer = new ExpressionReplacer(interpreterContext);
            var calc = new CalculatorProxy();
            var variableParser = new VariableParser(replacer,calc);
            var booleanExpressionSolver = new BooleanExpressionSolver(calc,replacer);
            interpreter.
                InitBooleanExpressionSolver(booleanExpressionSolver).
                InitVariableParser(variableParser).
                InitExpressionReplacer(replacer).
                InitForKeyKodeFabric();
            return interpreter;
        }
    }
}