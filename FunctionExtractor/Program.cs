using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace FunctionExtractor
{
    /// <summary>
    /// Extracts all function wrappers from the GearConsole file so
    /// it can be used in dummy assemblies and in the wrapper class.
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            String contents = new StreamReader("GearConsole.cs").ReadToEnd();
            String template = new StreamReader("Debug.Template.cs").ReadToEnd();

            int startIndex = contents.IndexOf("// WRAPPER FUNCTIONS BEGIN") +"// WRAPPER FUNCTIONS BEGIN".Length;
            int endIndex = contents.IndexOf("// WRAPPER FUNCTIONS END");
            contents = contents.Substring(startIndex , endIndex - startIndex);

            String result = String.Empty;

            MatchCollection matches = Regex.Matches(contents, "((?: */// .*\\n)+) *public (.*)");
            foreach (Match match in matches)
            {
                String comment = match.Groups[1].Captures[0].Value;
                String signature = match.Groups[2].Captures[0].Value;
                String paramStr = Regex.Matches(signature, ".*\\((.*)\\)")[0].Groups[1].ToString();
                String[] parameters = paramStr.Split(',');
                String indentation = new String(' ', 4);
                String methodName = signature.TrimStart(' ').Split('(')[0].Split(' ').Last();
                String passOfParameters = Regex.Replace(paramStr, "((?:ref )?)(?:[a-zA-Z0-9_]+ )([a-zA-Z0-9_]+,?)", "$1$2");
                String methodCall = "Console." + methodName + "(" + passOfParameters + ")";

                result += comment;
                result += Indentation(2) + "[Conditional(\"USE_GEARSET\")]\n";
                result += Indentation(2) + "public static " + signature + "\n";
                result += Indentation(2) + "{\n";
                result += Indentation(3) + "if (SameThread())\n";
                result += Indentation(4) + methodCall + ";\n";
                result += Indentation(3) + "else\n";
                if (paramStr.Contains("ref "))
                    result += Indentation(4) + "// TODO: CACHE THE REF PARAMETERS\n";
                if (paramStr.Contains("params "))
                    result += Indentation(4) + "// TODO: FIX THE \"PARAMS\" PARAMETER(S)\n";
                result += Indentation(4) + "EnqueueAction(new Action(() => " + methodCall + "));\n";
                result += Indentation(2) + "}\n\n";
            }

            result = template.Replace("// FUNCTION WRAPPERS PLACEHOLDER", result);

            var outputFile = new StreamWriter("Debug.cs");
            outputFile.Write(result);
            outputFile.Close();
            Console.WriteLine("Output written to Debug.cs");
            Console.WriteLine("Remember to check for stuff marked with TODO");
            Console.ReadLine();
        }

        static String Indentation(int count)
        {
            String result = String.Empty;
            for (int i = 0; i < count; i++)
            {
                result += "    ";
            }
            return result;
        }
    }
}
