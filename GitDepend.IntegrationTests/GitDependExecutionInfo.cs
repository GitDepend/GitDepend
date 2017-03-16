namespace GitDepend.IntegrationTests
{
    public class GitDependExecutionInfo
    {
        /// <summary>
        /// The return code of GitDepend.
        /// </summary>
        public ReturnCode ReturnCode { get; set; }

        /// <summary>
        /// The output to standard out.
        /// </summary>
        public string StandardOut { get; set; }

        /// <summary>
        /// The output to standard error.
        /// </summary>
        public string StandardError { get; set; }
    }
}