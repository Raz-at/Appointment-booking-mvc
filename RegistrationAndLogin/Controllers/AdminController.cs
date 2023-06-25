using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegistrationAndLogin.Models;

namespace RegistrationAndLogin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(AdminLogin user)
        {
            MyLoginsEntities dc = new MyLoginsEntities();
            var adminEmail = IsEmailExist(user.EmailID);
            if (adminEmail)
            {
                ModelState.AddModelError("EmailExist", "Email already exists");
                return View(user);
            }
            else
            {
                dc.AdminLogins.Add(user);
                dc.SaveChanges();
                return View();
            }
        }

        #region checkEmail
        public bool IsEmailExist(string emailID)
        {
            using (MyLoginsEntities dc = new MyLoginsEntities())
            {
                var v = dc.AdminLogins.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v == null ? false : true;
            }
        }
        #endregion


    }
}