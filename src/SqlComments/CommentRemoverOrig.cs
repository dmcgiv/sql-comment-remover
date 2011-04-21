using System.IO;


namespace McGiv.SqlUtil
{
	public static class SqlCommentRemoverOrig
	{


		public static int FindStringEndIndex(string line, int stringStartIndex, char stringChar)
		{
			bool lastCharIsStringChar = false;
			int i;
			for (i = stringStartIndex + 1; i < line.Length; i++)
			{
				var c = line[i];

				if (c == stringChar)
				{
					lastCharIsStringChar = !lastCharIsStringChar;
				}
				else if (lastCharIsStringChar)
				{
					return i - 1;
				}
			}

			return lastCharIsStringChar
				? i - 1
				: -1; // not found
		}




		public static int FindEndOfMultiLineComment(string line, int startIndex)
		{
			int len = line.Length;
			if (len < startIndex + 2)
			{
				return -1;
			}

			int s = startIndex;
		findEndOfMultiLineComment:

			int end = line.IndexOf('*', s);
			if (end == -1)
			{
				return -1;
			}

			if (end < len - 1 && line[end + 1] != '/')
			{
				s = end + 1;
				goto findEndOfMultiLineComment;
			}


			return end + 1;
		}



		public static void CommentRemover(Stream input, Stream output)
		{

			const int OK = 0;

			const int inMultiLineComment = 2;
			const int inString = 3;

			//const int searchingForComments = 0;

			int state = OK;
			//using (var reader = new StreamReader(input))
			var reader = new StreamReader(input);
			{



				char stringStarter = '0';
				var writer = new StreamWriter(output);
				bool writeNewLine = false;
				string line;
				while ((line = reader.ReadLine()) != null)
				{

					if (writeNewLine)
					{
						writer.WriteLine();
						writer.Flush();
						writeNewLine = false;
					}
					int i;
					int len = line.Length;


					if (len == 0 && state == inMultiLineComment)
					{
						continue;
					}
					for (i = 0; i < len; i++)
					{
						char c = line[i];

						switch (state)
						{
							case OK:
								{
									switch (c)
									{
										case '\'':
										case '"':
											{

												int end = FindStringEndIndex(line, i, c);
												if (end == -1)
												{
													state = inString;
													stringStarter = c;
													writer.Write(line.Substring(i));

													goto endOfLine;
												}


												writer.Write(line.Substring(i, end - i + 1));
												i = end;
												continue;


											}

										case '-':
											{

												// possible start of sinlge line comment
												if (i != len - 1 && line[i + 1] == '-')
												{
													if (i == 0)
													{
														goto endOfLineSkip;
													}
													goto endOfLine;
												}

												writer.Write(c);
												continue;

											}
										case '/':
											{
												// possible start of multiple line comment
												if (i != len - 1 && line[i + 1] == '*')
												{
													if (i != len - 2)
													{
														int end = line.IndexOf("*/", i + 1);

														if (end == -1)
														{
															state = inMultiLineComment;
															if (i == 0)
															{
																goto endOfLineSkip;
															}
															goto endOfLine;
														}



														i = end + 1;
														if (i != len - 1)
														{
															// add space to sepeate TSQL now joined after removal of comment
															writer.Write(' ');
														}
														continue;

													}

													// comment is end of line
													state = inMultiLineComment;
													if (i == 0)
													{
														goto endOfLineSkip;
													}
													goto endOfLine;
												}
												break;
											}
										default:
											{
												writer.Write(c);
												break;
											}
									}

									break;

								} // case -OK

							case inString:
								{
									int end = FindStringEndIndex(line, -1, stringStarter);

									if (end == -1)
									{
										writer.Write(line);

										goto endOfLine;
									}

									writer.Write(line.Substring(i, end - i + 1));
									i = end;
									state = OK;
									continue;

								}

							case inMultiLineComment:
								{

									int end = line.IndexOf("*/");

									if (end == -1)
									{
										goto endOfLineSkip;
									}

									i = end + 1;
									state = OK;
									continue;

								}
						} // switch - state

					}// for

				endOfLine:
					writeNewLine = true;
					continue;
				endOfLineSkip:
					writeNewLine = false;
				}

				writer.Flush();

			}


		}


		public static void CommentRemover2(Stream input, Stream output)
		{

			const int OK = 0;

			const int inMultiLineComment = 2;
			const int inString = 3;

			//const int searchingForComments = 0;

			int state = OK;
			//using (var reader = new StreamReader(input))
			var reader = new StreamReader(input);
			{



				char stringStarter = '0';
				var writer = new StreamWriter(output);
				bool writeNewLine = false;
				string line;
				while ((line = reader.ReadLine()) != null)
				{

					if (writeNewLine)
					{
						writer.WriteLine();
						//writer.Flush();
						writeNewLine = false;
					}
					int i;
					int len = line.Length;


					if (len == 0 && state == inMultiLineComment)
					{
						continue;
					}
					for (i = 0; i < len; i++)
					{
						char c = line[i];

						switch (state)
						{
							case OK:
								{
									switch (c)
									{
										case '\'':
										case '"':
											{

												int end = FindStringEndIndex(line, i, c);
												if (end == -1)
												{
													state = inString;
													stringStarter = c;
													writer.Write(line.Substring(i));

													goto endOfLine;
												}


												writer.Write(line.Substring(i, end - i + 1));
												i = end;
												continue;


											}

										case '-':
											{

												// possible start of sinlge line comment
												if (i != len - 1 && line[i + 1] == '-')
												{
													if (i == 0)
													{
														goto endOfLineSkip;
													}
													goto endOfLine;
												}

												writer.Write(c);
												continue;

											}
										case '/':
											{
												// possible start of multiple line comment
												if (i != len - 1 && line[i + 1] == '*')
												{
													int end = FindEndOfMultiLineComment(line, i+2);

													if (end == -1)
													{
														state = inMultiLineComment;
														if (i == 0)
														{
															goto endOfLineSkip;
														}
														goto endOfLine;
													}


													i = end;
													if (i != len - 1)
													{
														// add space to sepeate TSQL now joined after removal of comment
														writer.Write(' ');
													}
													continue;

													
												}
												break;
											}
										default:
											{
												writer.Write(c);
												break;
											}
									}

									break;

								} // case -OK

							case inString:
								{
									int end = FindStringEndIndex(line, -1, stringStarter);

									if (end == -1)
									{
										writer.Write(line);

										goto endOfLine;
									}

									writer.Write(line.Substring(i, end - i + 1));
									i = end;
									state = OK;
									continue;

								}

							case inMultiLineComment:
								{

									//int end = line.IndexOf("*/");
									int end = FindEndOfMultiLineComment(line, i);


									if (end == -1)
									{
										goto endOfLineSkip;
									}

									i = end;
									state = OK;
									continue;

								}
						} // switch - state

					}// for

				endOfLine:
					writeNewLine = true;
					continue;
				endOfLineSkip:
					writeNewLine = false;
				}

				writer.Flush();

			}


		}


	}
}
