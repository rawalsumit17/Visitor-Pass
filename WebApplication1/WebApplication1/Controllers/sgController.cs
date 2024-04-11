using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using WebApplication1.Models;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;
using System.Net.Http;

namespace WebApplication1.Controllers
{
    public class sgController : Controller
    {

        SqlConnection con = new SqlConnection(@"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\user\OneDrive\Desktop\Project\WebApplication1\WebApplication1\App_Data\Database.mdf;Integrated Security=True");


        public ActionResult Index()
        {
            return View();
        }
        // GET: sg
        [HttpPost]
        public ActionResult Index(string Imagename)
        {
            ViewBag.pic = "~/Content/capture" + Session["val"].ToString();
            return View();
        }


        public ActionResult Sdashbord()
        {
            if (Session["email"] == null)
            {

                return RedirectToAction("slogin");
            }
            con.Open();
            SqlCommand cmd1 = new SqlCommand("Select Count(DISTINCT v_id) from Visitor", con);
            object CAN1 = cmd1.ExecuteScalar();
            TempData["Msg"] = CAN1.ToString();


            SqlCommand cmd = new SqlCommand("SELECT COUNT(DISTINCT v_id) FROM Visitor WHERE Status = 0", con);


            object CAN2 = cmd.ExecuteScalar();
            TempData["Msg1"] = CAN2.ToString();

            return View();
        }

        public ActionResult addvisitor()
        {
            return View();
        }
        //[HttpPost]
        //public ActionResult addvisitor(string fn_name, string ln_name, string v_email, string v_phno, string v_date, string v_intime, string v_outtime, string v_address, string v_stf_id, string v_photo, string Status, FormCollection data)
        //{

        //    byte[] imageBytes = Convert.FromBase64String("AABCGHJKIOHHHJJloiuytreqwaskjkkkkkkkkkkkkkkkkkkpoppppppppppp");
        //    Session["v_email"] = v_email;
        //    con.Open();
        //    SqlCommand cmd2 = new SqlCommand("Select * from Visitor where  v_email='" + v_email + "'", con);
        //    cmd2.ExecuteScalar();
        //    SqlDataAdapter da2 = new SqlDataAdapter(cmd2);
        //    DataSet ds2 = new DataSet();
        //    da2.Fill(ds2);
        //    con.Close();
        //    if (ds2.Tables[0].Rows.Count > 0)
        //    {
        //        TempData["msg1"] = "You Have already registered email";
        //        return RedirectToAction("addvisitor");
        //    }
        //    else
        //    {


        //        con.Open();
        //        SqlCommand cmd = new SqlCommand("Insert INTO Visitor values(@fn_name,@ln_name,@v_email,@v_phno,@v_date,@v_intime,@v_outtime,@v_address,@v_sg_id,@v_stf_id,@v_photo,@Status)", con);
        //        cmd.Parameters.AddWithValue("@fn_name", fn_name);
        //        cmd.Parameters.AddWithValue("@ln_name", ln_name);
        //        cmd.Parameters.AddWithValue("@v_email", v_email);
        //        cmd.Parameters.AddWithValue("@v_phno", v_phno);
        //        cmd.Parameters.AddWithValue("@v_date", v_date);
        //        cmd.Parameters.AddWithValue("@v_intime", v_intime);
        //        cmd.Parameters.AddWithValue("@v_outtime", v_intime);
        //        cmd.Parameters.AddWithValue("@v_address", v_address);
        //        cmd.Parameters.AddWithValue("@v_sg_id", 1);
        //        cmd.Parameters.AddWithValue("@v_stf_id", v_stf_id);
        //        cmd.Parameters.AddWithValue("@v_photo", imageBytes);
        //        cmd.Parameters.AddWithValue("@Status", 0);




        //        cmd.ExecuteNonQuery();
        //        con.Close();


        //    }
        //    TempData["msg"] = "Visitor  Registered Successfully";
        //    return View();




