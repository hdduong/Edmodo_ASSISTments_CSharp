using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.Configuration;
using connectToASSISTments_1.Utilities;
using RestSharp;
using System.IO;
using Newtonsoft;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using connectToASSISTments_1.EdmodoStruct;
using connectToASSISTments_1.EdmodoJob;
using connectToASSISTments_1.ASSISTmentsJob;
using connectToASSISTments_1.ASSISTmentsStruct;


namespace connectToASSISTments_1
{
    public partial class _default : System.Web.UI.Page
    {
        // for Edmodo
        string launchKey = "";

        string edmodoUserToken = "";
        string edmodoAccessToken = "";
        string edmodoFirstName = "";
        string edmodoLastName = "";
        string edmodoGroupId = "";
        string edmodoUserType = "";
        string onBehalfOf = "";
        string schoolRef = "";
        string classRef = "";
        string studentRef = "";
        string assignmentRef = "";



        protected void Page_Load(object sender, EventArgs e)
        {

            //Response.Write(IsPostBack.ToString());
            if (IsPostBack)
                return;

            if (Request.QueryString["launch_key"] != null)
            {
                launchKey = Request.QueryString["launch_key"].ToString();

                GetEdmodoUser(launchKey);

                edmodoAccessToken = Session["access_token"].ToString();
                edmodoUserToken = Session["user_token"].ToString();
                edmodoFirstName = Session["first_name"].ToString();
                edmodoLastName= Session["last_name"].ToString();
                edmodoGroupId = Session["group_id"].ToString();
                edmodoUserType = Session["user_type"].ToString();

                bool isExistUser = DataAccess.isEdmodoUserAvailable(edmodoUserToken);

                if (isExistUser)
                {
                    DataAccess.updateEdmodoAccessToken(edmodoUserToken, edmodoAccessToken);
                } 
                else 
                {
                    DataAccess.addEdmodoTeacher(edmodoUserType, edmodoUserToken, edmodoAccessToken, edmodoFirstName, edmodoLastName, edmodoGroupId);
                }

                switch (edmodoUserType)
                {
                    case "STUDENT":
                        // just login and do the homework
                        string studentOnBehalfOfGo = DataAccess.getStudentOnBehalfOf(edmodoUserToken);
                        string assingment_ref = DataAccess.getAssignmentRef();
                        string assingmentHandler = Assignment.getAssignmentHandler(assingment_ref, studentOnBehalfOfGo);
                        assingmentHandler = assingmentHandler.Substring(0, 53);
                        string linkToGo = "https://test1.assistments.org" + assingmentHandler;
                        ASSISTmentsUserLogin(edmodoUserType, studentOnBehalfOfGo, linkToGo);
                        break;

                    case "TEACHER":
                        
                        // install all students in class
                        List<Member> members = GetStudentsByGroup.GetStudents(edmodoAccessToken, edmodoGroupId);
                        IEnumerator<Member> aMember = members.GetEnumerator();
                        while (aMember.MoveNext())
                        {
                            
                            if (aMember.Current.user_type != "TEACHER")
                            {
                                // Create Proxy User
                                string studentToken = aMember.Current.user_token;
                                string studentFirstName = aMember.Current.first_name;
                                string studentLastName = aMember.Current.last_name;
                                string studentUserType = aMember.Current.user_type;

                                bool studentExist = DataAccess.isStudentAvailable(studentToken); //false if not exist
                                if (studentExist)
                                {
                                    // studentRef is created at the same time with studentToken.
                                    // Do not neet to check studentRef exist
                                    string studentRef = DataAccess.getStudentRef(studentToken);
                                    string studentOnBehalfOf = ProxyUser.activateProxyUser(studentRef); //if created then ASSISTments will not re-created.
                                    DataAccess.setStudentOnBehalfOf(studentToken, studentOnBehalfOf);
                                }
                                else //not exist
                                {
                                    //create new proxy student
                                    Response.Write("New Student Account Created in ASSISTments! <br /> ");

                                    //insert into DB
                                    DataAccess.addEdmodoStudent(studentUserType, studentToken, studentFirstName, studentLastName, edmodoGroupId);

                                    //create account in ASSISTments
                                    string studentUsername = studentFirstName + studentLastName;
                                    studentRef = ProxyUser.CreateUser(studentToken,studentFirstName,studentLastName, Global.password, studentUsername);

                                    //set studentRef
                                    DataAccess.setStudentRef(studentToken, studentUsername, studentRef);
                                }

                                
                            }
                            else
                            {
                                // get a Teacher
                                string teacherRef = DataAccess.getASSISTmentsTeacherRef(edmodoUserToken);

                                if (teacherRef == "")
                                {
                                    // Create Principal User in ASSISTments
                                    Response.Write("New teacher created in ASSISTments! <br />");
                                    teacherRef = PrincipalUser.CreateUser(edmodoUserToken);  //also update DB: teacherRef, username.
                                    onBehalfOf = PrincipalUser.linkPrincipalUser(teacherRef);
                                    DataAccess.setTeacherOnBehalfOf(edmodoUserToken, onBehalfOf);

                                }
                                else // teacherRef != ""
                                {
                                    // Create Principal User in ASSISTments if it is deleted
                                    if (!PrincipalUser.isTeacherExist(teacherRef))
                                    {
                                        Response.Write("Teacher is re-created in ASSISTments because of deleted during backup! <br />");
                                        teacherRef = PrincipalUser.CreateUser(edmodoUserToken);  //also update db
                                        onBehalfOf = PrincipalUser.linkPrincipalUser(teacherRef);
                                        DataAccess.setTeacherOnBehalfOf(edmodoUserToken, onBehalfOf);
                                    }
                                    else
                                    {
                                        Response.Write("Teacher already in ASSISTments! <br /> ");
                                        onBehalfOf = DataAccess.getASSISTmentsTeacherOnBehalfOf(edmodoUserToken);
                                        if (onBehalfOf == "")
                                        {
                                            onBehalfOf = PrincipalUser.linkPrincipalUser(teacherRef);
                                            DataAccess.setTeacherOnBehalfOf(edmodoUserToken, onBehalfOf);
                                        }
                                    }
                                }
                            }

                        }
                        

                        // create school 
                        bool schoolExist = DataAccess.isSchoolAvailable(); //false if not exist
                        if (schoolExist)
                        {
                            schoolRef = DataAccess.getSchoolRef();
                            // if exist in DB
                            // if exist in ASSISTments
                            if (School.isSchoolExist(schoolRef))
                            {
                                Response.Write("School already in ASSISTments! <br /> ");
                            }
                            else
                            {
                                // else deleted by backup
                                Response.Write("School recreated because of deleting data during backup! <br /> ");
                                schoolRef = School.CreateNew(Global.SchoolNCES);
                                DataAccess.setSchoolRef(schoolRef);
                            }

                        }
                        else //not exist
                        {
                            //create brand new school 
                            Response.Write("New School created in ASSISTments! <br /> ");
                            schoolRef = School.CreateNew(Global.SchoolNCES);
                            DataAccess.addSchoolRef(schoolRef);

                        }

                        // create class
                        bool classExist = DataAccess.isClassAvailable(); //false if not exist
                        if (classExist)
                        {
                            classRef = DataAccess.getClassRef();
                            // if exist in DB
                            // if exist in ASSISTments
                            if (Class.isClassExist(classRef,onBehalfOf))
                            {
                                Response.Write("Class already in ASSISTments! <br /> ");
                            }
                            else
                            {
                                // else deleted by backup
                                Response.Write("Class recreated because of deleting data during backup! <br /> ");
                                
                                // THERE IS A BUG IN ASSISTMENTS API. CANNOT GET CREATED Class
                                /*
                                string courseName = "Edmodo Math Class";
                                string courseNumber = "1";
                                string courseSection = "1";

                                classRef = Class.CreateNew(courseName, courseNumber, courseSection, onBehalfOf);
                                DataAccess.setClassRef(classRef);
                                */
                            }

                        }
                        else //not exist
                        {
                            //create brand new school 
                            Response.Write("New Class created in ASSISTments! <br /> ");

                            string courseName = "Edmodo Math Class";
                            string courseNumber = "1";
                            string courseSection = "1";

                            classRef = Class.CreateNew(courseName, courseNumber, courseSection, onBehalfOf);
                            DataAccess.setClassRef(classRef);
                        }

                        // create assingment                
                        bool assingmentExist = DataAccess.isAssignmentAvailable(); //false if not exist
                        if (assingmentExist)
                        {
                            assignmentRef = DataAccess.getAssignmentRef();
                            // if exist in DB
                            // if exist in ASSISTments
                            if (Assignment.isAssignmentExist(assignmentRef,onBehalfOf))
                            {
                                Response.Write("Assignment already in ASSISTments! <br /> ");
                            }
                            else
                            {
                                // else deleted by backup
                                Response.Write("Assignment recreated because of deleting data during backup! <br /> ");
                                assignmentRef = Assignment.CreateAssignment(Global.problemSetId, classRef, onBehalfOf);
                                DataAccess.setAssignmentRef(assignmentRef);
                            }

                        }
                        else //not exist
                        {
                            //create brand new school 
                            Response.Write("New Assignment created in ASSISTments! <br /> ");
                            assignmentRef = Assignment.CreateAssignment(Global.problemSetId, classRef, onBehalfOf);
                            DataAccess.setAssignmentRef(assignmentRef);

                        }

                        // enroll in school
                        List<LinkPrincipalUser> schoolUsers = DataAccess.getUserFromDB();
                        IEnumerator<LinkPrincipalUser> schoolUser = schoolUsers.GetEnumerator();
                        while (schoolUser.MoveNext())
                        {
                            School.enrollUserInSchool(schoolUser.Current.user, schoolRef, onBehalfOf);
                            Response.Write("user_ref " + schoolUser.Current.user + " enrolled in school! <br /> ");
                        }

                        //enroll in class
                        List<LinkPrincipalUser> classUsers = DataAccess.getStuentFromDB();
                        IEnumerator<LinkPrincipalUser> classUser = schoolUsers.GetEnumerator();
                        while (classUser.MoveNext())
                        {
                            if (Class.enrollStuentInClass(classUser.Current.user, classRef, onBehalfOf))
                                Response.Write("user_ref " + classUser.Current.user + " successfully enrolled in class! <br /> ");
                            else
                                Response.Write("NOT Enrolled because user_ref " + classUser.Current.user + " already in class! <br /> ");
                        }

                        break;
                }

            } 
            else //if (Request.QueryString["launch_key"] != null)
            {
                
            } // end if user_token
            


            //ASSISTmentsUserLogin();
        }

