using Shopping.Models;
using System.Linq;
using System.Web.Mvc;

namespace Shopping.Controllers
{
    public class ProductController : Controller
    {
        //Connect database.
        DB_CartEntities db = new DB_CartEntities();

        // GET: Product
        public ActionResult Index()
        {
            //Check đăng nhập.
            var check = Session["User"];

            if (check == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return View(db.tProducts.ToList());
        }

        // GET: Product/Create
        public ActionResult Create()
        {
            //Hiển thị trang view.
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        public ActionResult Create(tProduct product)
        {
            try
            {
                // TODO: Add insert logic here
                db.tProducts.Add(product);
                db.SaveChanges();

                //Điều hướng về ds sản phẩm.
                return RedirectToAction("Index");
            }
            catch
            {
                //Đối với lỗi sẽ ở lại giao diện hiện tại.
                return View();
            }
        }

        // GET: Product/Edit/5
        public ActionResult Edit(int id)
        {
            //Lấy ra sản phẩm được chọn để edit.
            return View(db.tProducts.Where(p => p.pID == id).FirstOrDefault());
        }

        // POST: Product/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, tProduct product)
        {
            try
            {
                // TODO: Add update logic here
                var update = db.tProducts.Where(p => p.pID == id).FirstOrDefault();
                update.pName = product.pName;
                update.pPrice = product.pPrice;
                db.SaveChanges();

                //Điều hướng về ds sản phẩm.
                return RedirectToAction("Index", "Product");
            }
            catch
            {
                //Đối với lỗi truy vấn điều hướng về trang chủ.
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Product/Delete/5
        public ActionResult Delete(int id)
        {
            //Lấy ra sản phẩm dựa trên id cần xóa.
            return View(db.tProducts.Where(p => p.pID == id).FirstOrDefault());
        }

        // POST: Product/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, tProduct product)
        {
            try
            {
                // TODO: Add delete logic here
                product = db.tProducts.Where(p => p.pID == id).FirstOrDefault();
                db.tProducts.Remove(product);
                db.SaveChanges();

                //Điều hướng về ds sản phẩm.
                return RedirectToAction("Index");
            }
            catch
            {
                //Trường hợp lỗi sẽ giữ nguyên giao diện.
                return View();
            }
        }
    }
}
