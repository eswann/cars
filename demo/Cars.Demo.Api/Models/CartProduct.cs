using System;

namespace Cars.Demo.Api.Models
{
    public class CartProduct
    {
		public Guid Id { get; set; }

		public string Name { get; set; }

		public ProductType ProductType { get; set; }

		public decimal Price { get; set; }
    }
}
