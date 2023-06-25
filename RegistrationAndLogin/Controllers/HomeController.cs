using System;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using RegistrationAndLogin.Models;

namespace RegistrationAndLogin.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        [Authorize]
        public ActionResult Index() //index
        {
            return View();
        }

        public ActionResult Home()
        {
            return View();
        }

        public ActionResult Contact()
        {

            return View();
        }


        #region message
        [HttpGet]
        public ActionResult about()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult about(message user_data)
        {
            var msg = "";
            using (MyLoginsEntities dc = new MyLoginsEntities())
            {
                dc.messages.Add(entity: user_data);
                dc.SaveChanges();
                msg = "success";
                ViewBag.message = msg;
                return View();
            }
        }

        [Authorize(Users = "sunshine_appointment@outlook.com")]
        public ActionResult view_message(string email)
        {
            MyLoginsEntities dc = new MyLoginsEntities();
            var email_comp = email;
            ViewBag.email = email_comp;
            var messages = dc.messages.ToList();
            return View(messages);
        }       

        public ActionResult delete_msg(int id)
        {
            var data = dc.messages.Where(x => x.Id == id).FirstOrDefault();
            dc.messages.Remove(data);
            dc.SaveChanges();
            ViewBag.message = "Data is deleted successfully";
            ViewBag.email = data.EmailID;
            return View();
        }
        #endregion



        #region Edit User Data
        
        public ActionResult user_view(string email)
        {
            MyLoginsEntities dc = new MyLoginsEntities();
            var email_user = email;
            ViewBag.email = email_user;
            var messages = dc.messages.ToList();
            return View(messages);
        }


        public ActionResult edit_data(int id) 
        {
            MyLoginsEntities dc = new MyLoginsEntities();
            var data = dc.messages.Where(a => a.Id == id).ToList();
            return View(data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult edit_data(message message)
        {
            if (ModelState.IsValid)
            {
                dc.Entry(message).State = EntityState.Modified;
                dc.SaveChanges();
                return RedirectToAction("user_view");
            }
            return View(message);
        }

        #endregion


        #region Dashboard
        MyLoginsEntities dc = new MyLoginsEntities();

        [Authorize(Users = "sunshine_appointment@outlook.com")]
        public ActionResult Dashboard()
        {
            
            var ListOfData = dc.Users.ToList();
            return View(ListOfData);
        }

        public ActionResult Detail(int id)
        {
            var data = dc.Users.FirstOrDefault(x => x.UserId == id);
            if (data == null)
            {
                return HttpNotFound();
            }

            return View(data);
        }

        public ActionResult delete(int id)
        {
            var data = dc.Users.Where(x => x.UserId == id).FirstOrDefault();
            dc.Users.Remove(data);
            dc.SaveChanges();
            ViewBag.message = "Data is deleted successfully";
            return RedirectToAction("Dashboard");
        }

        #endregion

        #region unauthorized
        public ActionResult Unauthorized()
        {
            return View();
        }
        #endregion


    }
}