namespace Website.Siegwart.BLL.Services.Interfaces
{
    public interface IProductService
    {
        Task<int> CreateProductAsync(CreateProductDto input);
        Task<int> DeleteProductAsync(int id);
        Task<int> UpdateProductAsync(UpdateProductDto input);
        Task<List<ProductListDto>> GetAllProductsAsync();
        Task<ProductDetailsDto?> GetProductByIdAsync(int id);
        Task<UpdateProductDto?> GetProductForEditAsync(int id);
    }
}