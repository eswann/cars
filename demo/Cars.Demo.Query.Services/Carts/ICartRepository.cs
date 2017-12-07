using System;
using System.Threading.Tasks;

namespace Cars.Demo.Query.Services.Carts
{
    public interface ICartRepository
    {
        Task<CartProjection> GetProjectionAsync(Guid cartId);
    }
}