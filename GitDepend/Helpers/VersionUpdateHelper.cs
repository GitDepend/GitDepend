using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace GitDepend.Helpers
{
    /// <summary>
    /// This class will check to see if there is a new version of gitdepend available from the github api.
    /// </summary>
    internal static class VersionUpdateHelper
    {
        public const string BaseUrl = "https://api.github.com";
        public const string LatestReleaseApiCall = "/repos/kjjuno/gitdepend/releases/latest";
        public const string NewVersionAvailable = "\n\nThere is a new version of GitDepend available. Please update via NuGet or visit https://github.com/repos/kjjuno/gitdepend";
        public const string VersionNumberKey = "name";

        public static string CheckVersion()
        {
            string appendString = string.Empty;

            var client = new RestClient(BaseUrl);

            var request = new RestRequest(LatestReleaseApiCall, Method.GET);
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
                    appendString = NewVersionAvailable;
                }
            }

            return appendString;
        }
    }
}
