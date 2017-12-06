namespace Cars.Demo.Api.Command.Models
{
    public class AddCartItemRequest
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
