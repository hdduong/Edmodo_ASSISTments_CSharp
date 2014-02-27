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
using System.Text.RegularExpressions;

namespace connectToASSISTments_1.ASSISTmentsJob
{
    public class Report
    {
        public static string getReportHandler(string assignment_ref, string onBehalf)
        {
            var client = new RestClient(Global.ASSITmentsBaseAPI);
            var request = new RestRequest(String.Format("report/{0}/1", assignment_ref), Method.GET);

            request.AddHeader("Content-Type", Global.ASSITments_ContentType);
            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalf + "\"";
            request.AddHeader("assistments-auth", AuthOnBehalf);

            var retStr = client.Execute(request);

            JObject response = JObject.Parse(retStr.Content);
            string reportHandler = response["unencodedHandler"].ToString();

            return reportHandler;

        }

        public static string getHTMLReport(string reportHandler, string onBehalfOf)
        {
            string assingmentReport = string.Format("{2}/user_login?partner=Hien-Ref&access={0}&on_success={1}&on_failure=yahoo.com", onBehalfOf, reportHandler, Global.ASSITmentsAPI_Helper);
            
            var restClient = new RestClient(assingmentReport);
            var request = new RestRequest(Method.GET);
            
            RestResponse response = (RestResponse)restClient.Execute(request);
            
            string data = response.Content;
            return data;
        }

        public static int numberStudentsDidAssignment(string reportData)
        {
            string text = @"Not started";
            int numberStudents = Regex.Matches(reportData, text).Count;

            return numberStudents;
        }
    }
}