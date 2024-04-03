using System;
using System.Linq;
using System.Reflection;

namespace Lab5
{
    public class MemberReplacer
    {
        private InterpreterContext _interpreterContext;
        private ReplacerHelper _replacerHelper = new ReplacerHelper();
        
        public string ReplaceMembers(string expression)
        {
            expression = FieldsReplace(expression);
            return expression;
        }
        private string FieldsReplace(string expression)
        {
            string token = "";
            int countOfDots = expression.Count(c => c.Equals('.'));
            int dotIndex = 0;
            for (int i = 0; i < countOfDots; i++)
            {
                dotIndex = expression.IndexOf('.',dotIndex);
                string entityStr = GetWordByDotSide(dotIndex, expression, true);
                string fieldName = GetWordByDotSide(dotIndex, expression, false);
                if (TryParseEntity(entityStr, out object entityObj))
                {
                    FieldInfo propertyInfo = entityObj.GetType().GetField(fieldName);
                    object value = propertyInfo.GetValue(entityObj);
                    expression = _replacerHelper.ReplaceVariableWithValue(expression, entityStr + "." + fieldName, value, dotIndex-entityStr.Length);
                }
                dotIndex++;
            }
            return expression;
        }

        private string GetWordByDotSide(int dotIndex, string expression, bool takeWordOnLeft)
        {
            string word = "";
            if (takeWordOnLeft)
            {
                for (int i = dotIndex-1; i >= 0; i--)
                {
                    if (!char.IsLetter(expression[i]) && !char.IsDigit(expression[i]))
                    {
                        break;
                    }
                    word += expression[i];
                    
                }
            }
            else
            {
                for (int i = dotIndex+1; i < expression.Length; i++)
                {
                    if (!char.IsLetter(expression[i]) && !char.IsDigit(expression[i]))
                        break;
                    word += expression[i];
                }
            }

            if (takeWordOnLeft) word = new string(word.Reverse().ToArray());
            return word;
        }

        private bool TryParseEntity(string entity, out object result)
        {
            result = null;
            if (_interpreterContext.Variables.TryGetValue(entity, out object value))
            {
                result = value;
            } 
            if (_interpreterContext.SupportedVariableTypesMap.TryGetValue(entity, out Type type))
            {
                result = type;
            }

            if (result != null) 
                return true;
            return false;
        }

        public MemberReplacer(InterpreterContext interpreterContext)
        {
            _interpreterContext = interpreterContext;
        }
        
    }
}