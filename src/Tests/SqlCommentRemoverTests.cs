using System;
using McGiv.SqlUtil;
using NUnit.Framework;

namespace Tests
{
	[TestFixture]
	public class SqlCommentRemoverTests
	{


		[TestCase("''", "''")]
		[TestCase("a''", "a''")]
		[TestCase("''a", "''a")]
		[TestCase("a''b", "a''b")]
		public void Strings(string sql, string expected)
		{
			Test(sql, expected);
		}

		[TestCase("", "")]
		[TestCase("@", "@")]
		[TestCase("@@", "@@")]
		[TestCase("@@@", "@@@")]
		public void MulitLines(string sql, string expected)
		{
			Test(sql, expected);
		}

		// replaces @ with new line to make tests more readable
		[TestCase("/**/", "")]
		[TestCase("/*@xxx@*/", "")]
		[TestCase("a/**/", "a")]
		[TestCase("/**/a", "a")] 
		[TestCase("a/**/b", "a b")]
		[TestCase("@/**/", "@")]
		[TestCase("@a/**/", "@a")]
		[TestCase("@/**/a", "@a")]
		[TestCase("@a/**/b", "@a b")]
		[TestCase("/**/@", "@")]
		[TestCase("a/**/@", "a@")]
		[TestCase("/**/a@", "a@")]
		[TestCase("a/**/b@", "a b@")]
		[TestCase("@/**/@", "@@")]
		[TestCase("@a/**/@", "@a@")]
		[TestCase("@/**/a@", "@a@")]
		[TestCase("@a/**/b@", "@a b@")]
		[TestCase("@/*@*/@", "@@")]
		[TestCase("@a/*@*/@", "@a@")]
		[TestCase("@/*@*/a@", "@a@")]
		[TestCase("@a/*@*/b@", "@a b@")]

		[TestCase("/*/*/**/*//*/**/*/*/", "")]
		[TestCase("/*/**/*/", "")]

		[TestCase("/**/@a", "a")]
		public void RemoveMultiLineComment(string sql, string expected)
		{
			Test(sql, expected);
		}


		// valid
		[TestCase("--a", "")]
		[TestCase("a--b", "a")]
		// invalid 
		[TestCase("-a", "-a")]
		[TestCase(" -a", " -a")]

		[TestCase("--@--@--", "")]
		public void RemoveSingleLineComments(string sql, string expected)
		{
			//string output = Helper.CommentRemover(sql,
			//                                      new SqlCommentRemoverv4.Settings {RemoveCommentsFromMultiLineStrings = true});

			//return output;

			Test(sql, expected);
		}


		[TestCase("'--'", "''")]
		[TestCase("'a--'", "'a'")]
		[TestCase("'a--b'", "'a'")]

		[TestCase("'--@--@--'", "''")]
		public void RemoveSingleLineCommentWithinString(string sql, string expected)
		{
			Test(sql, expected);
		}







		[TestCase("'/**/'", "' '")]
		[TestCase("a'/**/'", "a' '")]
		[TestCase("'/**/'a", "' 'a")]
		[TestCase("'/*a*/'", "' '")]
		[TestCase("'a/**/'", "'a '")]
		[TestCase("'/**/a'", "' a'")]
		[TestCase("'a/**/b'", "'a b'")]


		[TestCase("'/*@xxx@*/'", "' '")]
		[TestCase("'a/*@xxx@*/'", "'a '")]
		[TestCase("'/*@xxx@*/a'", "' a'")]
		[TestCase("'a/*@xxx@*/b'", "'a b'")]

		[TestCase("'/*@xxx@*/@'", "' @'")]
		[TestCase("'@/*@xxx@*/'", "'@ '")]
		[TestCase("'@/*@xxx@*/@'", "'@ @'")]


		[TestCase("a'/**/'", "a' '")]
		[TestCase("'/**/'a", "' 'a")]
		[TestCase("'/*a*/'", "' '")]
		[TestCase("'a/**/'", "'a '")]
		[TestCase("'/**/a'", "' a'")]
		[TestCase("'a/**/b'", "'a b'")]

		[TestCase("'/**/a/**/b/**/'", "' a b '")]

		// multiple inner comments
		[TestCase("'/*/**/*/'", "' '")]
		[TestCase("'/*/*/**/*/*/'", "' '")]

		[TestCase("'/*/*/**/*//*/**/*/*/'", "' '")]
		[TestCase("'/*/**/*/'", "' '")]
		public void RemoveMultiLineCommentWithinString(string sql, string expected)
		{
			Test(sql, expected);

			
		}


		/// <summary>
		/// Replaces the @ character with new lines
		/// @ is used to make the test cases more readable.
		/// </summary>
		/// <param name="sql">The SQL to convert</param>
		/// <param name="expected"></param>
		private static void Test(string sql, string expected)
		{
			

			string toConvert = sql.Replace("@", Environment.NewLine);

			string result = Helper.CommentRemover(toConvert,
			                                      new SqlCommentRemoverv4.Settings {RemoveCommentsFromMultiLineStrings = true});
			result = result.Replace(Environment.NewLine, "@");

			if(expected != result)
			{
				result = result + '@';
			}
			Assert.AreEqual(expected, result, "origional = " + sql);
		}



	}
}