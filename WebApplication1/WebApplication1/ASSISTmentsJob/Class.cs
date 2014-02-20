using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Net;
using System.IO;
using RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using connectToASSISTments_1.Utilities;

namespace connectToASSISTments_1.ASSISTmentsJob
{
    public class Class
    {
        public static string CreateNew(string courseName, string courseNumber, string sectionNumber, string onBehalf)
        {
            string retStr = "";

            WebClient restClient = new WebClient();
            restClient.Headers.Add("Content-Type", Global.ASSITments_ContentType);

            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalf + "\"";
            restClient.Headers.Add("assistments-auth", AuthOnBehalf);


            string createClassURL = String.Format("{0}/student_class", Global.ASSITmentsBaseAPI);

            //string postData = "{" + "\"" + "courseName" + "\"" + ":" + "\"" + courseName + "\"" + "," +
            //          "\"" + "courseNumber" + "\"" + ":" + "\"" + courseNumber + "\"" + "," +
            //          "\"" + "sectionNumber" + "\"" + ":" + "\"" + sectionNumber + "\"" + "}";

            string postData = "{" + "\"" + "courseName" + "\"" + ":" + "\"" + courseName + "\"" + "}";

            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            byte[] byteResult = restClient.UploadData(createClassURL, "POST", byteArray);
            retStr = Encoding.ASCII.GetString(byteResult);

            JObject response = JObject.Parse(retStr);
            string classRef = response["class"].ToString();

            return classRef;
        }

        public static bool isClassExist(string class_ref, string onBehalf)
        {
            var client = new RestClient(Global.ASSITmentsBaseAPI);
            var request = new RestRequest(String.Format("class/{0}", class_ref), Method.GET);

            request.AddHeader("Content-Type", Global.ASSITments_ContentType);
            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalf + "\"";
            request.AddHeader("assistments-auth", AuthOnBehalf);

            var response = client.Execute(request);

            int responseStatus = (int)response.StatusCode;

            return (responseStatus == 200) ? true : false;

        }

        public static bool enrollStuentInClass(string user_ref, string class_ref, string onBehalfOf)
        {
            WebClient restClient = new WebClient();
            restClient.Headers.Add("Content-Type", Global.ASSITments_ContentType);

            String AuthOnBehalf = "partner=" + "\"" + "Hien-Ref" + "\",onBehalfOf=" + "\"" + onBehalfOf + "\"";
            restClient.Headers.Add("assistments-auth", AuthOnBehalf);

            string enrollStudentClass = String.Format("{0}/class_membership", Global.ASSITmentsBaseAPI);

            string postData = "{" + "\"" + "user" + "\"" + ":" + "\"" + user_ref + "\"" + "," +
                                    "\"" + "class" + "\"" + ":" + "\"" + class_ref + "\"" + "}";
            byte[] byteResult = new byte[]{};
            byte[] byteArray = Encoding.ASCII.GetBytes(postData);
            try
            {
                byteResult = restClient.UploadData(enrollStudentClass, "POST", byteArray);
            }
            catch (Exception e)
            {
                return false;
            }
            string retStr = Encoding.ASCII.GetString(byteResult);
            return true;

        }
    }
}