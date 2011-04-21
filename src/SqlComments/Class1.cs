using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SqlComments
{
	public class Class1
	{

		/*
		 * rules
		 * -- = start of single line comment
		 * 
		 * /* = start of milti line comment
		 * 
		 * * / = end of milti line comments
		 * 
		 * */
		public static void CommentRemover(Stream input, Stream output)
		{

			const int OK = 0;
			const int inSingleLineComment = 1;
			const int inMultiLineComment = 2;
			const int inString = 3;

			//const int searchingForComments = 0;

			int state = OK;
			using (var reader = new StreamReader(input))
			{

				//string line = reader.ReadLine();

				//int singleComment = line.IndexOf("--");
				//int miltiComment = line.IndexOf("/*");
				//int stringStart = line.IndexOfAny(new [] {'\'', '"'});
				var buf = new char[1];
				int i = 0;
				char stringStarter;
				char lastChar;
				var writer = new StreamWriter(output);
				while(reader.Read(buf, ++i, 1) == 1)
				{
					char c = buf[0];

					switch(state)
					{
						case OK:
							{
								switch(c)
								{
									case '\'':
									case '"':
										{
											state = inString;
											stringStarter = c;
											writer.Write(c);

											// read to end of string

											while (reader.Read(buf, ++i, 1) == 1)
											{
												char c2 = buf[0];

												if(c2 == stringStarter)
												{
													// possible end of string or double string char
													if(reader.Read(buf, ++i, 1) == 1)
													{
														char c3 = buf[0];
														if(c2 == c3)
														{
															
														}
													}
												}
												else
												{
													writer.Write(c2);
												}

											}
											break;
										}
								}
							}
					}
				}



			}
		}
	}
}
