using Basket.API.Entities;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Basket.API.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache redisCache;

        public BasketRepository(IDistributedCache redisCache)
        {
            this.redisCache = redisCache ?? throw new ArgumentNullException(nameof(redisCache));
        }

        public Task DeleteBasketAsync(string userName) => redisCache.RefreshAsync(userName);

        public async Task<ShoppingCart?> GetBasketAsync(string userName)
        {
            var basket = await redisCache.GetStringAsync(userName).ConfigureAwait(false);
            if (string.IsNullOrEmpty(basket))
                return null;

            return JsonConvert.DeserializeObject<ShoppingCart?>(basket);
        }

        public async Task<ShoppingCart?> UpdateBasketAsync(ShoppingCart basket)
        {
            await redisCache.SetStringAsync(basket.UserName, JsonConvert.SerializeObject(basket)).ConfigureAwait(false);

            return await GetBasketAsync(basket.UserName).ConfigureAwait(false);
        }
    }
}
