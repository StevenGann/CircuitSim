using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSim
{
    internal static class CSDL
    {
        public static string[] Keywords = { "module", "input", "output", "wire" };
        public static char[] SpecialCharacters = { '(', ')', '{', '}', '[', ']', ':', ';', '=', '-', '+', '\\', '/', '-', '~', '!', '^', '&', '|', '*', '$', '@', '#', '%', '<', '>', '?', '"', '\'', '`', ',', '.', };
        public static char[] WhitespaceCharacters = { ' ', '\n', '\r', '\t' };

        public static void Parse(string Code)
        {
            stateStack.Clear();
            stateStack.Push(ParserStates.Root);
            string code = Scrub(Code);
            while (code.Length > 0)
            {
                string next = NextSymbol(code);
                code = code.Trim().Remove(0, next.Length);
                symbols.Enqueue(next);
            }

            while (symbols.Count > 0)
            {
                ParseSymbol(symbols.Dequeue());
            }
        }

        private static Queue<string> symbols = new Queue<string>();
        private static Stack<ParserStates> stateStack = new Stack<ParserStates>();
        private static List<string> Modules = new List<string>();
        private static List<string> Wires = new List<string>();

        private static void ParseSymbol(string Symbol)
        {
            Console.Write(Symbol + "\t");
            ParserStates state = stateStack.Peek();

            if (state == ParserStates.Root) //Root of the document
            {
                if (Symbol == "module")
                {
                    Console.Write("Defining module");
                    stateStack.Push(ParserStates.Module);
                    stateStack.Push(ParserStates.ModuleSignature);
                }
                else
                {
                    Console.WriteLine("Error: only modules can be defined on the document root");
                }
            }
            else if (state == ParserStates.Module)
            {
                if (Symbol == "wire")
                {
                    stateStack.Push(ParserStates.WireDeclaration);
                }
                else if (Symbol == "}")
                {
                    Console.Write("End Module");
                    stateStack.Pop();
                }
                else
                {
                    Console.Write($"Assigning to {Symbol}");
                    stateStack.Push(ParserStates.Assignment);
                }
            }
            else if (state == ParserStates.Assignment)
            {
                if (Symbol == ";")
                {
                    stateStack.Pop();
                }
                else if (Symbol == "[")
                {
                    stateStack.Push(ParserStates.BusAssignment);
                }
            }
            else if (state == ParserStates.BusAssignment)
            {
                if (Symbol == "]")
                {
                    stateStack.Pop();
                }
            }
            else if (state == ParserStates.ModuleSignature)
            {
                if (Symbol == "input")
                {
                    stateStack.Push(ParserStates.InputDeclaration);
                }
                else if (Symbol == "output")
                {
                    stateStack.Push(ParserStates.OutputDeclaration);
                }
                else if (Symbol == "{")
                {
                    stateStack.Pop();
                }
                else if (Symbol == ")")
                {
                    stateStack.Pop();
                }
                else if (Symbol == "(")
                {
                }
                else
                {
                    Modules.Add(Symbol);
                    Console.Write($"Module named {Symbol}");
                }
            }
            else if (state == ParserStates.InputDeclaration
                || state == ParserStates.OutputDeclaration
                || state == ParserStates.WireDeclaration)
            {
                /*if (Symbol == ";" || Symbol == "," || Symbol == ")")
                {
                    stateStack.Pop();
                }
                else
                {
                    Wires.Add(Symbol);
                    Console.Write($"Wire named {Symbol}");
                }*/

                if (Symbol == "[")
                {
                    stateStack.Push(ParserStates.BusDeclaration);
                }
                else
                {
                    Wires.Add(Symbol);
                    Console.Write($"Wire named {Symbol}");
                    stateStack.Pop();
                }
            }
            else if (state == ParserStates.BusDeclaration)
            {
                if (Symbol == "]")
                {
                    stateStack.Pop();
                }
            }

            Console.CursorLeft = 32;
            Console.Write(stateStack.Peek().ToString());

            Console.WriteLine();
        }

        private enum ParserStates
        {
            Root,
            Module,
            ModuleSignature,
            InputDeclaration,
            OutputDeclaration,
            WireDeclaration,
            BusDeclaration,
            Assignment,
            BusAssignment,
            ModuleReference,
            ModuleInput,
            ModuleOutput
        }

        private static string NextSymbol(string Code)
        {
            char[] chars = Code.Trim().ToCharArray();
            int index = 0;

            while (WhitespaceCharacters.Contains(chars[index])) { index++; }

            if (SpecialCharacters.Contains(chars[index]))
            {
                return chars[0].ToString();
            }

            string result = "";
            while (!SpecialCharacters.Contains(chars[index]) && !WhitespaceCharacters.Contains(chars[index]))
            {
                result += chars[index];
                index++;
            }

            return result;
        }

        public static string Scrub(string Code)
        {
            string[] lines = Code.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            bool inBlockComment = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (!inBlockComment)
                {
                    if (line.Contains("/*"))
                    {
                        line = line.Substring(0, line.IndexOf("/*"));
                        inBlockComment = true;
                    }

                    if (line.Contains("//"))
                    {
                        line = line.Substring(0, line.IndexOf("//"));
                    }
                }
                else
                {
                    if (line.Contains("*/"))
                    {
                        inBlockComment = false;
                        int index = line.IndexOf("*/") + 2;
                        line = line.Substring(index, line.Length - index);
                    }
                    else
                    {
                        line = "";
                    }
                }

                lines[i] = line;
            }

            string result = string.Join(' ', lines);
            result = result.Replace("\r", " ");
            result = result.Replace("\t", " ");
            int length;
            do
            {
                length = result.Length;
                result = result.Replace("  ", " ");
            }
            while (length != result.Length);

            return result;
        }
    }
}