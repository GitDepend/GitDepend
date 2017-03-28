using System.Collections.Generic;
using GitDepend.Busi;
using NUnit.Framework;

namespace GitDepend.UnitTests.Busi
{
    public class ReturnCodeExtensionsTests
    {

        [TestCaseSource("testCases")]
        public void GetResxKey_ShouldSucceed(ReturnCode code, string expectedKey)
        {
            var key = ReturnCodeExtensions.GetResxKey(code);

            Assert.AreEqual(expectedKey, key);
        }

        IEnumerable<TestCaseData> testCases
        {
            get
            {
                yield return new TestCaseData(ReturnCode.Success, "RET_SUCCESS");
                yield return new TestCaseData(ReturnCode.GitRepositoryNotFound, "RET_GIT_REPO_NOT_FOUND");
                yield return new TestCaseData(ReturnCode.FailedToRunGitCommand, "RET_GIT_COMMAND_FAILED");
                yield return new TestCaseData(ReturnCode.FailedToRunNugetCommand, "RET_NUGET_COMMAND_FAILED");
                yield return new TestCaseData(ReturnCode.FailedToRunBuildScript, "RET_BUILD_SCRIPT_FAILED");
                yield return new TestCaseData(ReturnCode.DirectoryDoesNotExist, "RET_DIRECTORY_DOES_NOT_EXIST");
                yield return new TestCaseData(ReturnCode.CouldNotCreateCacheDirectory, "RET_CREATE_CACHE_DIR_FAILED");
                yield return new TestCaseData(ReturnCode.InvalidUrlFormat, "RET_INVALID_URI_FORMAT");
                yield return new TestCaseData(ReturnCode.MissingDependency, "RET_MISSING_DEPENDENCY");
                yield return new TestCaseData(ReturnCode.InvalidBranchCheckedOut, "RET_INVALID_BRANCH_CHECKED_OUT");
                yield return new TestCaseData(ReturnCode.DependencyPackagesNotBuilt, "RET_DEPENDENCY_PACKAGES_NOT_BUILT");
                yield return new TestCaseData(ReturnCode.DependencyPackagesMisMatch, "RET_DEPENDENCY_PACKAGES_MISTMATCH");
                yield return new TestCaseData(ReturnCode.NameDidNotMatchRequestedDependency, "RET_NAME_DID_NOT_MATCH");
                yield return new TestCaseData(ReturnCode.DependencyAlreadyExists, "RET_DEPENDENCY_ALREADY_EXISTS");
                yield return new TestCaseData(ReturnCode.FailedToLocateArtifactsDir, "RET_ARTIFACTS_DIR_NOT_FOUND");
                yield return new TestCaseData(ReturnCode.InvalidArguments, "RET_INVALID_ARGS");
                yield return new TestCaseData(ReturnCode.InvalidCommand, "RET_INVALID_COMMAND");
                yield return new TestCaseData(ReturnCode.UnknownError, "RET_UNKNOWN_ERROR");
            }
        }
    }
}
