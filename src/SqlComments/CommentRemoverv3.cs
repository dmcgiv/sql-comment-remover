using System.IO;


namespace McGiv.SqlUtil
{
	public static class SqlCommentRemoverv3
	{



		public static void CommentRemover(Stream input, Stream output)
		{

			const int OK = 0;

			const int inMultiLineComment = 2;
			const int inString = 3;

			//const int searchingForComments = 0;

			int state = OK;
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
						writeNewLine = false;
					}

					int len = line.Length;

					int i = 0;

					if (len == 0)
					{
						if (state == inMultiLineComment)
						{
							continue;
						}

						goto endOfLine;
					}


					while (i < len)
					{
						switch (state)
						{
							case OK:
								{
									int next = line.IndexOfAny(new[] { '\'', '"', '-', '/' }, i);

									if (next == -1)
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

												int end = Helper.FindStringEndIndex(line, next, c);


												

												 if (end == -1)
												 {
													 Helper.WriteStringWithoutComments(line, i, next, end, c, writer);
												 	state = inString;
												 	stringStarter = c;

													goto endOfLine;
												 }


												 Helper.WriteStringWithoutComments(line.Substring(i, end-i), i, next, end, c, writer);
												 i = end + 1;

					
												continue;
											}


										case '-':
											{
												// possible start of single line comment

												if (next + 1 < len && line[next + 1] == '-')
												{
													// is comment
													if (next != i)
													{
														writer.Write(line.Substring(i, next-i));
													}

													goto endOfLine;

												}

												i = next + 1;

												continue;
											}

										case '/':
											{
												// possible start of multi line comment
												if (next + 1 < len && line[next + 1] == '*')
												{
													// is comment

													int end = Helper.FindEndOfMultiLineComment(line, next + 2);

													if (end == -1)
													{
														if (next != i)
														{
															writer.Write(line.Substring(i, next - i));
														}
														state = inMultiLineComment;
														goto endOfLine;
													}

													if( next != i)
													{
														writer.Write(line.Substring(i, next - i));
														if (end < len-1)
														{
															writer.Write(' ');
														}
													}

													i = end;
													continue;

												}

												i = next + 1;

												continue;
											}
									}

									break;
								}
							case inMultiLineComment:
								{


									int end = Helper.FindEndOfMultiLineComment(line, 0);


									if (end == -1)
									{
										goto endOfLineSkip;
									}

									state = OK;
									i = end + 1;
									continue;
								}
							case inString:
								{
									int end = Helper.FindStringEndIndex(line, -1, stringStarter);

									if (end == -1)
									{
										writer.Write(line);

										goto endOfLine;
									}

									writer.Write(line.Substring(i, end - i + 1));
									state = OK;
									i = end + 1;
									continue;

								}
						}


					} // line while

				endOfLine:
					writeNewLine = true;
					continue;


				endOfLineSkip:
					writeNewLine = false;


				} // file while

				writer.Flush();

			}


		}


	}
}
