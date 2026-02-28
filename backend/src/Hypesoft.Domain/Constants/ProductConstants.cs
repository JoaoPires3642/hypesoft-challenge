namespace Hypesoft.Domain.Constants
{
    /// <summary>
    /// Constantes relacionadas a produtos e gerenciamento de estoque.
    /// </summary>
    public static class ProductConstants
    {
        /// <summary>
        /// Threshold padrão para considerar um produto com estoque baixo (em unidades).
        /// Produtos com quantidade em estoque menor que este valor são considerados com estoque baixo.
        /// </summary>
        public const int LOW_STOCK_THRESHOLD = 10;

        /// <summary>
        /// Tempo padrão de expiração do cache para produtos (em minutos).
        /// </summary>
        public const int CACHE_EXPIRATION_MINUTES = 10;
    }
}
