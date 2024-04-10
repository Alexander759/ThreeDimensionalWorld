using Newtonsoft.Json;

namespace ThreeDimensionalWorld.Web.OrderPriceConfiguration
{
    public class ShippingPrice
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ShippingPrice(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        
        public decimal Price { get => GetPrice(); set => SetPrice(value); }

        public decimal GetPrice()
        {
            decimal price = 0;

            try
            {
                string path = Path.Combine(_webHostEnvironment.ContentRootPath, "OrderPriceConfiguration", "JSONData", "Price.json");
                string json = File.ReadAllText(path);

                dynamic? data = JsonConvert.DeserializeObject(json);

                if (data == null)
                {
                    throw new NullReferenceException();
                }

                price = data.Price;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving price: {ex.Message}");
            }

            return price;
        }

        public void SetPrice(decimal price)
        {
            try
            {
                dynamic jsonData = new
                {
                    Price = price
                };

                string jsonString = JsonConvert.SerializeObject(jsonData);

                string path = Path.Combine(_webHostEnvironment.ContentRootPath, "OrderPriceConfiguration", "JSONData", "Price.json");
                File.WriteAllText(path, jsonString);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving price: {ex.Message}");
            }
        }
    }
}