using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RestSharp;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using connectToASSISTments_1.Utilities;

namespace connectToASSISTments_1.ASSISTmentsJob
{
    public class School
    {
        public static string CreateNew(string nces)
        {
            string retStr = "";

            WebClient restClient = new WebClient();
            restClient.Headers.Add("Content-Type", Global.ASSITments_ContentType);
            restClient.Headers.Add("assistments-auth", Global.ASSITments_Auth_WOBehalf);

            string createSchoolURL = String.Format("{0}/school", Global.ASSITmentsBaseAPI);

            string postData = "{" + "\"" + "nces" + "\"" + ":" + "\"" + nces + "\"" + "}";
            
            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            byte[] byteResult = restClient.UploadData(createSchoolURL, "POST", byteArray);
            retStr = Encoding.ASCII.GetString(byteResult);

            JObject response = JObject.Parse(retStr);
            string schoolRef = response["school"].ToString();

            return schoolRef;
        }

        public static bool isSchoolExist(string school_ref)
        {
            var client = new RestClient(Global.ASSITmentsBaseAPI);
            var request = new RestRequest(String.Format("school/{0}", school_ref), Method.GET);
            request.AddHeader("Content-Type", Global.ASSITments_ContentType);
            request.AddHeader("assistments-auth", Global.ASSITments_Auth_WOBehalf);

            var response = client.Execute(request);

            int responseStatus = (int)response.StatusCode;

            return (responseStatus == 200) ? true : false;

        }

        public static void enrollUserInSchool(string user_ref, string school_ref, string onBehalfOf)
        {
            WebClient restClient = new WebClient();
            restClient.Headers.Add("Content-Type", Global.ASSITments_ContentType);

            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalfOf + "\"";
            restClient.Headers.Add("assistments-auth", AuthOnBehalf);

            string createUserURL = String.Format("{0}/school_membership", Global.ASSITmentsBaseAPI);

            string postData = "{" + "\"" + "user" + "\"" + ":" + "\"" + user_ref + "\"" + "," +
                                    "\"" + "school" + "\"" + ":" + "\"" + school_ref + "\"" + "}";

            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            byte[] byteResult = restClient.UploadData(createUserURL, "POST", byteArray);
            string retStr = Encoding.ASCII.GetString(byteResult);


        }

    }
}