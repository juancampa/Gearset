//
// Xna Console
// www.codeplex.com/XnaConsole
// Copyright (c) 2008 Samuel Christie
//
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

#if !XBOX
using IronPython.Hosting;
using IronPython.Runtime.Exceptions;
using IronPython.Modules;
#endif
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace DebugConsole.Components
{
    /// <remarks>
    /// This class implements an interpreter using IronPython
    /// </remarks>
    public class PythonInterpreter : DebugComponent
    {
        const string Prompt = ">>> ";
        const string PromptCont = "... ";
        string multi;
        public XnaConsoleComponent Console;

#if !XBOX
        private ClrModule clr;
#endif
        #region Python execution stuff
#if !XBOX
        PythonEngine PythonEngine;
#endif
        MemoryStream PythonOutput;
        ASCIIEncoding ASCIIEncoder;

        #endregion

        /// <summary>
        /// Creates a new PythonInterpreter
        /// </summary>
        public PythonInterpreter()
        {
#if !XBOX
            this.PythonEngine = new PythonEngine();
            this.PythonOutput = new MemoryStream();
            this.PythonEngine.SetStandardOutput(PythonOutput);
            this.ASCIIEncoder = new ASCIIEncoding();


            clr = this.PythonEngine.Import("clr") as ClrModule;
            clr.AddReference("Microsoft.Xna.Framework");
            clr.AddReference("Microsoft.Xna.Framework.Game");

            this.PythonEngine.Execute("from Microsoft.Xna.Framework import *");
            this.PythonEngine.Execute("from Microsoft.Xna.Framework.Graphics import *");
            this.PythonEngine.Execute("from Microsoft.Xna.Framework.Content import *");
            //this.PythonEngine.Execute("from System import *");
            
            multi = "";

            Console = new XnaConsoleComponent();
            XdtkResources.Console.Components.Add(Console);
            Console.Prompt(Prompt, Execute);
            AddGlobal("Console", Console);
            AddGlobal("dc", XdtkResources.Console);
            AddGlobal("game", XdtkResources.Game);

            Execute(
@"class Tracer(GameComponent):
    def Update(self, gameTime):
        dc.Show(self.k, eval(self.v))
");
            Execute(
@"def trace(key, value):
    a = Tracer(game)
    a.k = key
    a.v = value
    game.Components.Add(a)
");
#endif
        }

        /// <summary>
        /// Get string output from IronPythons MemoryStream standard out
        /// </summary>
        /// <returns></returns>
        private string getOutput()
        {
#if XBOX
            return null;
#else
            byte[] statementOutput = new byte[PythonOutput.Length];
            PythonOutput.Position = 0;
            PythonOutput.Read(statementOutput, 0, (int)PythonOutput.Length);
            PythonOutput.Position = 0;
            PythonOutput.SetLength(0);
            
            return ASCIIEncoder.GetString(statementOutput);
#endif
        }

        /// <summary>
        /// Executes python commands from the console.
        /// </summary>
        /// <param name="input"></param>
        /// <returns>Returns the execution results or error messages.</returns>
        public void Execute(string input)
        {
#if !XBOX
            try
            {
                if ((input != "") && ((input[input.Length - 1].ToString() == ":") || (multi != ""))) //multiline block incomplete, ask for more
                {
                    multi += input + "\n";
                    Console.Prompt(PromptCont, Execute);
                }
                else if (multi != "" && input == "") //execute the multiline code after block is finished
                {
                    string temp = multi; // make sure that multi is cleared, even if it returns an error
                    multi = "";
                    PythonEngine.ExecuteToConsole(temp);
                    Console.WriteLine(getOutput());
                    Console.Prompt(Prompt, Execute);
                }
                else // if (multi == "" && input != "") execute single line expressions or statements
                {
                    PythonEngine.ExecuteToConsole(input);
                    Console.WriteLine(Console.Chomp(getOutput()));
                    Console.Prompt(Prompt, Execute);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                Console.Prompt(Prompt, Execute);
            }
#endif

        }

        /// <summary>
        /// Adds a global variable to the environment of the interpreter.
        /// </summary>
        public void AddGlobal(string name, object value)
        {
#if !XBOX
            try
            {
                PythonEngine.Globals.Add(name, value);
            }
            catch (ArgumentException)
            {
                // The item was already in the globals.
                PythonEngine.Globals.Remove(name);
                PythonEngine.Globals.Add(name, value);
            }
            
#endif
        }

        /// <summary>
        /// Adds a reference to an assembly
        /// </summary>
        /// <param name="name">The name of the assembly</param>
        public void AddReference(string name)
        {
#if !XBOX
            clr.AddReference(name);
#endif
        }
    }
}
