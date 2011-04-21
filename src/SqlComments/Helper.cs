
using System.IO;


namespace McGiv.SqlUtil
{
	public static class Helper
	{


		/// <summary>
		/// Searches for the index of the end of a string.
		/// </summary>
		/// <param name="line">The line of text to search.</param>
		/// <param name="stringStartIndex">The index to start the search.</param>
		/// <param name="stringChar">The character that indicats the end of the string.</param>
		/// <returns>The index of the end of the string or -1 if not found.</returns>
		public static int FindStringEndIndex(string line, int stringStartIndex, char stringChar)
		{
			int len = line.Length;
		restart:
			int end = line.IndexOf(stringChar, stringStartIndex/* + 1*/);

			if (end == -1 )
			{
				return -1;
			}

			// to encode the character that starts the string ' or " within the string
			// simply place the same chracter next to it.
			// check that the chracter found isn't just a quote that is being encoded.
			if ((end < len - 1) && line[end + 1] == stringChar)
			{
				if(end == len -2)
				{
					// at end of line cannot restart
					return -1;
				}
				stringStartIndex = end + 2;
				goto restart;
			}

			return end;

		}




				/// <summary>
		/// Searches for the end of a multi line comment '*/'  (excluding quotes).
		/// </summary>
		/// <param name="line">The line of text to search.</param>
		/// <param name="startIndex">The index to start the search.</param>
		/// <returns>The index of the last charcater of the comment or -1 if not found.</returns>
		public static int FindStartOfEndOfMultiLineComment(string line, int startIndex)
				{
					return FindIndex(line, startIndex, '*', '/');
				
				}

		/// <summary>
		/// Searches for the end of a multi line comment '*/'  (excluding quotes).
		/// </summary>
		/// <param name="line">The line of text to search.</param>
		/// <param name="startIndex">The index to start the search.</param>
		/// <returns>The index of the last charcater of the comment or -1 if not found.</returns>
		public static int FindEndOfMultiLineComment(string line, int startIndex)
		{
			var i= FindIndex(line, startIndex, '*', '/');
			return i == -1 ? -1 : ++i;
			int len = line.Length;
			if (len < startIndex + 2)
			{
				return -1;
			}

			int s = startIndex;
		retry:

			int end = line.IndexOf('*', s);
			if (end == -1 || !(end < len - 1))
			{
				return -1;
			}

			if (line[end + 1] != '/')
			{
				s = end + 1;
				goto retry;
			}


			return end + 1;
		}

		/// <summary>
		/// Searches for the start of a multi line comment '/*'  (excluding quotes).
		/// </summary>
		/// <param name="line">The line of text to search.</param>
		/// <param name="startIndex">The index to start the search.</param>
		/// <returns>The index of the first character of the comment or -1 if not found.</returns>
		public static int FindStartOfMultiLineComment(string line, int startIndex)
		{
			return FindIndex(line, startIndex, '/', '*');
			int len = line.Length;
			if (len < startIndex + 2)
			{
				return -1;
			}

			int s = startIndex;
		retry:

			int end = line.IndexOf('/', s);
			if (end == -1 || !(end < len - 1))
			{
				return -1;
			}

			if (line[end + 1] != '*')
			{
				s = end + 1;
				goto retry;
			}


			return end;
		}


		static int FindIndex(string line, int startIndex, char firstChar, char secondChar)
		{
			int len = line.Length;
			if (len < startIndex + 2)
			{
				return -1;
			}

			int s = startIndex;
		retry:

			int end = line.IndexOf(firstChar, s);
			if (end == -1 || !(end < len - 1))
			{
				return -1;
			}

			if (line[end + 1] != secondChar)
			{
				s = end + 1;
				goto retry;
			}


			return end;
		}


		/// <summary>
		/// Searches for the start of a single line comment '--'  (excluding quotes).
		/// </summary>
		/// <param name="line">The line of text to search.</param>
		/// <param name="startIndex">The index to start the search.</param>
		/// <returns>The index of the first character of the comment or -1 if not found.</returns>
		public static int FindStartOfSingleLineComment(string line, int startIndex)
		{
			return FindIndex(line, startIndex, '-', '-');
			int len = line.Length;
			if (len < startIndex + 2)
			{
				return -1;
			}

			int s = startIndex;
		retry:

			int end = line.IndexOf('-', s);
			if (end == -1 || !(end < len - 1))
			{
				return -1;
			}

			if (line[end + 1] != '-')
			{
				s = end + 1;
				goto retry;
			}


			return end;
		}

		private static MemoryStream GetMemoryStreamFromString(string s)
		{
			if (s == null)
				return null;

			var m = new MemoryStream();
			var sw = new StreamWriter(m);
			sw.Write(s);
			sw.Flush();
			m.Flush();
			m.Position = 0;

			return m;
		}

		private static string GetStringFromMemoryStream(MemoryStream m)
		{
			if (m == null || m.Length == 0)
				return string.Empty;

			m.Flush();
			m.Position = 0;
			var sr = new StreamReader(m);
			string s = sr.ReadToEnd();

			return s;
		}

		public static string CommentRemover(string sql)
		{
			var input = GetMemoryStreamFromString(sql);

			var m = new MemoryStream();

			SqlCommentRemoverv4.CommentRemover(input, new StreamWriter(m), null);

			return GetStringFromMemoryStream(m);
		}

		public static string CommentRemover(string sql, SqlCommentRemoverv4.Settings settings)
		{
			var input = GetMemoryStreamFromString(sql);

			var m = new MemoryStream();

			SqlCommentRemoverv4.CommentRemover(input, new StreamWriter(m), settings);

			return GetStringFromMemoryStream(m);
		}


		public static bool WriteStringWithoutComments(string line, int start, int stringStartIndex, int stringEndIndex,  char stringChar, StreamWriter writer)
		{



			if (stringEndIndex == -1)
			{
				// todo comment search

			}


			
			int commentSearchStart = stringStartIndex+1;
			int writeStart = stringStartIndex;

			int commentStart = line.IndexOf("--", commentSearchStart);
			if (commentStart != -1)
			{
				writer.Write(line.Substring(writeStart, commentStart - writeStart));
				writer.Write(stringChar);
				return false;
			}


												// multi line comments
		restartMulteLineCommentInStringSearch:
			commentStart = line.IndexOf("/*", commentSearchStart);
			if (commentStart != -1)
			{
				writer.Write(line.Substring(writeStart, commentStart - writeStart));

				// search for exn of comment
				int endOfComment = line.IndexOf("*/", commentStart + 2);

				if (endOfComment != -1)
				{
					commentSearchStart = endOfComment + 2;
					writeStart = commentSearchStart;
					goto restartMulteLineCommentInStringSearch;
				}

				return true;

			}

			writer.Write(line);

			return false;
		}
	}
}
