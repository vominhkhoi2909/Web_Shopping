using Shopping.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PayPal.Api;
using Shopping.App_Start;

namespace Shopping.Controllers
{
    public class ShoppingCartController : Controller
    {
        //Connect database.
        DB_CartEntities db = new DB_CartEntities();

        //Thêm 1 đối tượng vào session.
        public Cart GetCart()
        {
            //Gán dữ liệu session về cho đối tượng
            Cart cart = Session["Cart"] as Cart;

            //Kiểm tra xem dữ liệu đối tượng hoặc session có rỗng k?
            if (cart == null || Session["Cart"] == null)
            {
                //Với trường hợp rỗng khởi tạo mới 1 đối tượng và gán lại cho session.
                cart = new Cart();
                Session["Cart"] = cart;
            }

            //Trả về đối tượng.
            return cart;
        }

        //Hàm add 1 đối tượng vào danh sách session của list giỏ hàng.
        public ActionResult AddToCart(int id)
        {
            //Kiểm tra xem đối tượng muốn truyền vào có tồn tại trong db k?
            var product = db.tProducts.SingleOrDefault(p => p.pID == id);

            if (product != null)
            {
                //Nếu đối tượng có tồn tại thì gọi hàm add vào session.
                GetCart().Add(product);
            }

            return RedirectToAction("Index", "Home");
        }

        //Hàm điều hướng url sang view của giỏ hàng.
        public ActionResult GoToCart()
        {
            //Check đăng nhập.
            var check = Session["User"];

            if (check == null)
            {
                return RedirectToAction("Index", "Login");
            }

            return RedirectToAction("ShowToCart", "ShoppingCart");
        }

        //Trang giao diện giỏ hàng.
        public ActionResult ShowToCart()
        {
            Cart cart = Session["Cart"] as Cart;

            return View(cart);
        }

        //Hàm xử lý cập nhật số lượng sản phẩm.
        public ActionResult UpdateAmount(FormCollection frm)
        {
            //Gán session hiện tại vào model để thực hiện thay đổi.
            Cart cart = Session["Cart"] as Cart;
            //Lấy id để xác định đối tượng cần thay đổi và giá trị để thay đổi.
            int idProduct = Convert.ToInt32(frm["idProduct"]);
            int amount = Convert.ToInt32(frm["fAmount"]);
            //Gọi hàm trong model để thay đổi.
            if(amount <= 0)
            {
                amount = 1;
            }
            cart.UpdateAmount(idProduct, amount);

            //Ở lại giao diện giỏ hàng.
            return RedirectToAction("ShowToCart", "ShoppingCart");
        }

        //Hàm thực hiện xóa sản phẩm ra khỏi list giỏ hàng.
        public ActionResult RemoveCart(int id)
        {
            //Gán session lại cho biến model để
            Cart cart = Session["Cart"] as Cart;
            cart.RemoveCartItem(id);

            //Ở lại giao diện giỏ hàng.
            return RedirectToAction("ShowToCart", "ShoppingCart");
        }

        //Hiển số lượng / icon giỏ hàng để ng dùng xem.
        public PartialViewResult BagCart()
        {
            int it = 0;
            Cart cart = Session["Cart"] as Cart;

            if(cart != null)
            {
                //Trường hợp session k rỗng sẽ gọi hàm model sum tất cả số lượng.
                it = cart.TotalAmount();
            }
            ViewBag.infoCart = it;

            return PartialView("BagCart");
        }

        //Hàm điều hướng sang trang checkout.
        public ActionResult CheckOut()
        {
            return View();
        }

