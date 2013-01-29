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
        enum OutputType
        {
            /// <summary>
            /// Gearset wrapper class.
            /// </summary>
            GSFile,
            /// <summary>
            /// Dummies for Xbox and WP
            /// </summary>
            DummyGearConsole,
        }

        /// <summary>
        /// Determines what the tool will output
        /// </summary>
        private static OutputType Type { get { return OutputType.DummyGearConsole; } }

        static void Main(string[] args)
        {
            String contents = new StreamReader("GearConsole.cs").ReadToEnd();

            String template = String.Empty;
            String outputFilename = String.Empty;
            switch (Type)
            {
                case OutputType.GSFile: 
                    template = new StreamReader("GS.Template.cs").ReadToEnd();
                    outputFilename = "GS.cs";
                    break;
                case OutputType.DummyGearConsole: 
                    template = new StreamReader("GearConsoleDummy.Template.cs").ReadToEnd();
                    outputFilename = "GearConsole (Dummy).cs";
                    break;
            }

            int startIndex = contents.IndexOf("// WRAPPER FUNCTIONS BEGIN") +"// WRAPPER FUNCTIONS BEGIN".Length;
            int endIndex = contents.IndexOf("// WRAPPER FUNCTIONS END");
            contents = contents.Substring(startIndex , endIndex - startIndex);

            String result = String.Empty;

            MatchCollection matches = Regex.Matches(contents, "((?: */// .*\\n)+) *public (.*)");
            foreach (Match match in matches)
            {
                String comment = match.Groups[1].Captures[0].Value;
                String signature = match.Groups[2].Captures[0].Value.TrimEnd('\r', '\n');
                String paramStr = Regex.Matches(signature, ".*\\((.*)\\)")[0].Groups[1].ToString();
                String[] parameters = paramStr.Split(',');
                String indentation = new String(' ', 4);
                String methodName = signature.TrimStart(' ').Split('(')[0].Split(' ').Last();
                String passOfParameters = Regex.Replace(paramStr, "((?:ref )?)(?:[a-zA-Z0-9_]+ )([a-zA-Z0-9_]+,?)", "$1$2");
                String methodCall = "Console." + methodName + "(" + passOfParameters + ")";

                switch (Type)
                {
                    case OutputType.GSFile:
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
                        break;
                    case OutputType.DummyGearConsole:
                        result += Indentation(2) + signature + " { }\n";
                        break;
                }
                
            }

            result = template.Replace("// FUNCTION WRAPPERS PLACEHOLDER", result);

            // Write the output.
            var outputFile = new StreamWriter(outputFilename);
            outputFile.Write(result);
            outputFile.Close();
            Console.WriteLine("Output written to " + outputFilename);
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
