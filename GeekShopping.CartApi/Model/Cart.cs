namespace GeekShopping.CartApi.Model
{
    public class Cart
    {
        public virtual CartHeader CartHeader { get; set; }
        public IEnumerable<CartDetail> CartDetails { get; set; }
    }
}