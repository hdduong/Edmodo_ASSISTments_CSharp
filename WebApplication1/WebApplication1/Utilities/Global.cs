using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Collections;

namespace connectToASSISTments_1.Utilities
{
    public class Global
    {
        // For Edmodo

        public static string EdmodoAPIKey = ConfigurationManager.AppSettings["EdmodoAPIKey"];
        public static string EdmodoAPIBase = ConfigurationManager.AppSettings["EdmodoAPIBase"];

        // For ASSITments
        public static string ASSITmentsBaseAPI = ConfigurationManager.AppSettings["ASSITmentsBaseAPI"];
        public static string ASSITmentsAPI_Helper = ConfigurationManager.AppSettings["ASSITmentsAPI_Helper"];

        public static string ASSITments_ContentType = "application/json";
        public static string ASSITments_Auth_WOBehalf = "partner=" + "\"" + "Hien-Ref" + "\"";

        public static string SchoolNCES = "250936001503";
        public static string password = "1234";

        public static string problemSetId = "148300";
    }
}