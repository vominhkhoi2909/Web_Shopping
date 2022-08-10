using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Controllers
{
    public class HomeController : Controller
    {
        //Connect database.
        DB_CartEntities db = new DB_CartEntities();

        // GET: Home
        public ActionResult Index()
        {
            //Check đăng nhập.
            var check = Session["User"];

            if(check == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //Truyền danh sách sản phẩm về cho view Index.
            return View(db.tProducts.ToList());
        }

        //Thể hiện dữ liệu lên combo box.
        public ActionResult CartITem()
        {
            //Truy vấn lấy dữ liệu dạng list.
            var getProduct = db.tProducts.ToList();
            SelectList lst = new SelectList(getProduct, "pID", "pName");
            //Gán ds vào viewbag để truyền dữ liệu sang view.
            ViewBag.lstProduct = lst;

            return View();
        }
    }
}