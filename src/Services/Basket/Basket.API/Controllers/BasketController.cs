using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BasketController : ControllerBase
    {
        private readonly IBasketRepository basketRepository;
        private readonly DiscountGrpcService discountGrpcService;

        public BasketController(IBasketRepository basketRepository, DiscountGrpcService discountGrpcService)
        {
            this.basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            this.discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
        }

        [HttpGet("{userName}", Name = "GetBasket")]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> GetBasketAsync(string userName)
        {
            ShoppingCart? basket = await basketRepository.GetBasketAsync(userName);

            return Ok(basket ?? new ShoppingCart(userName));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ShoppingCart), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> UpdateBasketAsync([FromBody] ShoppingCart basket, CancellationToken cancellationToken)
        {
            foreach(var item in basket.Items)
            {
                var coupon = await discountGrpcService.GetCouponModel(item.ProductName, cancellationToken);
                item.Price -= coupon.Amount;
            }
            return Ok(await basketRepository.UpdateBasketAsync(basket));
        }


        [HttpDelete("{userName}", Name = "DeleteBasket")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<ActionResult<ShoppingCart>> DeleteBasketAsync(string userName)
        {
            await basketRepository.DeleteBasketAsync(userName);
            return Ok();
        }
    }
}
