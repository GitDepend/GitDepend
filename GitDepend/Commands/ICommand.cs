namespace GitDepend.Commands
{
	/// <summary>
	/// An action that will be taken as the result of parsing the command line options.
	/// </summary>
	public interface ICommand
	{
		/// <summary>
		/// Executes the command.
		/// </summary>
		/// <returns>The return code.</returns>
		ReturnCode Execute();
	}
}
