using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GitDepend.Busi
{
    /// <summary>
    /// This class will check to see if there is a new version of gitdepend available from the github api.
    /// </summary>
    internal class GitHubVersionUpdateChecker : IVersionUpdateChecker
    {
        private const string VersionNumberKey = "name";
        private const string BaseUrl = "https://api.github.com";


        public string CheckVersion(string packageName, string owner)
        {
            string latestReleaseApiCall = $"/repos/{owner}/{packageName}/releases/latest";
            string newVersionAvailable =
                $"\n\nThere is a new version of {packageName} available.Please update via NuGet.org, chocolatey.org or visit http://github.com/repos/{owner}/{packageName}";

            string appendString = string.Empty;

            var client = new RestClient(BaseUrl);

            var request = new RestRequest(latestReleaseApiCall, Method.GET);
            IRestResponse response = client.Execute(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var content = response.Content;
                var responseObject = JObject.Parse(content);
                var releaseVersionString = responseObject[VersionNumberKey].ToString();
                releaseVersionString = releaseVersionString.Replace("v", "");
                var currentRelease = Version.Parse(releaseVersionString);

                var currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

                if (currentRelease > currentVersion)
                {
                    appendString = newVersionAvailable;
                }
            }

            return appendString;
        }
    }
}
