namespace GeekShopping.CartApi.Data.ValueObjects
{
    public class CartVO
    {
        public virtual CartHeaderVO CartHeader { get; set; }
        public IEnumerable<CartDetailVO> CartDetails { get; set; }
    }
}