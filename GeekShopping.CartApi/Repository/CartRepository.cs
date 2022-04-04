﻿using AutoMapper;
using GeekShopping.CartApi.Data.ValueObjects;
using GeekShopping.CartApi.Model;
using GeekShopping.CartApi.Model.Context;
using Microsoft.EntityFrameworkCore;

namespace GeekShopping.CartApi.Repository
{
    public class CartRepository : ICartRepository
    {
        private readonly CartContext _context;
        private IMapper _mapper;

        public CartRepository(CartContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<CartVO> SaveOrUpdateCart(CartVO vo)
        {
            Cart cart = _mapper.Map<Cart>(vo);

            Product product = await ProductAlreadyExists(vo);
            if (product == null)
                await SaveProduct(cart);

            CartHeader cartHeader = await VerifyIfCartHeaderIsNull(cart);
            if (cartHeader == null)
            {
                await CreateCartHeader(cart);

                await CreateCartDetails(cart);
            }
            else
            {
                CartDetail cartDetail = await VerifyIfCartDetailsHasSameProduct(vo, cartHeader);

                if (cartDetail == null)
                {
                    await CreateCartDetails(cart);
                }
                else
                {
                    await UpdateCartDetails(cart, cartDetail);
                }
            }
            return _mapper.Map<CartVO>(cart);
        }        

        public async Task<bool> ApplyCoupon(string userId, string couponCode)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> ClearCart(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<CartVO> FindCartByUserId(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveCoupon(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> RemoveFromCart(long cartDetailsId)
        {
            throw new NotImplementedException();
        }

        private async Task UpdateCartDetails(Cart cart, CartDetail cartDetail)
        {
            cart.CartDetails.FirstOrDefault().Product = null;
            cart.CartDetails.FirstOrDefault().Count += cartDetail.Count;
            cart.CartDetails.FirstOrDefault().Id = cartDetail.Id;
            cart.CartDetails.FirstOrDefault().CartHeaderId = cartDetail.CartHeaderId;
            _context.CartDetails.Update(cart.CartDetails.FirstOrDefault());
            await _context.SaveChangesAsync();
        }

        private async Task<CartDetail> VerifyIfCartDetailsHasSameProduct(CartVO vo, CartHeader cartHeader)
        {
            return await _context.CartDetails.AsNoTracking()
                .FirstOrDefaultAsync(p => p.ProductId == vo.CartDetails
                    .FirstOrDefault().ProductId &&
                    p.CartHeaderId == cartHeader.Id);
        }

        private async Task CreateCartDetails(Cart cart)
        {
            cart.CartDetails.FirstOrDefault().CartHeaderId = cart.CartHeader.Id;
            cart.CartDetails.FirstOrDefault().Product = null;
            _context.CartDetails.Add(cart.CartDetails.FirstOrDefault());
            await _context.SaveChangesAsync();
        }

        private async Task CreateCartHeader(Cart cart)
        {
            _context.CartHeaders.Add(cart.CartHeader);
            await _context.SaveChangesAsync();
        }

        private async Task<CartHeader> VerifyIfCartHeaderIsNull(Cart cart)
        {
            return await _context.CartHeaders.AsNoTracking()
                            .FirstOrDefaultAsync(c => c.UserId == cart.CartHeader.UserId);
        }

        private async Task SaveProduct(Cart cart)
        {
            _context.Products.Add(cart.CartDetails.FirstOrDefault().Product);
            await _context.SaveChangesAsync();
        }

        private async Task<Product> ProductAlreadyExists(CartVO vo)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.Id == vo.CartDetails.FirstOrDefault().ProductId);
        }
    }
}
