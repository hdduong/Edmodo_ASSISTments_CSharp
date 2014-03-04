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
    public class PrincipalUser
    {
        public static string CreateUser(string user_token)
        {
            string retStr = "";
            User teacher = DataAccess.getEdmodoTeacher(user_token);

            string userType = "principal";
            string password = Global.password;
            string firstName = teacher.first_name;
            string lastName = teacher.last_name;
            string email = "4" + firstName + lastName + "@junk.com";
            string username = email;
            string displayName = firstName + " " + lastName;
            string timeZone = "GMT-4";
            string registrationCode = "HIEN-API";


            WebClient restClient = new WebClient();
            restClient.Headers.Add("Content-Type", Global.ASSITments_ContentType);
            restClient.Headers.Add("assistments-auth", Global.ASSITments_Auth_WOBehalf);
            string createUserURL = String.Format("{0}/user", Global.ASSITmentsBaseAPI);

            string postData = "{" + "\"" + "userType" + "\"" + ":" + "\"" + userType + "\"" + "," +
                                    "\"" + "username" + "\"" + ":" + "\"" + username + "\"" + "," +
                                    "\"" + "password" + "\"" + ":" + "\"" + password + "\"" + "," +
                                    "\"" + "email" + "\"" + ":" + "\"" + email + "\"" + "," +
                                    "\"" + "firstName" + "\"" + ":" + "\"" + firstName + "\"" + "," +
                                    "\"" + "lastName" + "\"" + ":" + "\"" + lastName + "\"" + "," +
                                    "\"" + "displayName" + "\"" + ":" + "\"" + displayName + "\"" + "," +
                                    "\"" + "timeZone" + "\"" + ":" + "\"" + timeZone + "\"" + "," +
                                    "\"" + "registrationCode" + "\"" + ":" + "\"" + registrationCode + "\"" + "}";
            
            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            byte[] byteResult = restClient.UploadData(createUserURL, "POST", byteArray);
            retStr = Encoding.ASCII.GetString(byteResult);

            JObject response = JObject.Parse(retStr);
            string teacher_ref = response["user"].ToString();

            DataAccess.setTeacherRef(user_token, username, teacher_ref);
            return teacher_ref;
        }


        public static bool isTeacherExist(string user_ref)
        {
            var client = new RestClient(Global.ASSITmentsBaseAPI);
            var request = new RestRequest(String.Format("user/{0}",user_ref), Method.GET);
            request.AddHeader("Content-Type", Global.ASSITments_ContentType);
            request.AddHeader("assistments-auth", Global.ASSITments_Auth_WOBehalf);

            var response = client.Execute(request);

            int responseStatus = (int)response.StatusCode;

            return (responseStatus == 200) ? true : false;

        }

        public static string linkPrincipalUser(string user_ref)
        {
            var client = new RestClient(Global.ASSITmentsAPI_Helper);
            var request = new RestRequest("link_user?partner={partner}&user={user}&on_success={on_success}&on_failure={on_failure}", Method.GET);
            request.AddParameter("partner", "Hien-Ref", ParameterType.UrlSegment);
            request.AddParameter("user", user_ref, ParameterType.UrlSegment);
            request.AddParameter("on_sucess", "http://localhost:36031/", ParameterType.UrlSegment);
            request.AddParameter("on_failure", "http://localhost:36031/ErrorPage.aspx", ParameterType.UrlSegment);
            var response = client.Execute(request);

            //Session["OnBehalfOf"] = ASSISTmentsExtractAccessToken(response.Content);
            String jsonReturn = response.Content;

            string onBehalfOf = ASSISTmentsExtractAccessToken(jsonReturn);

            return onBehalfOf;
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