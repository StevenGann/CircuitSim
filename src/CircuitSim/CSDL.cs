using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSim
{
    internal static class CSDL
    {
        public static ModuleStructure[] IdentifyModules(string Code)
        {
            string[] lines = Code.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            int moduleIndex = -1;
            List<string> moduleLines = new List<string>();
            Stack<int> scope = new Stack<int>();
            int moduleDepth = 0; ;
            List<ModuleStructure> modules = new List<ModuleStructure>();

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains("module"))
                {
                    moduleIndex = i;
                    moduleDepth = scope.Count;
                }

                if (line.Contains('{')) { scope.Push(i); }

                if (moduleIndex >= 0)
                {
                    moduleLines.Add(line);
                }

                if (line.Contains('}') && lines[scope.Peek()].Contains('{'))
                {
                    scope.Pop();

                    if (scope.Count == moduleDepth)
                    {
                        moduleIndex = -1;
                        ModuleStructure module = new ModuleStructure();
                        module.Code = Join(moduleLines.ToArray());
                        modules.Add(module);
                        moduleLines.Clear();
                    }
                }
            }

            return modules.ToArray();
        }

        public static WireStructure[] IdentifyWires(string Code)
        {
            string[] lines = Code.Split('\n', StringSplitOptions.RemoveEmptyEntries);
            bool inSignature = false;
            bool inBody = false;
            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i];

                if (line.Contains('(') && !inBody)
                {
                    inSignature = true;
                }

                if (inSignature)
                {
                    while (line.IndexOf("input") != -1)
                    {
                    }
                }

                if (line.Contains(')') && !inBody)
                {
                    inSignature = false;
                }

                if (line.Contains('{'))
                {
                    inSignature = false;
                    inBody = true;
                }

                if (inBody)
                {
                }

                if (line.Contains('}'))
                {
                    inBody = false;
                }
            }

            return null;
        }

        private static Tuple<string, string>

        public static string FilterComments(string Code)
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

            return Join(lines);
        }

        public static string Join(string[] Lines)
        {
            string result = "";
            for (int i = 0; i < Lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(Lines[i]))
                {
                    result += Lines[i] + '\n';
                }
            }

            return result.Replace("\r", "").Trim();
        }

        public struct ModuleStructure
        {
            public string Code;
            public string Body;
            public string Name;
            public WireStructure[] Wires;

            public override string ToString()
            {
                return Name;
            }

            public static void UpdateFromCode(ref ModuleStructure Module)
            {
            }
        }

        public struct WireStructure
        {
            public string Name;
            public WireType Type;
        }

        private enum WireType
        {
            Neutral,
            Input,
            Output
        }
    }
}