using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ExpressionProcessing;
using CalcModule;
using KeyWord;

namespace Lab5
{
    public class Interpreter
    {
        private int _pc = 0;
        private VariableParser _variableParser;
        private InterpreterContext _interpreterContext;
        public InterpreterContext InterpreterContext => _interpreterContext;
        private BooleanExpressionSolver _booleanExpressionSolver;
        private Stack<ForKeyWord> _keyWordsStack = new Stack<ForKeyWord>();
        private ForKeyWordFabric _forKeyWordFabric;
        private ExpressionReplacer _expressionReplacer;
        private CalculatorProxy _calculator = new CalculatorProxy();
        private VectorExpressionHelper _vectorExpressionHelper = new VectorExpressionHelper();
        private ConditionKeyWord _conditionKeyWord;
        public object ReturnValue { get; private set; }
        private string _programText;
        public void Interpret(string programText)
        {
            _programText = programText;
            try
            {
                string codeLine = "";
                do
                {
                    codeLine = GetCodeLine(_programText, _pc).Trim();
                    if (string.IsNullOrEmpty(codeLine))
                        break;
                    HandleCodeLine(codeLine);
                    _pc++;
                } while (ReturnValue==null);
            }
            catch (Exception e)
            {
                Console.WriteLine(e + " " + _pc + 1);
            }
        }

        private string GetCodeLine(string programText, int numberOfSearchedCodeLine)
        {
            var codeLine = new StringBuilder();
            int lineNumber = 0;
            bool lineFound = false;
            for (int i = 0; i < programText.Length; i++)
            {
                if (programText[i].Equals('\n'))
                {
                    if (lineFound)
                        break;
                    lineNumber++;
                }
                lineFound = lineNumber == numberOfSearchedCodeLine;
                if (lineFound)
                {
                    codeLine.Append(programText[i]);
                }
            }

            if (codeLine.Length==0) return null;
            if (codeLine[0] == '\n') codeLine.Remove(0, 1);
            return codeLine.ToString();
        }

        private void HandleCodeLine(string codeLine)
        {
            codeLine = codeLine.Trim();
            if (!codeLine.Last().Equals(';')) throw new Exception("Syntax error: code line must end with a sign ';'");
            string firstOperator = new string(codeLine.TakeWhile(c => c != ' ').ToArray());
            
            if (_interpreterContext.SupportedVariableTypes.Contains(firstOperator))
            {
               string nameOfVariable =  _variableParser.GetVariable(codeLine,firstOperator, out object variable);
               _interpreterContext.Variables.Add(nameOfVariable, variable);
            }

            if (firstOperator == "for")
            {
                AddForLoopToStack(codeLine);
            }

            if (codeLine.Contains('}'))
            {
                DoLoopInteraction();
            }
            
            if (_interpreterContext.Variables.ContainsKey(firstOperator))
            {
                DoInstractionWithVariable(codeLine,_interpreterContext.Variables[firstOperator].GetType());
            }

            if (char.IsLetter(firstOperator.First()) && firstOperator.Contains('(') && firstOperator.Contains(')'))
            {
                codeLine = RemoveSemicolon(codeLine);
                _expressionReplacer.ReplaceTokensInExpression(codeLine);
            }
            if (firstOperator == "return")
            {
                codeLine = codeLine.Replace("return", "").Trim();
                codeLine = codeLine.Remove(codeLine.Length - 1);
                codeLine = _expressionReplacer.ReplaceTokensInExpression(codeLine);
                if (ExpressionIsBool(codeLine))
                {
                    ReturnValue = _booleanExpressionSolver.Solve(codeLine);
                }
                else
                {
                    ReturnValue = _calculator.GetResult(codeLine);
                }
            }

            if (firstOperator == "if")
            {
                if (_conditionKeyWord.TryHandleCodeLine(_programText,ref _pc))
                {
                    
                }
            }
            
        }

        private void AddForLoopToStack(string codeLine)
        {
            var forKeyWord = _forKeyWordFabric.GetKeyWord(codeLine, _pc);
            _interpreterContext.Variables.Add(forKeyWord.NameOfCounter,forKeyWord.StartValue);
            _keyWordsStack.Push(forKeyWord);
        }

        private void DoLoopInteraction()
        {
            if (_keyWordsStack.Count == 0) throw new Exception("Некорректная закрывающаяся фигурная скобка");
            var forKeyWord = _keyWordsStack.Peek();
            DoAddExpression(forKeyWord);
            string condition = forKeyWord.Condition;//i<10
            bool exitConditionIsNotSatisfied = _booleanExpressionSolver.Solve(condition);//false
            if (exitConditionIsNotSatisfied)
            {
                _pc = forKeyWord.CodeLineNumber;

            }
            else
            {
                
                _interpreterContext.Variables.Remove(forKeyWord.NameOfCounter);
                _keyWordsStack.Pop();
            }
        }

        private void DoAddExpression(ForKeyWord forKeyWord)
        {
            string addedExpression = forKeyWord.AddedExpression;
            DoInstractionWithVariable(addedExpression + ';',typeof(Int32));
        }

        private void DoInstractionWithVariable(string codeLine, Type variableType)
        {
            string variableName = new string(codeLine.TakeWhile(c => c != '=').ToArray());
            variableName = variableName.Trim();
            if (!_interpreterContext.Variables.ContainsKey(variableName)) throw new Exception("Неизвестная переменная");
            string expression = new string(codeLine.SkipWhile(c => c != '=').ToArray());
            expression = expression.Trim().Remove(0,1).Trim();
            expression = expression.Remove(expression.Length - 1);
            expression = _expressionReplacer.ReplaceTokensInExpression(expression);
            object result = _calculator.GetResult(expression);
            _interpreterContext.Variables[variableName] = result;
        }

        private string RemoveSemicolon(string codeLine)
        {
            codeLine = codeLine.Trim();
            if (codeLine.Last() == ';')
                codeLine = codeLine.Remove(codeLine.Length - 1);
            return codeLine;
        }

        private bool ExpressionIsBool(string expression)
        {
            if (_vectorExpressionHelper.ExpressionHasVector(expression))
                return false;
            string pattern = @"(False|True|<|>|>=|<=|==|!=)";
            return Regex.IsMatch(expression, pattern);  
        }


        public Interpreter InitVariableParser(VariableParser variableParser)
        {
            _variableParser = variableParser;
            return this;
        }

        public Interpreter InitBooleanExpressionSolver(BooleanExpressionSolver booleanExpressionSolver)
        {
            _booleanExpressionSolver = booleanExpressionSolver;
            return this;
        }

        public Interpreter InitExpressionReplacer(ExpressionReplacer expressionReplacer)
        {
            _expressionReplacer = expressionReplacer;
            return this;
        }
        public Interpreter InitForKeyKodeFabric()
        {
            _forKeyWordFabric = new ForKeyWordFabric(_variableParser);
            return this;
        }

        public Interpreter InitConditionKeyWord()
        {
            _conditionKeyWord = new ConditionKeyWord(_booleanExpressionSolver);
            return this;
        }
        public Interpreter(InterpreterContext interpreterContext)
        {
            _interpreterContext = interpreterContext;
        }
    }
}