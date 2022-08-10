using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Controllers
{
    public class LoginController : Controller
    {
        //Connect database.
        DB_CartEntities db = new DB_CartEntities();

        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(tUser user)
        {
            if(ModelState.IsValid)
            {
                if(db.tUsers.FirstOrDefault(u => u.uEmail == user.uEmail) != null)
                {
                    ViewBag.error = "Email đã đã được đăng ký.";
                }
                else if (db.tUsers.FirstOrDefault(u => u.uPhone == user.uPhone) != null)
                {
                    ViewBag.error = "Số điện thoại đã được đăng ký.";
                }
                else if (db.tUsers.FirstOrDefault(u => u.uUsername == user.uUsername) != null)
                {
                    ViewBag.error = "Tên tài khoản đã đã được đăng ký.";
                }
                else
                {
                    db.Configuration.ValidateOnSaveEnabled = false;
                    db.tUsers.Add(user);
                    db.SaveChanges();

                    return RedirectToAction("RegisterSuccess");
                }
            }
            return View();
        }

        public ActionResult RegisterSuccess()
        {
            return View();
        }

        public ActionResult Login(tUser user)
        {
            var check = db.tUsers.Where(u => u.uUsername.Equals(user.uUsername) && u.uPassword.Equals(user.uPassword)).FirstOrDefault();
            
            if (check == null)
            {
                return View("Index", user);
            }
            else
            {
                Session["User"] = check;
                Session["UserName"] = check.uUsername;
            }

            return RedirectToAction("Index","Home");
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Login");
        }

        public ActionResult ForgetPassword()
        {
            return View();
        }
    }
}