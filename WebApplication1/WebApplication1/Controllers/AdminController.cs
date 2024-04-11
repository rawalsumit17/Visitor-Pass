
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Http;



namespace WebApplication1.Controllers
{


    public class AdminController : Controller
    {
        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\Project\WebApplication1\WebApplication1\App_Data\Database.mdf;Integrated Security=True");
        // GET: Admin
        public ActionResult Index()
        {
            var dailyVisitorCount = GetDailyVisitorCount(); // This method should retrieve the data
            ViewBag.DailyVisitorCount = dailyVisitorCount;
            return View();
        }

        public ActionResult Dashboard(string graphType = "month", int year = 0, int month = 0)
        {
            con.Open();
            SqlCommand cmd1 = new SqlCommand("Select Count(DISTINCT v_id) from Visitor", con);
            object CAN1 = cmd1.ExecuteScalar();
            TempData["Msg"] = CAN1.ToString();


            SqlCommand cmd2 = new SqlCommand("SELECT COUNT(DISTINCT v_id) FROM Visitor WHERE Status = 0", con);
            object CAN2 = cmd2.ExecuteScalar();
            TempData["Msg1"] = CAN2.ToString();



            //SqlCommand cmd = new SqlCommand("SELECT CONVERT(date, v_date) AS VisitDate, COUNT(*) AS DacilyVisitorCount FROM Visitor WHERE DATEPART(YEAR, v_date) = DATEPART(YEAR, GETDATE()) AND DATEPART(MONTH, v_date) = DATEPART(MONTH, GETDATE()) GROUP BY CONVERT(date, v_date)", con);
            //SqlDataReader rdr = cmd.ExecuteReader();

            List<string> dates = new List<string>();
            List<int> counts = new List<int>();

            if (graphType == "year")
            {
                SqlCommand cmd = new SqlCommand("SELECT CONVERT(date, v_date) AS VisitDate, COUNT(*) AS DailyVisitorCount FROM Visitor WHERE DATEPART(YEAR, v_date) = @Year GROUP BY CONVERT(date, v_date)", con);
                cmd.Parameters.AddWithValue("@Year", year);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    dates.Add(Convert.ToDateTime(rdr["VisitDate"]).ToShortDateString());
                    counts.Add(Convert.ToInt32(rdr["DailyVisitorCount"]));
                }
                rdr.Close();
            }
            else if (graphType == "month")
            {
                SqlCommand cmd = new SqlCommand("SELECT CONVERT(date, v_date) AS VisitDate, COUNT(*) AS DailyVisitorCount FROM Visitor WHERE DATEPART(YEAR, v_date) = @Year AND DATEPART(MONTH, v_date) = @Month GROUP BY CONVERT(date, v_date)", con);
                cmd.Parameters.AddWithValue("@Year", year);
                cmd.Parameters.AddWithValue("@Month", month);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    dates.Add(Convert.ToDateTime(rdr["VisitDate"]).ToShortDateString());
                    counts.Add(Convert.ToInt32(rdr["DailyVisitorCount"]));
                }

                rdr.Close();
            }
                con.Close();

                // Generating all dates in the month
                DateTime startDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                DateTime endDate = startDate.AddMonths(1).AddDays(-1);