        //}


        [HttpPost]
        public JsonResult SaveCapture(string fn_name, string ln_name, string v_email, string v_phno, string v_intime, string v_address, string v_stf_id, DateTime v_date, string imageData)
        {

            
           

            string fileName = fn_name + ln_name;
            string sg_id = Session["sg_id"].ToString();
            byte[] imageBytes = Convert.FromBase64String(imageData.Split(',')[1]);
            //string filePath = Server.MapPath(string.Format("~/Content/capture/{0}.jpg", fileName));
            string filePath = Server.MapPath($"~/Content/capture/{fileName}.jpg");
            string relativepath = ($"~/Content/capture/{fileName}.jpg");



            using (Image image = Image.FromStream(new MemoryStream(imageBytes)))
            {
                image.Save(filePath, ImageFormat.Jpeg);  // Or Png
            }


            con.Open();

            SqlCommand cmd = new SqlCommand("Insert into Visitor values(@fn_name,@ln_name,@v_email,@v_phno,@v_date,@v_intime,@v_outtime,@v_address,@v_sg_id,@v_stf_id,@v_photo,@Status,@email_status)", con);
            cmd.Parameters.AddWithValue("@fn_name", fn_name);
            cmd.Parameters.AddWithValue("@ln_name", ln_name);
            cmd.Parameters.AddWithValue("@v_email", v_email);
            cmd.Parameters.AddWithValue("@v_phno", v_phno);
            cmd.Parameters.AddWithValue("@v_date", v_date);
            cmd.Parameters.AddWithValue("@v_intime", v_intime);
            cmd.Parameters.AddWithValue("@v_outtime", v_intime);
            cmd.Parameters.AddWithValue("@v_address", v_address);
            cmd.Parameters.AddWithValue("@v_sg_id", sg_id);
            cmd.Parameters.AddWithValue("@v_stf_id", v_stf_id);
            cmd.Parameters.AddWithValue("@v_photo", relativepath);
            cmd.Parameters.AddWithValue("@Status", 0);
            cmd.Parameters.AddWithValue("@email_status", 0);
           

            cmd.ExecuteNonQuery();

            con.Close();

           //TempData["Msg1"] = "please check your email account";


            string redirectUrl = Url.Action("CurrentVisitor", "sg");

            return Json(new { success = true, redirectUrl });


            //  TempData["msg1"] = "Visitor Registered Successfully";

            //return Json("true");
            // return RedirectToAction("generateslip");


        }

        public string GenerateRandomOtp()
        {
            Random random = new Random();
            int otpnumber = random.Next(100000, 999999);

            return otpnumber.ToString();
        }



