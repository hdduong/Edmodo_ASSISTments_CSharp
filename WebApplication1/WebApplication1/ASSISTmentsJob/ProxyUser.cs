using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using connectToASSISTments_1.EdmodoStruct;
using connectToASSISTments_1.Utilities;
using connectToASSISTments_1.EdmodoJob;
using connectToASSISTments_1.ASSISTmentsStruct;
using RestSharp;
using System.Net;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace connectToASSISTments_1.ASSISTmentsJob
{
    public class ProxyUser
    {
        public static string CreateUser(string user_token, string firstname, string lastname,string password, string username)
        {
            WebClient restClient = new WebClient();
            restClient.Headers.Add("Content-Type", Global.ASSITments_ContentType);
            restClient.Headers.Add("assistments-auth", Global.ASSITments_Auth_WOBehalf);
            string createUserURL = String.Format("{0}/user", Global.ASSITmentsBaseAPI);

            string postData = "{" + "\"" + "userType" + "\"" + ":" + "\"" + "proxy" + "\"" + "," +
                                    "\"" + "username" + "\"" + ":" + "\"" + "1" + username + "\"" + "," +
                                    "\"" + "password" + "\"" + ":" + "\"" + password + "\"" + "," +
                                    "\"" + "email" + "\"" + ":" + "\"" + "1" + firstname + lastname + "@junk.com" + "\"" + "," +
                                    "\"" + "firstName" + "\"" + ":" + "\"" + firstname + "\"" + "," +
                                    "\"" + "lastName" + "\"" + ":" + "\"" + lastname + "\"" + "," +
                                    "\"" + "displayName" + "\"" + ":" + "\"" + firstname + " " + lastname + "\"" + "," +
                                    "\"" + "timeZone" + "\"" + ":" + "\"" + "GMT-4" + "\"" + "," +
                                    "\"" + "registrationCode" + "\"" + ":" + "\"" + "HIEN-API" + "\"" + "}";
            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            byte[] byteResult = restClient.UploadData(createUserURL, "POST", byteArray);
            string retStr = Encoding.ASCII.GetString(byteResult);

            JObject response = JObject.Parse(retStr);
            string student_ref = response["user"].ToString();

            return student_ref;
        }

        public static string activateProxyUser(string student_ref)
        {
            var client = new RestClient(Global.ASSITmentsAPI_Helper);
            var request = new RestRequest("activate_proxy_user?partner={partner}&user={user}&on_success={on_success}&on_failure={on_failure}", Method.GET);
            request.AddParameter("partner", "Hien-Ref", ParameterType.UrlSegment);
            request.AddParameter("user", student_ref, ParameterType.UrlSegment);
            request.AddParameter("on_sucess", "http://localhost:36031/", ParameterType.UrlSegment);
            request.AddParameter("on_failure", "http://localhost:36031/ErrorPage.aspx", ParameterType.UrlSegment);
            var response = client.Execute(request);

            //Session["OnBehalfOf"] = ASSISTmentsExtractAccessToken(response.Content);
            String jsonReturn = response.Content;

            string proxyAccess = ASSISTmentsExtractAccessToken(jsonReturn);

            return proxyAccess;
        }

        protected static string ASSISTmentsExtractAccessToken(string strResponse)
        {
            string strFind = "access=";
            int startStr = strResponse.IndexOf(strFind, 0);
            string retStr = strResponse.Substring(startStr + 7, 24);

            return retStr;
        }
    }
}