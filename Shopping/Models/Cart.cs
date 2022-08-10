using System;
using System.Collections.Generic;
using System.Linq;

namespace Shopping.Models
{
    //Class thực hiện lưu từng dòng của giỏ hàng.
    public class CartItem
    {
        public tProduct _product { get; set; }
        public int _amount { get; set; }
    }

    //Class sẽ thực thiện tao tác trên giỏ hàng.
    public class Cart
    {
        //Tạo 1 mảng để lưu tất cả dữ liệu của giỏ hàng.
        List<CartItem> items = new List<CartItem>();

        public IEnumerable<CartItem> Items
        {
            get { return items; }
        }

        //Hàm thực hiện add 1 đối tượng vào giỏ hàng.
        public void Add(tProduct product, int amount = 1)
        {
            //Tìm kiếm đối tượng cần add.
            var item = items.FirstOrDefault(p => p._product.pID == product.pID);

            //Trường hợp đối tượng đó chưa có trong ds thì thêm mới vào với sl = 1;
            if (item == null)
            {
                items.Add(new CartItem
                {
                    _product = product,
                    _amount = amount
                });
            }
            //Nếu số lượng đã có thì sẽ + thêm sl đó.
            else
            {
                item._amount += amount;
            }
        }

        //Hàm thực hiện khi sử dụng nút update số lượng 
        public void UpdateAmount(int id, int amount)
        {
            //Tim đến đối tượng cần cập nhật.
            var item = items.Find(p => p._product.pID == id);

            if (item != null)
            {
                //Nếu đối tượng k null thì gán lại sl.
                item._amount = amount;
            }
        }

        //Hàm tính tổng tiền của giỏ hàng.
        public int TotalMoney()
        {
            try
            {
                //Gọi method linq để tính tổng tiền của tất cả dòng dữ liệu.
                int total = Convert.ToInt32(items.Sum(p => p._product.pPrice * p._amount));

                //Trả về tổng giá tiền của tất cả sp trong giỏ hàng.
                return total;
            }
            catch
            {
                return 0;
            }
        }

        //Hàm thực hiện xóa sản phẩm ra khỏi giỏ hàng.
        public void RemoveCartItem(int id)
        {
            //Thực hiện tìm sản phẩn cần xóa dựa trên id và remove khỏi ds.
            items.RemoveAll(p => p._product.pID == id);
        }

        //Hàm thực hiện tính tổng sl của tất cả sp trong giỏ hàng.
        public int TotalAmount()
        {
            return items.Sum(p => p._amount);
        }

        //Hàm thực hiện xóa toàn bộ giỏ hàng.
        public void ClearCart()
        {
            items.Clear();
        }
    }
}