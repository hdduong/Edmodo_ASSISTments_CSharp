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
    public class Assignment
    {
        public static string CreateAssignment(string problemSet, string class_ref, string onBehalfOf)
        {
            WebClient restClient = new WebClient();
            restClient.Headers.Add("Content-Type", Global.ASSITments_ContentType);

            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalfOf + "\"";
            restClient.Headers.Add("assistments-auth", AuthOnBehalf);


            string createAssignmentURL = String.Format("{0}/assignment", Global.ASSITmentsBaseAPI);

            string postData = "{" + "\"" + "problemSet" + "\"" + ":" + "\"" + Global.problemSetId + "\"" + "," +
                   "\"" + "class" + "\"" + ":" + "\"" + class_ref + "\"" + "," +
                   "\"" + "scope" + "\"" + ":" + "\"" + class_ref + "\"" + "}";

            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            byte[] byteResult = restClient.UploadData(createAssignmentURL, "POST", byteArray);
            string retStr = Encoding.ASCII.GetString(byteResult);

            JObject response = JObject.Parse(retStr);
            string assignment_ref = response["assignment"].ToString();

            return assignment_ref;
        }


        public static bool isAssignmentExist(string assignment_ref, string onBehalf)
        {
            var client = new RestClient(Global.ASSITmentsBaseAPI);
            var request = new RestRequest(String.Format("assignment/{0}?onExit={1}", assignment_ref,"http://localhost:36031/"), Method.GET);

            request.AddHeader("Content-Type", Global.ASSITments_ContentType);
            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalf + "\"";
            request.AddHeader("assistments-auth", AuthOnBehalf);

            var response = client.Execute(request);

            int responseStatus = (int)response.StatusCode;

            return (responseStatus == 200) ? true : false;

        }


        public static string getAssignmentHandler(string assignment_ref, string onBehalf)
        {
            var client = new RestClient(Global.ASSITmentsBaseAPI);
            var request = new RestRequest(String.Format("assignment/{0}?onExit={1}", assignment_ref, "http://localhost:36031/"), Method.GET);

            request.AddHeader("Content-Type", Global.ASSITments_ContentType);
            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalf + "\"";
            request.AddHeader("assistments-auth", AuthOnBehalf);

            var retStr = client.Execute(request);

            JObject response = JObject.Parse(retStr.Content);
            string assignmentHandler = response["unencodedHandler"].ToString();

            return assignmentHandler;

        }

    }
}