        protected void GetEdmodoUser(string launch_key)
        {
            string user_type = "";
            string user_token = "";
            string access_token = "";
            string first_name = "";
            string last_name = "";
            string group_id = "";
            string is_owner = "";
           

            string launchRequest = String.Format("{0}/launchRequests?api_key={1}&launch_key={2}",
                                                    Global.EdmodoAPIBase,
                                                    Global.EdmodoAPIKey,
                                                    launch_key);
            string jsonReturn = APIClient.doGET(launchRequest);

            User user = JsonConvert.DeserializeObject<User>(jsonReturn);

            first_name = user.first_name;
            last_name = user.last_name;
            user_type = user.user_type;
            user_token = user.user_token;
            access_token = user.access_token;

            IEnumerator<Group> iGroup = user.groups.GetEnumerator();
            while (iGroup.MoveNext())
            {
                group_id = iGroup.Current.group_id;
                is_owner = iGroup.Current.is_owner;
            }
            Session["user_type"] = user_type;
            Session["user_token"] = user_token;
            Session["access_token"] = access_token;
            Session["first_name"] = first_name;
            Session["last_name"] = last_name;
            Session["group_id"] = group_id;
            Session["is_owner"] = is_owner;

        }

        protected void ASSISTmentsUserLogin(string user_type,string userOnBehalfOf,string linkToGo)
        {
            
            var client = new RestClient(Global.ASSITmentsAPI_Helper);
            var request = new RestRequest("user_login?partner={partner}&access={access}&on_success={on_success}&on_failure={on_failure}", Method.GET);
            request.AddParameter("partner", "Hien-Ref", ParameterType.UrlSegment);
            if (user_type == "STUDENT")
            {
                request.AddParameter("access", userOnBehalfOf, ParameterType.UrlSegment);
                request.AddParameter("on_success", linkToGo, ParameterType.UrlSegment);
            }
            else if (user_type == "TEACHER")
            {
                //request.AddParameter("access", Global.OnBehalfOf, ParameterType.UrlSegment);
                request.AddParameter("access", userOnBehalfOf, ParameterType.UrlSegment);
                request.AddParameter("on_success", linkToGo, ParameterType.UrlSegment);
            }
            
            request.AddParameter("on_failure", "http://localhost:36031/ErrorPage.aspx", ParameterType.UrlSegment);
            var response = client.Execute(request);

            string redirectURL = String.Format("https://test1.assistments.org/api2_helper/user_login?partner={0}&access={1}&on_success={2}&on_failure={3}", "Hien-Ref", userOnBehalfOf, linkToGo, "http://localhost:36031/ErrorPage.aspx");
            Response.Redirect(redirectURL);
        }

