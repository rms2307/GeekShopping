using GeekShopping.CartApi.Data.ValueObjects;
using GeekShopping.CartApi.Messages;
using GeekShopping.CartApi.RabbitMQSender;
using GeekShopping.CartApi.Repository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShopping.CartApi.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartRepository _repository;
        private readonly IRabbitMQMessageSender _rabbitMQMessageSender;

        private const string NAME_QUEUE = "checkoutqueue";

        public CartController(ICartRepository repository, IRabbitMQMessageSender rabbitMQMessageSender)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _rabbitMQMessageSender = rabbitMQMessageSender ?? throw new ArgumentNullException(nameof(rabbitMQMessageSender));
        }

        [HttpGet("find-cart/{userId}")]
        public async Task<ActionResult<CartVO>> FindById([FromRoute] string userId)
        {
            var cart = await _repository.FindCartByUserId(userId);
            if (cart?.CartHeader == null) return NotFound("Cart is Empty");

            return Ok(cart);
        }

        [HttpPost("add-cart")]
        public async Task<ActionResult<CartVO>> AddCart(CartVO vo)
        {
            var cart = await _repository.SaveOrUpdateCart(vo);
            if (cart == null) return NotFound();

            return Ok(cart);
        }

        [HttpPut("update-cart")]
        public async Task<ActionResult<CartVO>> UpdateCart(CartVO vo)
        {
            var cart = await _repository.SaveOrUpdateCart(vo);
            if (cart == null) return NotFound();

            return Ok(cart);
        }

        [HttpDelete("remove-cart/{id}")]
        public async Task<ActionResult<CartVO>> RemoveCart(int id)
        {
            var status = await _repository.RemoveFromCart(id);
            if (!status) return BadRequest();

            return Ok(status);
        }

        [HttpPost("apply-coupon")]
        public async Task<ActionResult<CartVO>> ApplyCoupon(CartHeaderVO vo)
        {
            var status = await _repository.ApplyCoupon(vo.UserId, vo.CouponCode);
            if (!status) return NotFound();

            return Ok(status);
        }

        [HttpDelete("remove-coupon/{userId}")]
        public async Task<ActionResult<CartVO>> RemoveCoupon(string userId)
        {
            var status = await _repository.RemoveCoupon(userId);
            if (!status) return NotFound();

            return Ok(status);
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<CheckoutHeaderVO>> Checkout(CheckoutHeaderVO vo)
        {
            if(vo?.UserId == null) return BadRequest();

            var cart = await _repository.FindCartByUserId(vo.UserId);
            if (cart?.CartHeader == null) return NotFound("Cart is Empty");
            vo.CartDetails = cart.CartDetails;
            vo.DateTime = DateTime.Now;

            _rabbitMQMessageSender.SendMessage(vo, NAME_QUEUE);

            return Ok(vo);
        }
    }
}
