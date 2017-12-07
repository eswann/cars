namespace Cars.Demo.Api.Command.Models
{
    public class AddCartItemRequest
    {
        public string Name { get; set; }
        public decimal SalePrice { get; set; }
        public string Image { get; set; }
        public bool CustomerTopRated { get; set; }
        public int Quantity { get; set; }
    }
}
