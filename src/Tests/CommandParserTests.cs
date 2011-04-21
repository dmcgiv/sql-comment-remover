using McGiv.SqlUtil;
using NUnit.Framework;

namespace Tests
{

	[TestFixture]
	public class CommandParserTests
	{
		[Test]
		public void SingleArgument()
		{
			var args = new []{@"C:\"}
			;

			var cmd = new CommandParser().RequiredArgument("input", "the description for input").Parse(args);


			Assert.IsTrue(cmd.IsValid);
			var arg = cmd.GetArgument("input");


			Assert.AreEqual(@"C:\", arg);

		}
	}
}
