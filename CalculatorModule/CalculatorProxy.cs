using System;
using System.Collections.Generic;

namespace CalcModule
{
    public class CalculatorProxy
    {
        public object GetResult(string expression)
        {
            object result = null;
            List<string> tokens = ExpressionTokenizer.TokenizeExpression(expression);
            List<string> output = ExpressionConvertor.ConvertToRPN(tokens); 
            result = Calculator.CalculateRPN(output);

            return result;
        }
    }
}