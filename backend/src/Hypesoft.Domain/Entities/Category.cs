namespace Hypesoft.Domain.Entities
{
    public class Category 
    {
        public Guid Id {get; private set;}
        public string Name { get; private set; }

        private Category() { }
        public Category(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("O nome da Categoria é obrigatório.", nameof(name));

            Id = Guid.NewGuid();
            Name = name;
        }

        public void UpdateName(string newName)
        {
            if (string.IsNullOrEmpty(newName))
                throw new ArgumentException("O nome da Categoria é obrigatório.", nameof(newName));

            Name = newName;
        }
    }
}