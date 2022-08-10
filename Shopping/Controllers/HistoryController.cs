using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Shopping.Controllers
{
    public class HistoryController : Controller
    {
        //Connect database.
        DB_CartEntities db = new DB_CartEntities();

        // GET: History
        public ActionResult Index()
        {
            //Check đăng nhập.
            var check = Session["User"];

            if (check == null)
            {
                return RedirectToAction("Index", "Login");
            }

            //Truyền dữ liệu dạng danh sách cho view.
            return View(db.tCarts.OrderBy(p => p.cID).ToList());
        }

        //Lấy ra ds dữ liệu của 1 mã đơn hàng nhất định.
        public ActionResult Details(int id)
        {
            /*var res = db.tCartDetails.
                Where(c => c.ciID == id).
                Join(db.tProducts, c => c.ciProduct, p => p.pID, (cart, product) => new {
                    ciID = cart.ciID,
                    ciAmount = cart.ciAmount,
                    ciPrice = cart.ciPrice,
                    pName = product.pName
                }).ToList();*/

            //Lấy ds dữ liệu dựa trên ID.
            var res = db.tCartDetails.Where(c => c.ciID == id).ToList();

            //Truyền ds dữ liệu cho view.
            return View(res);
        }
    }
}