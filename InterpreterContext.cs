using System;
using System.Collections.Generic;
using System.Numerics;

namespace Lab5
{
    public class InterpreterContext
    {
        public List<string> SupportedVariableTypes { get; } = new List<string>(){"int", "string", "float", "bool","double","Vector2"};
        public Dictionary<string, object> Variables { get; } = new Dictionary<string, object>();
        public Dictionary<string, Type> SupportedVariableTypesMap = new Dictionary<string, Type>()
        {
            { typeof(int).Name, typeof(int) },
            { typeof(string).Name, typeof(string) },
            { typeof(float).Name, typeof(float) },
            { typeof(bool).Name, typeof(bool) },
            { typeof(double).Name, typeof(double) },
            { typeof(Math).Name, typeof(Math)}
        };
        public Dictionary<string, Func<object[], object>> SupportFunctions { get; }=
            new Dictionary<string, Func<object[], object>>()
            {
                {
                    "scan",
                    (a)=>Console.ReadLine()
                },
                {
                    "print",
                    delegate(object[] objects)
                    {
                        string summaryStr = "";
                        foreach (var obj in objects)
                        {
                            summaryStr += obj.ToString();
                        }
                        Console.WriteLine(summaryStr);
                        return null;
                    }
                },
                {
                    "inc",
                    delegate(object[] objects)
                    {
                        double a = Convert.ToInt32(objects[0]);
                        return a + 1;
                    }
                }
            };
    }
}