        public ActionResult generateslip(int v_id)
        {
            SqlCommand cmd = new SqlCommand("Select * from Visitor where status = 0 and v_id = '" + v_id + "'", con);
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
                pd.v_outtime = ds.Tables[0].Rows[i]["v_outtime"].ToString();
                pd.v_address = ds.Tables[0].Rows[i]["v_address"].ToString();
                pd.v_sg_id = ds.Tables[0].Rows[i]["v_sg_id"].ToString();
                pd.v_stf_id = ds.Tables[0].Rows[i]["v_stf_id"].ToString();
                pd.v_photo = ds.Tables[0].Rows[i]["v_photo"].ToString();
                pd.status = ds.Tables[0].Rows[i]["status"].ToString();
                //pd.unicode = ds.Tables[0].Rows[i]["unicode"].ToString();
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
            if (Session["email"] == null)
            {

                return RedirectToAction("slogin");
            }

            SqlCommand cmd = new SqlCommand("Select * from Visitor where status = 1", con);
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
                pd.v_outtime = ds.Tables[0].Rows[i]["v_outtime"].ToString();
                pd.v_address = ds.Tables[0].Rows[i]["v_address"].ToString();
                pd.v_sg_id = ds.Tables[0].Rows[i]["v_sg_id"].ToString();
                pd.v_stf_id = ds.Tables[0].Rows[i]["v_stf_id"].ToString();
                pd.v_photo = ds.Tables[0].Rows[i]["v_photo"].ToString();
                pd.status = ds.Tables[0].Rows[i]["status"].ToString();
                //pd.email_status = ds.Tables[0].Rows[i]["email_status"].ToString();
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

        public ActionResult slogin()
        {
            return View();
        }

        [HttpPost]
        public ActionResult slogin(string email1, string password1)

        {
            Response.Write(email1);
            Response.Write(password1);
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from sec_guard where email = '" + email1 + "' and password = '" + password1 + "' ", con);

            cmd.ExecuteScalar();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            if (ds.Tables[0].Rows.Count > 0)
            {

                Session["email"] = email1;
                string e = ds.Tables[0].Rows[0]["fn_name"].ToString();
                string sg_id = ds.Tables[0].Rows[0]["Id"].ToString();
                Session["fn_name"] = e;
                Session["sg_id"] = sg_id;

                return RedirectToAction("Sdashbord");

            }

            else
            {
                return RedirectToAction("slogin");
            }
            con.Close();
        }

        public ActionResult MyProfile()
        {
            return View();
        }


        [HttpPost]
        public ActionResult MyProfile(FormCollection data)
        {
            string password1 = data["password1"];
            // Response.Write(password1);
            string password2 = data["password2"];
            string password = data["password"];


            con.Open();
            SqlCommand cmd = new SqlCommand("select password from sec_guard where email = '" + Session["email"].ToString() + "'", con);
            cmd.ExecuteScalar();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                string e = ds.Tables[0].Rows[0]["password"].ToString();


                if (e == password1 && password2 == password)
                {

                    con.Open();
                    SqlCommand cmd1 = new SqlCommand("Update sec_guard set password = '" + password2 + "' where email = '" + Session["email"].ToString() + "'", con);
                    cmd1.ExecuteNonQuery();
                    con.Close();
                    TempData["msg"] = "Password Update Sucessfully";
                }
                else
                {
                    TempData["msg1"] = "Old Password is Incorrect or Re-entered Password isn't Match";
                }
            }


            return View();
        }

        public ActionResult LogOut()
        {
            TempData["msg2"] = "Log out Sucessfully";

            Session["email"] = null;
            return RedirectToAction("slogin");

        }

        public ActionResult sgForgot()
        {
            //TempData["msg2"] = "Log out Sucessfully";


            return View();

        }

        [HttpPost]
        public ActionResult sgForgot(FormCollection data)
        {
            string email = data["email"];


            con.Open();
            SqlCommand cmd = new SqlCommand("select password from sec_guard where email = '" + email + "'", con);
            cmd.ExecuteScalar();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                string e = ds.Tables[0].Rows[0]["password"].ToString();



                //con.Open();
                //SqlCommand cmd1 = new SqlCommand("Update Admin set a_password = '" + password2 + "' where a_email = '" + Session["a_email"].ToString() + "'", con);
                //cmd1.ExecuteNonQuery();
                //con.Close();
                TempData["msg"] = "Password Update Sucessfully";
            }
            else
            {
                TempData["msg1"] = "Not Registred With us";
            }



            return View();
        }


        public ActionResult CurrentVisitor()
        {
            if (Session["email"] == null)
            {

                return RedirectToAction("slogin");
            }
            if (Convert.ToString(Session["val"]) != string.Empty)
            {
                ViewBag.pic = "~/Content/capture" + Session["val"].ToString();
            }
            else
            {
                ViewBag.pic = "~/Content/Staff/assets/img/staff.jpg";
            }


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
                pd.status = ds.Tables[0].Rows[i]["status"].ToString();
                pd.email_status = ds.Tables[0].Rows[i]["email_status"].ToString();
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


        public ActionResult updatestatus(int v_id)
        {
            
            con.Open();
            SqlCommand cmd = new SqlCommand("update Visitor set status = 1,v_outtime = '" + DateTime.Now + "' where v_id = '" + v_id + "'", con);
            cmd.ExecuteNonQuery();
            TempData["msg"] = "Check Out Sucessfully";
            con.Close();

            return RedirectToAction("CurrentVisitor");
        }

        public ActionResult searchbyname(FormCollection data)
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


            return View("Viewvisitor", pd1);

        }
        public ActionResult searchbydate(FormCollection data)
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


