using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RegistrationAndLogin.Models;


namespace RegistrationAndLogin.Controllers
{
    public class UserController : Controller
    {

        #region Registration
        public ActionResult Registration()
        {
            return View();
        }

        //registration post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified , ActivationCode")] User user)
        {
            bool Status = false;
            string Message = "";
            if (ModelState.IsValid)
            {
                #region Check if email already exists
                var isExist = IsEmailExist(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exists");
                    return View(user);
                }
                #endregion

                #region Generate activation code
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region Password hashing
                user.Password = crypto.Hash(user.Password);
                user.ConfirmPassword = crypto.Hash(user.ConfirmPassword);
                #endregion

                user.IsEmailVerified = false;

                #region Save to database
                using (MyLoginsEntities dc = new MyLoginsEntities())
                {
                    dc.Users.Add(user);
                    dc.SaveChanges();

                    sendVerificationEmail(user.EmailID, user.ActivationCode.ToString());
                    Message = "Registration done " + "has been sent to your email " + user.EmailID;
                    Status = true;
                }
                #endregion
            }
            else
            {
                Message = "Invalid request";
            }

            ViewBag.Message = Message;
            ViewBag.Status = Status;
            return View(user);
        }
        #endregion


        #region verifyAccount
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (MyLoginsEntities dc = new MyLoginsEntities())
            {
                dc.Configuration.ValidateOnSaveEnabled = false;//this line is to avoid
                var v = dc.Users.Where(a =>a.ActivationCode == new Guid(id)).FirstOrDefault();                                              //confirm password doesnt work issuse on save change     
                if(v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }

            }
            ViewBag.Status = Status;
            return View();
        }
        #endregion


        #region login
        [HttpGet]
        public ActionResult login()
        {
            return View();
        }

        //login post
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult login(Userlogin login , string ReturnUrl="" )
        {
            string message = "";
            bool status = false;
            using (MyLoginsEntities dc = new MyLoginsEntities())
            {
                var v = dc.Users.Where(a => a.EmailID == login.EmailId).FirstOrDefault();
                if (v != null)
                { 
                    if(string.Compare(crypto.Hash(login.Password), v.Password) == 0)
                    {
                        status = true;
                        int timeout = login.RememberMe ? 525600 : 1;  //525600min
                        var ticket = new FormsAuthenticationTicket(login.EmailId, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);

                        if(Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("home", "Home");
                        }
                        
                    }
                    else
                    {
                        status = false;
                        message = "invalid Password";
                    }
                }
                else
                {
                    status = false;
                    message = "invalid Email ";
                }

            }
            ViewBag.message = message;
            ViewBag.status = status;
            return View();
        }
        #endregion


        #region logout
        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("login" , "User");
        }
        #endregion


        #region Forget password
        public ActionResult ForgetPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgetPassword(string Email)  //this email is received from the textbox in forgetpassword.cshtml
        {
            string message = "";
            bool status = false;
            using (MyLoginsEntities dc = new MyLoginsEntities())
            {
                var account = dc.Users.Where(x => x.EmailID == Email).FirstOrDefault();
                if(account != null)
                {
                    string resetCode = Guid.NewGuid().ToString();
                    sendVerificationEmail(account.EmailID, resetCode, "ResetPassword");

                    account.ResetPasswordCode = resetCode;
                    dc.Configuration.ValidateOnSaveEnabled = false; //for removing confirm password doesnt match issuse
                    dc.SaveChanges();
                    message = "email has been sent";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            ViewBag.Status = status;
            return View();
        }
        #endregion


        #region ResetPassword
        public ActionResult ResetPassword(string id) //id will come from the url
        {
            using (MyLoginsEntities dc = new MyLoginsEntities())
            {
                var v = dc.Users.Where(x => x.ResetPasswordCode == id).FirstOrDefault();
                if(v != null)
                {
                    ResetPassword model = new ResetPassword();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPassword model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (MyLoginsEntities dc = new MyLoginsEntities())
                {   // this user is the user of the main model ie dc.user
                    var user = dc.Users.Where(x => x.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.AutoDetectChangesEnabled = false;
                        dc.SaveChanges();
                        message = "Password Succefully Update";
                    }
                }
            }
            else
            {
                message = "invalid";
            }
            
            ViewBag.message = message;
            return View(model);
        }

        #endregion


        #region Check if email already exists or not
        [NonAction]
        public bool IsEmailExist(string emailID)
        {
            using (MyLoginsEntities dc = new MyLoginsEntities())
            {
                var v = dc.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
                return v == null ? false : true;
            }
        }
        #endregion


        #region to send email to your email address
        [NonAction]
        public void sendVerificationEmail(string emailId , string activationCode , string emailfor = "VerifyAccount")
        {
            var verifyUrl = "/User/"+emailfor+"/"+ activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("sunshine_appointment@outlook.com", "SunShine Appointment");
            var toEmail = new MailAddress(emailId);
            var fromEmailPassword = "Nepal@1000";
            string subject = "";
            string body = "";
            if(emailfor == "VerifyAccount")
            {
                subject = "Your account is successfully created! ";
                body = "<br/><br/>We are very happy you joined. " +
                    "Your account has been successfully created. " +
                    "Please click on the link below to verify your account:<br/><a href='" + link + "'>" + link + "</a>";
            }
            else if(emailfor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi<br/><br/> Hello You want to reset you password. So click the link below" +
                    "<br/><a href="+link+">Reset Password Link</a>";
            }

            var smtp = new SmtpClient
            {
                Host = "smtp.outlook.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }
        #endregion



        

    }

}
