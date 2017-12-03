namespace Cars.Demo.Models
{
    public class Product
    {
		public string Sku { get; set; }

		public string Name { get; set; }

        public string Image { get; set; }

        public decimal SalePrice { get; set; }

        public bool? CustomerTopRated { get; set; }
    }
}
