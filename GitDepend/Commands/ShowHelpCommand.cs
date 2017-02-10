using System;

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
