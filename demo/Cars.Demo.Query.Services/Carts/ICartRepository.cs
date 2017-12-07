using System;
using System.Threading.Tasks;

namespace Cars.Demo.Query.Services.Carts
{
    public interface ICartRepository
    {
        Task<CartView> GetCartProjectionAsync(Guid cartId);
    }
}