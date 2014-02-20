using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Net;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using connectToASSISTments_1.Utilities;

namespace connectToASSISTments_1.ASSISTmentsJob
{
    public class Problem
    {
        public static string getProblemTestDrive(string problemId, string onBehalfOf, string onExit)
        {
            WebClient restClient = new WebClient();
            restClient.Headers.Add("Content-Type", Global.ASSITments_ContentType);

            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalfOf + "\"";
            restClient.Headers.Add("assistments-auth", AuthOnBehalf);

            string testDriveURL = string.Format("{0}/problem_set/{1}?onExit={2}", Global.ASSITmentsBaseAPI,onExit);

            string result = restClient.DownloadString(testDriveURL);

            JObject responseURL = JObject.Parse(result);

            string handler_ref = responseURL["unencodedHandler"].ToString();

            return handler_ref;
        }
    }
}