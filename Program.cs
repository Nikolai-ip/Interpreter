using System;
using System.IO;

namespace Lab5
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            string programText = File.ReadAllText("Data/Program2.txt");
            programText = programText.Replace("\r", "");
            var interpreter = InterpreterFabric.GetInstance();
            interpreter.Interpret(programText);
            Console.WriteLine(interpreter.ReturnValue);
        }
    }
}