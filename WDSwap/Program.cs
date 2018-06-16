using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CLAP;

namespace WDSwap
{
	class Program
	{
		static void Main(string[] args)
		{
			try
			{
				var commands = new CliSwapManager();
				Parser.Run(args, commands);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
		}
	}
}
