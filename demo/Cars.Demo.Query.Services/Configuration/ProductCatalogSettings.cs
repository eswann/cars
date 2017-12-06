namespace Cars.Demo.Query.Services.Configuration
{
    public class ProductCatalogSettings : IProductCatalogSettings
    {
        private string _productListUri;

        public string ProductListBaseUri { get; set; }

        public string ApiKey { get; set; }

        public string ProductListUri => _productListUri ?? (_productListUri = ProductListBaseUri.Replace("{{apiKey}}", ApiKey));
    }
}