            return View("Viewvisitor", pd1);

        }


        public ActionResult liveimage()
        {
            return View();
        }

        public ActionResult sendnotification(int v_id)
        {
            if (!HasInternetConnection())
            {
                TempData["Msg3"] = "You are not connected to the internet.";
                return RedirectToAction("CurrentVisitor", "sg");
            }


            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Visitor where v_id ='" + v_id + "'", con);
            cmd.ExecuteScalar();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (ds.Tables[0].Rows.Count > 0)
            {
                string fn_name = ds.Tables[0].Rows[0]["fn_name"].ToString();
                string ln_name = ds.Tables[0].Rows[0]["ln_name"].ToString();
                string v_email = ds.Tables[0].Rows[0]["v_email"].ToString();
                string v_intime = ds.Tables[0].Rows[0]["v_intime"].ToString();
                string v_date = ds.Tables[0].Rows[0]["v_date"].ToString();

                SqlCommand cmd1 = new SqlCommand("Update Visitor set email_status = 1 Where v_id ='" + v_id + "'", con);

                cmd1.ExecuteNonQuery();
                con.Close();

                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("rawalsumit69@gmail.com");
                msg.To.Add(v_email);
                msg.Subject = "Meeting";
                msg.Body = "Dear MR/Mrs. " + fn_name + ln_name + " Wants To Meet You At " + v_intime + " on " + v_date + ". Please Take note of it. <hr/>";
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


            }
            else
            {
                TempData["Msg2"] = "not registered";
            }

            return RedirectToAction("CurrentVisitor", "sg");
        }

        private bool HasInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }


        public ActionResult send(int v_id)
        {

            if (!HasInternetConnection())
            {
                TempData["Msg3"] = "You are not connected to the internet.";
                return RedirectToAction("CurrentVisitor", "sg");
            }

            string otp = GenerateRandomOtp();
            con.Open();
            SqlCommand cmd = new SqlCommand("select * from Visitor where v_id = '"+v_id+"' ", con);

            cmd.ExecuteScalar();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);
            con.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                
                string email = ds.Tables[0].Rows[0]["v_email"].ToString();
                MailMessage msg = new MailMessage();
                msg.From = new MailAddress("rawalsumit69@gmail.com");
                msg.To.Add(email);
                msg.Subject = "OTP Verification";
                msg.Body = "Your OTP is " + otp + ". Please do not share it. <hr/>";
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
                

            }
           // TempData["email"] = email;


            Session["otp"] = otp;
            Session["ot"] = v_id;
            return RedirectToAction("SendOtp", "sg");




        }

        public ActionResult SendOtp()
        {
            return View();
        }


        [HttpPost]
        public ActionResult SendOtp(FormCollection data)
        {
            string newotp = data["newotp"];
            //string otp = Session["ot"].ToString();
            string otp = data["otp"];
            int v_id = Convert.ToInt32(data["v_id"]);

            Response.Write(newotp);
            Response.Write(otp);


            if (otp == newotp)
            {
                
                return RedirectToAction("generateslip", new { v_id = v_id });

            }
            else
            {
                TempData["ErrorMessage"] = "Incorrect OTP. Please try again.";
                TempData["v_id"] = v_id;
                return RedirectToAction("SendOtp");
            }

        }

        public ActionResult Success()
        {
            return View();
        }

 

    }
}