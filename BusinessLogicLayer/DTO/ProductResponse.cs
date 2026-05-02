namespace BusinessLogicLayer.DTO
{
    public record ProductResponse
        (Guid productID,
        string? ProductName,
        CategoryOptions Category,
        double? UnitPrice,
        int? QuantityInStock)
    {
        public ProductResponse() : this(default, default, default, default, default)
        {
            
        }
    }
}
