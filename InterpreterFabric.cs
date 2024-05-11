using ExpressionProcessing;
using CalcModule;

namespace Lab5
{
    public class InterpreterFabric
    {
        public static Interpreter GetInstance()
        {
            var interpreterContext = new InterpreterContext();
            var interpreter = new Interpreter(interpreterContext);
            var calc = new CalculatorProxy();
            var replacer = new ExpressionReplacer(interpreterContext, calc);
            var booleanExpressionSolver = new BooleanExpressionSolver(calc,replacer);
            var variableParser = new VariableParser(replacer,calc,booleanExpressionSolver);
            interpreter.
                InitBooleanExpressionSolver(booleanExpressionSolver).
                InitVariableParser(variableParser).
                InitExpressionReplacer(replacer).
                InitForKeyKodeFabric();
            return interpreter;
        }
    }
}