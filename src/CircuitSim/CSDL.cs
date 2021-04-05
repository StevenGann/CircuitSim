using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CircuitSim
{
    internal static class CSDL
    {
        public static void IdentifyModules(string Code)
        {
            if (Code.IndexOf("")
        }

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

            return result;
        }
    }
}