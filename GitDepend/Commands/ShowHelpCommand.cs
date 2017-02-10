using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitDepend.Commands
{
	class ShowHelpCommand : ICommand
	{
		#region Implementation of ICommand

		public int Execute()
		{
			Console.WriteLine(Options.Default.GetUsage());
			return ReturnCodes.Success;
		}

		#endregion
	}
}
