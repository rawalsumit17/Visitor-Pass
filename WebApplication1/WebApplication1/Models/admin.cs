using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{

    public class admin
    {
        public int a_id { get; set; }
        public string a_name { get; set; }
        public string a_email { get; set; }
        public string a_phno { get; set; }
        public string a_password { get; set; }
        //public string address { get; set; }

        public List<admin> admin_details { get; set; }




    }
    public class DateRange
    {
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
    public class VisitorCountModel
    {
        public int DayOfWeek { get; set; }
        public int DailyVisitorCount { get; set; }
    }
    public class guard
    {
        public int Id { get; set; }
        public string fn_name { get; set; }
        public string ln_name { get; set; }
        public string email { get; set; }
        public string phone_no { get; set; }
        public string address { get; set; }

        public List<guard> guard_details { get; set; }




    }

    public class staff
    {
        public int stf_id { get; set; }
        public string fn_name { get; set; }
        public string ln_name { get; set; }
        public string email { get; set; }
        public string phone_no { get; set; }
        public string address { get; set; }

        public List<staff> staff_details { get; set; }




    }



    public class visitor
    {
        public int v_id { get; set; }
        public string fn_name { get; set; }
        public string ln_name { get; set; }
        public string v_email { get; set; }
        public string v_phno { get; set; }
        public string v_date { get; set; }
        public string v_intime { get; set; }

        public string v_outtime { get; set; }

        public string v_address { get; set; }

        public string v_sg_id { get; set; }
        public string v_stf_id { get; set; }

        public string v_photo { get; set; }
        public string status { get; set; }
        
        public string email_status { get; set; }
        public string unicode { get; set; }


        public List<visitor> visitor_details { get; set; }
    }



}