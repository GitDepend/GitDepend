using GitDepend.CommandLine;
using GitDepend.Visitors;

namespace GitDepend.Commands
{
    /// <summary>
    /// This command will do a git log on the named dependencies.
    /// </summary>
    public class LogCommand : NamedDependenciesCommand<LogSubOptions>
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        public const string Name = "log";

        /// <summary>
        /// The constructor for the LogCommand.
        /// </summary>
        /// <param name="options"></param>
        public LogCommand(LogSubOptions options) : base(options)
        {
        }

        /// <summary>
        /// Creates the visitor that will be used to traverse the dependency graph.
        /// </summary>
        /// <param name="options">The options for the command.</param>
        /// <returns></returns>
        protected override NamedDependenciesVisitor CreateVisitor(LogSubOptions options)
        {
            return new LogVisitor(options.GitArguments, options.Dependencies);
        }
    }
}
