namespace Hypesoft.Domain.Entities
{
    public class Product
    {
        private const int LOW_STOCK_THRESHOLD = 10;

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Description { get; private set; }
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public Guid CategoryId { get; private set; }


        private Product() { }
        public Product(string name, string description, decimal price, int stockQuantity, Guid categoryId)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("O nome do Produto é obrigatório.", nameof(name));
            if (price < 0)
                throw new ArgumentException("O preço do Produto não pode ser negativo.", nameof(price));
            if (stockQuantity < 0)
                throw new ArgumentException("A quantidade em estoque do Produto não pode ser negativa.", nameof(stockQuantity));

            Id = Guid.NewGuid();
            Name = name;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            CategoryId = categoryId;
        }
        public void UpdateStock (int newQuantity)
        {
            if (newQuantity < 0)
                throw new ArgumentException("A quantidade em estoque do Produto não pode ser negativa.", nameof(newQuantity));

            StockQuantity = newQuantity;
        }

        public void UpdateDetails(string name, string description, decimal price)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("O nome do Produto é obrigatório.", nameof(name));
            if (price < 0)
                throw new ArgumentException("O preço do Produto não pode ser negativo.", nameof(price));

            Name = name;
            Description = description;
            Price = price;
        }

        public bool IsStockLow() => StockQuantity < LOW_STOCK_THRESHOLD;

    }
}