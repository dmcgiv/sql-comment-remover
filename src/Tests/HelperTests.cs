using McGiv.SqlUtil;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class HelperTests
	{
		const char StringStart = '\'';

		[TestCase("''", 1, StringStart, Result = 1)]
		[TestCase("'' ", 1, StringStart, Result = 1)]
		[TestCase("'", 1, StringStart, Result = -1)]
		[TestCase("' ", 1, StringStart, Result = -1)]
		[TestCase("'a23''678'", 1, StringStart, Result = 9)]
		[TestCase("'b23''678' ", 1, StringStart, Result = 9)]
		[TestCase("'''3''''8' ", 1, StringStart, Result = 9)]
		[TestCase("'''", 1, StringStart, Result = -1)]
		public int FindEndOfString(string line, int start, char stringChar)
		{
			return Helper.FindStringEndIndex(line, start, stringChar);
		}


		[TestCase("/**/ ", 2, Result = 3)]
		[TestCase("/*", 2, Result = -1)]
		[TestCase("/**/", 2, Result = 3)]
		[TestCase("/* ", 2, Result = -1)]

		[TestCase("*/", 0, Result = 1)]
		[TestCase("*/ ", 0, Result = 1)]
		[TestCase(" */ ", 0, Result = 2)]
		[TestCase(" */ ", 1, Result = 2)]
		[TestCase(" */ ", 2, Result = -1)]


		[TestCase("*", 0, Result=-1)]
		[TestCase(" *", 0, Result = -1)]
		[TestCase(" * ", 0, Result = -1)]
		public int FindEndOfMultiLineComment(string line, int start)
		{
			return Helper.FindEndOfMultiLineComment(line, start);
		}


		[TestCase("--", 0, Result=0)]
		[TestCase("", 0, Result = -1)]
		[TestCase(" --", 0, Result = 1)]

		[TestCase("--", 1, Result = -1)]
		[TestCase("", 0, Result = -1)]
		[TestCase(" --", 1, Result = 1)]

		[TestCase(" -- ", 2, Result = -1)]


		[TestCase("-", 0, Result = -1)]
		[TestCase(" -", 0, Result = -1)]
		[TestCase(" - ", 0, Result = -1)]
		public int FindStartOfSingleLineComment(string line, int start)
		{
			return Helper.FindStartOfSingleLineComment(line, start);
		}


		[TestCase("/*", 0, Result = 0)]
		[TestCase("/*", 1, Result = -1)]

		[TestCase(" /*", 0, Result = 1)]
		[TestCase(" /*", 1, Result = 1)]
		[TestCase(" /*", 2, Result = -1)]


		[TestCase("/", 0, Result = -1)]
		[TestCase(" /", 0, Result = -1)]
		[TestCase(" / ", 0, Result = -1)]
		public int FindStartOfMultiLineComment(string line, int start)
		{
			return Helper.FindStartOfMultiLineComment(line, start);
		}
	}
}
