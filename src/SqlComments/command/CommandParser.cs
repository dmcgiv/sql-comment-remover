using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;

namespace McGiv.SqlUtil
{
	public class CommandParser
	{
		private readonly OrderedDictionary _requiredArgs = new OrderedDictionary();
		private TextWriter _errorOutput;
		private TextWriter _output;

		public CommandParser()
		{
			_output = Console.Out;
			_errorOutput = Console.Error;
		}

		public CommandParser RequiredArgument(string name, string description)
		{
			_requiredArgs.Add(name, new Argument {Name = name, Description = description});

			return this;
		}

		//public CommandParser OptionalArgument(string name, string description)
		//{
		//    return this;
		//}

		public CommandParser Option<T>(string command, string name, string description, T defaultValue)
		{
			return this;
		}


		public Commands Parse(string[] args)
		{
			var reqArgs = new Dictionary<string, string>();

			// required arguments
			if (_requiredArgs.Count > 0)
			{
				if (args.Length < _requiredArgs.Count)
				{
					// throw exception
				}

				
				for(int i=0; i<_requiredArgs.Count; i++)
				{
					var a = (Argument)_requiredArgs[i];
					reqArgs.Add(a.Name, args[i]);
				}
			}
			return new Commands(reqArgs);
		}

		public CommandParser Output(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			_output = writer;

			return this;
		}

		public CommandParser Error(TextWriter writer)
		{
			if (writer == null)
			{
				throw new ArgumentNullException("writer");
			}

			_errorOutput = writer;

			return this;
		}

		#region Nested type: Argument

		public class Argument
		{
			public string Description;
			public string Name;
		}

		#endregion

		#region Nested type: Commands

		public class Commands
		{
			private readonly Dictionary<string, string> _args;
			public Commands(Dictionary<string, string> args)
			{
				_args = args;
				IsValid = true;
			}


			public bool IsValid { get; private set; }

			public string GetArgument(string name)
			{
				string value;
				if (!_args.TryGetValue(name, out value))
				{
					return null;
				}

				return value;
			}

			public T GetOption<T>(string name)
			{
				return default(T);
			}
		}

		#endregion
	}
}