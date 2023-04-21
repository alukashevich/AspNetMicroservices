using AspnetRunBasics.Models;

namespace AspnetRunBasics.Services
{
    public interface IBasketService
    {
        Task CheckoutBasket(BasketCheckoutModel model);
        Task<BasketModel> GetBasket(string userName);
        Task<BasketModel> UpdateBasket(BasketModel model);
    }
}