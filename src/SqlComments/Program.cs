

using System;
using System.IO;

namespace McGiv.SqlUtil
{
	public class Program
	{
		public static void Main(string[] args)
		{

			//var commands = new CommandParser()
			//    .RequiredArgument("input", "Either a file or directory.")
			//    .RequiredArgument("output", "File or directory.")
			//    .Option("/p", "filename postfix", "Postfixes the value onto output files.", string.Empty)
			//    .Option("/-y", "auto confirm", "Answer yes to any prompts to overwrite a file.", false)
			//    .Parse(args);


			
			//string inputCmd = commands.GetArgument("input");

			// usage
			//
			// [tool] [input] [output] 
			//
			// options
			//
			//
			// /overwiter
			//

			try
			{
				if (args.Length != 2)
				{
					Help();
					return;
				}

				string inputPath = args[0];
				string outputPath = args[1];


				//using (var input = File.OpenRead(inputPath))
				//{
				//    using (var output = File.OpenWrite(outputPath))
				//    {
				//        SqlCommentRemoverOrig.CommentRemover(input, output);
				//    }
				//}




				//using (var input = File.OpenRead(inputPath))
				//{
				//    using (var output = File.OpenWrite(outputPath))
				//    {
				//        SqlCommentRemover.CommentRemover(input, output);
				//    }
				//}


				using (var input = File.OpenRead(inputPath))
				{
					using (var output = File.CreateText(outputPath))
					{
						var settings = new SqlCommentRemoverv4.Settings
						               	{
						               		RemoveCommentsFromMultiLineStrings = true
											//, Debug = true
						               	};

						SqlCommentRemoverv4.CommentRemover(input, output, settings);
					}
				}
			}
			catch(Exception e)
			{
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				Console.Read();
			}

		}

		static void Help()
		{
			Console.WriteLine("usage:");
			Console.WriteLine("sqlutil [input] [output]");
		}
	}
}