                List<string> allDatesInMonth = new List<string>();
                for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    allDatesInMonth.Add(date.ToShortDateString());
                }

                List<int> countsForAllDates = new List<int>();
                foreach (string date in allDatesInMonth)
                {
                    int index = dates.IndexOf(date);
                    if (index != -1)
                    {
                        countsForAllDates.Add(counts[index]);
                    }
                    else
                    {
                        countsForAllDates.Add(0);
                    }
                }

                TempData["Dates"] = allDatesInMonth;
                TempData["Counts"] = countsForAllDates;

                return View();

            
        }


        


            public ActionResult Viewsecurity()
        {
            if (Session["a_email"] == null)
            {

                return RedirectToAction("Login");
            }

            SqlCommand cmd = new SqlCommand("Select * from sec_guard", con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);
            guard pd1 = new guard();
            List<guard> guard_details = new List<guard>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                guard pd = new guard();
                pd.Id = Convert.ToInt32(ds.Tables[0].Rows[i]["Id"].ToString());
                pd.fn_name = ds.Tables[0].Rows[i]["fn_name"].ToString();
                pd.ln_name = ds.Tables[0].Rows[i]["ln_name"].ToString();
                pd.email = ds.Tables[0].Rows[i]["email"].ToString();

                pd.phone_no = ds.Tables[0].Rows[i]["phone_no"].ToString();
                pd.address = ds.Tables[0].Rows[i]["address"].ToString();
                guard_details.Add(pd);
            }
            pd1.guard_details = guard_details;
            con.Close();

            return View(pd1);



        }

        public ActionResult Addsecurityg()
        {
            if (Session["a_email"] == null)
            {

                return RedirectToAction("Login");
            }
            return View();
        }

        public ActionResult Addstaff()
        {
            if (Session["a_email"] == null)
            {

                return RedirectToAction("Login");
            }
            return View();
        }
        public ActionResult Viewstaff()
        {
            if (Session["a_email"] == null)
            {

                return RedirectToAction("Login");
            }

            SqlCommand cmd = new SqlCommand("Select * from staff", con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);
            staff pd1 = new staff();
            List<staff> staff_details = new List<staff>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                staff pd = new staff();
                pd.stf_id = Convert.ToInt32(ds.Tables[0].Rows[i]["stf_id"].ToString());
                pd.fn_name = ds.Tables[0].Rows[i]["fn_name"].ToString();
                pd.ln_name = ds.Tables[0].Rows[i]["ln_name"].ToString();
                pd.email = ds.Tables[0].Rows[i]["email"].ToString();
                pd.phone_no = ds.Tables[0].Rows[i]["phone_no"].ToString();
                pd.address = ds.Tables[0].Rows[i]["address"].ToString();
                staff_details.Add(pd);
            }
            pd1.staff_details = staff_details;
            con.Close();


            return View(pd1);
        }

        public ActionResult CurrentVadmin()
        {
            if (Session["a_email"] == null)
            {

                return RedirectToAction("Login");
            }
            //if (Convert.ToString(Session["val"]) != string.Empty)
            //{
            //    ViewBag.pic = "~/Content/capture" + Session["val"].ToString();
            //}
            //else
            //{
            //    ViewBag.pic = "~/Content/Staff/assets/img/staff.jpg";
            //}


            SqlCommand cmd = new SqlCommand("Select * from Visitor where status = 0", con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);
            visitor pd1 = new visitor();
            List<visitor> Visitor_details = new List<visitor>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                visitor pd = new visitor();
                pd.v_id = Convert.ToInt32(ds.Tables[0].Rows[i]["v_id"].ToString());
                pd.fn_name = ds.Tables[0].Rows[i]["fn_name"].ToString();
                pd.ln_name = ds.Tables[0].Rows[i]["ln_name"].ToString();
                pd.v_email = ds.Tables[0].Rows[i]["v_email"].ToString();
                pd.v_phno = ds.Tables[0].Rows[i]["v_phno"].ToString();
                string Date = ds.Tables[0].Rows[i]["v_date"].ToString();
                //pd.v_date = ds.Tables[0].Rows[i]["v_date"].ToString();
                pd.v_intime = ds.Tables[0].Rows[i]["v_intime"].ToString();
                //pd.v_outtime = ds.Tables[0].Rows[i]["v_outtime"].ToString();
                pd.v_address = ds.Tables[0].Rows[i]["v_address"].ToString();
                pd.v_sg_id = ds.Tables[0].Rows[i]["v_sg_id"].ToString();
                pd.v_stf_id = ds.Tables[0].Rows[i]["v_stf_id"].ToString();
                pd.v_photo = ds.Tables[0].Rows[i]["v_photo"].ToString();
                Visitor_details.Add(pd);

                DateTime dt;
                dt = Convert.ToDateTime(Date);
                string dt1 = dt.ToString("dd/MM/yyyy");
                pd.v_date = dt1;
            }
            pd1.visitor_details = Visitor_details;
            con.Close();


            return View(pd1);
        }

        public ActionResult Viewvisitor()
        {
            {
                if (Session["a_email"] == null)
                {

                    return RedirectToAction("Login");
                }

                SqlCommand cmd = new SqlCommand("Select * from visitor where status = 1", con);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                visitor pd1 = new visitor();
                List<visitor> visitor_details = new List<visitor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    visitor pd = new visitor();
                    pd.v_id = Convert.ToInt32(ds.Tables[0].Rows[i]["v_id"].ToString());
                    pd.fn_name = ds.Tables[0].Rows[i]["fn_name"].ToString();
                    pd.ln_name = ds.Tables[0].Rows[i]["ln_name"].ToString();
                    pd.v_email = ds.Tables[0].Rows[i]["v_email"].ToString();
                    pd.v_phno = ds.Tables[0].Rows[i]["v_phno"].ToString();
                    // pd.v_date = ds.Tables[0].Rows[i]["v_date"].ToString();
                    string date = ds.Tables[0].Rows[i]["v_date"].ToString();
                    pd.v_intime = ds.Tables[0].Rows[i]["v_intime"].ToString();
                    pd.v_outtime = ds.Tables[0].Rows[i]["v_outtime"].ToString();
                    pd.v_address = ds.Tables[0].Rows[i]["v_address"].ToString();
                    pd.v_sg_id = ds.Tables[0].Rows[i]["v_sg_id"].ToString();
                    pd.v_stf_id = ds.Tables[0].Rows[i]["v_stf_id"].ToString();
                    pd.v_photo = ds.Tables[0].Rows[i]["v_photo"].ToString();
                    visitor_details.Add(pd);
                    DateTime dt;
                    dt = Convert.ToDateTime(date);
                    string dt1 = dt.ToString("dd/MM/yyyy");
                    pd.v_date = dt1;
                }
                pd1.visitor_details = visitor_details;
                con.Close();


                return View(pd1);
            }
        }
        public ActionResult Check_in_Check_out()
        {
            if (Session["a_email"] == null)
            {

                return RedirectToAction("Login");
            }

            return View();
        }


        public ActionResult Login()
        {
            return View();
        }


        [HttpPost]
        public ActionResult Login(string email1, string password1)
        {

            Response.Write(email1);
            Response.Write(password1);
            con.Open();

            SqlCommand cmd = new SqlCommand("Select * from Admin where a_email='" + email1 + "'  and a_password='" + password1 + "' ", con);
            cmd.ExecuteScalar();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {

                Session["a_email"] = email1;
                string e = ds.Tables[0].Rows[0]["a_name"].ToString();
                Session["a_name"] = e;

                TempData["msg2"] = "Login Sucessfully";
                return RedirectToAction("Dashboard");

            }

            else
            {
                TempData["msg3"] = "Invalid Email or Password";
                return RedirectToAction("Login");
            }
            con.Close();

        }

        [HttpPost]
        public ActionResult Addsecurityg(string fn, string ln, string password, string email, string phno, string address)
        {

            con.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO sec_guard values(@fn_name,@ln_name,@email,@phone_no,@password,@address) ", con);
            cmd.Parameters.AddWithValue("@fn_name", fn);
            cmd.Parameters.AddWithValue("@ln_name", ln);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone_no", phno);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@address", address);





            cmd.ExecuteNonQuery();

            con.Close();
            return View();


        }


        [HttpPost]
        public ActionResult Addstaff(string fn, string ln, string password, string email, string ph_no, string address)
        {

            con.Open();

            SqlCommand cmd = new SqlCommand("INSERT INTO staff values(@fn_name,@ln_name,@email,@phone_no,@password,@address) ", con);
            cmd.Parameters.AddWithValue("@fn_name", fn);
            cmd.Parameters.AddWithValue("@ln_name", ln);
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@phone_no", ph_no);
            cmd.Parameters.AddWithValue("@password", password);
            cmd.Parameters.AddWithValue("@address", address);





            cmd.ExecuteNonQuery();

            con.Close();
            return View();


        }

        [HttpPost]

        public ActionResult delete_guard(FormCollection data)
        {
            string Id = data["id"];
            con.Open();
            SqlCommand cmd = new SqlCommand("delete from sec_guard where Id = '" + Id + "'", con);
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Viewsecurity");
        }

        [HttpPost]
        public ActionResult Edit_guard(FormCollection data)
        {
            string Id = data["id"];
            string phno = data["phno"];
            string address = data["address"];
            con.Open();
            SqlCommand cmd = new SqlCommand("Update sec_guard set phone_no='" + phno + "' ,address='" + address + "' where Id = '" + Id + "'", con);
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Viewsecurity");
        }



        [HttpPost]

        public ActionResult delete_staff(FormCollection data)
        {
            string stf_id = data["stf_id"];
            con.Open();
            SqlCommand cmd = new SqlCommand("delete from staff where stf_id = '" + stf_id + "'", con);
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Viewstaff");
        }

        [HttpPost]
        public ActionResult Edit_Staff(FormCollection data)
        {
            string Id = data["id"];
            string phno = data["phno"];
            string address = data["address"];
            con.Open();
            SqlCommand cmd = new SqlCommand("Update staff set phone_no='" + phno + "' ,address='" + address + "' where stf_id = '" + Id + "'", con);
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("Viewstaff");
        }

        //public ActionResult Edit_profile(FormCollection data)
        //{
        //    string Id = data["id"];
        //    string password = data["password"];

        //    con.Open();
        //    SqlCommand cmd = new SqlCommand("Update sec_guard set password='" + password + "' where Id = '" + Id + "'", con);
        //    cmd.ExecuteNonQuery();
        //    con.Close();

        //    return RedirectToAction("Viewsecurity");
        //}


        public ActionResult AdminProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AdminProfile(FormCollection data)
        {
            string password1 = data["password1"];
            // Response.Write(password1);
            string password2 = data["password2"];
            string password = data["password"];


            con.Open();
            SqlCommand cmd = new SqlCommand("select a_password from Admin where a_email = '" + Session["a_email"].ToString() + "'", con);
            cmd.ExecuteScalar();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                string e = ds.Tables[0].Rows[0]["a_password"].ToString();
                Response.Write(e);

                if (e == password1 && password2 == password)
                {



                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("Update Admin set a_password = '" + password2 + "' where a_email = '" + Session["a_email"].ToString() + "'", con);
                    cmd1.ExecuteNonQuery();
                    con.Close();
                    TempData["msg"] = "Password Update Sucessfully";
                }
                else
                {
                    TempData["msg1"] = "does not match";
                }
            }


            return View();
        }

        public ActionResult LogOut()
        {
            TempData["msg2"] = "Log out Sucessfully";

            Session["a_email"] = null;
            return RedirectToAction("Login");

        }

        public ActionResult EditProfile()
        {
            if (Session["a_email"] == null)
            {

                return RedirectToAction("Login");
            }

            SqlCommand cmd = new SqlCommand("Select * from Admin", con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);
            admin pd1 = new admin();
            List<admin> admin_details = new List<admin>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                admin pd = new admin();
                pd.a_id = Convert.ToInt32(ds.Tables[0].Rows[i]["a_id"].ToString());
                pd.a_name = ds.Tables[0].Rows[i]["a_name"].ToString();
                //pd.ln_name = ds.Tables[0].Rows[i]["ln_name"].ToString();
                pd.a_email = ds.Tables[0].Rows[i]["a_email"].ToString();
                pd.a_phno = ds.Tables[0].Rows[i]["a_phno"].ToString();
                //pd.address = ds.Tables[0].Rows[i]["address"].ToString();
                admin_details.Add(pd);
            }
            pd1.admin_details = admin_details;
            con.Close();


            return View(pd1);
        }

        [HttpPost]
        public ActionResult Edit_Profile(FormCollection data)
        {
            string Id = data["id"];
            string name = data["name"];
            string email = data["email"];
            string phno = data["phno"];

            con.Open();
            SqlCommand cmd = new SqlCommand("Update Admin set a_name='" + name + "' ,a_email='" + email + "',a_phno='" + phno + "' where a_id = '" + Id + "'", con);
            cmd.ExecuteNonQuery();
            con.Close();

            return RedirectToAction("EditProfile");
        }

        public ActionResult searchname(FormCollection data)
        {
            if (string.IsNullOrEmpty("name"))
            {
                return RedirectToAction("Viewvisitor");
            }
            else
            {
                string name = data["name"];
                SqlCommand cmd = new SqlCommand("select * from Visitor where fn_name like '%" + name + "%'", con);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                visitor pd1 = new visitor();
                List<visitor> Visitor_details = new List<visitor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    visitor pd = new visitor();
                    pd.v_id = Convert.ToInt32(ds.Tables[0].Rows[i]["v_id"].ToString());
                    pd.fn_name = ds.Tables[0].Rows[i]["fn_name"].ToString();
                    pd.ln_name = ds.Tables[0].Rows[i]["ln_name"].ToString();
                    pd.v_email = ds.Tables[0].Rows[i]["v_email"].ToString();
                    pd.v_phno = ds.Tables[0].Rows[i]["v_phno"].ToString();
                    string Date = ds.Tables[0].Rows[i]["v_date"].ToString();
                    // pd.v_date = ds.Tables[0].Rows[i]["v_date"].ToString();
                    pd.v_intime = ds.Tables[0].Rows[i]["v_intime"].ToString();
                    pd.v_outtime = ds.Tables[0].Rows[i]["v_outtime"].ToString();
                    pd.v_address = ds.Tables[0].Rows[i]["v_address"].ToString();
                    pd.v_sg_id = ds.Tables[0].Rows[i]["v_sg_id"].ToString();
                    pd.v_stf_id = ds.Tables[0].Rows[i]["v_stf_id"].ToString();
                    pd.v_photo = ds.Tables[0].Rows[i]["v_photo"].ToString();

                    Visitor_details.Add(pd);

                    DateTime dt;
                    dt = Convert.ToDateTime(Date);
                    string dt1 = dt.ToString("dd/MM/yyyy");
                    pd.v_date = dt1;
                }
                pd1.visitor_details = Visitor_details;
                con.Close();


                return View(pd1);
            }

        }
        public ActionResult searchdate(FormCollection data)
        {
            if (string.IsNullOrEmpty("v_date"))
            {
                return RedirectToAction("Viewvisitor");
            }
            else
            {
                string v_date = data["v_date"];
                DateTime db;
                db = Convert.ToDateTime(v_date);
                string dt1 = db.ToString("dd-MM-yyyy");


                SqlCommand cmd = new SqlCommand("select * from Visitor where v_date = '" + v_date + "'", con);
                SqlDataAdapter da = new SqlDataAdapter();
                da.SelectCommand = cmd;
                DataSet ds = new DataSet();
                da.Fill(ds);
                visitor pd1 = new visitor();
                List<visitor> Visitor_details = new List<visitor>();
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    visitor pd = new visitor();
                    pd.v_id = Convert.ToInt32(ds.Tables[0].Rows[i]["v_id"].ToString());
                    pd.fn_name = ds.Tables[0].Rows[i]["fn_name"].ToString();
                    pd.ln_name = ds.Tables[0].Rows[i]["ln_name"].ToString();
                    pd.v_email = ds.Tables[0].Rows[i]["v_email"].ToString();
                    pd.v_phno = ds.Tables[0].Rows[i]["v_phno"].ToString();
                    string Date = ds.Tables[0].Rows[i]["v_date"].ToString();
                    // pd.v_date = ds.Tables[0].Rows[i]["v_date"].ToString();
                    pd.v_intime = ds.Tables[0].Rows[i]["v_intime"].ToString();
                    pd.v_outtime = ds.Tables[0].Rows[i]["v_outtime"].ToString();
                    pd.v_address = ds.Tables[0].Rows[i]["v_address"].ToString();
                    pd.v_sg_id = ds.Tables[0].Rows[i]["v_sg_id"].ToString();
                    pd.v_stf_id = ds.Tables[0].Rows[i]["v_stf_id"].ToString();
                    pd.v_photo = ds.Tables[0].Rows[i]["v_photo"].ToString();


                    Visitor_details.Add(pd);
                    DateTime dt;
                    dt = Convert.ToDateTime(Date);
                    string dt2 = dt.ToString("dd/MM/yyyy");

                    pd.v_date = dt2;

                }
                pd1.visitor_details = Visitor_details;
                con.Close();


                return View(pd1);
            }

        }


        public ActionResult AdminForgot()
        {
            //TempData["msg2"] = "Log out Sucessfully";


            return View();

        }

        [HttpPost]
        public ActionResult AdminForgot(FormCollection data)
        {
            string email = data["email"];


            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Admin where a_email = '" + email + "'", con);
            cmd.ExecuteScalar();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();

            da.Fill(ds);
            con.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                string a_email = ds.Tables[0].Rows[0]["a_email"].ToString();
                string a_password = ds.Tables[0].Rows[0]["a_password"].ToString();
                con.Close();

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("rawalsumit69@gmail.com");
                msg.To.Add(a_email);
                msg.Subject = "Password";
                msg.Body = "<hr>" + a_email + "Your Password is" + a_password + "Do not Share with Anyone<hr/>";
                msg.IsBodyHtml = true;

                SmtpClient smt = new SmtpClient();
                smt.Host = "smtp.gmail.com";
                System.Net.NetworkCredential ntwd = new NetworkCredential();
                ntwd.UserName = "rawalsumit69@gmail.com";
                ntwd.Password = "yhlk ixij qybx xjeb";
                smt.UseDefaultCredentials = true;
                smt.Credentials = ntwd;
                smt.Port = 587;
                smt.EnableSsl = true;

                smt.Send(msg);
                TempData["Msg1"] = "please check your email account";
                return View();
            }
            else
            {
                TempData["msg1"] = "Not Registred With us";
                return View();
            }


        }




        private List<int> GetDailyVisitorCount()
        {
            // Logic to fetch daily visitor count data from the database
            // Example data for demonstration
            return new List<int> { 100, 150, 200, 180, 220, 250, 300 };
        }

        public ActionResult generateReport(FormCollection data)
        {

            string start_date = data["start_date"];
            string end_date = data["end_date"];



            SqlCommand cmd = new SqlCommand("select * from Visitor where v_date BETWEEN '" + start_date + "' AND '" + end_date + "'", con);
            SqlDataAdapter da = new SqlDataAdapter();
            da.SelectCommand = cmd;
            DataSet ds = new DataSet();
            da.Fill(ds);
            visitor pd1 = new visitor();
            List<visitor> Visitor_details = new List<visitor>();
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                visitor pd = new visitor();
                pd.v_id = Convert.ToInt32(ds.Tables[0].Rows[i]["v_id"].ToString());
                pd.fn_name = ds.Tables[0].Rows[i]["fn_name"].ToString();
                pd.ln_name = ds.Tables[0].Rows[i]["ln_name"].ToString();
                pd.v_email = ds.Tables[0].Rows[i]["v_email"].ToString();
                pd.v_phno = ds.Tables[0].Rows[i]["v_phno"].ToString();
                string Date = ds.Tables[0].Rows[i]["v_date"].ToString();
                // pd.v_date = ds.Tables[0].Rows[i]["v_date"].ToString();
                pd.v_intime = ds.Tables[0].Rows[i]["v_intime"].ToString();
                pd.v_outtime = ds.Tables[0].Rows[i]["v_outtime"].ToString();
                pd.v_address = ds.Tables[0].Rows[i]["v_address"].ToString();
                pd.v_sg_id = ds.Tables[0].Rows[i]["v_sg_id"].ToString();
                pd.v_stf_id = ds.Tables[0].Rows[i]["v_stf_id"].ToString();
                pd.v_photo = ds.Tables[0].Rows[i]["v_photo"].ToString();


                Visitor_details.Add(pd);
                DateTime dt;
                dt = Convert.ToDateTime(Date);
                string dt2 = dt.ToString("dd/MM/yyyy");

                pd.v_date = dt2;

            }

            pd1.visitor_details = Visitor_details;
            con.Close();


            return View(pd1);

        }






    }
}
