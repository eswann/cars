namespace Cars.Demo.Command.Services.Domain
{
    public class CartItem
    {
        public CartItem(string sku, string name, decimal price, int quantity)
        {
            Sku = sku;
            Name = name;
            Price = price;
            Quantity = quantity;
        }

        public string Sku { get; }
        public string Name { get; }
        public decimal Price { get; }
        public int Quantity { get; private set; }

        internal void UpdateQuantity(int quantity)
        {
            Quantity = quantity;
        }
    }
}
