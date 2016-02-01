﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;

namespace Orb.App
{
	static public class Console
	{
		public static string ReadPassword(char mask)
		{
			const int ENTER = 13, BACKSP = 8, CTRLBACKSP = 127;
			int[] FILTERED = { 0, 27, 9, 10 };

			var pass = new Stack<char>();
			char chr = (char)0;

			while ((chr = System.Console.ReadKey(true).KeyChar) != ENTER)
			{
				if (chr == BACKSP)
				{
					if (pass.Count > 0)
					{
						System.Console.Write("\b \b");
						pass.Pop();
					}
				}
				else if (chr == CTRLBACKSP)
				{
					while (pass.Count > 0)
					{
						System.Console.Write("\b \b");
						pass.Pop();
					}
				}
				else if (FILTERED.Count(x => chr == x) > 0) { }
				else
				{
					pass.Push((char)chr);
					System.Console.Write(mask);
				}
			}

			System.Console.WriteLine();

			return new string(pass.Reverse().ToArray());
		}

		public static string ReadPassword()
		{
			return Orb.App.Console.ReadPassword('*');
		}
	}
}