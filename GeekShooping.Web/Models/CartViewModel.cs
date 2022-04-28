using System.Collections.Generic;

namespace GeekShopping.Web.Models
{
    public class CartViewModel
    {
        public virtual CartHeaderViewModel CartHeader { get; set; }
        public IEnumerable<CartDetailViewModel> CartDetails { get; set; }
        public bool CouponIsValid  { get; set; }
    }
}