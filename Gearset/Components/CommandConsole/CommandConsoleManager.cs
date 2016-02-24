using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Gearset.UserInterface;

namespace Gearset.Components.CommandConsole
{
    public class CommandConsoleManager : Gear
    {
        public CommandConsoleConfig Config { get { return GearsetSettings.Instance.CommandConsoleConfig; } }

        readonly IUserInterface _userInterface;

        /// <summary>
        /// Message types for debug command.
        /// </summary>
        public enum DebugCommandMessage
        {
            Subtle = 0,
            Standard = 1,
            Warning = 2,  
            Error = 3
        }

        class CommandInfo
        {
            public readonly string Command;
            public readonly string Description;
            public readonly Action<CommandConsoleManager, string, IList<string>> Action;

            public CommandInfo(string command, string description, Action<CommandConsoleManager, string, IList<string>> callback)
            {
                Command = command;
                Description = description;
                Action = callback;
            }
        }

        readonly Dictionary<string, CommandInfo> _commandTable = new Dictionary<string, CommandInfo>();

        // Command history buffer.
        const int MaxCommandHistory = 32;
        private readonly List<string> _commandHistory = new List<string>();
        private int _commandHistoryIndex;

        public CommandConsoleManager(IUserInterface userInterface) : base(GearsetSettings.Instance.CommandConsoleConfig)
        {
            _userInterface = userInterface;
            _userInterface.CreateCommandConsole(Config);
            _userInterface.ExecuteCommand += (sender, args) =>
            {
                ExecuteCommand(args.Command);
            };

            RegisterCommand("help", "Show Command helps", (host, command, args) =>
            {
                int maxLen = 0;
                foreach (CommandInfo cmd in _commandTable.Values)
                    maxLen = Math.Max(maxLen, cmd.Command.Length);

                string fmt = $"{{0,-{maxLen}}}    {{1}}";

                foreach (CommandInfo cmd in _commandTable.Values)
                {
                    Echo(String.Format(fmt, cmd.Command, cmd.Description));
                }
            });

            // Clear screen command
            RegisterCommand("cls", "Clear Screen", (host, command, args) =>
            {
                ClearOutput();
            });

            // Echo command
            RegisterCommand("echo", "Display Messages", (host, command, args) =>
            {
                Echo(command.Substring(5));
            });
        }

        public void ClearOutput()
        {
            _userInterface.ClearCommandOutput();
        }

        public void RegisterCommand(string name, string description, Action<CommandConsoleManager, string, IList<string>> action)
        {
            var lowerCommand = name.ToLower();
            if (_commandTable.ContainsKey(lowerCommand))
            {
                throw new InvalidOperationException($"Command \"{name}\" is already registered.");
            }

            _commandTable.Add(lowerCommand, new CommandInfo(name, description, action));
        }

        public void Echo(string text)
        {
            Echo(DebugCommandMessage.Standard, text);
        }

        public void Echo(DebugCommandMessage messageType, string text)
        {
            _userInterface.EchoCommand(messageType, text);
        }

        void EchoSubtle(string text)
        {
            Echo(DebugCommandMessage.Subtle, text);
        }

        public void EchoWarning(string text)
        {
            Echo(DebugCommandMessage.Warning, text);
        }

        public void EchoError(string text)
        {
            Echo(DebugCommandMessage.Error, text);
        }

        public void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command))
                return;

            // Run the command.
            var spaceChars = new[] { ' ' };

            EchoSubtle($"Executing command> {command}");

            command = command.TrimStart(spaceChars);

            //Previously split arguments on spaces but we want to be able to do
            //command arg1 "some stuff" arg3
            //var args = new List<string>(command.Split(spaceChars));

            const RegexOptions options = RegexOptions.None;
            var regex = new Regex(@"((""((?<token>.*?)(?<!\\)"")|(?<token>[\w]+))(\s)*)", options);
            var input = command;
            var result = (regex.Matches(input).Cast<Match>().Where(m => m.Groups["token"].Success).Select(m => m.Groups["token"].Value)).ToList();

            //for (var i = 0; i < result.Count(); i++)
            //    Debug.WriteLine($"Token[{i}]: '{result[i]}'");

            var args = result.ToList();

            var cmdText = args[0];
            args.RemoveAt(0);

            CommandInfo cmd;
            if (_commandTable.TryGetValue(cmdText.ToLower(), out cmd))
            {
                try
                {
                    // Call registered command delegate.
                    cmd.Action(this, command, args);
                }
                catch (Exception ex)
                {
                    // Exception occurred while running command.
                    EchoError("Unhandled Exception occurred");

                    var lines = ex.Message.Split('\n');
                    foreach (var line in lines)
                        EchoError(line);
                }
            }
            else
            {
                EchoWarning("Unknown Command");
            }

            // Add to command history.
            if (_commandHistory.Count == 0 || command != _commandHistory[_commandHistory.Count - 1])
            {
                _commandHistory.Add(command);
                while (_commandHistory.Count > MaxCommandHistory)
                    _commandHistory.RemoveAt(0);

                _commandHistoryIndex = _commandHistory.Count;
            }
        }

        internal string PreviousCommand()
        {
            if (_commandHistory.Count <= 0)
                return string.Empty;

            _commandHistoryIndex = Math.Max(0, _commandHistoryIndex - 1);

            return _commandHistory[_commandHistoryIndex];
        }

        internal string NextCommand()
        {
            if (_commandHistory.Count <= 0)
                return string.Empty;

            if (_commandHistoryIndex == _commandHistory.Count)
                return string.Empty;

            if (_commandHistoryIndex == _commandHistory.Count - 1)
                return string.Empty;

            _commandHistoryIndex = Math.Min(_commandHistory.Count - 1, _commandHistoryIndex + 1);
            return _commandHistory[_commandHistoryIndex];
        }

        protected override void OnVisibleChanged()
        {
            _userInterface.CommandConsoleVisible = Visible;
        }
    }
}
