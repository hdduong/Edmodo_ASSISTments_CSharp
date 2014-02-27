using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using connectToASSISTments_1.EdmodoStruct;
using connectToASSISTments_1.ASSISTmentsStruct;

namespace connectToASSISTments_1
{
    public class DataAccess
    {
        public static bool isEdmodoUserAvailable(string user_token)
        {
            int numUser = 0;

            string connectionString =
                            @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                            @"Data Source=D:\EdmodoData_1.accdb;" +
                            @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT user_type FROM tblUsers WHERE edmodo_user_token='" + user_token + "'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();
            
            while (myReader.Read())
            {
                numUser += 1;
            }

            dataConnection.Close();

            return (numUser==0)?false:true;
        }

        public static void addEdmodoTeacher(string user_type, string user_token, string access_token, string first_name, string last_name, 
            string groupId)
        {
            string connectionString =
                           @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                           @"Data Source=D:\EdmodoData_1.accdb;" +
                           @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "INSERT INTO tblUsers (edmodo_first_name, edmodo_last_name,edmodo_user_token,edmodo_access_token,group_id, user_type) VALUES('"
             + first_name + "','" + last_name + "','" + user_token + "','" + access_token + "','" + groupId + "','" + user_type + "')";
            
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();
        }

        public static void updateEdmodoAccessToken(string user_token, string access_token)
        {
            string connectionString =
                           @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                           @"Data Source=D:\EdmodoData_1.accdb;" +
                           @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "Update tblUsers SET edmodo_access_token = " + "'" + access_token + "' WHERE edmodo_user_token = '" + user_token + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();
        }

        public static User getEdmodoTeacher(string user_token)
        {
            User teacher = new User();

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT edmodo_user_token, edmodo_access_token, edmodo_first_name, edmodo_last_name FROM tblUsers WHERE edmodo_user_token='" + user_token + "'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                teacher.access_token = myReader["edmodo_access_token"].ToString();
                teacher.first_name = myReader["edmodo_first_name"].ToString();
                teacher.last_name = myReader["edmodo_last_name"].ToString();
            }

            dataConnection.Close();

            return teacher;

        }


        public static string getASSISTmentsTeacherRef(string user_token)
        {
            String teacherRef = "";

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT assistments_user_ref FROM tblUsers WHERE edmodo_user_token='" + user_token + "'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                teacherRef = myReader["assistments_user_ref"].ToString();
            }

            dataConnection.Close();

