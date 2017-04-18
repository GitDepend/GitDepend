using System.Collections.Generic;

namespace GitDepend.Busi
{
    /// <summary>
    /// A helper class for dealing with git.exe
    /// </summary>
    public interface IGit
    {
        /// <summary>
        /// The working directory for all git operations.
        /// </summary>
        string WorkingDirectory { get; set; }

        /// <summary>
        /// Adds files into the staging area to prepare them for a commit.
        /// </summary>
        /// <param name="files">The files to add to the staging area.</param>
        /// <returns>The git return code.</returns>
        ReturnCode Add(params string[] files);

        /// <summary>
        /// Checks out the given branch.
        /// </summary>
        /// <param name="branch">The branch to check out.</param>
        /// <param name="create">Should the branch be created?</param>
        /// <returns>The git return code.</returns>
        ReturnCode Checkout(string branch, bool create);

        /// <summary>
        /// Creates the given branch.
        /// </summary>
        /// <param name="branch">The branch to create</param>
        /// <returns>The git return code.</returns>
        ReturnCode CreateBranch(string branch);

        /// <summary>
        /// Clones a repository.
        /// </summary>
        /// <param name="url">The repository url.</param>
        /// <param name="directory">The directory where the repository should be cloned.</param>
        /// <param name="branch">The branch that should be checked out.</param>
        /// <returns>The git return code.</returns>
        ReturnCode Clone(string url, string directory, string branch);

        /// <summary>
        /// Makes a commit with the given message.
        /// </summary>
        /// <param name="message"></param>
        /// <returns>The git return code.</returns>
        ReturnCode Commit(string message);

        /// <summary>
        /// Fetches the latest repository state.
        /// </summary>
        /// <returns>The git return code.</returns>
        ReturnCode Fetch();

        /// <summary>
        /// Shows the status of the repository.
        /// </summary>
        /// <returns>The git return code.</returns>
        ReturnCode Status();

        /// <summary>
        /// Deletes the specified branch.
        /// </summary>
        /// <param name="branch">The branch to delete.</param>
        /// <param name="force">Should the deletion be forced or not.</param>
        /// <returns></returns>
        ReturnCode DeleteBranch(string branch, bool force);

        /// <summary>
        /// Lists all merged branches.
        /// </summary>
        /// <returns></returns>
        ReturnCode ListMergedBranches();

        /// <summary>
        /// Lists all branches.
        /// </summary>
        /// <returns></returns>
        ReturnCode ListAllBranches();

        /// <summary>
        /// Gets the current branch.
        /// </summary>
        /// <returns></returns>
        string GetCurrentBranch();

        /// <summary>
        /// Cleans the directory.
        /// </summary>
        /// <returns></returns>
        ReturnCode Clean(string gitCleanArguments);

        /// <summary>
        /// Performs the push command
        /// </summary>
        /// <returns></returns>
        ReturnCode Push();
        
        /// <summary>
        /// Runs the pull command with the arguments provided.
        /// </summary>
        /// <returns></returns>
        ReturnCode Pull(string gitPullArguments);

        /// <summary>
        /// Runs the log command with the arguments provided.
        /// </summary>
        /// <returns></returns>
        ReturnCode Log(string gitLogArguments);
    }
}