        protected void teacherTutor1_Click(Object sender, EventArgs e)
        {
            string userToken = DataAccess.getEdmodoTeacherToken();
            string OnBehalfOf = DataAccess.getASSISTmentsTeacherOnBehalfOf(userToken);
            string assingment_ref = DataAccess.getAssignmentRef();
            string assingmentHandler = Assignment.getAssignmentHandler(assingment_ref, OnBehalfOf);
            assingmentHandler = assingmentHandler.Substring(0, 53);
            string linkToGo = "https://test1.assistments.org" + assingmentHandler;
            ASSISTmentsUserLogin(edmodoUserType, OnBehalfOf, linkToGo);
        }


        protected void goToViewProlem(Object sender, EventArgs e)
        {
            Response.Redirect("default.aspx");
        }

        protected void teacherReport1_Click(Object sender, EventArgs e)
        {
            string userToken = DataAccess.getEdmodoTeacherToken();
            string OnBehalfOf = DataAccess.getASSISTmentsTeacherOnBehalfOf(userToken);
            string assingment_ref = DataAccess.getAssignmentRef();
            string reportHandler = Report.getReportHandler(assingment_ref, OnBehalfOf);
            reportHandler = reportHandler.Substring(0, 53);
            string linkToGo = "https://test1.assistments.org" + reportHandler;
            ASSISTmentsUserLogin(edmodoUserType, OnBehalfOf, linkToGo);
        }
    }
}