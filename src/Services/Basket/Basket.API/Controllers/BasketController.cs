using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Message.Events;
using MassTransit;
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
        private readonly IPublishEndpoint publishEndpoint;
        private readonly IMapper mapper;

        public BasketController(IBasketRepository basketRepository, 
            DiscountGrpcService discountGrpcService,
            IMapper mapper,
            IPublishEndpoint publishEndpoint)
        {
            this.basketRepository = basketRepository ?? throw new ArgumentNullException(nameof(basketRepository));
            this.discountGrpcService = discountGrpcService ?? throw new ArgumentNullException(nameof(discountGrpcService));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.publishEndpoint = publishEndpoint ?? throw new ArgumentNullException(nameof(publishEndpoint));
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

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            if (basketCheckout is null)
            {
                throw new ArgumentNullException(nameof(basketCheckout));
            }
            // get existing basket with total price            
            // Set TotalPrice on basketCheckout eventMessage
            // send checkout event to rabbitmq
            // remove the basket

            // get existing basket with total price
            ShoppingCart? basket = await basketRepository.GetBasketAsync(basketCheckout.UserName);
            if (basket == null)
            {
                return BadRequest();
            }

            // send checkout event to rabbitmq
            var eventMessage = mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMessage.TotalPrice = basket.TotalPrice;
            await publishEndpoint.Publish(eventMessage);

            // remove the basket
            await basketRepository.DeleteBasketAsync(basket.UserName);

            return Accepted();
        }
    }
}