        //Hàm thực hiện xử lý thanh toán.
        public ActionResult Pay(FormCollection frm)
        {
            try
            {
                //Gán dữ liệu session vào biến model để thao tác.
                Cart cart = Session["Cart"] as Cart;

                //Gán dữ liệu giỏ hàng vào biến để add lên db.
                
                tCart order = new tCart();
                order.cDate = DateTime.Now;
                order.cTotal = Convert.ToInt32(cart.TotalMoney());
                order.cPayType = frm["ThanhToan"];
                db.tCarts.Add(order);
                db.SaveChanges();
                var id = db.tCarts.OrderByDescending(p => p.cID).FirstOrDefault();

                foreach (var item in cart.Items)
                {
                    tCartDetail orderDetail = new tCartDetail();
                    orderDetail.ciID = id.cID;
                    orderDetail.ciProduct = item._product.pID;
                    orderDetail.ciAmount = item._amount;
                    orderDetail.ciPrice = item._product.pPrice;
                    db.tCartDetails.Add(orderDetail);
                }

                //Lưu dữ liệu db và xóa hết những sản phẩm trong giỏ hàng.
                db.SaveChanges();

                if (frm["ThanhToan"] == "PayPal")
                {
                    return PaypalCheckout();
                }

                cart.ClearCart();
                //Khi check out xog điều hướng về trang chủ.
                return RedirectToAction("AccelpOrder", "ShoppingCart");
            }
            catch (Exception ex)
            {
                //Trường hợp lỗi sẽ hiển thị giao diện vs content lỗi.
                return Content("Lỗi checkout vui lòng kiểm tra lại thông tin.\n" + ex.ToString());
            }
        }

        public ActionResult AccelpOrder()
        {
            return View();
        }

        //Hàm thực hiện xử lý thanh toán.
        public ActionResult PaypalCheckout()
        {
            //getting the apiContext
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = Request.Params["PayerID"];
                if (string.IsNullOrEmpty(payerId))
                {
                    string baseURI = Request.Url.Scheme + "://" + Request.Url.Authority + "/Cart/PaypalCheckout?";
                    var guid = Convert.ToString((new Random()).Next(100000));
                    var createdPayment = this.CreatePayment(apiContext);
                    var links = createdPayment.links.GetEnumerator();
                    string paypalRedirectUrl = null;
                    while (links.MoveNext())
                    {
                        Links lnk = links.Current;
                        if (lnk.rel.ToLower().Trim().Equals("approval_url"))
                        {
                            paypalRedirectUrl = lnk.href;
                        }
                    }
                    Session.Add(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    var guid = Request.Params["guid"];
                    var executedPayment = ExecutePayment(apiContext, payerId, Session[guid] as string);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        return View("FailureView");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return View("CheckoutSuccess");
        }

        private PayPal.Api.Payment payment;

        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
        }

        private Payment CreatePayment(APIContext apiContext)
        {
            //creat paypal order
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };
            Cart cart = Session["Cart"] as Cart;
            var total = cart.Items.Sum(c => c._amount * c._product.pPrice);

            foreach (var item in cart.Items)
            {
                itemList.items.Add(new Item()
                {
                    name = item._product.pName,
                    currency = "USD",
                    price = item._product.pPrice.ToString(),
                    quantity = item._amount.ToString(),
                    sku = "sku",
                    tax = "0"
                });
            }
            var paypalOrderId = DateTime.Now.Ticks;
            var hostname = Request.Url.Scheme + "://" + Request.Url.Authority;
            this.payment = new Payment()
            {
                intent = "sale",
                transactions = new List<Transaction>() {new Transaction()
                    {
                        amount = new Amount()
                        {
                            total = total.ToString(),
                            currency = "USD",
                            details = new Details
                            {
                                tax = "0",
                                shipping = "0",
                                subtotal = total.ToString()
                            }
                        },
                        item_list = itemList,
                        description = $"Invoice #{paypalOrderId}",
                        invoice_number = paypalOrderId.ToString()
                    } },
                redirect_urls = new RedirectUrls()
                {
                    cancel_url = $"{hostname}/ShoppingCart/ShowToCart",
                    return_url = $"{hostname}/ShoppingCart/CheckoutSuccess"
                },
                payer = new Payer()
                {
                    payment_method = "paypal"
                }
            };
            // Create a payment using a APIContext  
            return this.payment.Create(apiContext);
        }

        public ActionResult CheckoutFail()
        {
            //Tạo đơn hàng trong database với trạng thái thanh toán là "Chưa thanh toán"
            //Xóa session
            return View();
        }

        public ActionResult CheckoutSuccess()
        {
            //Tạo đơn hàng trong database với trạng thái thanh toán là "Paypal" và thành công
            //Xóa session
            Session["Cart"] = null;
            return View();
        }
    }
}