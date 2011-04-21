using System;
using System.IO;


namespace McGiv.SqlUtil
{


	public static class SqlCommentRemoverv4
	{

		public class Settings
		{
			public bool ThrowExceptions;
			public bool RemoveCommentsFromMultiLineStrings;

			public bool Debug;
		}





		/// <summary>
		/// Identifies should index occures first
		/// 
		/// </summary>
		/// <param name="indexes">-1 = not found</param>
		/// <returns>index of the first found index or -1 if none are found</returns>
		public static int GetFirstFound(params int[] indexes)
		{
			int first = -1;
			int firstIndex = -1;
			for(int i=0; i<indexes.Length; i++)
			{
				int f = indexes[i];
				if(f != -1 && (first == -1 || f < first))
				{
					first = f;
					firstIndex = i;
				}
			}

			return firstIndex;
		}


		public static void CommentRemover(Stream input, StreamWriter output, Settings settings)
		{
			if (settings == null)
			{
				settings = new Settings();
			}


			const int notFound = -1;

			// states
			const int OK = 0;
			const int inMultiLineComment = 2;
			const int inString = 3;
			const int inStringMultiLineComment = 4;


			bool addSpacerInsteadOfComment = false;
			// if a comment has a comment with in it then the first closing will be ignored
			int multiLineCommentCount = 0;
			int multiLineCommentStartIndex = -1;
			bool skipNextNewLine = false;

			// keeps track of the state
			int state = OK;

			
			// debug function prefixes each line with the state at the end of the line.
			Func<int, int, string> debug = (s, c) =>
							{
								switch (s)
								{
									case OK:
										{
											return "-- OK " + c;
										}
									case inMultiLineComment:
										{
											return "-- in multi line comment " + c;
										}
									case inString:
										{
											return "-- in string " + c;
										}
									case inStringMultiLineComment:
										{
											return "-- in multi line comment within string " + c;
										}
								}

								return "-- unknown";
							};

			var reader = new StreamReader(input);
			{
				char stringStarter = '0';
				var writer = output;// new StreamWriter(output);
				bool writeNewLine = false;
				string line;
				while ((line = reader.ReadLine()) != null)
				{

					int stringStartIndex = -1;
					if (writeNewLine)
					{
						if (state != inStringMultiLineComment && state != inMultiLineComment)
						{
							if(skipNextNewLine)
							{
								skipNextNewLine = false;
							}
							else if (settings.Debug)
							{
								writer.WriteLine(debug(state, multiLineCommentCount));
							}
							else
							{
								writer.WriteLine();
							}
						}
						writeNewLine = false;
					}

					int len = line.Length;

					int i = -1;

					if (len == 0)
					{
						if (state == inMultiLineComment)
						{
							continue;
						}

						goto endOfLine;
					}


					while (i < len - 1)
					{
						i++;

						try
						{
							switch (state)
							{
								case OK:
									{
										int next = line.IndexOfAny(new[] { '\'', '"', '-', '/' }, i);

										if (next == notFound)
										{
											writer.Write(i == 0 ? line : line.Substring(i));
											goto endOfLine;
										}


										char c = line[next];
										switch (c)
										{
											case '\'':
											case '"':
												{

													state = inString;
													stringStarter = c;
													writer.Write(line.Substring(i, next-i+1));
													stringStartIndex = next;
													i = next;
													continue;

												}


											case '-':
												{
													// possible start of single line comment

													if (next + 1 < len && line[next + 1] == '-')
													{
														if (next == 0)
														{
															goto endOfLineSkip;
														}
														// is comment
														if (next != i)
														{
															writer.Write(line.Substring(i, next - i));
														}

														goto endOfLine;

													}

													//writer.Write(i == 0 ? line : line.Substring(i));
													writer.Write(line.Substring(i, next - i + 1));
													i = next;

													continue;
												}

											case '/':
												{
													// possible start of multi line comment
													if (next + 1 < len && line[next + 1] == '*')
													{
														addSpacerInsteadOfComment = next != 0;
														writer.Write(line.Substring(i, next - i));

														if(multiLineCommentCount != 0)
														{
															throw new InvalidOperationException(line);
														}
														multiLineCommentCount = 1;
														multiLineCommentStartIndex = next;
														state = inMultiLineComment;
														i = next+1;
														continue;
													}

													writer.Write(line.Substring(i, next - i+1));
													i = next;

													continue;
												}
										}

										break;
									}
								case inMultiLineComment:
									{

										
										int start = Helper.FindStartOfMultiLineComment(line, i);
										int end = Helper.FindStartOfEndOfMultiLineComment(line, i);

										
										
										if (start == notFound)
										{
											if (end == notFound)
											{
												goto endOfLineSkip;
											}

										}
										else if (end == notFound || start < end)
										{
											multiLineCommentCount++;
											i = start+1;
											continue;
										}



										multiLineCommentCount--;
										if (multiLineCommentCount == 0)
										{
											state = OK;

											if(multiLineCommentStartIndex == 0 && end == len-2)
											{
												skipNextNewLine = true;
											}
											if (addSpacerInsteadOfComment)
											{
												if (multiLineCommentStartIndex > 0 && end != len - 2) // not at end of line
												{
													writer.Write(' ');
												}
												addSpacerInsteadOfComment = false;
											}

										}
										i = end+1;
										continue;
									}
								case inString:
									{
										int endOfStr = Helper.FindStringEndIndex(line, i, stringStarter);

										if (settings.RemoveCommentsFromMultiLineStrings)
										{
											int mlStart = Helper.FindStartOfMultiLineComment(line, i);
											int slStart = Helper.FindStartOfSingleLineComment(line, i);

											int first = GetFirstFound(endOfStr, mlStart, slStart);

											switch(first)
											{
												case notFound:
													{
														writer.Write(i == 0 ? line : line.Substring(i));
														goto endOfLine;
													}
												case 0 : // end of string
													{
														writer.Write(line.Substring(i, endOfStr -i + 1));
														state = OK;
														i = endOfStr;
														continue;
													}
												case 1: // start of multi line comment within string
													{
														if (multiLineCommentCount != 0)
														{
															throw new InvalidOperationException(line);
														}
														multiLineCommentCount = 1;
														state = inStringMultiLineComment;
														writer.Write(line.Substring(i, mlStart - i));
														i = mlStart + 1;
														continue;
													}
												case 2: // start of single line comment in string
													{
														writer.Write(line.Substring(i, slStart - i));
														if (endOfStr != notFound)
														{
															writer.Write(stringStarter);
															i = endOfStr;
															continue;
														}

														if (i == 0 || i == stringStartIndex+1)
														{
															goto endOfLineSkip;
														}
														goto endOfLine;
													}
													
											}
											
										
										}


										if (endOfStr == notFound)
										{
											writer.Write(i == 0 ? line : line.Substring(i));

											goto endOfLine;
										}

										writer.Write(line.Substring(i, endOfStr - i + 1));
										state = OK;
										i = endOfStr;
										continue;

									}
								case inStringMultiLineComment:
									{
										
										int commentStart = Helper.FindStartOfMultiLineComment(line, i);
										int commentEnd = Helper.FindStartOfEndOfMultiLineComment(line, i);
										int stringEnd = Helper.FindStringEndIndex(line, i, stringStarter);

										//if(commentEnd
										int first = GetFirstFound(commentStart, commentEnd, stringEnd);


										switch (first)
										{
											case notFound:
												{
													goto endOfLineSkip;
												}

											case 0: // comment start
												{
													multiLineCommentCount++;
													i = commentStart + 1;
													continue;
													// todo turn off other searches until end of line
												}
											case 1: // comment end
												{
													
													multiLineCommentCount--;
													i = commentEnd+1;
													if (multiLineCommentCount == 0)
													{
														// end of multi line comment, still within string
														state = inString;
														writer.Write(' ');
													}
													continue;

												}
											case 2: // string end
												{
													state = OK;
													i = stringEnd;
													writer.Write(line.Substring(i, stringEnd - i + 1));
													//writer.Write(stringStarter);
													continue;
												}
											default:
												{
													// error
													throw new InvalidOperationException("invalid index");
												}
										}

									}
							}


						} // line while
						catch (Exception)
						{
							Console.WriteLine(line);
							Console.WriteLine("i = " + i);
							Console.WriteLine("start = " + state);

							throw;
						}
					}


				endOfLine:
					writeNewLine = true;
					continue;


				endOfLineSkip:
					writeNewLine = false;


				} // file while

				//if(writeNewLine)
				//{
				//    writer.WriteLine();
				//}
				writer.Flush();

			}


		}


	}
}