            return (teacherRef != "") ? teacherRef : "";
        }

        public static void setTeacherRef(string user_token, string assistments_username, string assistments_user_ref)
        {
            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "Update tblUsers SET assistments_user_ref = " + "'" + assistments_user_ref + "'," +
                "assistments_username = '" + assistments_username + "'" +
                " WHERE edmodo_user_token = '" + user_token + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();

        }


        public static string getASSISTmentsTeacherOnBehalfOf(string user_token)
        {
            String teacherRef = "";

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT onBehalfOf FROM tblUsers WHERE edmodo_user_token='" + user_token + "'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                teacherRef = myReader["onBehalfOf"].ToString();
            }

            dataConnection.Close();

            return (teacherRef != "") ? teacherRef : "";
        }


        public static void setTeacherOnBehalfOf(string user_token, string onBehalfOf)
        {
            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "Update tblUsers SET onBehalfOf = " + "'" + onBehalfOf +  "'" +
                " WHERE edmodo_user_token = '" + user_token + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();

        }



        public static bool isSchoolAvailable()
        {
            int numSchool = 0;

            string connectionString =
                            @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                            @"Data Source=D:\EdmodoData_1.accdb;" +
                            @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT * FROM tblRefTable WHERE school_ref IS NOT NULL";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                numSchool += 1;
            }

            dataConnection.Close();

            return (numSchool == 0) ? false : true;
        }

        public static void addSchoolRef(string school_ref)
        {
            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "INSERT INTO tblRefTable (school_ref) VALUES " + "('" + school_ref + "')";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();

            dataConnection.Close();

        }


        public static string getSchoolRef()
        {
            String schoolRef = "";

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT school_ref FROM tblRefTable";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                schoolRef = myReader["school_ref"].ToString();
            }

            dataConnection.Close();

            return (schoolRef != "") ? schoolRef : "";

        }


        public static void setSchoolRef(string school_ref)
        {
            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "Update tblRefTable SET school_ref = " + "'" + school_ref + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();

        }


        public static bool isClassAvailable()
        {
            int numClass = 0;

            string connectionString =
                            @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                            @"Data Source=D:\EdmodoData_1.accdb;" +
                            @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT * FROM tblRefTable WHERE class_ref IS NOT NULL";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                numClass += 1;
            }

            dataConnection.Close();

            return (numClass == 0) ? false : true;
        }

        public static void setClassRef(string class_ref)
        {
            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "Update tblRefTable SET class_ref = " + "'" + class_ref + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();

        }


        public static string getClassRef()
        {
            String classRef = "";

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT class_ref FROM tblRefTable";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                classRef = myReader["class_ref"].ToString();
            }

            dataConnection.Close();

            return (classRef != "") ? classRef : "";

        }


        public static bool isStudentAvailable(string user_token)
        {
            int numUser = 0;

            string connectionString =
                            @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                            @"Data Source=D:\EdmodoData_1.accdb;" +
                            @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT user_type FROM tblUsers WHERE edmodo_user_token='" + user_token + "'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                numUser += 1;
            }

            dataConnection.Close();

            return (numUser == 0) ? false : true;
        }


        public static void addEdmodoStudent(string user_type, string user_token, string first_name, string last_name,
            string groupId)
        {
            string connectionString =
                           @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                           @"Data Source=D:\EdmodoData_1.accdb;" +
                           @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "INSERT INTO tblUsers (edmodo_first_name, edmodo_last_name,edmodo_user_token,group_id, user_type) VALUES('"
             + first_name + "','" + last_name + "','" + user_token + "','" + groupId + "','" + user_type + "')";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();
        }

        public static void setStudentRef(string user_token, string assistments_username, string assistments_user_ref)
        {
            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "Update tblUsers SET assistments_user_ref = " + "'" + assistments_user_ref + "'," +
                "assistments_username = '" + assistments_username + "'" +
                " WHERE edmodo_user_token = '" + user_token + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();

        }

        public static string getStudentRef(string studentToken)
        {
            String schoolRef = "";

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT assistments_user_ref FROM tblUsers WHERE edmodo_user_token ='" + studentToken + "'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                schoolRef = myReader["assistments_user_ref"].ToString();
            }

            dataConnection.Close();

            return (schoolRef != "") ? schoolRef : "";

        }


        public static void setStudentOnBehalfOf(string user_token, string onBehalfOf)
        {
            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "Update tblUsers SET onBehalfOf = " + "'" + onBehalfOf + "'" +
                " WHERE edmodo_user_token = '" + user_token + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();

        }

        public static string getStudentOnBehalfOf(string studentToken)
        {
            string onBehalfOf = "";

            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT onBehalfOf FROM tblUsers WHERE edmodo_user_token ='" + studentToken + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                onBehalfOf = myReader["onBehalfOf"].ToString();
            }

            dataConnection.Close();

            return (onBehalfOf != "") ? onBehalfOf : "";

        }


        public static bool isAssignmentAvailable()
        {
            int numClass = 0;

            string connectionString =
                            @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                            @"Data Source=D:\EdmodoData_1.accdb;" +
                            @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT * FROM tblRefTable WHERE assignment_ref IS NOT NULL";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                numClass += 1;
            }

            dataConnection.Close();

            return (numClass == 0) ? false : true;
        }

        public static void setAssignmentRef(string assignment_ref)
        {
            string connectionString =
               @"Provider=Microsoft.ACE.OLEDB.12.0;" +
               @"Data Source=D:\EdmodoData_1.accdb;" +
               @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "Update tblRefTable SET assignment_ref = " + "'" + assignment_ref + "'";

            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            dataCommand.ExecuteNonQuery();
            dataConnection.Close();

        }


        public static string getAssignmentRef()
        {
            String assignmentRef = "";

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT assignment_ref FROM tblRefTable";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                assignmentRef = myReader["assignment_ref"].ToString();
            }

            dataConnection.Close();

            return (assignmentRef != "") ? assignmentRef : "";

        }


        public static List<LinkPrincipalUser> getUserFromDB()
        {
            List<LinkPrincipalUser> users = new List<LinkPrincipalUser>();

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT assistments_user_ref FROM tblUsers";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                LinkPrincipalUser user = new LinkPrincipalUser();
                user.user = myReader["assistments_user_ref"].ToString();
                users.Add(user);
            }

            dataConnection.Close();

            return users;
        }

        public static List<LinkPrincipalUser> getStuentFromDB()
        {
            List<LinkPrincipalUser> users = new List<LinkPrincipalUser>();

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT assistments_user_ref FROM tblUsers WHERE user_type='STUDENT'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                LinkPrincipalUser user = new LinkPrincipalUser();
                user.user = myReader["assistments_user_ref"].ToString();
                users.Add(user);
            }

            dataConnection.Close();

            return users;
        }

        public static string getEdmodoTeacherToken()
        {

            string teacherToken = "";

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT edmodo_user_token FROM tblUsers WHERE user_type='TEACHER'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                teacherToken = myReader["edmodo_user_token"].ToString();
               
            }

            dataConnection.Close();

            return teacherToken;

        }

        public static int countNumberOfStudentInGroup()
        {

            string student_counter = "";

            string connectionString =
                          @"Provider=Microsoft.ACE.OLEDB.12.0;" +
                          @"Data Source=D:\EdmodoData_1.accdb;" +
                          @"Persist Security Info=False;";

            OleDbConnection dataConnection = new OleDbConnection();
            dataConnection.ConnectionString = connectionString;
            dataConnection.Open();

            string commandStr = "SELECT COUNT(edmodo_user_token) AS student_counter FROM tblUsers WHERE user_type='STUDENT'";
            OleDbCommand dataCommand = new OleDbCommand(commandStr, dataConnection);

            OleDbDataReader myReader = dataCommand.ExecuteReader();

            while (myReader.Read())
            {
                student_counter = myReader["student_counter"].ToString();

            }

            dataConnection.Close();

            return Convert.ToInt32(student_counter) ;

        }
    }
}